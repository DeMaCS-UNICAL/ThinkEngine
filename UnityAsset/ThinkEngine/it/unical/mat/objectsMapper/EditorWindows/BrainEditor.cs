using EmbASP4Unity.it.unical.mat.objectsMapper.BrainsScripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace EmbASP4Unity.it.unical.mat.objectsMapper.EditorWindows
{
    [CustomEditor(typeof(Brain))]
    public class BrainEditor:Editor
    {
        List<string> excludedProperties=new List<string>();
        List<string> methodsToShow = new List<string>();
        List<string> methodsToShowForReasoner = new List<string>();
        Dictionary<int, List<string>> sensorsConfigurationNames = new Dictionary<int, List<string>>();
        Dictionary<int, List<string>> actuatorsConfigurationNames = new Dictionary<int, List<string>>();
        Dictionary<int, string> gameObjectNamesPerIndex = new Dictionary<int, string>();
        Dictionary<string, bool> toggledSensorsConfigurations = new Dictionary<string, bool>();
        Dictionary<string, bool> toggledActuatorsConfigurations = new Dictionary<string, bool>();
        Dictionary<int, bool> toggledGameObjectsForSensors = new Dictionary<int, bool>();
        Dictionary<int, bool> toggledGameObjectsForActuators = new Dictionary<int, bool>();
        public int sensorsUpdateIndex = 0;
        public int reasoningExecutionIndex = 0;
        public int applyActuatorsIndex=0;
        private List<string> methodsToShowForActuators=new List<string>();
        private Brain myScript;

        void OnEnable()
        {
            myScript = target as Brain;
            basicConfiguration();
            synchWithBrain();
        }

        private void synchWithBrain()
        {
            
            for (int i = 0; i < methodsToShowForReasoner.Count; i++)
            {
                if (myScript.executeReasonerOn.Equals(methodsToShowForReasoner[i]))
                {
                    reasoningExecutionIndex = i;
                    break;
                }
            }
            
            foreach (SensorConfiguration sensorConf in myScript.sensorsConfigurations)
            {
                toggledGameObjectsForSensors[sensorConf.GetComponent<IndexTracker>().currentIndex] = true;
                toggledSensorsConfigurations[sensorConf.name] = true;
            }
            foreach (ActuatorConfiguration actuatorConf in myScript.actuatorsConfigurations)
            {
                toggledGameObjectsForActuators[actuatorConf.GetComponent<IndexTracker>().currentIndex] = true;
                toggledActuatorsConfigurations[actuatorConf.name] = true;
            }
        }

        private void basicConfiguration()
        {
            var triggerClass = ScriptableObject.CreateInstance("Trigger");
            MethodInfo[] methods = triggerClass.GetType().GetMethods();

            foreach (MethodInfo mI in methods)
            {
                if (mI.ReturnType == typeof(bool) && mI.GetParameters().Length == 0)
                {
                    methodsToShow.Add(mI.Name);
                    MyDebugger.MyDebug(mI.Name);
                }
            }
            methodsToShowForReasoner.Add("When Sensors are ready");
            methodsToShowForReasoner.AddRange(methodsToShow);

            foreach (SensorConfiguration sensorConf in FindObjectsOfType<SensorConfiguration>())
            {
                addConfigurationName(sensorConf, sensorsConfigurationNames, toggledGameObjectsForSensors, toggledSensorsConfigurations);
            }
            foreach (ActuatorConfiguration actuatorConf in FindObjectsOfType<ActuatorConfiguration>())
            {
                addConfigurationName(actuatorConf, actuatorsConfigurationNames, toggledGameObjectsForActuators, toggledActuatorsConfigurations);
            }
        }

        private void addConfigurationName(AbstractConfiguration configuration, Dictionary<int, List<string>> configurationsNames, Dictionary<int,bool> toggledGameObjects, Dictionary<string, bool> toggledConfigurations)
        {
            int currentGameObjectIndex = configuration.GetComponent<IndexTracker>().currentIndex;
            if (!configurationsNames.ContainsKey(currentGameObjectIndex))
            {
                configurationsNames.Add(currentGameObjectIndex, new List<string>());
                if (!gameObjectNamesPerIndex.ContainsKey(currentGameObjectIndex))
                {
                    gameObjectNamesPerIndex.Add(currentGameObjectIndex, configuration.gameObject.name);
                    toggledGameObjects.Add(currentGameObjectIndex, false);
                }
            }
            configurationsNames[currentGameObjectIndex].Add(configuration.name);
            toggledConfigurations.Add(configuration.name, false);
        }

        public override void OnInspectorGUI()
        {
            //SENSORS
            showConfigurations(toggledGameObjectsForSensors, sensorsConfigurationNames, toggledSensorsConfigurations);
            //ACTUATORS
            showConfigurations(toggledGameObjectsForActuators, actuatorsConfigurationNames, toggledActuatorsConfigurations);
            
            excludedProperties = new List<string>();
            excludedProperties.Add("executeReasonerOn");
            DrawPropertiesExcluding(serializedObject, excludedProperties.ToArray());
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Choose when to run the reasoner");
            reasoningExecutionIndex = EditorGUILayout.Popup(reasoningExecutionIndex, methodsToShowForReasoner.ToArray());
            EditorGUILayout.EndHorizontal();
            myScript.executeReasonerOn = methodsToShowForReasoner[reasoningExecutionIndex];
            serializedObject.ApplyModifiedProperties();//CHECK SERIALIZATION

            Brain current = (Brain)target;
            if (GUILayout.Button("Generate ASP file template", GUILayout.Width(300)))
            {
                current.generateFile();
            }
            EditorGUILayout.HelpBox("Generating a new file will delete the previouse template!", MessageType.Warning);

        }

        private void showConfigurations(Dictionary<int,bool> toggledGameObjects, Dictionary<int,List<string>> configurationNames, Dictionary<string, bool> toggledConfigurations)
        {
            foreach (int gameObjectIndex in toggledGameObjects.Keys)
            {
                toggledGameObjects[gameObjectIndex] = EditorGUILayout.Foldout(toggledGameObjects[gameObjectIndex], gameObjectNamesPerIndex[gameObjectIndex]);
                if (toggledGameObjects[gameObjectIndex])
                {
                    foreach (string confName in configurationNames[gameObjectIndex])
                    {
                        toggledConfigurations[confName] = EditorGUILayout.ToggleLeft(confName, toggledConfigurations[confName]);
                    }
                }
            }
        }
    }
}
