#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace ThinkEngine.it.unical.mat.objectsMapper.EditorWindows
{
    [CustomEditor(typeof(AbstractConfiguration))]
    public abstract class AbstractConfigurationEditor : Editor
    {
        static int cont = 0;
        protected GameObjectsTracker tracker;
        
        [SerializeField]
        protected bool showProperties = false;
        [SerializeField]
        protected bool disableProperties;
        [SerializeField]
        protected string typeOfConfiguration;
        protected Vector2 mainScroll;
        protected Vector2 helpScroll;
        protected Vector2 configurationScroll;
        protected bool objectMode;
        protected FieldOrProperty objectToConfigure;

        private AbstractConfiguration configuration;
#region Unity Messages
        protected void Reset()
        {
            configuration = target as AbstractConfiguration;
            MyReset();
        }
        override public void OnInspectorGUI()
        {
            if (Application.isPlaying)
            {
                DrawDefaultInspector();
                return;
            }
            try
            {
                if (tracker is null)
                {
                    MyReset();
                }
                GUILayout.Label(typeOfConfiguration + " Configuration", EditorStyles.boldLabel);
                if (tracker.configurationName.Equals(""))
                {
                    ChooseNewConfigurationName();
                }
                else
                {
                    Configure();
                }
                if (GUI.changed)
                {
                    EditorUtility.SetDirty(configuration);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                MyReset(true);
            }
        }
#endregion
        protected void MyReset(bool fault = false)
        {
            tracker = new GameObjectsTracker();
            tracker.gameObject = configuration.gameObject;
            if (fault)
            {
                objectMode = false;
            }
            else
            {
                if (!configuration.configurationName.Equals(""))
                {
                    tracker.UpdateDataStructures(configuration);
                }
                if (objectMode && objectToConfigure == null)
                {
                    objectMode = false;
                }
            }
        }
        private void Configure()
        {
            showProperties = EditorGUILayout.Foldout(showProperties, "Show game object properties");
            bool scrollView = false;
            if (showProperties)
            {
                scrollView = true;
                mainScroll = EditorGUILayout.BeginScrollView(mainScroll, GUILayout.Height(400));
            }
            EditorGUI.indentLevel++;
            if (showProperties)
            {
                AddProperties();
            }
            EditorGUI.indentLevel--;
            AddSavingCleaningButtons(scrollView);

        }
        private void AddSavingCleaningButtons(bool scrollView)
        {
            string buttonContent;
            bool disabled = false;
            if (!tracker.configurationName.Equals(configuration.configurationName) && ExistsConfigurationWithName(tracker.configurationName))
            {
                buttonContent = "Name used for another sensor";
                disabled = true;
            }
            else if (configuration.saved)
            {
                buttonContent = "Override Configuration";
            }
            else
            {
                buttonContent = "Save Configuration";
            }
            EditorGUIUtility.labelWidth = 100;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();//flexible spaces surround the name of the configuration and the two buttons. The latter are on the same row, vertically aligned with the name.
            GUILayout.BeginVertical();
            tracker.configurationName = EditorGUILayout.TextField(typeOfConfiguration + " name: ", tracker.configurationName, GUILayout.MinWidth(200));
            EditorGUILayout.BeginHorizontal();
            GUI.enabled = !disabled;
            bool save = GUILayout.Button(buttonContent);
            GUI.enabled = true;
            bool clean = GUILayout.Button("Clean");
            EditorGUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            if (scrollView)
            {
                EditorGUILayout.EndScrollView();
            }
            if (clean)
            {
                configuration.Clean();
            }
            if (save)
            {
                configuration.SaveConfiguration(tracker);
                MyReset(false);
            }
        }
        internal virtual bool ExistsConfigurationWithName(string configurationName) { return false;}
        private void ChooseNewConfigurationName()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Choose a name for the new configuration.", EditorStyles.boldLabel);
            configuration.configurationName = GUILayout.TextField(configuration.configurationName);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (!ExistsConfigurationWithName(configuration.configurationName))
            {
                if (GUILayout.Button("Ok"))
                {
                    tracker.configurationName = configuration.configurationName;
                    tracker.UpdateDataStructures(configuration);
                }
            }
            else
            {
                GUI.enabled = false;
                GUILayout.Button("Name already used.");
                GUI.enabled = true;
            }
            if (GUILayout.Button("Clean"))
            {
                configuration.configurationName = "";
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        private void AddProperties()
        {
            using (var h = new EditorGUILayout.VerticalScope())
            {
                GameObject gameObject = tracker.gameObject;
                foreach (FieldOrProperty currentGOProperty in tracker.objectsProperties[gameObject].Values)
                {
                    bool disabled = tracker.objectDerivedFromFields[gameObject][currentGOProperty.Name()] == null;
                    EditorGUI.BeginDisabledGroup(disabled);
                    if (IsMappable(currentGOProperty))
                    {
                        AddMappableProperty(currentGOProperty);
                    }
                    else
                    {
                        AddExpandableProperty(gameObject, currentGOProperty, disabled);

                    }
                    EditorGUI.EndDisabledGroup();
                }
                foreach (Component currentComponent in tracker.gameObjectsComponents[gameObject])
                {
                    if (currentComponent != null)
                    {
                        AddComponent(gameObject, currentComponent);
                    }
                }
            }
        }
        private void AddComponent(object gameObject, Component currentComponent)
        {
            EditorGUI.indentLevel++;//need to add an indent level because foldout are misplaced wrt toggle
            tracker.objectsToggled[currentComponent] = EditorGUILayout.Foldout(tracker.objectsToggled[currentComponent], currentComponent.GetType().ToString());
            EditorGUI.indentLevel--;
            if (tracker.objectsToggled[currentComponent])
            {
                EditorGUI.indentLevel++;
                AddSubProperties(currentComponent, currentComponent.GetType().ToString(), gameObject, currentComponent);
                EditorGUI.indentLevel--;
            }
        }
        private void AddExpandableProperty(object currentObject, FieldOrProperty currentProperty, bool disabled)
        {
            EditorGUI.indentLevel++;//need to add an indent level because foldout are misplaced wrt toggle
            tracker.objectsToggled[currentProperty] = EditorGUILayout.Foldout(tracker.objectsToggled[currentProperty], currentProperty.Name()) && !disabled;
            EditorGUI.indentLevel--;
            if (tracker.objectsToggled[currentProperty])
            {
                EditorGUI.indentLevel++;
                if (currentObject is FieldOrProperty || currentObject.GetType().IsValueType)
                {
                    AddValueTypeSubProperties(currentProperty);
                }
                else
                {
                    AddSubProperties(tracker.objectDerivedFromFields[currentObject][currentProperty.Name()], currentProperty.Name(), currentObject, currentProperty);
                }
                EditorGUI.indentLevel--;
            }
        }
        private void AddMappableProperty(FieldOrProperty currentProperty)
        {
            EditorGUILayout.BeginHorizontal();
            tracker.objectsToggled[currentProperty] = EditorGUILayout.ToggleLeft(currentProperty.Name(), tracker.objectsToggled[currentProperty]);
            AddCustomFields(currentProperty);
            if (!tracker.IsBaseType(currentProperty))
            {
                if (tracker.objectsToggled[currentProperty])
                {
                    if (tracker.HasBasicGenericArgument(currentProperty))
                    {
                        AddBasicComplexDataStructure(currentProperty);
                    }
                    else
                    {
                        ConfigureNonBasicComplexDataStructure(currentProperty);
                    }
                }
                else
                {
                    if (tracker.HasBasicGenericArgument(currentProperty))
                    {
                        tracker.RemoveSimpleTracker(currentProperty);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void AddBasicComplexDataStructure(FieldOrProperty currentProperty)
        {
            tracker.TogglePropertySimpleTracker(currentProperty,"",true);
            tracker.SaveSimpleTracker(currentProperty);
        }

        private void ConfigureNonBasicComplexDataStructure(FieldOrProperty currentProperty)
        {
            bool configure = GUILayout.Button("Configure Object");
            if (configure)
            {
                objectMode = true;
                objectToConfigure = currentProperty;
                helpScroll = new Vector2(0, 0);
                tracker.LoadBasicPropertiesSimpleTracker(currentProperty);
                DrawObjectProperties();
            }
        }

        protected virtual bool IsMappable(FieldOrProperty currentProperty)
        {
            return tracker.IsMappable(currentProperty);//for actuators a property is mappable iff it's a basic type
        }
        public void AddPropertyName(object currentObject)
        {
            if (!tracker.propertiesName.ContainsKey(currentObject))
            {
                tracker.propertiesName.Add(currentObject, "Choose a name");
            }
            tracker.propertiesName[currentObject] = EditorGUILayout.TextField(tracker.propertiesName[currentObject]);
        }
        protected void AddSubProperties(object currentObject, string currentPropertyName, object objectOwner,object currentPropertyOrComponent)
        {
            bool objectOwnerAvailable = tracker.objectsOwners.ContainsKey(currentObject);
            bool objectOwnerMatches = objectOwnerAvailable && (tracker.objectsOwners[currentObject].Key.Equals(objectOwner) && tracker.objectsOwners[currentObject].Value.Equals(currentPropertyName));
            if (objectOwnerAvailable && !objectOwnerMatches || currentObject.Equals(tracker.gameObject))
            {
                AddNotExpandableProperty(currentObject, currentPropertyName);
                return;
            }
            if (!tracker.objectsToggled.ContainsKey(currentPropertyOrComponent))
            {
                tracker.objectsToggled.Add(currentPropertyOrComponent, false);
            }
            if (!tracker.objectsProperties.ContainsKey(currentObject))
            {
                tracker.UpdateDataStructures(currentObject, null, new MyListString());
            }
            Expand(currentObject);
        }
        protected void AddValueTypeSubProperties(FieldOrProperty currentProperty)
        {
            if (!tracker.objectsToggled.ContainsKey(currentProperty))
            {
                tracker.objectsToggled.Add(currentProperty, false);
            }
            if (!tracker.objectsProperties.ContainsKey(currentProperty))
            {
                tracker.UpdateValueTypeDataStructures(currentProperty, null, new MyListString());//if it is a value type, currentObject can match with tons of others objects
            }
            Expand(currentProperty);
        }

        private void Expand(object currentObjectOrProperty)
        {
            foreach (FieldOrProperty currentSubProperty in tracker.objectsProperties[currentObjectOrProperty].Values)
            {
                bool disabled = tracker.objectDerivedFromFields[currentObjectOrProperty][currentSubProperty.Name()] == null;
                EditorGUI.BeginDisabledGroup(disabled);
                if (IsMappable(currentSubProperty))
                {
                    AddMappableProperty(currentSubProperty);
                }
                else
                {
                    AddExpandableProperty(currentObjectOrProperty, currentSubProperty, disabled);
                }
                EditorGUI.EndDisabledGroup();
            }
        }

        private void AddNotExpandableProperty(object currentObject, string currentPropertyName)
        {
            EditorGUI.BeginDisabledGroup(true);
            if (currentObject.Equals(tracker.gameObject))
            {
                EditorGUILayout.ToggleLeft("This is " + configuration.gameObject + " object", true);
            }
            else if (!tracker.objectsOwners[currentObject].Value.Equals(currentPropertyName))
            {
                EditorGUILayout.ToggleLeft("object already listed as " + tracker.objectsOwners[currentObject].Value, true);
            }
            else
            {
                EditorGUILayout.ToggleLeft("object already listed in an upper level", true);
            }
            EditorGUI.EndDisabledGroup();
        }
        protected void DrawObjectProperties()
        {
            GUILayout.Label("Configure " + tracker.GetSimpleTracker(objectToConfigure).objType + " object for " + objectToConfigure.Name() + " property ", EditorStyles.boldLabel);
            List<string> propertiesNames = new List<string>();
            foreach (string currentProperty in tracker.GetSimpleTracker(objectToConfigure).propertiesToggled.Keys)
            {
                propertiesNames.Add(currentProperty);
            }
            using (var h = new EditorGUILayout.VerticalScope())
            {
                using (var scrollView = new EditorGUILayout.ScrollViewScope(helpScroll))
                {
                    foreach (string currentPropertyName in propertiesNames)
                    {
                        tracker.TogglePropertySimpleTracker(objectToConfigure,currentPropertyName, EditorGUILayout.ToggleLeft(currentPropertyName, tracker.IsPropertySimpleTrackerToggled(objectToConfigure, currentPropertyName)));
                    }
                    helpScroll = scrollView.scrollPosition;
                }
            }
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            bool cancel = GUILayout.Button("Cancel");
            bool save = GUILayout.Button("Save");
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            if (cancel)
            {
                if (tracker.SimpleTrackerToSaveCount(objectToConfigure) == 0)
                {
                    tracker.RemoveSimpleTracker(objectToConfigure);
                    tracker.objectsToggled[objectToConfigure] = false;
                }
                objectMode = false;
            }
            if (save)
            {
                tracker.SaveSimpleTracker(objectToConfigure);
                if (tracker.SimpleTrackerToSaveCount(objectToConfigure) == 0)
                {
                    tracker.RemoveSimpleTracker(objectToConfigure);
                    tracker.objectsToggled[objectToConfigure] = false;
                }
                objectMode = false;
            }
        }
        internal virtual void AddCustomFields(FieldOrProperty obj) { }
    }
}
#endif