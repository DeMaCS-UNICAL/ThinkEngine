using EmbASP4Unity.it.unical.mat.objectsMapper.BrainsScripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;

namespace EmbASP4Unity.it.unical.mat.objectsMapper.EditorWindows
{
    [CustomEditor(typeof(Brain))]
    public class BrainEditor:Editor
    {
        List<string> excludedProperties=new List<string>();
        List<string> methodsToShow = new List<string>();
        List<string> sensorsConfigurationNames;
        List<string> actuatorsConfigurationNames;
        Dictionary<string, bool> toggledSensorsConfigurations = new Dictionary<string, bool>();
        Dictionary<string, bool> toggledActuatorsConfigurations = new Dictionary<string, bool>();
        public int reasoningExecutionIndex = 0;
        private Brain myScript;

        void OnEnable()
        {
            Utility.loadPrefabs();
            reset();
            myScript.prefabBrain = PrefabStageUtility.GetCurrentPrefabStage() != null;
        }
        void reset()
        {
            myScript = target as Brain;
            if (sensorsConfigurationNames == null)
            {
                sensorsConfigurationNames = new List<string>();
            }
            if (actuatorsConfigurationNames == null)
            {
                actuatorsConfigurationNames = new List<string>();
            }
            basicConfiguration();
            readingFromBrain();
        }

        private void readingFromBrain()
        {
            reasoningExecutionIndex = Utility.getTriggerMethodIndex(myScript.executeReasonerOn);
            bool delete = false;
            foreach (string sensorConfName in myScript.chosenSensorConfigurations)
            {
                if (!Utility.sensorsManager.existsConfigurationWithName(sensorConfName))
                {
                    delete = true;
                    continue;
                }
                //toggledGameObjectsForSensors[sensorConf.GetComponent<IndexTracker>().currentIndex] = true;
                toggledSensorsConfigurations[sensorConfName] = true;
            }
            if (delete)
            {
                myScript.RemoveNullSensorConfigurations();
                delete = false;
            }
            foreach (string actuatorConfName in myScript.chosenActuatorConfigurations)
            {
                if (!Utility.actuatorsManager.ExistsConfigurationWithName(actuatorConfName,myScript))
                {
                    delete = true;
                    continue;
                }
                //toggledGameObjectsForActuators[actuatorConfName.GetComponent<IndexTracker>().currentIndex] = true;
                toggledActuatorsConfigurations[actuatorConfName] = true;
            }
            if (delete)
            {
                myScript.RemoveNullActuatorConfigurations();
                delete = false;
            }
        }

        private void basicConfiguration()
        {
            methodsToShow = Utility.triggerMethodsToShow;
            methodsToShow.Add("When Sensors are ready");
            removeUnexistingActuators();
            removeUnexistingSensors();
            addNewActuatorsConfigurations();
            addNewSensorsConfigurations();

        }

        private void removeUnexistingActuators()
        {
            List<string> toDelete = new List<string>();
            bool removed = false;
            foreach (string actuatorName in actuatorsConfigurationNames)
            {
                if (!Utility.actuatorsManager.ExistsConfigurationWithName(actuatorName, myScript))
                {
                    toDelete.Add(actuatorName);
                    toggledActuatorsConfigurations.Remove(actuatorName);
                    removed = true;
                }
            }
            foreach (string current in toDelete)
            {
                actuatorsConfigurationNames.Remove(current);
            }
            if (removed)
            {
                myScript.RemoveNullActuatorConfigurations();
            }
            
        }

        private void removeUnexistingSensors()
        {
            List<string> toDelete = new List<string>();
            bool removed = false;
            foreach (string sensorName in sensorsConfigurationNames)
            {
                if (!Utility.sensorsManager.existsConfigurationWithName(sensorName))
                {
                    toDelete.Add(sensorName);
                    toggledSensorsConfigurations.Remove(sensorName);
                    removed = true;
                }
            }
            if (removed)
            {
                myScript.RemoveNullSensorConfigurations();
                removed = false;
            }
            foreach (string current in toDelete)
            {
                sensorsConfigurationNames.Remove(current);
            }
        }
        

