using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Threading;


namespace EmbASP4Unity.it.unical.mat.objectsMapper.EditorWindows
{
    [CustomEditor(typeof(AbstractConfigurator))]
    public abstract class AbstractConfiguratorEditor : Editor
    {
        protected GameObjectsTracker tracker;
        
        [SerializeField]
        protected string chosenGO;
        [SerializeField]
        protected bool showProperties = false;
        [SerializeField]
        protected int chosenConfig = 0;
        [SerializeField]
        protected bool disableProperties;
        protected string typeOfConfigurator;

        //mode
        protected bool showingConfigurations;
        protected bool configuringConfiguration;
        protected bool chosingNewConfigurationName;

        protected Vector2 mainScroll;
        protected Vector2 helpScroll;
        protected Vector2 configurationScroll;
        protected string chosenName;
        protected List<string> gOToShow;
        protected bool objectMode;
        protected FieldOrProperty objectToConfigure;
        private AbstractConfigurator configurator;
        [SerializeField]
        private AbstractConfiguration configuration;



        protected void OnEnable()
        {
            reset();
        }

        protected void reset(bool fault = false)
        {
            configurator = target as AbstractConfigurator;
            tracker = new GameObjectsTracker();
            tracker.GO = configurator.gameObject;
            if (fault)
            {
                objectMode = false;
                showingConfigurations = true;
                configuringConfiguration = false;
                chosingNewConfigurationName = false;
            }
            else
            {
                if (!(configuringConfiguration || chosingNewConfigurationName))
                {
                    showingConfigurations = true;
                }
                if (!(configuration is null))
                {
                    tracker.updateDataStructures(null, configuration);
                }
                if (objectMode && objectToConfigure is null)
                {
                    objectMode = false;
                }
            }
        }

        override public void OnInspectorGUI()
        {
            //Debug.Log("manager "+manager);
            try
            {
                GUILayout.Label(typeOfConfigurator + " Configurator", EditorStyles.boldLabel);

                chosenGO = configurator.gameObject.name;
                if (showingConfigurations)
                {
                    showConfigurations();
                    if (configuringConfiguration)
                    {
                        tracker.updateDataStructures(null, configuration);
                    }
                }

                if (chosingNewConfigurationName)
                {
                    choseNewConfigurationName();
                    if (configuringConfiguration && !(configuration is null))
                    {
                        tracker.updateDataStructures(null, configuration);
                    }
                }

                if (configuringConfiguration)
                {
                    configure();
                }
            }catch(Exception e)
            {
                Debug.Log(e.StackTrace);
                reset(true);
            }
        }

