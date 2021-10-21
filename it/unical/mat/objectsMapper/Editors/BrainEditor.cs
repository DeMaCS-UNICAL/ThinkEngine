using Planner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;

namespace Editors
{
    [CustomEditor(typeof(Brain))]
    public class BrainEditor : Editor
    {
        protected List<string> _excludedProperties;
        protected List<string> _methodsToShow;
        protected List<string> _sensorsConfigurationNames;
        protected Dictionary<string, bool> _toggledSensorsConfigurations;
        Brain brainTarget;
        protected virtual Brain Target
        {
            get
            {
                return brainTarget;
            }
        }

        protected int reasoningExecutionIndex = 0;

        protected List<string> ExcludedProperties
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
        protected List<string> MethodsToShow
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
        protected List<string> SensorsConfigurationNames
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
        protected Dictionary<string, bool> ToggledSensorsConfigurations
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
        protected virtual void OnEnable()
        {
            brainTarget = (Brain)target;
            Utility.LoadPrefabs();
            brainTarget.prefabBrain = PrefabStageUtility.GetCurrentPrefabStage() != null;
            MyReset();
        }
        void MyReset()
        {
            BasicConfiguration();
            ReadingFromBrain();
        }
        protected virtual void ReadingFromBrain()
        {
            reasoningExecutionIndex = Utility.GetTriggerMethodIndex(brainTarget.ExecuteReasonerOn);
            bool delete = false;
            foreach (string sensorConfName in brainTarget.ChosenSensorConfigurations)
            {
                if (!Utility.SensorsManager.ExistsConfigurationWithName(sensorConfName))
                {
                    delete = true;
                    continue;
                }
                ToggledSensorsConfigurations[sensorConfName] = true;
            }
            if (delete)
            {
                brainTarget.RemoveNullSensorConfigurations();
            }
        }
        private void RemoveUnexistingSensors()
        {
            List<string> toDelete = new List<string>();
            bool removed = false;
            foreach (string sensorName in SensorsConfigurationNames)
            {
                if (!Utility.SensorsManager.ExistsConfigurationWithName(sensorName))
                {
                    toDelete.Add(sensorName);
                    ToggledSensorsConfigurations.Remove(sensorName);
                    removed = true;
                }
            }
            if (removed)
            {
                (brainTarget).RemoveNullSensorConfigurations();
            }
            foreach (string current in toDelete)
            {
                SensorsConfigurationNames.Remove(current);
            }
        }
        private void AddNewSensorsConfigurations()
        {
            foreach (string sensorConfName in Utility.SensorsManager.ConfigurationNames())
            {
                AddConfigurationName(sensorConfName, SensorsConfigurationNames, ToggledSensorsConfigurations);
            }
        }

        protected void AddConfigurationName(string configuration, List<string> configurationsNames, Dictionary<string, bool> toggledConfigurations)
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
        protected virtual void BasicConfiguration()
        {
            MethodsToShow = Utility.TriggerMethodsToShow;
            MethodsToShow.Add("When Sensors are ready");
            AddNewSensorsConfigurations();
            RemoveUnexistingSensors();
        }
        protected void GenerateASPTemplateFileButton()
        {
            if (GUILayout.Button("Generate ASP file template", GUILayout.Width(300)))
            {
                brainTarget.GenerateFile();
            }
            EditorGUILayout.HelpBox("Generating a new file will delete the previouse template!", MessageType.Warning);
        }
        protected void ChooseReasonerTriggerMethod()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Choose when to run the reasoner");
            reasoningExecutionIndex = EditorGUILayout.Popup(reasoningExecutionIndex, MethodsToShow.ToArray());
            EditorGUILayout.EndHorizontal();
            brainTarget.ExecuteReasonerOn = MethodsToShow[reasoningExecutionIndex];
        }
        protected virtual void ListAvailableConfigurations()
        {
            EditorGUILayout.LabelField("In scene Sensor Configurations", EditorStyles.boldLabel);
            ShowConfigurations(SensorsConfigurationNames, ToggledSensorsConfigurations);
        }
        protected void ShowConfigurations(List<string> configurationNames, Dictionary<string, bool> toggledConfigurations, bool checkIfDisableGUI = false)
        {
            foreach (string confName in configurationNames)
            {
                if (checkIfDisableGUI)
                {
                    CheckIfDisableGUI(confName);
                }
                if (toggledConfigurations.ContainsKey(confName))
                {
                    toggledConfigurations[confName] = EditorGUILayout.ToggleLeft(confName, toggledConfigurations[confName]);
                }
                GUI.enabled = true;
            }
        }