        private void addNewActuatorsConfigurations()
        {
            IEnumerable<string> configurations = Utility.actuatorsManager.AvailableConfigurationNames(myScript);
            foreach (string actuatorConfName in configurations)
            {
                addConfigurationName(actuatorConfName, actuatorsConfigurationNames, toggledActuatorsConfigurations);
            }
        }

        private void addNewSensorsConfigurations()
        {
            foreach (string sensorConfName in Utility.sensorsManager.configurationNames())
            {
                addConfigurationName(sensorConfName, sensorsConfigurationNames, toggledSensorsConfigurations);
            }
        }

        private void addConfigurationName(string configuration, List<string> configurationsNames, Dictionary<string, bool> toggledConfigurations)
        {
            if (!configurationsNames.Contains(configuration))
            {
                configurationsNames.Add(configuration);
            }
            if (!toggledConfigurations.ContainsKey(configuration))
            {
                toggledConfigurations.Add(configuration, false);
            }
        }

        public override void OnInspectorGUI()
        {
            hotReloadCheck();
            if (myScript.sensorsConfigurationsChanged)
            {
                removeUnexistingSensors();
                addNewSensorsConfigurations();
                myScript.sensorsConfigurationsChanged = false;
            }
            if (myScript.actuatorsConfigurationsChanged)
            {
                removeUnexistingActuators();
                addNewActuatorsConfigurations();
                myScript.actuatorsConfigurationsChanged = false;
            }
            excludedProperties = new List<string>();
            excludedProperties.Add("executeReasonerOn");
            //excludedProperties.Add("sensorsConfigurations");
            //excludedProperties.Add("actuatorsConfigurations");
            DrawPropertiesExcluding(serializedObject, excludedProperties.ToArray());
            GUI.enabled = false;
            EditorGUILayout.TextField("Trigger Script Path", Utility.triggerClassPath);
            EditorGUILayout.TextField("ASP Template File Path", myScript.ASPFileTemplatePath);
            GUI.enabled = true;
            if (!myScript.prefabBrain)
            {
                GUI.enabled = false;
                myScript.ASPFilePath = @".\Assets\Resources\" + myScript.gameObject.name + ".asp";
                EditorGUILayout.TextField("ASP File Path", myScript.ASPFilePath);
                GUI.enabled = true;
            }
            else
            {
                configurePrefabASPFile();
            }
            //SENSORS
            EditorGUILayout.LabelField("In scene Sensor Configurations", EditorStyles.boldLabel);
            showConfigurations(sensorsConfigurationNames, toggledSensorsConfigurations);
            //ACTUATORS
            if (!myScript.prefabBrain)
            {
                EditorGUILayout.LabelField("In scene Actuator Configurations", EditorStyles.boldLabel);
            }
            else
            {
                EditorGUILayout.LabelField(myScript.gameObject.name+" available Actuator Configurations", EditorStyles.boldLabel);

            }
            showConfigurations(actuatorsConfigurationNames, toggledActuatorsConfigurations,true);
            //EditorGUILayout.HelpBox("Generating a new file will delete the previouse template!", MessageType.Warning);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Choose when to run the reasoner");
            reasoningExecutionIndex = EditorGUILayout.Popup(reasoningExecutionIndex, methodsToShow.ToArray());
            EditorGUILayout.EndHorizontal();
            myScript.executeReasonerOn = methodsToShow[reasoningExecutionIndex];
            serializedObject.ApplyModifiedProperties();//CHECK SERIALIZATION
            Brain current = (Brain)target;
            if (GUILayout.Button("Generate ASP file template", GUILayout.Width(300)))
            {
                current.GenerateFile();
            }
            EditorGUILayout.HelpBox("Generating a new file will delete the previouse template!", MessageType.Warning);
            savingInBrain();
            if (GUI.changed)
            {
                EditorUtility.SetDirty(myScript);
            }
        }

