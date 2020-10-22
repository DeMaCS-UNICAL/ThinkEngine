using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using NUnit.Framework;
using System.IO;

public class TestingWindow : EditorWindow
{
    //GUI Style, Test Style
    static string[] nameColors = new string[] { "White", "Grey", "Red", "Blue", "Yellow", "Green","Magenta","Cyan" };
    static int indexBackgroundColor, indexContentColor;

    //Sections of general tab
    static string[] oneTab = new string[] { "TEST RESULTS" };
    static string[] threeTab = new string[] { "GENERAL SETTING", "TEST SETTING", "TEST RESULTS" };
    static string[] operators = new string[] { ">=", "=", "<=", "DON'T CHECK" };
    static bool[] foldouts = new bool[5] { false, false, false, false, false };
    static bool levelActive, threeDimensionActive, customInterface, changes, warnings, testsLoaded, pauseTestFailedActive;
    static int indexLevelScriptAttribute, tabActive;

    //Field and Class names 
    static string levelScriptAttribute; //name of script attribute in tab general
    static List<string> allFieldNamesLoaded = new List<string>(); //name of all attributes loaded
    static List<string> allClassNamesFieldsLoaded = new List<string>(); //name of classes that contains fields

    static Vector2 scrollPos; //window scroll

    // Scripts inserted from user
    static List<MonoScript> attributeScripts = new List<MonoScript>();
    static List<MonoScript> testScripts = new List<MonoScript>();
    static MonoScript levelScript, triggerConditionsScript;

    static List<Test> tests = new List<Test>(); //contains all test loaded from testScripts
    static PersistentObject persistentObject = new PersistentObject(); //to save data in play mode
    static GameObject objectToTest; //game object to test

    // This function load the window properties.
    [MenuItem("ThinkEngine/Testing")]
    public static void ShowWindow()
    {
        GetWindow(typeof(TestingWindow));
        loadProperties();
    }

    // This function loads the window properties
    public static  void loadProperties()
    {
        string[] frameworkDirectory = System.IO.Directory.GetDirectories(Application.dataPath, "Icons", SearchOption.AllDirectories);
        string path = frameworkDirectory[0].Replace("\\", "/").Substring(frameworkDirectory[0].IndexOf("Assets")) + "/test.png";
        Texture icon = AssetDatabase.LoadAssetAtPath<Texture>(path);
        EditorWindow.GetWindow<TestingWindow>().titleContent = new GUIContent("TE Testing Runner", icon);
        indexBackgroundColor = 0;
        indexContentColor = 0;
        indexLevelScriptAttribute = 0;
        tabActive = 0;
        changes = false;
        testsLoaded = false;
        pauseTestFailedActive = false;
    }

    // It deals with designing the window interface
    private void OnGUI()
    {
        // I set the color of the menu tab and activate the scroll on the window
        GUI.backgroundColor = Color.cyan;
        scrollPos = GUILayout.BeginScrollView(scrollPos, GUIStyle.none);

        // If you are in playback mode I only display tab 2 (test results tab)
        if (EditorApplication.isPlaying)
        {
            tabActive = GUILayout.Toolbar(tabActive, oneTab); //load one tab
            tabActive = 2;
        }
        else tabActive = GUILayout.Toolbar(tabActive, threeTab); //load three tabs

        // At first these two functions will return the color white. 
        //Later, if the tester changes color, these two functions will take care of making the changes visible
        changeBackgroundColor();
        changeContentColor();

        // The switch construct is used to load each tab, based on the user's selection. 
        // The try-catch structure in this case catches any exceptions and bugs regarding the loading
        try
        {
            // The warning is always set to false, the functions on each tab will change its value
            warnings = false;

            switch (tabActive)
            {
                case 0: loadGeneralSettingTab();
                        break;

                case 1: loadTestSettingTab();
                        break;

                case 2: loadTestsResultTab();
                        break;
            }
            
        } 
        catch { EditorGUILayout.HelpBox("Error loading information. Some files may have been moved or deleted", MessageType.Error); }

        GUILayout.EndScrollView();
    }

