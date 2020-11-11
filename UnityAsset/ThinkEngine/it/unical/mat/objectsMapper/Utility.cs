using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;


public static class Utility
{
    private static bool prefabsLoaded = false;
    private static string _triggerClassPath= @".\Assets\Scripts\Trigger.cs";
    private static MethodInfo[] _triggerMethods;
    private static List<string> _triggerMethodsToShow;
    internal static object _triggerClass;
    private static GameObject _hiddenGameObject;
    private static bool _managersDestroyed;
    private static SensorsManager _sensorsManager;
    private static ActuatorsManager _actuatorsManager;
    internal static List<string> triggerMethodsToShow
    {
        get
        {
            checkTriggerClass();
            _triggerMethodsToShow = findMethodsToShow();
            return _triggerMethodsToShow;
        }
        set
        {
            _triggerMethodsToShow = value;
        }
    }
    internal static object triggerClass
    {
        get
        {
            checkTriggerClass();
            _triggerClass = ScriptableObject.CreateInstance("Trigger");
            return _triggerClass;
        }
        set
        {
            _triggerClass = value;
        }
    }
    internal static GameObject hiddenGameObject
    {
        get
        {
            if (_hiddenGameObject == null)
            {
                _hiddenGameObject = GameObject.Find("Utility");
                if (_hiddenGameObject==null)
                {
                    _hiddenGameObject = new GameObject("Utility");
                    //_hiddenGameObject.hideFlags = HideFlags.HideInHierarchy & HideFlags.HideInInspector;
                }
            }
            return _hiddenGameObject;
        }
    }
    internal static bool managersDestroyed
    {
        get
        {
            return SensorsManager.destroyed || ActuatorsManager.destroyed;
        }
    }
    internal static SensorsManager sensorsManager
    {
        get
        {
            if (_sensorsManager == null)
            {
                _sensorsManager = hiddenGameObject.GetComponent<SensorsManager>();
                if (_sensorsManager == null)
                {
                    Debug.Log("i'm trying to add a sensorsmanager");
                    _sensorsManager = hiddenGameObject.AddComponent<SensorsManager>();
                }
            }
            return _sensorsManager;
        }
    }
    internal static ActuatorsManager actuatorsManager
    {
        get
        {
            if (_actuatorsManager == null)
            {
                _actuatorsManager = hiddenGameObject.GetComponent<ActuatorsManager>();
                if (_actuatorsManager == null)
                {
                    _actuatorsManager = hiddenGameObject.AddComponent<ActuatorsManager>();
                }
            }
            return _actuatorsManager;
        }
    }
    internal static int getTriggerMethodIndex(string name)
    {
        int index = triggerMethodsToShow.IndexOf(name);
        if (index != -1)
        {
            return index;
        }
        return triggerMethodsToShow.Count;
    }

    internal static string triggerClassPath
    {
        get
        {
            checkTriggerClass();
            return _triggerClassPath;
        }
        set
        {
            _triggerClassPath = value;
        }
    }

    private static List<string> findMethodsToShow()
    {
        _triggerMethods = triggerClass.GetType().GetMethods();
        List<string> toReturn = new List<string>();

        foreach (MethodInfo mI in _triggerMethods)
        {
            if (mI.ReturnType == typeof(bool) && mI.GetParameters().Length == 0)
            {
                toReturn.Add(mI.Name);
            }
        }
        return toReturn;
    }

    internal static MethodInfo getTriggerMethod(int chosenMethod)
    {
        if(chosenMethod>_triggerMethods.Length || chosenMethod < 0)
        {
            return null;
        }
        return _triggerMethods[chosenMethod];
    }
    internal static MethodInfo getTriggerMethod(string chosenMethod)
    {
        if (!triggerMethodsToShow.Contains(chosenMethod))
        {
            return null;
        }
        return _triggerMethods[triggerMethodsToShow.IndexOf(chosenMethod)];
    }

    internal static void checkTriggerClass()
    {
        if (!Directory.Exists(@"Assets\Scripts"))
        {
            Directory.CreateDirectory(@"Assets\Scripts");
        }
        if (!File.Exists(_triggerClassPath))
        {
            createTriggerScript();
        }
    }
    private static void createTriggerScript()
    {
        using (FileStream fs = File.Create(_triggerClassPath))
        {
            string triggerClassContent = "using System;\n";
            triggerClassContent += "using UnityEngine;\n\n";
            triggerClassContent += @"// every method of this class without parameters and that returns a bool value can be used to trigger the reasoner.";
            triggerClassContent += "\n public class Trigger:ScriptableObject{\n\n";
            triggerClassContent += "}";
            Byte[] info = new UTF8Encoding(true).GetBytes(triggerClassContent);
            fs.Write(info, 0, info.Length);
        }
        AssetDatabase.Refresh();
    }

    internal static void loadPrefabs()
    {
        if (!prefabsLoaded)
        {
            Resources.LoadAll<GameObject>("Prefabs");
        }
    }
}

