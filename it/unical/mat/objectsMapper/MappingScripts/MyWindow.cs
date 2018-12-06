using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Threading;
using EmbASP4Unity.it.unical.mat.objectsMapper.MappingScripts;

[Serializable]
public class MyWindow : EditorWindow
{
    GameObjectsTracker tracker;
    SensorsManager manager;
    [SerializeField]
    int chosenGOIndex = 0;
    [SerializeField]
    string chosenGO;
    [SerializeField]
    bool showProperties = false;
    [SerializeField]
    int chosenConfig = 0;
    [SerializeField]
    bool disableProperties;
    string showingConfigurationof;
    Vector2 scroll;
    List<string> gOToShow;



    [MenuItem("Window/My Window")]
    public static void Init()
    {
        Debug.Log("going to show");
        EditorWindow.GetWindow(typeof(MyWindow));
        Debug.Log("showed");
    }


    void OnEnable()
    {
        if (AssetDatabase.LoadAssetAtPath("Assets/Plugins/SensorsManager.asset", typeof(SensorsManager)) == null)
        {
            manager = CreateInstance<SensorsManager>();
        }
        else
        {
            manager = (SensorsManager)AssetDatabase.LoadAssetAtPath("Assets/Plugins/SensorsManager.asset", typeof(SensorsManager));

        }
        tracker = new GameObjectsTracker();
    }

    /*void awake()
    {
        tracker = new GameObjectsTracker();
    }*/
    
    private void OnFocus()
    {
        refreshAvailableGO();
    }