        protected virtual void SavingInBrain()
        {
            SaveConfigurations(ToggledSensorsConfigurations, brainTarget.ChosenSensorConfigurations);
        }

        protected void SaveConfigurations(Dictionary<string, bool> toggledConfigurations, List<string> chosenConfigurations)
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
        private void ExclusiveTogglesCheck(string currentTrue)
        {
            if (brainTarget.specificASPFile && brainTarget.globalASPFile)
            {
                if (currentTrue.Equals("Specific"))
                {
                    brainTarget.specificASPFile = false;
                }
                else
                {
                    brainTarget.globalASPFile = false;
                }
            }
            if (!brainTarget.specificASPFile && !brainTarget.globalASPFile)
            {
                brainTarget.globalASPFile = true;
            }
        }
        private void ConfigurePrefabASPFile()
        {
            string currentTrue = "";
            if (brainTarget.specificASPFile)
            {
                currentTrue = "Specific";
            }
            if (brainTarget.globalASPFile)
            {
                currentTrue = "Global";
            }
            EditorGUILayout.HelpBox("You are configuring a Prefab. Please choose if you want \n" +
                "to use a specific ASP program file for each instantiation or a global one!", MessageType.Warning, true);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            brainTarget.specificASPFile = EditorGUILayout.ToggleLeft("Specific file.", brainTarget.specificASPFile);
            brainTarget.globalASPFile = EditorGUILayout.ToggleLeft("Global file.", brainTarget.globalASPFile);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            ExclusiveTogglesCheck(currentTrue);
            if (brainTarget.specificASPFile)
            {
                EditorGUILayout.HelpBox("At runtime the system will look for the following files pattern \n" +
                    "in which \" nameOfTheInstantiation will be the name of the GameObject instantiated at runtime.", MessageType.Warning, true);
                GUI.enabled = false;
                brainTarget.ASPFilesPrefix = @"nameOfTheInstantiation"+GetASPFilesPrefixSpecifications();
                EditorGUILayout.TextField("ASP Files Pattern", brainTarget.ASPFilesPrefix + "*.asp");
                GUI.enabled = true;
                EditorGUILayout.HelpBox("DO NOT use the default GameObject name when instantiating.", MessageType.Warning, true);
            }
            if (brainTarget.globalASPFile)
            {
                GUI.enabled = false;
                brainTarget.ASPFilesPrefix = brainTarget.gameObject.name;
                EditorGUILayout.TextField("ASP File Path", brainTarget.ASPFilesPrefix);
                GUI.enabled = true;
            }


        }
        private void ShowNotEditableInformations()
        {
            GUI.enabled = false;
            EditorGUILayout.TextField("Trigger Script Path", Utility.TriggerClassPath);
            EditorGUILayout.TextField("ASP Files Path", brainTarget.ASPFilesPath);
            brainTarget.ASPFilesPath = @".\Assets\StreamingAssets\";
            GUI.enabled = true;
            if (!brainTarget.prefabBrain)
            {
                GUI.enabled = false;
                brainTarget.ASPFilesPrefix = Target.gameObject.name+GetASPFilesPrefixSpecifications();
                EditorGUILayout.TextField("ASP Files Pattern", brainTarget.ASPFilesPrefix + "*.asp");
                GUI.enabled = true;
            }
            else
            {
                ConfigurePrefabASPFile();
            }
            GUI.enabled = false;
            EditorGUILayout.TextField("ASP Template File Path", brainTarget.ASPFileTemplatePath);
            GUI.enabled = true;
        }

        protected virtual string GetASPFilesPrefixSpecifications() 
        {
            return "";
        }

        public override void OnInspectorGUI()
        {
            IfConfigurationChanged();
            DrawPropertiesExcluding(serializedObject, ExcludedProperties.ToArray());
            ShowSpecificFields();
            ShowNotEditableInformations();
            ListAvailableConfigurations();
            ChooseReasonerTriggerMethod();
            serializedObject.ApplyModifiedProperties();
            GenerateASPTemplateFileButton();
            SavingInBrain();
            if (GUI.changed)
            {
                EditorUtility.SetDirty(Target);
            }
        }

       

        protected virtual void IfConfigurationChanged()
        {
            if (brainTarget.sensorsConfigurationsChanged)
            {
                RemoveUnexistingSensors();
                AddNewSensorsConfigurations();
                brainTarget.sensorsConfigurationsChanged = false;
            }

        }

        protected virtual void CheckIfDisableGUI(string confName){}
        protected virtual void ShowSpecificFields() { }
    }
}
