using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/*
 * This class takes care of storing the data entered by the user from TAB-GENERAL configuration.
 * During the recompilation, the data previously entered in the interface is lost. 
 * Through the REVERT TO SAVED function it will be possible to restore the configuration through this class.
 */

[Serializable]
public class PersistentObject
{
    public GameObject objectToTest; //save the object
    public List<MonoScript> testScripts; //save the test scripts
    public List<MonoScript> fieldScripts; //save the field scripts
    public List<string> allNameClassFieldsLoaded; // save all loaded fields
    public MonoScript levelScript; //save level script for trigger condition level
    public MonoScript triggerConditionScript; //save trigger condition script
    public bool levelActive; //save the state of the trigger condition level
    public bool threeDimensionActive; //save the state of 3D - Game
    public bool pauseWhenTestsFail; //save the state of Pause when test fail
    public bool customInterface; //save the state of interface style
    public int indexBackgroundColor, indexContentColor; //save the index of background and content color
    public int indexFieldLevelScript; //save index field from levelScript
    public string nameFieldLevelScript; //save name field from levelScript

    //The constructor takes care of creating the empty lists. The other fields will be taken from the interface status    
    public PersistentObject()
    {
        testScripts = new List<MonoScript>();
        fieldScripts = new List<MonoScript>();
        allNameClassFieldsLoaded = new List<string>();
    }

}