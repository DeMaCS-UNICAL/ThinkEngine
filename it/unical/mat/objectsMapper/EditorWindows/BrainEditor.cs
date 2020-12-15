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
        List<string> _excludedProperties;
        List<string> _methodsToShow;
        List<string> _sensorsConfigurationNames;
        List<string> _actuatorsConfigurationNames;
        Dictionary<string, bool> _toggledSensorsConfigurations;
        Dictionary<string, bool> _toggledActuatorsConfigurations;
        private int reasoningExecutionIndex = 0;
        private Brain myScript;
        List<string> excludedProperties
        {
            get
            {
                if (_excludedProperties == null)
                {
                    _excludedProperties = new List<string>();
                }
                if (!_excludedProperties.Contains("executeReasonerOn"))
                {
                    _excludedProperties.Add("executeReasonerOn");
                }
                return _excludedProperties;
            }
        }
        List<string> methodsToShow
        {
            get
            {
                if (_methodsToShow == null)
                {
                    _methodsToShow = new List<string>();
                }
                return _methodsToShow;
            }
            set
            {
                _methodsToShow = value;
            }
        }
        List<string> sensorsConfigurationNames
        {
            get
            {
                if (_sensorsConfigurationNames == null)
                {
                    _sensorsConfigurationNames = new List<string>();
                }
                return _sensorsConfigurationNames;
            }
        }
        List<string> actuatorsConfigurationNames
        {
            get
            {
                if (_actuatorsConfigurationNames == null)
                {
                    _actuatorsConfigurationNames = new List<string>();
                }
                return _actuatorsConfigurationNames;
            }
        }
        Dictionary<string, bool> toggledSensorsConfigurations
        {
            get
            {
                if (_toggledSensorsConfigurations == null)
                {
                    _toggledSensorsConfigurations = new Dictionary<string, bool>();
                }
                return _toggledSensorsConfigurations;
            }
        }
        Dictionary<string, bool> toggledActuatorsConfigurations
        {
            get
            {
                if (_toggledActuatorsConfigurations == null)
                {
                    _toggledActuatorsConfigurations = new Dictionary<string, bool>();
                }
                return _toggledActuatorsConfigurations;
            }
        }


        void OnEnable()
        {
            Utility.loadPrefabs();
            MyReset();
            myScript.prefabBrain = PrefabStageUtility.GetCurrentPrefabStage() != null;
        }
        void MyReset()
        {
            myScript = target as Brain;
            BasicConfiguration();
            ReadingFromBrain();
        }
        public override void OnInspectorGUI()
        {
            if (myScript.sensorsConfigurationsChanged)
            {
                RemoveUnexistingSensors();
                AddNewSensorsConfigurations();
                myScript.sensorsConfigurationsChanged = false;
            }
            if (myScript.actuatorsConfigurationsChanged)
            {
                RemoveUnexistingActuators();
                AddNewActuatorsConfigurations();
                myScript.actuatorsConfigurationsChanged = false;
            }
            DrawPropertiesExcluding(serializedObject, excludedProperties.ToArray());
            ShowNotEditableInformations();
            ListAvailableConfigurations();
            ChooseReasonerTriggerMethod();
            serializedObject.ApplyModifiedProperties();
            GenerateASPTemplateFileButton();
            SavingInBrain();
            if (GUI.changed)
            {
                EditorUtility.SetDirty(myScript);
            }
        }
        private void ReadingFromBrain()
        {
            reasoningExecutionIndex = Utility.getTriggerMethodIndex(myScript.executeReasonerOn);
            bool delete = false;
            foreach (string sensorConfName in myScript.chosenSensorConfigurations)
            {
                if (!Utility.sensorsManager.ExistsConfigurationWithName(sensorConfName))
                {
                    delete = true;
                    continue;
                }
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
                toggledActuatorsConfigurations[actuatorConfName] = true;
            }
            if (delete)
            {
                myScript.RemoveNullActuatorConfigurations();
            }
        }
        private void BasicConfiguration()
        {
            methodsToShow = Utility.triggerMethodsToShow;
            methodsToShow.Add("When Sensors are ready");
            RemoveUnexistingActuators();
            RemoveUnexistingSensors();
            AddNewActuatorsConfigurations();
            AddNewSensorsConfigurations();
        }
        private void RemoveUnexistingActuators()
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
        private void RemoveUnexistingSensors()
        {
            List<string> toDelete = new List<string>();
            bool removed = false;
            foreach (string sensorName in sensorsConfigurationNames)
            {
                if (!Utility.sensorsManager.ExistsConfigurationWithName(sensorName))
                {
                    toDelete.Add(sensorName);
                    toggledSensorsConfigurations.Remove(sensorName);
                    removed = true;
                }
            }
            if (removed)
            {
                myScript.RemoveNullSensorConfigurations();
            }
            foreach (string current in toDelete)
            {
                sensorsConfigurationNames.Remove(current);
            }
        }
        private void AddNewActuatorsConfigurations()
        {
            foreach (string actuatorConfName in Utility.actuatorsManager.AvailableConfigurationNames(myScript))
            {
                AddConfigurationName(actuatorConfName, actuatorsConfigurationNames, toggledActuatorsConfigurations);
            }
        }
        private void AddNewSensorsConfigurations()
        {
            foreach (string sensorConfName in Utility.sensorsManager.ConfigurationNames())
            {
                AddConfigurationName(sensorConfName, sensorsConfigurationNames, toggledSensorsConfigurations);
            }
        }
        private void AddConfigurationName(string configuration, List<string> configurationsNames, Dictionary<string, bool> toggledConfigurations)
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
        
        private void GenerateASPTemplateFileButton()
        {
            Brain brain = (Brain)target;
            if (GUILayout.Button("Generate ASP file template", GUILayout.Width(300)))
            {
                brain.GenerateFile();
            }
            EditorGUILayout.HelpBox("Generating a new file will delete the previouse template!", MessageType.Warning);
        }
        private void ChooseReasonerTriggerMethod()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Choose when to run the reasoner");
            reasoningExecutionIndex = EditorGUILayout.Popup(reasoningExecutionIndex, methodsToShow.ToArray());
            EditorGUILayout.EndHorizontal();
            myScript.executeReasonerOn = methodsToShow[reasoningExecutionIndex];
        }
        private void ListAvailableConfigurations()
        {
            //SENSORS
            EditorGUILayout.LabelField("In scene Sensor Configurations", EditorStyles.boldLabel);
            ShowConfigurations(sensorsConfigurationNames, toggledSensorsConfigurations);
            //ACTUATORS
            if (!myScript.prefabBrain)
            {
                EditorGUILayout.LabelField("All the available Actuator Configurations", EditorStyles.boldLabel);
            }
            else
            {
                EditorGUILayout.LabelField(myScript.gameObject.name + " available Actuator Configurations", EditorStyles.boldLabel);
            }
            ShowConfigurations(actuatorsConfigurationNames, toggledActuatorsConfigurations, true);
        }
        private void ShowNotEditableInformations()
        {
            GUI.enabled = false;
            EditorGUILayout.TextField("Trigger Script Path", Utility.triggerClassPath);
            EditorGUILayout.TextField("ASP Template File Path", myScript.ASPFileTemplatePath);
            EditorGUILayout.TextField("ASP Files Path", myScript.ASPFilesPath);
            myScript.ASPFilesPath = @".\Assets\Resources\";
            GUI.enabled = true;
            if (!myScript.prefabBrain)
            {
                GUI.enabled = false;
                myScript.ASPFilesPrefix = myScript.gameObject.name;
                EditorGUILayout.TextField("ASP Files Pattern", myScript.ASPFilesPrefix+"*.asp");
                GUI.enabled = true;
            }
            else
            {
                ConfigurePrefabASPFile();
            }
        }
        private void ConfigurePrefabASPFile()
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
            GUILayout.FlexibleSpace();
            myScript.specificASPFile = EditorGUILayout.ToggleLeft("Specific file.", myScript.specificASPFile);
            myScript.globalASPFile = EditorGUILayout.ToggleLeft("Global file.", myScript.globalASPFile);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            ExclusiveTogglesCheck(currentTrue);
            if (myScript.specificASPFile)
            {
                EditorGUILayout.HelpBox("At runtime the system will look for the following files pattern \n" +
                    "in which \" nameOfTheInstantiation will be the name of the GameObject instantiated at runtime.", MessageType.Warning,true);
                GUI.enabled = false;
                myScript.ASPFilesPrefix = @"nameOfTheInstantiation";
                EditorGUILayout.TextField("ASP Files Pattern", myScript.ASPFilesPrefix + "*.asp");
                GUI.enabled = true;
                EditorGUILayout.HelpBox("DO NOT use the default GameObject name when instantiating.", MessageType.Warning, true);
            }
            if (myScript.globalASPFile)
            {
                GUI.enabled = false;
                myScript.ASPFilesPrefix = myScript.gameObject.name;
                EditorGUILayout.TextField("ASP File Path", myScript.ASPFilesPrefix);
                GUI.enabled = true;
            }


        }
        private void ExclusiveTogglesCheck(string currentTrue)
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
        private void ShowConfigurations(List<string> configurationNames, Dictionary<string, bool> toggledConfigurations, bool isActuator=false)
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
        private void SavingInBrain()
        {
            SaveConfigurations(toggledSensorsConfigurations,myScript.chosenSensorConfigurations);
            SaveConfigurations(toggledActuatorsConfigurations,myScript.chosenActuatorConfigurations);
        }
        private void SaveConfigurations(Dictionary<string, bool> toggledConfigurations, List<string> chosenConfigurations)
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
