using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Threading;


namespace EmbASP4Unity.it.unical.mat.objectsMapper.EditorWindows
{
    public abstract class AbstractConfigurationWindow : EditorWindow
    {
        protected GameObjectsTracker tracker;
        protected IManager manager;
        [SerializeField]
        protected int chosenGOIndex = 0;
        [SerializeField]
        protected string chosenGO;
        [SerializeField]
        protected bool showProperties = false;
        [SerializeField]
        protected int chosenConfig = 0;
        [SerializeField]
        protected bool disableProperties;
        protected string showingConfigurationof;
        protected Vector2 mainScroll;
        protected Vector2 helpScroll;
        protected List<string> gOToShow;
        protected bool objectMode;
        protected FieldOrProperty objectToConfigure;
        

        public void draw(string type)
        {
            
            chosenGO = EditorGUILayout.TextField("Chosen object", chosenGO);
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Choose tha name of a game object to map as "+type, EditorStyles.label);
            chosenGOIndex = EditorGUILayout.Popup(chosenGOIndex, gOToShow.ToArray());
            chosenGO = gOToShow[chosenGOIndex];
            GUILayout.EndHorizontal();
            if (disableProperties)
            {
                showConfigurations();
            }
            else if (tracker.GO != null)
            {
                showProperties = EditorGUILayout.Foldout(showProperties, "Show properties");
                EditorGUI.indentLevel++;

                if (showProperties)
                {
                    addProperties();
                }
                //showProperties = EditorGUILayout.Toggle("Toggle", showProperties);

                EditorGUI.indentLevel--;
                string buttonContent;
                if (manager.configuredGameObject().Contains(chosenGO))
                {
                    buttonContent = "Override Configuration";
                }
                else
                {
                    buttonContent = "Save Configuration";
                }
                tracker.configurationName= EditorGUILayout.TextField(type + " name: ", tracker.configurationName);
                EditorGUILayout.BeginHorizontal();
                bool save = GUILayout.Button(buttonContent);
                bool cancel = GUILayout.Button("Cancel");
                EditorGUILayout.EndHorizontal();
                if (cancel)
                {
                    chosenGOIndex = 0;
                }
                if (save)
                {
                    Debug.Log("saving fdgdfgdfgdfgdf");
                    updateConfiguredObject();
                }
            }

        }
        protected void updateConfiguredObject(AbstractConfiguration conf)
        {
            if (manager.configuredGameObject().Contains(chosenGO))
            {
                
                foreach (AbstractConfiguration s in manager.confs())
                {
                    if (s.gOName.Equals(chosenGO))
                    {
                        conf = s;
                        break;
                    }
                }
                manager.confs().Remove(conf);
            }
            else
            {
                manager.configuredGameObject().Add(chosenGO);
            }
            try
            {
                Debug.Log("before adding"+manager.confs().Count);
                manager.confs().Add(tracker.saveConfiguration(conf,chosenGO));
                Debug.Log("after adding" + manager.confs().Count);

            }
            catch (Exception e)
            {
                Debug.Log("error while adding");
                Debug.Log(e.StackTrace);
            }
        }
       
