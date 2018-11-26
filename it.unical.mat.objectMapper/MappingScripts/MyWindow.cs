using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Threading;
using Assets.Scripts.MappingScripts;

public class MyWindow : EditorWindow
{
    GameObjectsTracker tracker;
    int chosenGOIndex = 0;
    string chosenGO;
    bool showProperties = false;
    Vector2 scroll;


    [MenuItem("Window/My Window")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(MyWindow));
    }

    private void Awake()
    {
        tracker = new GameObjectsTracker();
        chosenGOIndex = EditorGUILayout.Popup(chosenGOIndex, tracker.AvailableGameObjects.ToArray());
        chosenGO = tracker.AvailableGameObjects[chosenGOIndex];      
        
    }

    void OnGUI()
    {

        tracker.updateGameObjects();
        tracker.AvailableGameObjects.Sort();
        chosenGO = EditorGUILayout.TextField("Choosen object", chosenGO);
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Choose a game object to map", EditorStyles.label);
        chosenGOIndex = EditorGUILayout.Popup(chosenGOIndex, tracker.AvailableGameObjects.ToArray());
        chosenGO = tracker.AvailableGameObjects[chosenGOIndex];
        GUILayout.EndHorizontal();
        showProperties = EditorGUILayout.BeginToggleGroup("Show properties", showProperties);
        EditorGUI.indentLevel++;
        
        if (showProperties)
        {

            addProperties();
        }
        //showProperties = EditorGUILayout.Toggle("Toggle", showProperties);
        EditorGUILayout.EndToggleGroup();
        EditorGUI.indentLevel--;
        bool done = GUILayout.Button("Done");
        if (done)
        {
            tracker.serialize(chosenGO);
        }
    }

    private void addProperties()
    {

        using (var h = new EditorGUILayout.VerticalScope())
        {
            using (var scrollView = new EditorGUILayout.ScrollViewScope(scroll))
            {
                GameObject gO = tracker.GetGameObject(chosenGO);
                foreach(FieldOrProperty obj in tracker.ObjectsProperties[gO].Values)
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
                     
                
                foreach(Component c in tracker.GOComponents[gO])
                {
                    //Debug.Log(c);
                    tracker.ObjectsToggled[c] = EditorGUILayout.BeginToggleGroup(c.GetType().ToString(), tracker.ObjectsToggled[c]);
                    if (tracker.ObjectsToggled[c])
                    {                        
                        EditorGUI.indentLevel++;
                        addSubProperties(c,gO);
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
        
        if(tracker.ObjectsOwners.ContainsKey(obj) && !tracker.ObjectsOwners[obj].Equals(objOwner)|| obj.Equals(tracker.GetGameObject(chosenGO)))
        {
            EditorGUI.BeginDisabledGroup(true);
            if (obj.Equals(tracker.GetGameObject(chosenGO)))
            {
                EditorGUILayout.ToggleLeft("This is "+chosenGO+" object", true);
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
            updateDataStructures(obj);
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
                    addSubProperties(tracker.ObjectDerivedFromFields[obj][f.Name()],obj);
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndToggleGroup();
            }
            EditorGUI.EndDisabledGroup();
        }
    }

    private void updateDataStructures(object obj)
    {
        List<FieldOrProperty> p = tracker.GetFieldsAndProperties(obj);
        tracker.ObjectsProperties.Add(obj, new Dictionary<string, FieldOrProperty>());
        tracker.ObjectDerivedFromFields.Add(obj, new Dictionary<string, object>());
        foreach (FieldOrProperty ob in p)
        {
            object objValueForOb;
            try
            {
                objValueForOb = ob.GetValue(obj);
            }
            catch (Exception e)
            {
                //Debug.Log("cannot get value for property " + ob.Name());
                objValueForOb = null;
            }
            tracker.ObjectsProperties[obj].Add(ob.Name(), ob);
            tracker.ObjectsToggled.Add(ob, false);
            tracker.ObjectDerivedFromFields[obj].Add(ob.Name(), objValueForOb);
            if (objValueForOb != null && !tracker.ObjectsOwners.ContainsKey(objValueForOb))
            {
                tracker.ObjectsOwners.Add(objValueForOb, obj);
            }

        }
        if (obj.GetType() == typeof(GameObject))
        {
            tracker.GOComponents[(GameObject)obj] = tracker.GetComponents((GameObject)obj);
            foreach (Component c in tracker.GOComponents[(GameObject)obj])
            {
                tracker.ObjectsToggled.Add(c, false);
            }
        }
    }

    private void updateDataStructures()
    {

        scroll = new Vector2(0, 0);
        //Debug.Log(chosenGO);
        GameObject gO = tracker.GetGameObject(chosenGO);
        tracker.ObjectsProperties = new Dictionary<object, Dictionary<string,FieldOrProperty>>();
        tracker.GOComponents = new Dictionary<GameObject, List<Component>>();
        tracker.ObjectsOwners = new Dictionary<object, object>();
        tracker.ObjectsToggled = new Dictionary<object, bool>();
        List<FieldOrProperty> p = tracker.GetFieldsAndProperties(gO); ;
        tracker.ObjectsProperties[gO] = new Dictionary<string, FieldOrProperty>();
        tracker.ObjectDerivedFromFields = new Dictionary<object, Dictionary<string, object>>();
        tracker.ObjectDerivedFromFields.Add(gO, new Dictionary<string, object>());
        foreach (FieldOrProperty obj in p)
        {
            object gOValueForObj;
            try
            {
                gOValueForObj = obj.GetValue(gO);
            }
            catch (Exception e)
            {
                gOValueForObj = null;
            }
            tracker.ObjectsProperties[gO].Add(obj.Name(), obj);
            if (gOValueForObj != null && !tracker.IsBaseType(obj))
            {
                tracker.ObjectsOwners.Add(gOValueForObj, gO);
            }

            tracker.ObjectDerivedFromFields[gO].Add(obj.Name(), gOValueForObj);
            if (!tracker.ObjectsToggled.ContainsKey(obj))
            {
                tracker.ObjectsToggled.Add(obj, false);               
            }
        }
        tracker.GOComponents[gO] = tracker.GetComponents(gO);
        foreach (Component c in tracker.GOComponents[gO])
        {
            tracker.ObjectsToggled.Add(c, false);
        }

    }

    void OnInspectorUpdate()
    {
        if (showProperties && (tracker.ObjectsProperties == null || !tracker.ObjectsProperties.ContainsKey(tracker.GetGameObject(chosenGO))))
        {
            updateDataStructures();
        }
        Repaint();
    }
}
