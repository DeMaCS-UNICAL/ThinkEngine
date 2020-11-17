using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Threading;


namespace EmbASP4Unity.it.unical.mat.objectsMapper.EditorWindows
{
    [CustomEditor(typeof(AbstractConfiguration))]
    public abstract class AbstractConfigurationEditor : Editor
    {
        protected GameObjectsTracker tracker;
        
        [SerializeField]
        protected bool showProperties = false;
        [SerializeField]
        protected bool disableProperties;
        protected string typeOfConfigurator;

        //mode

        protected Vector2 mainScroll;
        protected Vector2 helpScroll;
        protected Vector2 configurationScroll;
        protected bool objectMode;
        protected FieldOrProperty objectToConfigure;
        private AbstractConfiguration configuration;



        protected void Reset()
        {
            configuration = target as AbstractConfiguration;
            reset();
        }

        protected void reset(bool fault = false)
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
                if (objectMode && objectToConfigure is null)
                {
                    objectMode = false;
                }
            }
        }

        override public void OnInspectorGUI()
        {
            
            if (Application.isPlaying)
            {
                DrawDefaultInspector();
                return;
            }
                //MyDebugger.MyDebug("manager "+manager);
            try
            {
                if (tracker is null)
                {
                    reset();
                }
                GUILayout.Label(typeOfConfigurator + " Configuration", EditorStyles.boldLabel);

                if (tracker.configurationName.Equals(""))
                {
                    chooseNewConfigurationName();
                }
                else
                {
                    configure();
                }
                if (GUI.changed)
                {
                    EditorUtility.SetDirty(configuration);
                }
            }
            catch(Exception e)
            {
                Debug.LogError(e);
                reset(true);
            }
        }

        private void configure()
        {   
            showProperties = EditorGUILayout.Foldout(showProperties, "Show properties");
            bool scrollView = false;
            if (showProperties)
            {
                scrollView = true;
                mainScroll = EditorGUILayout.BeginScrollView(mainScroll, GUILayout.Height(400));
            }
            EditorGUI.indentLevel++;

            if (showProperties)
            {
                addProperties();
            }

            EditorGUI.indentLevel--;
            string buttonContent;
            bool disabled=false;
            if (!tracker.configurationName.Equals(configuration.configurationName) && existsConfigurationWithName(tracker.configurationName))
            {
                buttonContent = "Name used for another sensor";
                disabled = true;
            }
            else if(configuration.saved)
            {
                buttonContent = "Override Configuration";
            }
            else
            {
                buttonContent = "Save Configuration";
            }
            EditorGUIUtility.labelWidth=100;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            tracker.configurationName = EditorGUILayout.TextField(typeOfConfigurator + " name: ", tracker.configurationName,GUILayout.MinWidth(200));
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
                //MyDebugger.MyDebug("saving fdgdfgdfgdfgdf");
                // checkToggled();
                configuration.SaveConfiguration(tracker);
                reset(false);
            }

        }

        internal virtual bool existsConfigurationWithName(string configurationName) { return false;}

        private void chooseNewConfigurationName()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Choose a name for the new configuration.", EditorStyles.boldLabel);
            configuration.configurationName = GUILayout.TextField(configuration.configurationName);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (!existsConfigurationWithName(configuration.configurationName))
            {
                if (GUILayout.Button("Ok"))
                {
                    //configuration = configurator.newConfiguration(chosenName,chosenGO);
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
        


        
        
        protected void addProperties()
        {

            using (var h = new EditorGUILayout.VerticalScope())
            {
                GameObject gO = tracker.gameObject;
                foreach (FieldOrProperty obj in tracker.objectsProperties[gO].Values)
                {
                    bool disabled = tracker.objectDerivedFromFields[gO][obj.Name()] == null;
                    EditorGUI.BeginDisabledGroup(disabled);
                    if (isMappable(obj))
                    {
                        EditorGUILayout.BeginHorizontal();
                        tracker.objectsToggled[obj] = EditorGUILayout.ToggleLeft(obj.Name(), tracker.objectsToggled[obj]);
                        addCustomFields(obj);

                        if (tracker.objectsToggled[obj] && !tracker.IsBaseType(obj))
                        {
                            bool configure = GUILayout.Button("Configure Object");
                            if (configure)
                            {
                                objectMode = true;
                                objectToConfigure = obj;
                                helpScroll = new Vector2(0, 0);
                                if (!tracker.basicTypeCollectionsConfigurations.ContainsKey(obj))
                                {
                                    tracker.basicTypeCollectionsConfigurations.Add(obj, new SimpleGameObjectsTracker(objectToConfigure.Type()));
                                }
                                tracker.basicTypeCollectionsConfigurations[obj].getBasicProperties();
                                drawObjectProperties();
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    else
                    {

                        tracker.objectsToggled[obj] = EditorGUILayout.Foldout(tracker.objectsToggled[obj], obj.Name()) && !disabled;
                        if (tracker.objectsToggled[obj])
                        {
                            EditorGUI.indentLevel++;
                            addSubProperties(tracker.objectDerivedFromFields[gO][obj.Name()], obj.Name(), gO);
                            EditorGUI.indentLevel--;
                        }

                    }
                    EditorGUI.EndDisabledGroup();
                }


                foreach (Component c in tracker.gameObjectsComponents[gO])
                {
                    tracker.objectsToggled[c] = EditorGUILayout.Foldout(tracker.objectsToggled[c], c.GetType().ToString());
                    if (tracker.objectsToggled[c])
                    {
                        EditorGUI.indentLevel++;
                        addSubProperties(c, c.GetType().ToString(), gO);
                        EditorGUI.indentLevel--;
                    }
                }
            }
        }

        protected virtual bool isMappable(FieldOrProperty obj)
        {
            return tracker.IsMappable(obj);
        }

        public void addPropertyName(object obj)
        {
            if (!tracker.propertiesName.ContainsKey(obj))
            {
                tracker.propertiesName.Add(obj, "Choose a name");
            }
            tracker.propertiesName[obj] = EditorGUILayout.TextField(tracker.propertiesName[obj]);
        }
        /*void Update()
        {
            if (configuration is null && manager.configuredGameObject().Contains(chosenGO))
            {
                
                    disableProperties = true;
                    foreach (AbstractConfiguration s in manager.confs())
                    {

                        UnityEngine.Object parent = PrefabUtility.GetCorrespondingObjectFromSource(tracker.GetGameObject(chosenGO));
                        if (parent != null && s.gOType.Equals(parent.ToString()))
                        {
                            disableProperties = true;
                            showingConfigurationof = chosenGO;
                            Repaint();
                            return;
                        }
                    }
            }
            else 
            {
                    disableProperties = false;
            }

            updateDataStructures();
            
            Repaint();
        }*/
        protected void updateDataStructures()
        {

            mainScroll = new Vector2(0, 0);
            //MyDebugger.MyDebug(chosenGO);
            tracker.UpdateDataStructures(configuration);


        }
        
        protected void addSubProperties(object obj, string name, object objOwner)
        {

            if (tracker.objectsOwners.ContainsKey(obj) && (!tracker.objectsOwners[obj].Key.Equals(objOwner) || !tracker.objectsOwners[obj].Value.Equals(name)) || obj.Equals(tracker.gameObject))
            {
                EditorGUI.BeginDisabledGroup(true);
                if (obj.Equals(tracker.gameObject))
                {
                    EditorGUILayout.ToggleLeft("This is " + configuration.gameObject + " object", true);
                }
                else if (!tracker.objectsOwners[obj].Value.Equals(name))
                {
                    EditorGUILayout.ToggleLeft("object already listed as " + tracker.objectsOwners[obj].Value, true);
                }
                else
                {
                    EditorGUILayout.ToggleLeft("object already listed in an upper level", true);
                }
                EditorGUI.EndDisabledGroup();

                return;
            }
            if (!tracker.objectsToggled.ContainsKey(obj))
            {
                tracker.objectsToggled.Add(obj, false);
            }
            if (!tracker.objectsProperties.ContainsKey(obj))
            {
                tracker.UpdateDataStructures(obj, null, new MyListString());
            }
            foreach (FieldOrProperty f in tracker.objectsProperties[obj].Values)
            {
                bool disabled = tracker.objectDerivedFromFields[obj][f.Name()] == null;
                EditorGUI.BeginDisabledGroup(disabled);
                if (isMappable(f))
                {
                    EditorGUILayout.BeginHorizontal();
                    tracker.objectsToggled[f] = EditorGUILayout.ToggleLeft(f.Name(), tracker.objectsToggled[f]);
                    addCustomFields(f);
                    if (tracker.objectsToggled[f] && !tracker.IsBaseType(f))
                    {
                        bool configure = GUILayout.Button("Configure Object");
                        if (configure)
                        {
                            objectMode = true;
                            objectToConfigure = f;
                            helpScroll = new Vector2(0, 0);
                            if (!tracker.basicTypeCollectionsConfigurations.ContainsKey(f))
                            {
                                tracker.basicTypeCollectionsConfigurations.Add(f, new SimpleGameObjectsTracker(objectToConfigure.Type()));
                            }
                            tracker.basicTypeCollectionsConfigurations[f].getBasicProperties();
                            drawObjectProperties();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    tracker.objectsToggled[f] = EditorGUILayout.Foldout(tracker.objectsToggled[f], f.Name()) && !disabled;
                    if (tracker.objectsToggled[f])
                    {
                        EditorGUI.indentLevel++;
                        addSubProperties(tracker.objectDerivedFromFields[obj][f.Name()], f.Name(), obj);
                        EditorGUI.indentLevel--;
                    }
                }
                EditorGUI.EndDisabledGroup();
            }
        }

        protected void drawObjectProperties()
        {
            
            SimpleGameObjectsTracker st = tracker.basicTypeCollectionsConfigurations[objectToConfigure];
            GUILayout.Label("Configure " + st.objType + " object for " + objectToConfigure.Name() + " property ", EditorStyles.boldLabel);
            List<string> propertiesNames = new List<string>();
            //MyDebugger.MyDebug(st.propertiesToggled);
            foreach (string s in st.propertiesToggled.Keys)
            {
                propertiesNames.Add(s);
            }

            using (var h = new EditorGUILayout.VerticalScope())
            {
                using (var scrollView = new EditorGUILayout.ScrollViewScope(helpScroll))
                {
                    foreach (string s in propertiesNames)
                    {
                        st.propertiesToggled[s] = EditorGUILayout.ToggleLeft(s, st.propertiesToggled[s]);
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

                if (st.toSave.Count == 0)
                {
                    tracker.basicTypeCollectionsConfigurations.Remove(objectToConfigure);
                    tracker.objectsToggled[objectToConfigure] = false;
                }
                objectMode = false;
            }
            if (save)
            {
                st.save();
                if (st.toSave.Count == 0)
                {
                    tracker.basicTypeCollectionsConfigurations.Remove(objectToConfigure);
                    tracker.objectsToggled[objectToConfigure] = false;
                }
                objectMode = false;
            }

        }
        internal virtual void addCustomFields(FieldOrProperty obj) { }
    }
}