        protected void refreshAvailableGO()
        {
            tracker.updateGameObjects();
            gOToShow = new List<string>();
            gOToShow.Add("Chose a Game Object to configure");
            //tracker.AvailableGameObjects.Sort();
            gOToShow.AddRange(tracker.AvailableGameObjects);

        }
        protected void showConfigurations() {
            if (manager.configuredGameObject().Count > 0)
            {
                if (manager.configuredGameObject().Contains(chosenGO))
                {
                    GUILayout.Label("A configuration exists for " + chosenGO, EditorStyles.label);
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Do you want to load it?", EditorStyles.label);
                    bool yes = GUILayout.Button("Yes");
                    bool no = GUILayout.Button("No");
                    EditorGUILayout.EndHorizontal();
                    if (yes)
                    {
                        disableProperties = false;
                        foreach (AbstractConfiguration s in manager.confs())
                        {
                            if (s.gOName.Equals(chosenGO))
                            {
                                tracker.updateDataStructures(null, s);
                                Repaint();
                                return;
                            }
                        }
                    }
                    if (no)
                    {
                        disableProperties = false;
                        updateDataStructures();
                        Repaint();
                    }
                    return;
                }
                else
                {
                    List<AbstractConfiguration> relatedConfiguration = new List<AbstractConfiguration>();
                    List<string> relatedGO = new List<string>();
                    relatedGO.Add("Chose a configuration");
                    foreach (AbstractConfiguration s in manager.confs())
                    {
                        if (s.gOType != null)
                        {
                            UnityEngine.Object parent = PrefabUtility.GetCorrespondingObjectFromSource(tracker.GetGameObject(chosenGO));
                            if (parent != null && s.gOType.Equals(parent.ToString()))
                            {
                                relatedConfiguration.Add(s);
                                relatedGO.Add(s.gOName);
                            }
                        }
                    }
                    if (relatedConfiguration.Count > 0)
                    {
                        GUILayout.Label("Following object of type " + PrefabUtility.GetCorrespondingObjectFromSource(tracker.GetGameObject(chosenGO)).ToString() + " have been configured.", EditorStyles.label);
                        GUILayout.Label("Chose one to load its configuration or click \"Continue\" to start a new configuration.", EditorStyles.label);
                        EditorGUILayout.BeginHorizontal();
                        chosenConfig = EditorGUILayout.Popup(chosenConfig, relatedGO.ToArray());
                        bool cont = GUILayout.Button("Continue");
                        EditorGUILayout.EndHorizontal();
                        if (cont)
                        {
                            disableProperties = false;
                            updateDataStructures();
                            Repaint();
                        }
                        if (chosenConfig > 0)
                        {
                            disableProperties = false;
                            tracker.updateDataStructures(chosenGO, relatedConfiguration[chosenConfig - 1]);
                            Repaint();
                            chosenConfig = 0;
                        }
                        return;
                    }
                }
            }
        }
        protected void addProperties()
        {

            using (var h = new EditorGUILayout.VerticalScope())
            {
                using (var scrollView = new EditorGUILayout.ScrollViewScope(mainScroll))
                {
                    GameObject gO = tracker.GO;
                    foreach (FieldOrProperty obj in tracker.ObjectsProperties[gO].Values)
                    {
                        bool disabled = tracker.ObjectDerivedFromFields[gO][obj.Name()] == null;
                        EditorGUI.BeginDisabledGroup(disabled);
                        if (tracker.IsMappable(obj))
                        {
                            EditorGUILayout.BeginHorizontal();
                            tracker.ObjectsToggled[obj] = EditorGUILayout.ToggleLeft(obj.Name(), tracker.ObjectsToggled[obj]);
                            if (tracker.IsMappable(obj))
                            {
                                addCustomFields(obj);
                            }
                            if (tracker.ObjectsToggled[obj] && !tracker.IsBaseType(obj))
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
                                        tracker.basicTypeCollectionsConfigurations[obj].getBasicProperties();
                                    }
                                    drawObjectProperties();
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        else
                        {

                            tracker.ObjectsToggled[obj] = EditorGUILayout.Foldout(tracker.ObjectsToggled[obj], obj.Name()) && !disabled;
                            if (tracker.ObjectsToggled[obj])
                            {
                                EditorGUI.indentLevel++;
                                addSubProperties(tracker.ObjectDerivedFromFields[gO][obj.Name()], obj.Name(), gO);
                                EditorGUI.indentLevel--;
                            }

                        }
                        EditorGUI.EndDisabledGroup();
                    }


                    foreach (Component c in tracker.GOComponents[gO])
                    {
                        //Debug.Log(c);
                        tracker.ObjectsToggled[c] = EditorGUILayout.Foldout(tracker.ObjectsToggled[c], c.GetType().ToString());
                        if (tracker.ObjectsToggled[c])
                        {
                            EditorGUI.indentLevel++;
                            addSubProperties(c, c.name, gO);
                            EditorGUI.indentLevel--;
                        }
                    }

                    mainScroll = scrollView.scrollPosition;

                }
            }
        }

       
        public void addPropertyName(object obj)
        {
            if (!tracker.propertiesName.ContainsKey(obj))
            {
                tracker.propertiesName.Add(obj, "Choose a name");
            }
            tracker.propertiesName[obj] = EditorGUILayout.TextField(tracker.propertiesName[obj]);
        }
        public void OnInspectorUpdate()
        {
            if (chosenGOIndex > 0 && tracker != null && (tracker.GO == null || !tracker.GO.Equals(tracker.GetGameObject(chosenGO))))
            {
                if (!disableProperties)
                {
                    if (manager.configuredGameObject().Contains(chosenGO))
                    {
                        disableProperties = true;
                        showingConfigurationof = chosenGO;
                        Repaint();
                        return;

                    }
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
                    if (!showingConfigurationof.Equals(chosenGO))
                    {
                        disableProperties = false;
                    }
                }

                updateDataStructures();


            }
            if (chosenGOIndex == 0 && tracker.GO != null)
            {
                tracker.GO = null;
                tracker.cleanDataStructures();
            }
            Repaint();
        }
        protected void updateDataStructures()
        {

            mainScroll = new Vector2(0, 0);
            //Debug.Log(chosenGO);
            tracker.updateDataStructures(chosenGO, null);


        }
        protected abstract void updateConfiguredObject();
        protected void addSubProperties(object obj, string name, object objOwner)
        {

            if (tracker.ObjectsOwners.ContainsKey(obj) && (!tracker.ObjectsOwners[obj].Key.Equals(objOwner) || !tracker.ObjectsOwners[obj].Value.Equals(name)) || obj.Equals(tracker.GO))
            {
                EditorGUI.BeginDisabledGroup(true);
                if (obj.Equals(tracker.GO))
                {
                    EditorGUILayout.ToggleLeft("This is " + chosenGO + " object", true);
                }
                else if (!tracker.ObjectsOwners[obj].Value.Equals(name))
                {
                    EditorGUILayout.ToggleLeft("object already listed as " + tracker.ObjectsOwners[obj].Value, true);
                }
                else
                {
                    EditorGUILayout.ToggleLeft("object already listed in an upper level", true);
                }
                EditorGUI.EndDisabledGroup();

                return;
            }
            if (!tracker.ObjectsToggled.ContainsKey(obj))
            {
                tracker.ObjectsToggled.Add(obj, false);
            }
            if (!tracker.ObjectsProperties.ContainsKey(obj))
            {
                tracker.updateDataStructures(obj, null, null);
            }
            foreach (FieldOrProperty f in tracker.ObjectsProperties[obj].Values)
            {
                bool disabled = tracker.ObjectDerivedFromFields[obj][f.Name()] == null;
                EditorGUI.BeginDisabledGroup(disabled);
                if (tracker.IsMappable(f))
                {
                    EditorGUILayout.BeginHorizontal();
                    tracker.ObjectsToggled[f] = EditorGUILayout.ToggleLeft(f.Name(), tracker.ObjectsToggled[f]);
                    if (tracker.IsMappable(f))
                    {
                        addCustomFields(f);
                    }
                    if (tracker.ObjectsToggled[f] && !tracker.IsBaseType(f))
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
                                tracker.basicTypeCollectionsConfigurations[f].getBasicProperties();
                            }
                            drawObjectProperties();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    tracker.ObjectsToggled[f] = EditorGUILayout.Foldout(tracker.ObjectsToggled[f], f.Name()) && !disabled;
                    if (tracker.ObjectsToggled[f])
                    {
                        EditorGUI.indentLevel++;
                        addSubProperties(tracker.ObjectDerivedFromFields[obj][f.Name()], f.Name(), obj);
                        EditorGUI.indentLevel--;
                    }
                }
                EditorGUI.EndDisabledGroup();
            }
        }

        protected void drawObjectProperties()
        {
            GUILayout.Label("Configure "+objectToConfigure.Type().GetElementType()+" object for "+objectToConfigure.Name()+" property ", EditorStyles.boldLabel);
            SimpleGameObjectsTracker st = tracker.basicTypeCollectionsConfigurations[objectToConfigure];
            List<string> propertiesNames = new List<string>();
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
            bool cancel = GUILayout.Button("Cancel");
            bool save = GUILayout.Button("Save");
            EditorGUILayout.EndHorizontal();
            if (cancel)
            {
                
                if (st.toSave.Count == 0)
                {
                    tracker.basicTypeCollectionsConfigurations.Remove(objectToConfigure);
                    tracker.ObjectsToggled[objectToConfigure] = false;
                }
                objectMode = false;
            }
            if (save)
            {
                st.save();
                if (st.toSave.Count == 0)
                {
                    tracker.basicTypeCollectionsConfigurations.Remove(objectToConfigure);
                    tracker.ObjectsToggled[objectToConfigure] = false;
                }
                objectMode = false;
            }
            
        }

        protected abstract string onSaving();
        internal abstract void addCustomFields(FieldOrProperty obj);
    }
}