        private void configurePrefabASPFile()
        {
            string currentTrue = "";
            if (myScript.specificASPFile)
            {
                currentTrue = "Specific";
            }
            if (myScript.globalASPFile)
            {
                currentTrue = "Global";
            }
            EditorGUILayout.HelpBox("You are configuring a Prefab. Please choose if you want \n" +
                "to use a specific ASP program file for each instantiation or a global one!", MessageType.Warning, true);
            EditorGUILayout.BeginHorizontal();
            myScript.specificASPFile = EditorGUILayout.ToggleLeft("Specific file.", myScript.specificASPFile);
            myScript.globalASPFile = EditorGUILayout.ToggleLeft("Global file.", myScript.globalASPFile);
            EditorGUILayout.EndHorizontal();
            exclusiveTogglesCheck(currentTrue);
            if (myScript.specificASPFile)
            {
                EditorGUILayout.HelpBox("At runtime the system will look for the following file \n" +
                    "in which \" nameOfTheInstantiation will be the name of the GameObject instantiated at runtime.", MessageType.Warning,true);
                GUI.enabled = false;
                myScript.ASPFilePath = @".\Assets\Resources\nameOfTheInstantiation.asp";
                EditorGUILayout.TextField("ASP File Path", myScript.ASPFilePath);
                GUI.enabled = true;
                EditorGUILayout.HelpBox("DO NOT use the default GameObject name when instantiating .", MessageType.Warning, true);
            }
            if (myScript.globalASPFile)
            {
                GUI.enabled = false;
                myScript.ASPFilePath = @".\Assets\Resources\" + myScript.gameObject.name + ".asp";
                EditorGUILayout.TextField("ASP File Path", myScript.ASPFilePath);
                GUI.enabled = true;
            }


        }

        private void exclusiveTogglesCheck(string currentTrue)
        {
            if (myScript.specificASPFile && myScript.globalASPFile)
            {
                if (currentTrue.Equals("Specific"))
                {
                    myScript.specificASPFile = false;
                }
                else
                {
                    myScript.globalASPFile = false;
                }
            }
            if (!myScript.specificASPFile && !myScript.globalASPFile)
            {
                myScript.globalASPFile = true;
            }
        }

        private void hotReloadCheck()
        {
            if (toggledSensorsConfigurations == null)
            {
                reset();
            }
        }

        private void showConfigurations(List<string> configurationNames, Dictionary<string, bool> toggledConfigurations, bool isActuator=false)
        {
            foreach (string confName in configurationNames)
            {
                if (isActuator)
                {
                    Brain assignedTo = Utility.actuatorsManager.AssignedTo(confName);
                    if (assignedTo != null && !myScript.chosenActuatorConfigurations.Contains(confName))
                    {
                        GUI.enabled = false;
                    }
                }
                if (toggledConfigurations.ContainsKey(confName))
                {
                    toggledConfigurations[confName] = EditorGUILayout.ToggleLeft(confName, toggledConfigurations[confName]);
                }
                GUI.enabled = true;

            }
        }
        private void savingInBrain()
        {
            saveConfigurations(toggledSensorsConfigurations,myScript.chosenSensorConfigurations);
            saveConfigurations(toggledActuatorsConfigurations,myScript.chosenActuatorConfigurations);
        }

        private void saveConfigurations(Dictionary<string, bool> toggledConfigurations, List<string> chosenConfigurations)
        {
            foreach (string configurationName in toggledConfigurations.Keys)
            {
                if (toggledConfigurations[configurationName])
                {
                    if (!chosenConfigurations.Contains(configurationName))
                    {
                        chosenConfigurations.Add(configurationName);
                    }
                }
                else if (chosenConfigurations.Contains(configurationName))
                {
                    chosenConfigurations.Remove(configurationName);
                }
            }
        }
    }
}