        private void configure()
        {   
            showProperties = EditorGUILayout.Foldout(showProperties, "Show properties");
            EditorGUI.indentLevel++;

            if (showProperties)
            {
                addProperties();//----------------------------------
            }
            //showProperties = EditorGUILayout.Toggle("Toggle", showProperties);

            EditorGUI.indentLevel--;
            string buttonContent;
            bool disabled=false;
            if (configurator.configurationNames().Contains(tracker.configurationName))
            {
                buttonContent = "Override Configuration";
            }
            else if (configurator.generalUsedNames().Contains(tracker.configurationName))
            {
                buttonContent = "Name used for another game object sensor";
                disabled = true;
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
            bool cancel = GUILayout.Button("Cancel");
            EditorGUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            if (cancel)
            {
                configuringConfiguration = false;
                showingConfigurations = true;
            }
            if (save)
            {
                //Debug.Log("saving fdgdfgdfgdfgdf");
                // checkToggled();
                updateConfiguredObject();
                configurator.onSaving();
            }
            
        }

        private void choseNewConfigurationName()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Choose a name for the new configuration.", EditorStyles.boldLabel);
            chosenName = GUILayout.TextField(chosenName);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (!configurator.generalUsedNames().Contains(chosenName))
            {
                if (GUILayout.Button("Ok"))
                {
                    //configuration = configurator.newConfiguration(chosenName,chosenGO);
                    configuration = null;
                    chosingNewConfigurationName = false;
                    configuringConfiguration = true;
                    tracker.updateDataStructures(chosenGO, null);
                    tracker.configurationName = chosenName;
                }
            }
            else
            {
                GUI.enabled = false;
                GUILayout.Button("Name already used.");
                GUI.enabled = true;
            }
            if (GUILayout.Button("Back"))
            {
                chosingNewConfigurationName = false;
                showingConfigurations = true;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

        }

        private void checkToggled()
        {


            List<object> allObjects = new List<object>();
            allObjects.AddRange(tracker.ObjectsToggled.Keys);
            foreach (object obj in allObjects)
            {
                if (tracker.ObjectsToggled[obj])
                {
                    toggleAncestors(obj);
                }
            }
        }

        private void toggleAncestors(object obj)
        {
            //IMPLEMENT
        }


        protected void updateConfiguredObject(AbstractConfiguration conf)
        {
            if (configurator.generalUsedNames().Contains(tracker.configurationName))
            {
                configurator.deleteConfiguration(tracker.configurationName);
            }
            /*else
            {
                configurator.addConfiguredGameObject(chosenGO);
            }*/
            try
            {
                //Debug.Log("before adding"+manager.confs().Count);
                configurator.addConfiguration(tracker.saveConfiguration(conf, chosenGO));
                //Debug.Log("after adding" + manager.confs().Count);

            }
            catch (Exception e)
            {
                //Debug.Log("error while adding");
                //Debug.Log(e.StackTrace);
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
        protected void showConfigurations()
        {
            if (configurator.configurationNames().Count>0)
            {
                GUILayout.Label("Some configurations exist for " + chosenGO, EditorStyles.label);
            }
            /*EditorGUILayout.BeginHorizontal();
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
            return;*/
            List<string> confToShow = new List<string>();
            confToShow.Add("New Configuration");
            foreach (string c in configurator.configurationNames())
            {
                confToShow.Add(c);
            }
                    
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            chosenConfig = GUILayout.SelectionGrid(chosenConfig, confToShow.ToArray(), 1,new GUIStyle(GUI.skin.toggle), GUILayout.MaxWidth(300));//_------!!!!!!!!!!!!!!!!!!!!!!!!!!1
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Configure", GUILayout.MaxWidth(100)))
            {
                if (chosenConfig == 0)
                {
                    showingConfigurations = false;
                    chosingNewConfigurationName = true;
                }
                else
                {
                    configuration = configurator.findConfiguration(confToShow[chosenConfig]);
                    showingConfigurations = false;
                    configuringConfiguration = true;
                }
            }
            if (chosenConfig != 0)
            {
                Color contentTemp = GUI.contentColor;
                Color backTemp = GUI.backgroundColor;
                GUI.contentColor = Color.white;
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Delete configuration",GUILayout.MaxWidth(150)))
                {
                    configurator.deleteConfiguration(confToShow[chosenConfig]);
                    chosenConfig = 0;
                }
                GUI.contentColor = contentTemp;
                GUI.backgroundColor=backTemp;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            /*else
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
            }*/
            
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
                        if (isMappable(obj))
                        {
                            EditorGUILayout.BeginHorizontal();
                            tracker.ObjectsToggled[obj] = EditorGUILayout.ToggleLeft(obj.Name(), tracker.ObjectsToggled[obj]);
                            addCustomFields(obj);

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
                                        Debug.Log("adding simple tracker for " + objectToConfigure.Name() + " that is a " + objectToConfigure.Type());
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
                            addSubProperties(c, c.GetType().ToString(), gO);
                            EditorGUI.indentLevel--;
                        }
                    }

                    mainScroll = scrollView.scrollPosition;

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
            //Debug.Log(chosenGO);
            tracker.updateDataStructures(chosenGO, null);


        }
        
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
                if (isMappable(f))
                {
                    EditorGUILayout.BeginHorizontal();
                    tracker.ObjectsToggled[f] = EditorGUILayout.ToggleLeft(f.Name(), tracker.ObjectsToggled[f]);
                    addCustomFields(f);
                    if (tracker.ObjectsToggled[f] && !tracker.IsBaseType(f))
                    {
                        bool configure = GUILayout.Button("Configure Object");
                        if (configure)
                        {
                            objectMode = true;
                            objectToConfigure = f;
                            helpScroll = new Vector2(0, 0);
                            Debug.Log("num: "+tracker.basicTypeCollectionsConfigurations.Count);
                            Debug.Log("f " + tracker.basicTypeCollectionsConfigurations[f]);
                            if (!tracker.basicTypeCollectionsConfigurations.ContainsKey(f))
                            {
                                Debug.Log("adding simple tracker for " + objectToConfigure.Name() + " that is a " + objectToConfigure.Type());

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
            
            SimpleGameObjectsTracker st = tracker.basicTypeCollectionsConfigurations[objectToConfigure];
            GUILayout.Label("Configure " + st.objType + " object for " + objectToConfigure.Name() + " property ", EditorStyles.boldLabel);
            List<string> propertiesNames = new List<string>();
            //Debug.Log(st.propertiesToggled);
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

        
        internal virtual void addCustomFields(FieldOrProperty obj) { }
        protected virtual void updateConfiguredObject() { }
    }
}