    void OnGUI()
    {
        chosenGO = EditorGUILayout.TextField("Chosen object", chosenGO);
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Choose a game object to map", EditorStyles.label);
        chosenGOIndex = EditorGUILayout.Popup(chosenGOIndex, gOToShow.ToArray());
        chosenGO = gOToShow[chosenGOIndex];
        GUILayout.EndHorizontal();
        if (disableProperties)
        {
            showConfigurations();
        }
        else if (tracker.GO != null)
        {
            showProperties = EditorGUILayout.BeginToggleGroup("Show properties", showProperties);
            EditorGUI.indentLevel++;

            if (showProperties)
            {

                addProperties();
            }
            //showProperties = EditorGUILayout.Toggle("Toggle", showProperties);
            EditorGUILayout.EndToggleGroup();
            EditorGUI.indentLevel--;
            string buttonContent;
            if (manager.configuredGameObject.Contains(chosenGO))
            {
                buttonContent = "Override Configuration";
            }
            else
            {
                buttonContent = "Save Configuration";
            }
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
                updateConfiguredObject();
            }
        }

    }

    private void refreshAvailableGO()
    {
        tracker.updateGameObjects();
        gOToShow = new List<string>();
        gOToShow.Add("Chose a Game Object to configure");
        tracker.AvailableGameObjects.Sort();
        gOToShow.AddRange(tracker.AvailableGameObjects);

    }

    private void updateConfiguredObject()
    {
        if (manager.configuredGameObject.Contains(chosenGO))
        {
            SensorConfiguration toRemove = new SensorConfiguration();
            foreach (SensorConfiguration s in manager.sensConfs)
            {
                if (s.gOName.Equals(chosenGO))
                {
                    toRemove = s;
                    break;
                }
            }
            manager.sensConfs.Remove(toRemove);
        }
        else
        {
            manager.configuredGameObject.Add(chosenGO);
        }
        manager.sensConfs.Add(tracker.saveConfiguration(chosenGO));
    }

    private void showConfigurations()
    {
        if (manager.configuredGameObject.Count > 0)
        {
            if (manager.configuredGameObject.Contains(chosenGO))
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
                    foreach (SensorConfiguration s in manager.sensConfs)
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
            List<SensorConfiguration> relatedSensorConfiguration = new List<SensorConfiguration>();
            List<string> relatedGO = new List<string>();
            relatedGO.Add("Chose a configuration");
            foreach (SensorConfiguration s in manager.sensConfs)
            {
                if (s.gOType != null)
                {
                    UnityEngine.Object parent = PrefabUtility.GetCorrespondingObjectFromSource(tracker.GetGameObject(chosenGO));
                    if (parent != null && s.gOType.Equals(parent.ToString()))
                    {
                        relatedSensorConfiguration.Add(s);
                        relatedGO.Add(s.gOName);
                    }
                }
            }
            if (relatedSensorConfiguration.Count > 0)
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
                    tracker.updateDataStructures(chosenGO, relatedSensorConfiguration[chosenConfig - 1]);
                    Repaint();
                    chosenConfig = 0;
                }
                return;
            }
        }
    }

    private void addProperties()
    {

        using (var h = new EditorGUILayout.VerticalScope())
        {
            using (var scrollView = new EditorGUILayout.ScrollViewScope(scroll))
            {
                GameObject gO = tracker.GO;
                foreach (FieldOrProperty obj in tracker.ObjectsProperties[gO].Values)
                {
                    EditorGUI.BeginDisabledGroup(tracker.ObjectDerivedFromFields[gO][obj.Name()] == null);
                    if (tracker.IsBaseType(obj))
                    {
                        tracker.ObjectsToggled[obj] = EditorGUILayout.ToggleLeft(obj.Name(), tracker.ObjectsToggled[obj]);
                    }
                    else
                    {

                        tracker.ObjectsToggled[obj] = EditorGUILayout.BeginToggleGroup(obj.Name(), tracker.ObjectsToggled[obj]);
                        if (tracker.ObjectsToggled[obj])
                        {
                            EditorGUI.indentLevel++;
                            addSubProperties(tracker.ObjectDerivedFromFields[gO][obj.Name()], gO);
                            EditorGUI.indentLevel--;
                        }
                        EditorGUILayout.EndToggleGroup();
                    }
                    EditorGUI.EndDisabledGroup();
                }


                foreach (Component c in tracker.GOComponents[gO])
                {
                    //Debug.Log(c);
                    tracker.ObjectsToggled[c] = EditorGUILayout.BeginToggleGroup(c.GetType().ToString(), tracker.ObjectsToggled[c]);
                    if (tracker.ObjectsToggled[c])
                    {
                        EditorGUI.indentLevel++;
                        addSubProperties(c, gO);
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.EndToggleGroup();
                }

                scroll = scrollView.scrollPosition;

            }
        }
    }

    private void addSubProperties(object obj, object objOwner)
    {

        if (tracker.ObjectsOwners.ContainsKey(obj) && !tracker.ObjectsOwners[obj].Equals(objOwner) || obj.Equals(tracker.GO))
        {
            EditorGUI.BeginDisabledGroup(true);
            if (obj.Equals(tracker.GO))
            {
                EditorGUILayout.ToggleLeft("This is " + chosenGO + " object", true);
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
            EditorGUI.BeginDisabledGroup(tracker.ObjectDerivedFromFields[obj][f.Name()] == null);
            if (tracker.IsBaseType(f))
            {
                tracker.ObjectsToggled[f] = EditorGUILayout.ToggleLeft(f.Name(), tracker.ObjectsToggled[f]);
            }
            else
            {
                tracker.ObjectsToggled[f] = EditorGUILayout.BeginToggleGroup(f.Name(), tracker.ObjectsToggled[f]);
                if (tracker.ObjectsToggled[f])
                {
                    EditorGUI.indentLevel++;
                    addSubProperties(tracker.ObjectDerivedFromFields[obj][f.Name()], obj);
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndToggleGroup();
            }
            EditorGUI.EndDisabledGroup();
        }
    }



    private void updateDataStructures()
    {

        scroll = new Vector2(0, 0);
        //Debug.Log(chosenGO);

        tracker.updateDataStructures(chosenGO, null);


    }

    void OnInspectorUpdate()
    {
        if (chosenGOIndex > 0 && tracker != null && (tracker.GO == null || !tracker.GO.Equals(tracker.GetGameObject(chosenGO))))
        {
            if (!disableProperties)
            {
                if (manager.configuredGameObject.Contains(chosenGO))
                {
                    disableProperties = true;
                    showingConfigurationof = chosenGO;
                    Repaint();
                    return;

                }
                foreach (SensorConfiguration s in manager.sensConfs)
                {

                    UnityEngine.Object parent = PrefabUtility.GetCorrespondingObjectFromSource(tracker.GetGameObject(chosenGO));
                    if (parent!=null && s.gOType.Equals(parent.ToString()))
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

    void OnDestroy()
    {
        if (AssetDatabase.LoadAssetAtPath("Assets/Plugins/SensorsManager.asset", typeof(SensorsManager)) == null)
        {
            AssetDatabase.CreateAsset(manager, "Assets/Plugins/SensorsManager.asset");
        }
        else
        {
            EditorUtility.SetDirty(manager);
            AssetDatabase.SaveAssets();
        }
    }

}
