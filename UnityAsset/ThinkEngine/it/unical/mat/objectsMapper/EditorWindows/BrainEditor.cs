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
        Dictionary<int, List<string>> sensorsConfigurationNames = new Dictionary<int, List<string>>();
        Dictionary<string, ActuatorConfiguration> actuatorConfigurations;
        Dictionary<string, SensorConfiguration> sensorConfigurations;
        Dictionary<int, List<string>> actuatorsConfigurationNames = new Dictionary<int, List<string>>();
        Dictionary<int, string> gameObjectNamesPerIndex = new Dictionary<int, string>();
        Dictionary<string, bool> toggledSensorsConfigurations = new Dictionary<string, bool>();
        Dictionary<string, bool> toggledActuatorsConfigurations = new Dictionary<string, bool>();
        Dictionary<int, bool> toggledGameObjectsForSensors = new Dictionary<int, bool>();
        Dictionary<int, bool> toggledGameObjectsForActuators = new Dictionary<int, bool>();
        public int reasoningExecutionIndex = 0;
        private Brain myScript;

        void Reset()
        {
            myScript = target as Brain;
            sensorConfigurations = new Dictionary<string, SensorConfiguration>();
            actuatorConfigurations = new Dictionary<string, ActuatorConfiguration>();
            basicConfiguration();
            readingFromBrain();
        }

        private void readingFromBrain()
        {
            reasoningExecutionIndex = Utility.getTriggerMethodIndex(myScript.executeReasonerOn);
            MyDebugger.MyDebug("index " + reasoningExecutionIndex);
            bool delete = false;
            foreach (SensorConfiguration sensorConf in myScript.sensorsConfigurations)
            { 
                if(sensorConf == null)
                {
                    delete = true;
                    continue;
                }
                toggledGameObjectsForSensors[sensorConf.GetComponent<IndexTracker>().currentIndex] = true;
                toggledSensorsConfigurations[sensorConf.configurationName] = true;
            }
            if (delete)
            {
                myScript.removeNullSensorConfigurations();
                delete = false;
            }
            foreach (ActuatorConfiguration actuatorConf in myScript.actuatorsConfigurations)
            {
                if (actuatorConf == null)
                {
                    delete = true;
                    continue;
                }
                toggledGameObjectsForActuators[actuatorConf.GetComponent<IndexTracker>().currentIndex] = true;
                toggledActuatorsConfigurations[actuatorConf.configurationName] = true;
            }
            if (delete)
            {
                myScript.removeNullActuatorConfigurations();
                delete = false;
            }
        }

        private void basicConfiguration()
        {
            methodsToShow = Utility.triggerMethodsToShow;
            methodsToShow.Add("When Sensors are ready");
            
            removeUnexistingConfigurations();
            addNewConfigurations();
        }

        private void removeUnexistingConfigurations()
        {
            bool removed = false;
            List<string> toDelete = new List<string>();
            foreach(string sensorName in sensorConfigurations.Keys)
            {
                if (!FindObjectOfType<SensorsManager>().existsConfigurationWithName(sensorName)){
                    toDelete.Add(sensorName);
                    toggledSensorsConfigurations.Remove(sensorName);
                    removed = true;
                }
            }
            if (removed)
            {
                myScript.removeNullSensorConfigurations();
                removed = false;
            }
            foreach (string current in toDelete) {
                sensorConfigurations.Remove(current);
            }
            toDelete = new List<string>();
            foreach (string actuatorName in actuatorConfigurations.Keys)
            {
                if (!FindObjectOfType < ActuatorsManager>().existsConfigurationWithName(actuatorName))
                {
                    toDelete.Add(actuatorName);
                    toggledActuatorsConfigurations.Remove(actuatorName);
                    removed = true;
                }
            }
            foreach (string current in toDelete)
            {
                actuatorConfigurations.Remove(current);
            }
            if (removed)
            {
                myScript.removeNullActuatorConfigurations();
            }

        }

        private void addNewConfigurations()
        {
            if (FindObjectsOfType<SensorConfiguration>().Length > 0)
            {
                foreach (SensorConfiguration sensorConf in FindObjectsOfType<SensorConfiguration>())
                {
                    if (sensorConf.saved)
                    {
                        addConfigurationName(sensorConf, sensorsConfigurationNames, toggledGameObjectsForSensors, toggledSensorsConfigurations);
                    }
                }
            }
            if (FindObjectsOfType<ActuatorConfiguration>().Length > 0)
            {
                foreach (ActuatorConfiguration actuatorConf in FindObjectsOfType<ActuatorConfiguration>())
                {
                    if (actuatorConf.saved)
                    {
                        addConfigurationName(actuatorConf, actuatorsConfigurationNames, toggledGameObjectsForActuators, toggledActuatorsConfigurations);
                    }
                }
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
            if (!configurationsNames[currentGameObjectIndex].Contains(configuration.configurationName))
            {
                configurationsNames[currentGameObjectIndex].Add(configuration.configurationName);
                toggledConfigurations.Add(configuration.configurationName, false);
                if(configuration.GetType().Equals(typeof(SensorConfiguration)))
                {
                    sensorConfigurations.Add(configuration.configurationName, (SensorConfiguration)configuration);
                }
                else
                {
                    actuatorConfigurations.Add(configuration.configurationName, (ActuatorConfiguration)configuration);
                }
            }
        }

        public override void OnInspectorGUI()
        {
            basicConfiguration();
            GUI.enabled = false;
            EditorGUILayout.TextField("Trigger Script Path", Utility.triggerClassPath);
            EditorGUILayout.TextField("ASP File Path", myScript.ASPFilePath);
            EditorGUILayout.TextField("ASP Template File Path", myScript.ASPFileTemplatePath);
            GUI.enabled = true;
            //SENSORS
            EditorGUILayout.LabelField("GameObjects actually attached to some Sensor Configuration", EditorStyles.boldLabel);
            showConfigurations(toggledGameObjectsForSensors, sensorsConfigurationNames, toggledSensorsConfigurations);
            //ACTUATORS
            EditorGUILayout.LabelField("GameObjects actually attached to some Actuator Configuration", EditorStyles.boldLabel);
            showConfigurations(toggledGameObjectsForActuators, actuatorsConfigurationNames, toggledActuatorsConfigurations,true);

            excludedProperties = new List<string>();
            excludedProperties.Add("executeReasonerOn");
            DrawPropertiesExcluding(serializedObject, excludedProperties.ToArray());
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Choose when to run the reasoner");
            reasoningExecutionIndex = EditorGUILayout.Popup(reasoningExecutionIndex, methodsToShow.ToArray());
            EditorGUILayout.EndHorizontal();
            myScript.executeReasonerOn = methodsToShow[reasoningExecutionIndex];
            serializedObject.ApplyModifiedProperties();//CHECK SERIALIZATION

            Brain current = (Brain)target;
            if (GUILayout.Button("Generate ASP file template", GUILayout.Width(300)))
            {
                current.generateFile();
            }
            EditorGUILayout.HelpBox("Generating a new file will delete the previouse template!", MessageType.Warning);
            savingInBrain();
        }

        

        private void showConfigurations(Dictionary<int,bool> toggledGameObjects, Dictionary<int,List<string>> configurationNames, Dictionary<string, bool> toggledConfigurations, bool isActuator=false)
        {
            foreach (int gameObjectIndex in gameObjectNamesPerIndex.Keys)
            {
                if (toggledGameObjects.ContainsKey(gameObjectIndex))
                {
                    MyDebugger.MyDebug("index = " + gameObjectIndex + " go = " + gameObjectNamesPerIndex[gameObjectIndex]);
                    toggledGameObjects[gameObjectIndex] = EditorGUILayout.Foldout(toggledGameObjects[gameObjectIndex], gameObjectNamesPerIndex[gameObjectIndex]);
                    if (toggledGameObjects[gameObjectIndex])
                    {
                        foreach (string confName in configurationNames[gameObjectIndex])
                        {
                            if (isActuator && !(actuatorConfigurations[confName].assignedTo is null) && !actuatorConfigurations[confName].assignedTo.Equals(myScript))
                            {
                                GUI.enabled = false;
                            }
                            toggledConfigurations[confName] = EditorGUILayout.ToggleLeft(confName, toggledConfigurations[confName]);
                            GUI.enabled = true;

                        }
                    }
                }
            }
        }
        private void savingInBrain()
        {
            foreach(string sensorConfigurationName in toggledSensorsConfigurations.Keys)
            {
                SensorConfiguration currentConfiguration = sensorConfigurations[sensorConfigurationName];
                if (toggledSensorsConfigurations[sensorConfigurationName])
                {
                    if (!myScript.sensorsConfigurations.Contains(currentConfiguration))
                    {
                        myScript.sensorsConfigurations.Add(currentConfiguration);
                    }
                }else if (myScript.sensorsConfigurations.Contains(currentConfiguration))
                {
                    myScript.sensorsConfigurations.Remove(currentConfiguration);
                }
            }
            foreach (string actuatorConfigurationName in toggledActuatorsConfigurations.Keys)
            {
                ActuatorConfiguration currentConfiguration = actuatorConfigurations[actuatorConfigurationName];
                if (toggledActuatorsConfigurations[actuatorConfigurationName])
                {
                    if (!myScript.actuatorsConfigurations.Contains(currentConfiguration))
                    {
                        myScript.actuatorsConfigurations.Add(currentConfiguration);
                        currentConfiguration.assignedTo = myScript;
                    }
                }
                else if (myScript.actuatorsConfigurations.Contains(currentConfiguration))
                {
                    myScript.actuatorsConfigurations.Remove(currentConfiguration);
                    currentConfiguration.assignedTo = null;
                }
            }
        }
        
    }
}