    // Draw the general tab
    public void loadGeneralSettingTab()
    {
        EditorGUILayout.Space(5);

        // --------- Game object management for testing ----------- //
        foldouts[0] = EditorGUILayout.BeginFoldoutHeaderGroup(foldouts[0], "REFERENCE GAME OBJECTS");

        if (foldouts[0])
        {
            // Insertion, cancellation and change of the game object to be tested
            EditorGUILayout.BeginHorizontal();
            GameObject temp = objectToTest;
            objectToTest = EditorGUILayout.ObjectField(objectToTest, typeof(GameObject), true) as GameObject;

            if (temp != objectToTest)
                changes = true;

            if (GUILayout.Button("Cancel entry"))
                objectToTest = AssetDatabase.LoadAssetAtPath<GameObject>(null) as GameObject;

            // The game object needs the TestingManager.cs script to start the tests
            if (objectToTest != null && objectToTest.GetComponent<TestingManager>() == null)
                objectToTest.AddComponent<TestingManager>();

            EditorGUILayout.EndHorizontal();
        }
        else EditorGUILayout.HelpBox("The framework will monitor the state of this object to make assertions in tests", MessageType.Info);

        EditorGUILayout.EndFoldoutHeaderGroup();
        GUILayout.Space(25);

        // --------- Testing script management ----------- //
        foldouts[1] = EditorGUILayout.BeginFoldoutHeaderGroup(foldouts[1], "TESTING SCRIPTS");

        if (foldouts[1]) 
            drawScriptsList(testScripts, "Insert one or more testing scripts");       
        else EditorGUILayout.HelpBox("The framework will use these scripts to extract the tests associated with it", MessageType.Info);

        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Space(25);


        // --------- Scripts management with parametric methods ----------- //
        foldouts[2] = EditorGUILayout.BeginFoldoutHeaderGroup(foldouts[2], "SCRIPTS FOR PARAMETRIC TESTS");

        if (foldouts[2])
        {
            drawScriptsList(attributeScripts, "Insert one or more attribute monitoring scripts");
            GUILayout.Space(25);
        }
        else EditorGUILayout.HelpBox("The framework will monitor the state of this object to make assertions in tests", MessageType.Info);

        EditorGUILayout.EndFoldoutHeaderGroup();
        GUILayout.Space(25);


        // --------- Trigger conditions script management ----------- //
        foldouts[3] = EditorGUILayout.BeginFoldoutHeaderGroup(foldouts[3], "SCRIPT FOR CUSTOM TRIGGER CONDITIONS");

        if(foldouts[3])
        {
            EditorGUILayout.BeginHorizontal();

            // Check if the script has been changed
            MonoScript t = triggerConditionsScript;
            triggerConditionsScript = EditorGUILayout.ObjectField(triggerConditionsScript, typeof(MonoScript), false) as MonoScript;
            
            if (t != triggerConditionsScript)
                changes = true;

            if (GUILayout.Button("Remove script") && triggerConditionsScript!=null)
            {
                triggerConditionsScript = null;
                changes = true; 
            }

            EditorGUILayout.EndHorizontal();

            // If the script has been inserted, check if the object to be tested has it through a getComponent, 
            // otherwise I start the add procedure.
            if (triggerConditionsScript != null)
            {
                try
                {
                    if (objectToTest.GetComponent(triggerConditionsScript.name) == null)
                        addCustomScriptToObjectToTest(triggerConditionsScript);
                } 
                catch 
                {
                    warnings = true;
                    EditorGUILayout.HelpBox("You cannot add this script", MessageType.Error);
                }

            }
        }
        else EditorGUILayout.HelpBox("The framework will add functions created by the tester for starting tests", MessageType.Info);
        
        EditorGUILayout.EndFoldoutHeaderGroup();
        GUILayout.Space(25);

        // --------- General options management ----------- //
        foldouts[4] = EditorGUILayout.BeginFoldoutHeaderGroup(foldouts[4], "GLOBAL SETTING");

        if (foldouts[4])
        {
            // check if the player exists
            if (objectToTest!=null)
            {
                // Check if the level has changed state
                bool temp = levelActive;
                levelActive = EditorGUILayout.BeginToggleGroup("There are arcade-style levels", levelActive);

                if (temp != levelActive)
                    changes = true;

                // If the layer has been activated drawing the options of adding script
                if (levelActive)
                {
                    try
                    {
                        warnings = false;
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Reference script: ");

                        // Check if the script has been changed
                        MonoScript t = levelScript;
                        levelScript = EditorGUILayout.ObjectField(levelScript, typeof(MonoScript), false) as MonoScript;

                        if (!checkSameObjects(t, levelScript))
                        {
                            changes = true;
                            indexLevelScriptAttribute = 0;
                        }
                        EditorGUILayout.EndHorizontal();

                        // check if the script is valid
                        if (levelScript != null)
                        {
                            Type tipoScript = Type.GetType(levelScript.name);

                            // check if the inserted script is in the game object
                            if (objectToTest.GetComponent(levelScript.name) != null)
                            {
                                List<string> fields = new List<string>();
                                foreach (FieldInfo i in tipoScript.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance))
                                    fields.Add(i.Name);

                                // if the script does not contain public fields I cannot create a selection
                                if (fields.Count > 0)
                                {
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField("Reference field:");
                                    indexLevelScriptAttribute = EditorGUILayout.Popup(indexLevelScriptAttribute, fields.ToArray());
                                    levelScriptAttribute = fields[indexLevelScriptAttribute];
                                    EditorGUILayout.EndHorizontal();

                                    Type typeClassParamater = Type.GetType(levelScript.name);
                                    FieldInfo fieldType = typeClassParamater.GetField(levelScriptAttribute, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);

                                    if (fieldType.FieldType != typeof(int))
                                    {
                                        warnings = true;
                                        EditorGUILayout.HelpBox("This field is not Integer", MessageType.Warning);
                                    }

                                }
                                else //script not contains public fields
                                {
                                    EditorGUILayout.HelpBox("The script not contains public fields", MessageType.Warning);
                                    warnings = true;
                                }
                            }
                            else //script not insered
                            {
                                addCustomScriptToObjectToTest(levelScript);
                            }
                        }
                        else //script not valid
                        {
                            EditorGUILayout.HelpBox("The script cannot be null", MessageType.Error);
                            warnings = true;
                        }
                    } 
                    catch 
                    {
                        warnings = true;
                        EditorGUILayout.HelpBox("You cannot add this script", MessageType.Error); 
                    }

                }

                EditorGUILayout.EndToggleGroup();

                if (!levelActive)
                    EditorGUILayout.HelpBox("If activated, this option will allow you to edit your tests even on a certain game level", MessageType.Info);
                GUILayout.Space(25);

                // 3D SETTING
                temp = threeDimensionActive;
                threeDimensionActive = EditorGUILayout.BeginToggleGroup("It is a 3D game", threeDimensionActive);
                EditorGUILayout.EndToggleGroup();

                if (temp != threeDimensionActive)
                    changes = true;
                if (!threeDimensionActive)
                    EditorGUILayout.HelpBox("If activated, this option will allow you to work on three-dimensional space", MessageType.Info);
                GUILayout.Space(25);

                // TEST PAUSE SETTING
                temp = pauseTestFailedActive;
                pauseTestFailedActive = EditorGUILayout.BeginToggleGroup("Pause when tests fail", pauseTestFailedActive);
                EditorGUILayout.EndToggleGroup();

                if (temp != pauseTestFailedActive)
                    changes = true;
                if (!pauseTestFailedActive)
                   EditorGUILayout.HelpBox("If enabled, this option will allow the game to be paused when a test fails", MessageType.Info);
                EditorGUILayout.Space(25);

                // GUI STYLE SETTING
                customInterface = EditorGUILayout.BeginToggleGroup("Custom interface style: ", customInterface);
                if(customInterface)
                {
                    GUILayout.Space(5);

                    EditorGUILayout.BeginHorizontal();

                    // Check if there have been any changes for backgroundColor
                    GUILayout.Label("Background color window");
                    int i = indexBackgroundColor;
                    indexBackgroundColor = EditorGUILayout.Popup(indexBackgroundColor, nameColors);
                    if (i != indexBackgroundColor)
                        changes = true;

                    // Check if there have been any changes for contentColor
                    GUILayout.Label("Content color window");
                    i = indexContentColor;
                    indexContentColor = EditorGUILayout.Popup(indexContentColor, nameColors);
                    if (i != indexContentColor)
                        changes = true;
                    EditorGUILayout.EndHorizontal();

                    // Restore the style to the default color: White
                    if (GUILayout.Button("Default style"))
                    {
                        indexBackgroundColor = 0;
                        indexContentColor = 0;
                        changes = true;
                    }
                }
                EditorGUILayout.EndToggleGroup();

                if(!customInterface)
                    EditorGUILayout.HelpBox("If activated, this option allows you to change the style of the interface", MessageType.Info);

                GUILayout.Space(25);            
            }
            else EditorGUILayout.HelpBox("Make sure you have entered at least 1 game object before configuring these settings.", MessageType.Warning);
        }
        else EditorGUILayout.HelpBox("General options for the whole test environment", MessageType.Info);

