﻿#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ThinkEngine.Editors
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
        protected void GenerateAITemplateFileButton()
        {
            string paradigm = brainTarget.FileExtension.Equals("") ? "ASP-like" : brainTarget.FileExtension.ToUpper();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate "+paradigm+" file template", GUILayout.Width(200)))
            {
                brainTarget.GenerateFile();
            }
            if (GUILayout.Button("Show in explorer"))
            {
                EditorUtility.OpenWithDefaultApp(Path.Combine(".", "Assets", "Resources"));
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.HelpBox("Generating a new file will delete the previous template!", MessageType.Warning);
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
            if (brainTarget.specificAIFile && brainTarget.globalAIFile)
            {
                if (currentTrue.Equals("Specific"))
                {
                    brainTarget.specificAIFile = false;
                }
                else
                {
                    brainTarget.globalAIFile = false;
                }
            }
            if (!brainTarget.specificAIFile && !brainTarget.globalAIFile)
            {
                brainTarget.globalAIFile = true;
            }
        }
        private void ConfigurePrefabAIFile()
        {
            string currentTrue = "";
            if (brainTarget.specificAIFile)
            {
                currentTrue = "Specific";
            }
            if (brainTarget.globalAIFile)
            {
                currentTrue = "Global";
            }
            EditorGUILayout.HelpBox("You are configuring a Prefab. Please choose if you want \n" +
                "to use a specific ASP program file for each instantiation or a global one!", MessageType.Warning, true);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            brainTarget.specificAIFile = EditorGUILayout.ToggleLeft("Specific file.", brainTarget.specificAIFile);
            brainTarget.globalAIFile = EditorGUILayout.ToggleLeft("Global file.", brainTarget.globalAIFile);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            ExclusiveTogglesCheck(currentTrue);
            if (brainTarget.specificAIFile)
            {
                EditorGUILayout.HelpBox("At runtime the system will look for the following files pattern \n" +
                    "in which \" nameOfTheInstantiation will be the name of the GameObject instantiated at runtime.", MessageType.Warning, true);
                GUI.enabled = false;
                brainTarget.AIFilesPrefix = @"nameOfTheInstantiation"+GetAIFilesPrefixSpecifications();
                EditorGUILayout.TextField("ASP Files Pattern", brainTarget.AIFilesPrefix + "*.asp");
                GUI.enabled = true;
                EditorGUILayout.HelpBox("DO NOT use the default GameObject name when instantiating.", MessageType.Warning, true);
            }
            if (brainTarget.globalAIFile)
            {
                GUI.enabled = false;
                brainTarget.AIFilesPrefix = brainTarget.gameObject.name;
                EditorGUILayout.TextField("AI File Path", brainTarget.AIFilesPrefix);
                GUI.enabled = true;
            }


        }
        private void ShowNotEditableInformations()
        {
            GUI.enabled = false;
            EditorGUILayout.TextField("Trigger Script Path", Utility.TriggerClassPath);
            EditorGUILayout.TextField("AI Files Path", brainTarget.AIFilesPath);
            GUI.enabled = true;
            if (!brainTarget.prefabBrain)
            {
                GUI.enabled = false;
                brainTarget.AIFilesPrefix = Target.gameObject.name+GetAIFilesPrefixSpecifications();
                EditorGUILayout.TextField("AI Files Pattern", brainTarget.AIFilesPrefix + "*.asp");//(asp | pddl | dlp)");
                GUI.enabled = true;
            }
            else
            {
                ConfigurePrefabAIFile();
            }
            GUI.enabled = false;
            EditorGUILayout.TextField("AI Template File Path", brainTarget.AIFileTemplatePath);
            GUI.enabled = true;
        }

        protected virtual string GetAIFilesPrefixSpecifications() 
        {
            return "";
        }

        public override void OnInspectorGUI()
        {
            if (!CheckSolver())
            {
                EditorGUILayout.HelpBox("Please, download the Dlv2 solver suitable for your system. See documentation at \"How to add ThinkEngine to an existing project\".", MessageType.Error, true);
                return;
            }
            IfConfigurationChanged();
            DrawPropertiesExcluding(serializedObject, ExcludedProperties.ToArray());
            EditorGUILayout.BeginHorizontal();
            Target.maintainInputFile = EditorGUILayout.Toggle("Maintain input file", Target.maintainInputFile);
            if (GUILayout.Button("Open input folder"))
            {
                EditorUtility.RevealInFinder(Path.Combine(Path.GetTempPath(),"ThinkEngineFacts"+Utility.slash));
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
            ShowSpecificFields();
            ShowNotEditableInformations();
            ListAvailableConfigurations();
            ChooseReasonerTriggerMethod();
            serializedObject.ApplyModifiedProperties();
            GenerateAITemplateFileButton();
            SavingInBrain();
            if (GUI.changed)
            {
                EditorUtility.SetDirty(Target);
            }
        }

        private bool CheckSolver()
        {
            string[] libContent = Directory.GetFiles(Path.Combine(Application.streamingAssetsPath, "ThinkEngine", "lib"));
            if (libContent.Length == 0)
            {
                return false;
            }
            foreach(string filename in libContent)
            {
                string actualFileName = filename.Substring(filename.LastIndexOf(Utility.slash) + 1);
                if (actualFileName.StartsWith("dlv"))
                {
                    return true;
                }
            }
            return false;
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
#endif