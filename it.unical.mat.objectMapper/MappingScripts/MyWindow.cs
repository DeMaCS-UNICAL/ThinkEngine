using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using Assets.Scripts.MappingScripts;

public class MyWindow : EditorWindow
{
    ReflectionExecutor re;
    Serializator serializator;
    bool groupEnabled;
    List<string> availableGameObjects;
    int chosenGOIndex = 0;
    string chosenGO;
    Dictionary<object, bool> objectsToggled;
    Dictionary<object, object> objectsOwners;
    Dictionary<object,Dictionary< string,object>> objectDerivedFromFields;
    Dictionary<object, Dictionary<string,FieldOrProperty>> objectsProperties;
    Dictionary<GameObject, List<Component>> gOComponents;
    bool showProperties=false;
    Vector2 scroll;


    [MenuItem("Window/My Window")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(MyWindow));
    }

    private void Awake()
    {
        re = (ReflectionExecutor)GameObject.FindObjectOfType(typeof(ReflectionExecutor));
        chosenGOIndex = EditorGUILayout.Popup(chosenGOIndex, availableGameObjects.ToArray());
        chosenGO = availableGameObjects[chosenGOIndex];
        serializator = new Serializator(this);
        
    }

    void OnGUI()
    {
        
        availableGameObjects = re.GetGameObjects();
        availableGameObjects.Sort();
        chosenGO = EditorGUILayout.TextField("Choosen object", chosenGO);
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Choose a game object to map", EditorStyles.label);
        chosenGOIndex = EditorGUILayout.Popup(chosenGOIndex, availableGameObjects.ToArray());
        chosenGO = availableGameObjects[chosenGOIndex];
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
            serializator.serialize(re.GetGameObjectWithName(chosenGO));
        }
    }

    private void addProperties()
    {

        using (var h = new EditorGUILayout.VerticalScope())
        {
            using (var scrollView = new EditorGUILayout.ScrollViewScope(scroll))
            {
                GameObject gO = re.GetGameObjectWithName(chosenGO);
                foreach(FieldOrProperty obj in objectsProperties[gO].Values)
                {
                    EditorGUI.BeginDisabledGroup(objectDerivedFromFields[gO][obj.Name()] == null);
                    if (re.IsBaseType(obj))
                        {
                            objectsToggled[obj] = EditorGUILayout.ToggleLeft(obj.Name(), objectsToggled[obj]);
                        }
                        else
                        {
                        
                            objectsToggled[obj] = EditorGUILayout.BeginToggleGroup(obj.Name(), objectsToggled[obj]);
                            if (objectsToggled[obj])
                            {
                                EditorGUI.indentLevel++;
                                addSubProperties(objectDerivedFromFields[gO][obj.Name()], gO);
                                EditorGUI.indentLevel--;
                            }
                            EditorGUILayout.EndToggleGroup();
                        }
                    EditorGUI.EndDisabledGroup();
                }
                     
                
                foreach(Component c in gOComponents[gO])
                {
                    //Debug.Log(c);
                    objectsToggled[c] = EditorGUILayout.BeginToggleGroup(c.GetType().ToString(), objectsToggled[c]);
                    if (objectsToggled[c])
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
        
        if(objectsOwners.ContainsKey(obj) && !objectsOwners[obj].Equals(objOwner)|| obj.Equals(re.GetGameObjectWithName(chosenGO)))
        {
            EditorGUI.BeginDisabledGroup(true);
            if (obj.Equals(re.GetGameObjectWithName(chosenGO)))
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
        if (!objectsToggled.ContainsKey(obj))
        {
            objectsToggled.Add(obj, false);
        }
        if (!objectsProperties.ContainsKey(obj))
        {
            updateDataStructures(obj);
        }
        foreach (FieldOrProperty f in objectsProperties[obj].Values)
        {
            EditorGUI.BeginDisabledGroup(objectDerivedFromFields[obj][f.Name()] == null);
            if (re.IsBaseType(f))
            {
                objectsToggled[f] = EditorGUILayout.ToggleLeft(f.Name(), objectsToggled[f]);
            }
            else
            {
                objectsToggled[f] = EditorGUILayout.BeginToggleGroup(f.Name(), objectsToggled[f]);
                if (objectsToggled[f])
                {
                    EditorGUI.indentLevel++;
                    addSubProperties(objectDerivedFromFields[obj][f.Name()],obj);
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndToggleGroup();
            }
            EditorGUI.EndDisabledGroup();
        }
    }

    private void updateDataStructures(object obj)
    {
        List<FieldOrProperty> p = re.GetFieldsAndProperties(obj);
        objectsProperties.Add(obj, new Dictionary<string, FieldOrProperty>());
        objectDerivedFromFields.Add(obj, new Dictionary<string, object>());
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
            objectsProperties[obj].Add(ob.Name(), ob);
            objectsToggled.Add(ob, false);
            objectDerivedFromFields[obj].Add(ob.Name(), objValueForOb);
            if (objValueForOb != null && !objectsOwners.ContainsKey(objValueForOb))
            {
                objectsOwners.Add(objValueForOb, obj);
            }

        }
        if (obj.GetType() == typeof(GameObject))
        {
            gOComponents[(GameObject)obj] = re.GetComponents((GameObject)obj);
            foreach (Component c in gOComponents[(GameObject)obj])
            {
                objectsToggled.Add(c, false);
            }
        }
    }

    private void updateDataStructures()
    {

        scroll = new Vector2(0, 0);
        //Debug.Log(chosenGO);
        GameObject gO = re.GetGameObjectWithName(chosenGO);
        objectsProperties = new Dictionary<object, Dictionary<string,FieldOrProperty>>();
        gOComponents = new Dictionary<GameObject, List<Component>>();
        objectsOwners = new Dictionary<object, object>();
        objectsToggled = new Dictionary<object, bool>();
        List<FieldOrProperty> p = re.GetFieldsAndProperties(gO); ;
        objectsProperties[gO] = new Dictionary<string, FieldOrProperty>();
        objectDerivedFromFields = new Dictionary<object, Dictionary<string, object>>();
        objectDerivedFromFields.Add(gO, new Dictionary<string, object>());
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
            objectsProperties[gO].Add(obj.Name(), obj);
            if (gOValueForObj != null && !re.IsBaseType(obj))
            {
                objectsOwners.Add(gOValueForObj, gO);
            }
            
            objectDerivedFromFields[gO].Add(obj.Name(), gOValueForObj);
            if (!objectsToggled.ContainsKey(obj))
            {
                objectsToggled.Add(obj, false);               
            }
        }
        gOComponents[gO] = re.GetComponents(gO);
        foreach (Component c in gOComponents[gO])
        {
            objectsToggled.Add(c, false);
        }

    }

    void OnInspectorUpdate()
    {
        if (showProperties && (objectsProperties == null || !objectsProperties.ContainsKey(re.GetGameObjectWithName(chosenGO))))
        {
            updateDataStructures();
        }
        Repaint();
    }
}