        GUILayout.Space(15);

        // This key takes care of loading the persistenObject status and updating the loaded tests
        if (GUILayout.Button("REVERT TO SAVED"))
        {
            loadPersistentObject();
            updateTestScriptsLoaded();
        }

        // If the user has made changes I cannot apply the changes if there are warnings present
        if (changes && !warnings)
        {
            GUI.backgroundColor = Color.cyan;

            // If there are no warnings, on click I perform the following operations
            if (GUILayout.Button("APPLY CHANGES"))
            {
                // I delete any null scripts and update the list of fields
                removeNullScripts(testScripts);
                removeNullScripts(attributeScripts);
                updateListFieldsFromScripts();

                // I save the remaining data and update
                saveData();
                loadData();
                changes = false;

                // close all open tabs
                for (int i = 0; i < foldouts.Length; i++)
                    foldouts[i] = false;
                   
            }
            GUI.backgroundColor = Color.white;
        }
        else if (changes && warnings) // If there are warnings, I show the yellow button
        {
            GUI.backgroundColor = Color.yellow;
            GUILayout.Button("RESOLVE THE WARNINGS TO SAVE");
            changeBackgroundColor();
        }

        // At each frame, these two functions are called to ensure that the user does not enter two identical scripts
        removeDuplicateScripts(testScripts);
        removeDuplicateScripts(attributeScripts);
        GUILayout.Space(25);
    }

    // Draw the test setting tab
    public void loadTestSettingTab()
    {
        // I load the tests if a valid game object and test scripts have been entered
        if (objectToTest!=null && testScripts.Count > 0)
        {
            // If there are parametric tests but no script has been entered or if there are but invalid
            if (findParameterTests() && attributeScripts.Count == 0)
                EditorGUILayout.HelpBox("Presence of parametric tests in the inserted scripts. Make sure you have inserted one or more valid scripts on the object to be tested.", MessageType.Warning);           
            else
            {
                // I only draw tests if there are any
                if (tests.Count != 0)
                {
                    warnings = false;

                    // Draw all tests with various properties
                    foreach (Test t in tests)
                    {
                        // If the test has a bad setting and has been selected there will probably have been an error loading the selected fields
                        if (t.getBadSetting() && t.getSelected())
                        {
                            GUI.backgroundColor = Color.yellow;
                            EditorGUILayout.HelpBox("I was unable to load some previously selected fields. Make sure you haven't removed the script", MessageType.Warning);
                            warnings = true;
                        }

                        // If the test changes status, the modification is activated
                        if (t.setSelected(EditorGUILayout.BeginToggleGroup(t.getMethodInfo().ToString(), t.getSelected())))
                            changes = true;

                        changeBackgroundColor();

                        if (t.getSelected())
                        {
                            // PARAMETER & PROPERTY MANAGEMENT
                            t.setParametersTabActive(EditorGUILayout.Foldout(t.getParametersTabActive(), "Parameters setting"));
                            if(t.getParametersTabActive())
                            {
                                t.setBadSetting(false);

                                // Drawing, for each parameter a selection list
                                ParameterInfo[] parameters = t.getMethodInfo().GetParameters();
                                for (int i = 0; i < parameters.Length; i++)
                                {
                                    // If I don't find any type parameters I show a warning
                                    if (t.getFilteredList(i) == null || t.getFilteredList(i).Length == 0)
                                    {
                                        EditorGUILayout.HelpBox("No fields of type were found: " + parameters[i].ParameterType, MessageType.Warning);
                                        warnings = true;
                                    }

                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField("Specify the parameter: ( " + parameters[i].Name.ToString() + " )");
                                    
                                    if (t.setIndexForParameter(i, EditorGUILayout.Popup(t.getIndexForParameter(i), t.getFilteredList(i))))
                                        changes = true;
                                    
                                    EditorGUILayout.EndHorizontal();
                                }
                            }

                            // TRIGGER CONDITIONS MANAGEMENT FOR THE START OF THE TEST
                            t.setTriggerTabActive(EditorGUILayout.Foldout(t.getTriggerConditionsActive(), "Trigger conditions"));
                            if (t.getTriggerConditionsActive())
                            {
                                EditorGUILayout.LabelField("Default trigger conditions: ", EditorStyles.boldLabel);
                               
                                //For position
                                if (t.setTriggerPosition(EditorGUILayout.Toggle("In a specific point", t.getTriggerPosition())))
                                    changes = true;

                                if (t.getTriggerPosition())
                                {
                                    EditorGUILayout.BeginHorizontal();

                                    // If the variable V is not monitored I do not draw the field to set its value

                                    GUILayout.Label("X ");
                                    if (t.setIndexOperator(0, EditorGUILayout.Popup(t.getIndexOperator(0), operators)))
                                        changes = true;
                                    if (t.getOperator(0) != Operator.DONT_CHECK)
                                        if (t.setX(EditorGUILayout.TextField(t.getX())))
                                            changes = true;


                                    GUILayout.Label("Y ");
                                    if (t.setIndexOperator(1, EditorGUILayout.Popup(t.getIndexOperator(1), operators)))
                                        changes = true;
                                    if(t.getOperator(1) != Operator.DONT_CHECK)
                                        if (t.setY(EditorGUILayout.TextField(t.getY().ToString())))
                                            changes = true;

                                    // For 3D Game
                                    if (threeDimensionActive)
                                    {
                                        GUILayout.Label("Z ");
                                        if (t.setIndexOperator(2, EditorGUILayout.Popup(t.getIndexOperator(2), operators)))
                                            changes = true;
                                        if (t.getOperator(2) != Operator.DONT_CHECK)
                                            if (t.setZ(EditorGUILayout.TextField(t.getZ().ToString())))
                                                changes = true;
                                    }

                                    //If you want set it for all tests
                                    GUI.backgroundColor = Color.yellow;
                                    if (GUILayout.Button("Apply to all selected tests"))
                                    { 
                                        applyStartPositionAllTestSelected(t);
                                        changes = true;
                                    }
                                    EditorGUILayout.EndHorizontal();
                                    changeBackgroundColor();

                                    checkPositionFields(t);
                                }
                                
                                //For level
                                if (levelActive)
                                {
                                    // Check if a valid insertion script exists
                                    if (levelScript==null)
                                        EditorGUILayout.HelpBox("it looks like you haven't entered a valid script associated with the level", MessageType.Error);                               
                                    else
                                    {
                                        if (t.setTriggerLevel(EditorGUILayout.Toggle("In a specific level", t.getTriggerLevel())))
                                            changes = true;

                                        if (t.getTriggerLevel())
                                        {
                                            EditorGUILayout.BeginHorizontal();
                                            if(t.setLevel(EditorGUILayout.IntSlider("Level: ", t.getLevel(), 1, 100)))
                                                changes = true;
                                            GUI.backgroundColor = Color.yellow;

                                            if (GUILayout.Button("Apply to all selected tests"))
                                            {
                                                applyLevelAllTestSelected(t.getLevel());
                                                changes = true;
                                            }
                                            EditorGUILayout.EndHorizontal();
                                            changeBackgroundColor();
                                        }
                                    }
                                }

                                //For custom trigger conditions
                                if (triggerConditionsScript != null)
                                {
                                    EditorGUILayout.LabelField("Custom trigger conditions: ", EditorStyles.boldLabel);
                                    foreach (TriggerCondition tc in t.getTriggerConditions())
                                        if (tc.setSelected(EditorGUILayout.Toggle(tc.getMethodInfo().Name, tc.getSelected())))
                                            changes = true;
                                }
                            }

                            // CONSTRAINTS MANAGEMENT
                            t.setConstraintsActive(EditorGUILayout.Foldout(t.getConstraintsActive(), "Constraints"));
                            if (t.getConstraintsActive())
                            {
                                //Frame limit
                                GUILayout.BeginHorizontal();
                                if (t.setLimitFrame(EditorGUILayout.IntSlider("Frame limit: ", t.getFrameLimit(), 0, 99999)))
                                    changes = true;

                                GUI.backgroundColor = Color.yellow;
                                if (GUILayout.Button("Apply to all selected tests"))
                                    applyLimitFrameAllTestSelected(t.getFrameLimit());
                                changeBackgroundColor();

                                GUILayout.EndHorizontal();
                                EditorGUILayout.HelpBox("Frame limit within which the test must return a result. If set to 0 the limit will not be considered.", MessageType.None);

                                //Number of ripetitions
                                GUILayout.BeginHorizontal();
                                if (t.setNumberOfRipetitions(EditorGUILayout.IntSlider("Number of ripetions: ", t.getNumberOfRipetitions(), 0, 999)))
                                    changes = true;

                                GUI.backgroundColor = Color.yellow;
                                if (GUILayout.Button("Apply to all selected tests"))
                                    applyNumberOfRipetitionsAllTestSelected(t.getNumberOfRipetitions());
                                changeBackgroundColor();

                                GUILayout.EndHorizontal();
                                EditorGUILayout.HelpBox("Number of times the test will be rerun. If set to 0 it will always be performed.", MessageType.None);


                                //Refresh time
                                if (t.getNumberOfRipetitions() != 1)
                                {
                                    GUILayout.BeginHorizontal();
                                    if (t.seteRefreshTime(EditorGUILayout.Slider("Refresh time", t.getRefreshTime(), 0.01f, 60)))
                                        changes = true;

                                    GUI.backgroundColor = Color.yellow;
                                    if (GUILayout.Button("Apply to all selected tests"))
                                        applyRefreshTimeAllTestSelected(t.getRefreshTime());
                                    changeBackgroundColor();

                                    GUILayout.EndHorizontal();
                                    EditorGUILayout.HelpBox("Time between the test result and its re-execution.", MessageType.None);
                                }
                                else EditorGUILayout.HelpBox("You cannot set a refresh time if the number of repetitions is equal to 1", MessageType.Info);
                            }

                            GUILayout.Space(25);
                        }

                        EditorGUILayout.EndToggleGroup();
                    }

                    GUILayout.Space(25);
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("SELECT ALL")) setAllTestState(true);
                    if (GUILayout.Button("DESELECT ALL")) setAllTestState(false);
                    GUILayout.EndHorizontal();

                    // If there are no warnings I can save
                    if (changes && !warnings)
                    {
                        GUI.backgroundColor = Color.cyan;
                        if (GUILayout.Button("APPLY CHANGES"))
                        {
                            changes = false;
                            warnings = false;
                            saveData();
                            loadData();
                        }
                        GUI.backgroundColor = Color.white;
                    }
                    else if (warnings)
                    {
                        GUI.backgroundColor = Color.yellow;
                        if (GUILayout.Button("RESOLVE THE WARNINGS TO SAVE")) { }
                        GUI.backgroundColor = Color.white;
                        GUILayout.Space(25);
                    }
                
                } else EditorGUILayout.HelpBox("It appears that the scripts entered have no testing methods. Also make sure you have saved your changes in the general tab.", MessageType.Error);
            }
        } else EditorGUILayout.HelpBox("Make sure you have selected valid testing scripts and a test object.", MessageType.Warning);
        GUILayout.Space(25);

    }

    // Draw the tests result tab
    public static void loadTestsResultTab()
    {
        // I only draw if tests exist
        if (tests.Count>0)
        {
            // Option and property titles
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Selected test: result", EditorStyles.foldoutHeader);
            EditorGUILayout.LabelField("Frame limit ", EditorStyles.foldoutHeader);
            EditorGUILayout.LabelField("Repeated - Refresh Time", EditorStyles.foldoutHeader);
            EditorGUILayout.EndHorizontal();

            // For each test I draw its properties
            foreach (Test test in tests)
                if (test.getSelected())
                {
                    // I draw only if it has been configured correctly
                    if (!test.getBadSetting())
                    {
                        EditorGUILayout.Space(15);

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(test.getMethodInfo().Name.ToString() + ": " + changeTestState(test), EditorStyles.miniButton);

                        if (test.getFrameLimit() > 0)
                            EditorGUILayout.LabelField(test.getActualFrame() + " / " + test.getFrameLimit(), EditorStyles.miniButton);
                        else
                            EditorGUILayout.LabelField("none", EditorStyles.miniButton);

                        if (test.getNumberOfRipetitions() > 0)
                            EditorGUILayout.LabelField(test.getActualRipetition().ToString() + " / " + test.getNumberOfRipetitions() + " - " + test.getRefreshTime().ToString() + " second", EditorStyles.miniButton);
                        else
                            EditorGUILayout.LabelField("always - " + test.getRefreshTime().ToString() + " second", EditorStyles.miniButton);

                        GUI.backgroundColor = Color.white;
                        EditorGUILayout.EndHorizontal();

                        if (!test.getErrorMessage().Equals("none"))
                            EditorGUILayout.HelpBox(test.getErrorMessage(), MessageType.Warning);

                    }
                    else EditorGUILayout.HelpBox("Bad test setup for: " + test.getMethodInfo().Name.ToString(), MessageType.Error);
                  
                EditorGUILayout.Space(20);
                }
        }
        else EditorGUILayout.HelpBox("There are currently no tests configured.", MessageType.Warning);
    }

    // It deals with designing a mini interface for adding a new component script on the object to be tested
    public void addCustomScriptToObjectToTest(MonoScript script)
    {
        Type type = Type.GetType(script.name);

        EditorGUILayout.HelpBox("The object to be tested: " + objectToTest.name + " does not have a component named " + type.Name + ".cs", MessageType.Warning);
        warnings = true;

        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("Do you want to add it?"))
            objectToTest.AddComponent(Type.GetType(type.Name));
        GUI.backgroundColor = Color.white;
    }

    // Change the background color according to the choice index
    public void changeBackgroundColor()
    {
        switch (nameColors[indexBackgroundColor])
        {
            case "Red":   GUI.backgroundColor = Color.red;
                          break;
            case "Blue":  GUI.backgroundColor = Color.blue; 
                          break;
            case "Yellow":GUI.backgroundColor = Color.yellow;
                          break;
            case "White": GUI.backgroundColor = Color.white;
                          break;
            case "Grey":  GUI.backgroundColor = Color.gray;
                          break;
            case "Magenta": GUI.backgroundColor = Color.magenta;
                            break;
            case "Green": GUI.backgroundColor = Color.green;
                          break;
            case "Cyan":  GUI.backgroundColor = Color.cyan;
                          break;
        }
    }

    // Change the content color according to the choice index
    public void changeContentColor()
    {
        switch (nameColors[indexContentColor])
        {
            case "Red":    GUI.contentColor = Color.red;
                           break;
            case "Blue":   GUI.contentColor = Color.blue;
                           break;
            case "Yellow": GUI.contentColor = Color.yellow;
                           break;
            case "White":  GUI.contentColor = Color.white;
                           break;
            case "Grey":   GUI.contentColor = Color.gray;
                           break;
            case "Magenta": GUI.contentColor = Color.magenta;
                            break;
            case "Green":  GUI.contentColor = Color.green;
                           break;
            case "Cyan":   GUI.contentColor = Color.cyan;
                           break;
        }
    }

    // Returns an array of fields of type parameterType
    public static string[] createFilteredListForParameter(Type parameterType)
    {
        List<string> list = new List<string>();

        // I scan all the loaded parameters.
        for (int i = 0; i<allFieldNamesLoaded.Count; i++)
        {
            // The string is split, as it is in the format NAME - NAME_SCRIPT
            Type typeClassParamater = Type.GetType(allClassNamesFieldsLoaded[i]);
            string[] parameterName = allFieldNamesLoaded[i].Split(' ');
            FieldInfo fieldType = typeClassParamater.GetField(parameterName[0], BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

            // If it is the same I add it to the list
            if (fieldType.FieldType == parameterType)
                list.Add(allFieldNamesLoaded[i]);
        }

        return list.ToArray();
    }

    // It deals with designing an interface to be able to add / remove scripts
    public void drawScriptsList(List<MonoScript> list, string title)
    {
        // For each element of the drawing list these components
        for (int i = 0; i < list.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            // Check if the user has entered or changed a script for starting changes
            MonoScript temp = list[i];
            list[i] = EditorGUILayout.ObjectField(list[i], typeof(MonoScript), false) as MonoScript;

            if (list[i] != temp)
                changes = true;

            if (GUILayout.Button("Remove script"))
            {
                list.Remove(list[i]);
                changes = true;
            }
            EditorGUILayout.EndHorizontal();

            try
            {
                // check if the object to test has the scripts inserted
                if (title.Equals("Insert one or more attribute monitoring scripts"))
                    if (!list[i].name.Equals("") && objectToTest.GetComponent(list[i].name) == null)
                    {
                        addCustomScriptToObjectToTest(list[i]);
                        warnings = true;
                    }
            }
            catch 
            {
                warnings = true;
                EditorGUILayout.HelpBox("You cannot add this script", MessageType.Error); 
            }
        }

        if (!warnings)
        {
            // Adding and removing a script
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add new script"))
            {
                changes = true;
                list.Add(new MonoScript());
            }
            if (GUILayout.Button("Clear inserted scripts"))
            {
                changes = true;
                list.Clear();
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    // Returns a list of triggerConditions
    public static List<TriggerCondition> createTriggerConditionsList()
    {
        List<TriggerCondition> triggerConditions = new List<TriggerCondition>();

        // If the script is not null I fill in the list of triggerConditions that I find in the script
        if (triggerConditionsScript != null)
        {
            // Get all methods from inserted script
            MethodInfo[] scriptMethods = Type.GetType(triggerConditionsScript.name).GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            foreach (MethodInfo triggerCondition in scriptMethods)
            {
                string[] splitted = triggerCondition.GetBaseDefinition().ToString().Split(' ');

                try
                {
                    // I only add the Boolean functions to the list
                    if (splitted[0].Equals("Boolean"))
                        triggerConditions.Add(new TriggerCondition(triggerCondition));

                } catch { }
            }
        }

        return triggerConditions;
    }

    // ------------------------------------ SCRIPT FUNCTIONS ------------------------------------- //

    // Tester cannot insert duplicate scripts (same name)
    // This method is valid when a user is still setting up his scripts
    public static void removeDuplicateScripts(List<MonoScript> scripts)
    {
        MonoScript target = null;

        if (scripts.Count >= 1)
        {
            for (int i = 0; i < scripts.Count - 1; i++)
                for (int j = i + 1; j < scripts.Count; j++)
                {
                    if (scripts[i].name.Equals(scripts[j].name))
                        target = scripts[j];
                }
        }

        if (target != null)
            scripts.Remove(target);
    }

    // The tester cannot leave null scripts
    // This method is called only if you want to save the changes
    public static void removeNullScripts(List<MonoScript> scripts)
    {
        if (scripts.Count > 0)
            if (scripts[0].name.Equals(""))
                scripts.RemoveAt(0);

        for (int i = 0; i < scripts.Count; i++)
            if (scripts[i].name.Equals(""))
            {
                i = i - 1;
                scripts.RemoveAt(i + 1);
            }

    }

    // Update all fields found
    public static void updateListFieldsFromScripts()
    {
        allFieldNamesLoaded.Clear();
        allClassNamesFieldsLoaded.Clear();
        foreach (MonoScript s in attributeScripts)
        {
            try
            {
                if (!s.name.Equals(""))
                {
                    foreach (FieldInfo attributo in Type.GetType(s.name).GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance))
                    {
                        allFieldNamesLoaded.Add(attributo.Name.ToString()+" - "+s.name);
                        allClassNamesFieldsLoaded.Add(s.name);
                    }
                }
            }
            catch { EditorGUILayout.HelpBox("The selected object is not: Mono MonoBehavior", MessageType.Error); }
        }
    }

    // ------------------------------------ GLOBAL OPTION FUNCTIONS FOR TESTS ------------------------------------- //

    // Apply a condition position trigger set to all selected tests
    public void applyStartPositionAllTestSelected(Test t)
    {
        foreach (Test test in tests)
            if(test.getSelected())
            {
                test.setTriggerPosition(true);
                test.setX(t.getX());
                test.setY(t.getY());

                if(threeDimensionActive)
                    test.setZ(t.getZ());

                test.setIndexOperator(0, t.getIndexOperator(0));
                test.setIndexOperator(1, t.getIndexOperator(1));
                test.setIndexOperator(2, t.getIndexOperator(2));
            }     
    }

    // Apply a condition position level set to all selected tests
    public void applyLevelAllTestSelected(int level)
    {
        foreach (Test test in tests)
            if (test.getSelected())
                test.setLevel(level);
    }
    
    // Apply a refreshTime set to all selected tests
    public void applyRefreshTimeAllTestSelected(float time)
    {
        foreach (Test test in tests)
            if (test.getSelected())
                test.seteRefreshTime(time);                
    }

    // Apply a number of ripetion set to all selected tests
    public void applyNumberOfRipetitionsAllTestSelected(int number)
    {
        foreach (Test test in tests)
            if (test.getSelected())
                test.setNumberOfRipetitions(number);       
    }

    // Apply a limitFrame set to all selected tests
    public void applyLimitFrameAllTestSelected(int limit)
    {
        foreach (Test test in tests)        
            if (test.getSelected())
                test.setLimitFrame(limit);
    }

    // Returns true if the tests have parameters
    public bool findParameterTests()
    {
        foreach (Test test in tests)
            if (test.getMethodInfo().GetParameters().Length > 0)
                return true;

        return false;
    }

    // change the state with a color of an a test
    public static string changeTestState(Test test)
    {
        switch (test.getTestState())
        {
            case TestState.RUN:    GUI.backgroundColor = Color.yellow;
                                   return "Running";
            case TestState.PASSED: GUI.backgroundColor = Color.green;
                                   return "Passed";
            case TestState.FAILED: GUI.backgroundColor = Color.red;
                                   return "Failed";
        }

        return "Waiting";
    }

    // Set all tests to a state with the value isSelected
    public void setAllTestState(bool state)
    {
        foreach (Test t in tests)
            t.setSelected(EditorGUILayout.Toggle(t.getMethodInfo().ToString(), state));
    }

    // It is called when we open the interface
    public void Awake() { loadData(); }

    // It is called when destroy the interface
    void OnDestroy() { saveData(); }

    // Call at each frame
    public void Update() { Repaint(); }

    // ------------------------------------ SAVE DATA ------------------------------------- //

    // Data backup can only be called up in editor mode. 
    // It updates the state of the persistentObject and saves the state of the test setting tab
    public static void saveData()
    {
        if (!EditorApplication.isPlaying)
        {
            updatePersistentObject();
            saveTestSettingTab();
        }
    }

    // Loading information is done only once in playback mode.
    public static void loadData()
    {
        // I only load the tests once
        if (!testsLoaded && EditorApplication.isPlaying)
        {
            loadPersistentObject();
            testsLoaded = true; // Initially this variable is false
        }

        // This function is always called during editor mode
        updateTestScriptsLoaded();
    }

    // Update the state of the persistentObject by fetching data from the interface
    public static void updatePersistentObject()
    {
        // I clean up the lists
        persistentObject.testScripts.Clear();
        persistentObject.fieldScripts.Clear();
        persistentObject.allNameClassFieldsLoaded.Clear();

        // I set the object, the level script and the triggerConditions
        persistentObject.objectToTest = objectToTest;
        persistentObject.levelScript = levelScript;
        persistentObject.triggerConditionScript = triggerConditionsScript;

        // I set general options
        persistentObject.levelActive = levelActive;
        persistentObject.indexFieldLevelScript = indexLevelScriptAttribute;
        persistentObject.nameFieldLevelScript = levelScriptAttribute;

        persistentObject.threeDimensionActive = threeDimensionActive;
        persistentObject.pauseWhenTestsFail = pauseTestFailedActive;

        persistentObject.customInterface = customInterface;
        persistentObject.indexBackgroundColor = indexBackgroundColor;
        persistentObject.indexContentColor = indexContentColor;

        // I fill the list of script tests
        foreach (MonoScript script in testScripts)
            persistentObject.testScripts.Add(script);

        // I fill the list of script attributes
        foreach (MonoScript script in attributeScripts)
            persistentObject.fieldScripts.Add(script);

        // I fill the list with all the names uploaded
        foreach (string nameClass in allClassNamesFieldsLoaded)
            persistentObject.allNameClassFieldsLoaded.Add(nameClass);

        // I save the object through a JsonUtility
        PlayerPrefs.SetString("scripts", JsonUtility.ToJson(persistentObject));
    }

    // Load the state of the persistentObject. This function can be called. 
    // Following a save, a revert, or just once during playback mode
    public static void loadPersistentObject()
    {
        // I clean up the lists
        testScripts.Clear();
        attributeScripts.Clear();
        allClassNamesFieldsLoaded.Clear();

        // I load the object
        persistentObject = JsonUtility.FromJson<PersistentObject>(PlayerPrefs.GetString("scripts"));
        JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString("scripts"), persistentObject);

        // I fill in the lists and update the attribute fields
        foreach (MonoScript script in persistentObject.testScripts)
            testScripts.Add(script);
        foreach (MonoScript script in persistentObject.fieldScripts)
            attributeScripts.Add(script);

        updateListFieldsFromScripts();

        foreach (string nameClass in persistentObject.allNameClassFieldsLoaded)
            allClassNamesFieldsLoaded.Add(nameClass);

        // I take the item and reinsert the component if it was removed
        objectToTest = persistentObject.objectToTest;

        if (objectToTest != null && objectToTest.GetComponent<TestingManager>() == null)
            objectToTest.AddComponent<TestingManager>();

        // I take the level script and the triggerConditions
        levelScript = persistentObject.levelScript;
        triggerConditionsScript = persistentObject.triggerConditionScript;

        // I set the general options
        levelActive = persistentObject.levelActive;
        indexLevelScriptAttribute = persistentObject.indexFieldLevelScript;
        levelScriptAttribute = persistentObject.nameFieldLevelScript;

        threeDimensionActive = persistentObject.threeDimensionActive;
        pauseTestFailedActive = persistentObject.pauseWhenTestsFail;

        customInterface = persistentObject.customInterface;
        indexBackgroundColor = persistentObject.indexBackgroundColor;
        indexContentColor = persistentObject.indexContentColor;
    }

    // Save the configuration of the test setting tab
    public static void saveTestSettingTab()
    {
        foreach (Test t in tests)
        {
            // OPTIONS
            EditorPrefs.SetBool(t.getFullName(), t.getSelected());
            EditorPrefs.SetBool(t.getFullName()+"badSetting", t.getBadSetting());
            EditorPrefs.SetBool(t.getFullName() + "triggerPosition", t.getTriggerPosition());
            EditorPrefs.SetBool(t.getFullName() + "triggerLevel", t.getTriggerLevel());
            EditorPrefs.SetInt(t.getFullName() + "level", t.getLevel());
            EditorPrefs.SetString(t.getFullName() + "posX", t.getX());
            EditorPrefs.SetString(t.getFullName() + "posY", t.getY());
            EditorPrefs.SetString(t.getFullName() + "posZ", t.getZ());
            EditorPrefs.SetInt(t.getFullName() + "operatorX", t.getIndexOperator(0));
            EditorPrefs.SetInt(t.getFullName() + "operatorY", t.getIndexOperator(1));
            EditorPrefs.SetInt(t.getFullName() + "operatorZ", t.getIndexOperator(2));
            EditorPrefs.SetInt(t.getFullName() + "limitFrame", t.getFrameLimit());
            EditorPrefs.SetInt(t.getFullName() + "numberOfRipetitions", t.getNumberOfRipetitions());
            EditorPrefs.SetFloat(t.getFullName() + "refreshTime", t.getRefreshTime());

            // PARAMETERS
            ParameterInfo[] parameters = t.getMethodInfo().GetParameters();
            if (parameters.Length > 0)
                for (int i = 0; i < parameters.Length; i++)
                {
                    try
                    {
                        EditorPrefs.SetInt(t.getFullName() + ": " + parameters[i].Name, t.getIndexForParameter(i));
                        EditorPrefs.SetString(t.getFullName() + ": " + parameters[i].Name + ": ", t.getFieldNameFromFilteredList(i));

                    } catch { }
                }

            // TRIGGER CONDITIONS
            foreach (TriggerCondition tc in t.getTriggerConditions())
                EditorPrefs.SetBool(t.getFullName() + " - " + tc.getMethodInfo().Name, tc.getSelected());           
        }
    }

    // Update the status of the testScripts list
    public static void updateTestScriptsLoaded()
    {
        tests.Clear();
        foreach (MonoScript script in testScripts)
        {
            try
            {
                MethodInfo[] allMethods = Type.GetType(script.name).GetMethods(); // take all the methods of the script

                // For each method enter in the list only those with the [Test] attribute
                foreach (MethodInfo method in allMethods)
                    if (method.GetCustomAttribute<TestAttribute>().ToString().Equals("NUnit.Framework.TestAttribute"))
                        tests.Add(new Test(method, script.name, createTriggerConditionsList()));
                                  
            } catch { }
        }

        // For each test loaded I try to load a previously set setting
        foreach (Test test in tests)
        {
            test.setBadSetting(false);

            // LOAD PARAMETERS SETTING
            ParameterInfo[] parameters = test.getMethodInfo().GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                // I create the filtered lists for each parameter
                test.setFilteredList(i, createFilteredListForParameter(parameters[i].ParameterType));

                // I set the index on the basis of the memorized correspondence and take its value
                test.setIndexForParameter(i, EditorPrefs.GetInt(test.getFullName() + ": " + parameters[i].Name, 0));
                int valueForParameter = test.getIndexForParameter(i);

                try
                {
                    // Check if the indexes for the fields are the same or have changed after adding / removing scripts
                    string old = EditorPrefs.GetString(test.getFullName() + ": " + parameters[i].Name + ": ");
                    string current = test.getFieldNameFromFilteredList(i);

                    // If they are different I try to do a rematching again
                    if (!old.Equals(current))
                        if (!tryToRematchParameter(test, old, i))
                            test.setBadSetting(true);

                } 
                catch  
                { 
                    if (!tryToRematchParameter(test, EditorPrefs.GetString(test.getFullName() + ": " + parameters[i].Name + ": "), i)) 
                        test.setBadSetting(true);  
                }
            }

            // LOAD OPTIONS
            test.setSelected(EditorPrefs.GetBool(test.getFullName(), false));
            test.setTriggerPosition(EditorPrefs.GetBool(test.getFullName() + "triggerPosition", false));
            test.setTriggerLevel(EditorPrefs.GetBool(test.getFullName() + "triggerLevel", false));
            test.setLevel(EditorPrefs.GetInt(test.getFullName() + "level", 1));
            test.setX(EditorPrefs.GetString(test.getFullName() + "posX", "0"));
            test.setY(EditorPrefs.GetString(test.getFullName() + "posY", "0"));
            test.setZ(EditorPrefs.GetString(test.getFullName() + "posZ", "0"));
            test.setIndexOperator(0, EditorPrefs.GetInt(test.getFullName() + "operatorX", 0));
            test.setIndexOperator(1, EditorPrefs.GetInt(test.getFullName() + "operatorY", 0));
            test.setIndexOperator(2, EditorPrefs.GetInt(test.getFullName() + "operatorZ", 0));
            test.setLimitFrame(EditorPrefs.GetInt(test.getFullName() + "limitFrame", 0));
            test.setNumberOfRipetitions(EditorPrefs.GetInt(test.getFullName() + "numberOfRipetitions",0));
            test.seteRefreshTime(EditorPrefs.GetFloat(test.getFullName() + "refreshTime", 0.1f));
                
            // LOAD TRIGGER CONDITIONS
            foreach (TriggerCondition tc in test.getTriggerConditions())
                tc.setSelected(EditorPrefs.GetBool(test.getFullName() + " - " + tc.getMethodInfo().Name, false));
        }
    }

    // This function remakes an association between selected fields and indexes of filtered lists during a scripts update
    public static bool tryToRematchParameter(Test test, string nameField, int parameterIndex)
    {
        for (int i = 0; i < test.getFilteredList(parameterIndex).Length; i++)
            if (test.getFilteredList(parameterIndex)[i].Equals(nameField))
            {
                test.setIndexForParameter(parameterIndex, i);
                return true;
            }

        return false;
    }


    // ------------------------------------ UTILITY ------------------------------------- //

    // Returns true if 2 objects are equal
    public bool checkSameObjects(object o1, object o2)
    {
        if (o1 != o2)
            return false;

        return true;
    }

    // Checks if the values entered in the fields of a trigger condition position are real numbers
    public void checkPositionFields(Test test)
    {
        float cordinataIntera;

        if (float.TryParse(test.getX(), out cordinataIntera))  test.setX(cordinataIntera.ToString());
        else test.setX("0");

        if (float.TryParse(test.getY(), out cordinataIntera)) test.setY(cordinataIntera.ToString());
        else test.setY("0");

        if (float.TryParse(test.getZ(), out cordinataIntera))  test.setZ(cordinataIntera.ToString());
        else test.setZ("0");
    }


    // ------------------------------- CLASS PROPERTIES ------------------------------- //

    public static bool p_levelActive { get { return levelActive; } }
    public static bool p_threeDimensionActive { get { return threeDimensionActive; } }
    public static bool p_pauseTestFailedActive { get { return pauseTestFailedActive; } }
    public static GameObject p_objectToTest { get { return objectToTest; } }
    public static List<Test> p_tests { get { return tests; } }
    public static List<MonoScript> p_testScripts { get { return testScripts; } }
    public static List<MonoScript> p_attributeScripts { get { return attributeScripts; } }
    public static List<string> p_allFieldNamesLoaded { get { return allFieldNamesLoaded; } }
    public static List<string> p_allClassNamesFieldsLoaded { get { return allClassNamesFieldsLoaded; } }
    public static MonoScript p_levelScript { get { return levelScript; } }
    public static MonoScript p_triggerConditionsScript { get { return triggerConditionsScript; } }
    public static string p_levelScriptAttribute { get { return levelScriptAttribute; } }

}
