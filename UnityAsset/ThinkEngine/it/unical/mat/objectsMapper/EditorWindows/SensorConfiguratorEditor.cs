using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Threading;
using EmbASP4Unity.it.unical.mat.objectsMapper.EditorWindows;
using EmbASP4Unity.it.unical.mat.objectsMapper;
using System.IO;

[CustomEditor(typeof(SensorConfiguration))]
public class SensorConfiguratorEditor : AbstractConfigurationEditor
{

    public override void OnInspectorGUI()
    {
        try
        {
            if (!objectMode)
            {
                base.OnInspectorGUI();
            }
            else
            {
                drawObjectProperties();
            }
        }catch(Exception e)
        {
            MyDebugger.MyDebug(e.StackTrace);
            reset(true);
        }
    }

    new void Reset()
    {
        base.Reset();
        typeOfConfigurator = "Sensor";        
    }
    
    
    

    /*void OnDisable()
    {
        if (!Directory.Exists("Assets/Resources/Sensors"))
        {
            Directory.CreateDirectory("Assets/Resources/Sensors");
        }
        if (AssetDatabase.LoadAssetAtPath("Assets/Resources/SensorsManager.asset", typeof(SensorsManager)) == null)
        {
            AssetDatabase.CreateAsset((SensorsManager)manager, "Assets/Resources/SensorsManager.asset");
        }
        else
        {
            EditorUtility.SetDirty((SensorsManager)manager);
            AssetDatabase.SaveAssets();
        }
        foreach (AbstractConfiguration conf in ((SensorsManager)manager).confs())
        {
            SensorConfiguration sensorConf = (SensorConfiguration)conf;
            if (AssetDatabase.LoadAssetAtPath("Assets/Resources/Sensors/" + sensorConf.configurationName + ".asset", typeof(SensorConfiguration)) == null)
            {
                AssetDatabase.CreateAsset(sensorConf, "Assets/Resources/Sensors/" + sensorConf.configurationName + ".asset");
            }
            else
            {
                EditorUtility.SetDirty(sensorConf);
                AssetDatabase.SaveAssets();
            }
        }
    }*/

   

   
    internal override void addCustomFields(FieldOrProperty obj)
    {
        
        if (tracker.ObjectsToggled[obj])
        {
            if (!tracker.operationPerProperty.ContainsKey(obj))
            {
                tracker.operationPerProperty.Add(obj, 0);
            }
            
            tracker.operationPerProperty[obj] = EditorGUILayout.Popup(tracker.operationPerProperty[obj], Enum.GetNames(Operation.getOperationsPerType(obj.Type())));
            if(tracker.operationPerProperty[obj]== Operation.SPECIFIC)//thus is a specific_value
            {
                if (!tracker.specificValuePerProperty.ContainsKey(obj))
                {
                    tracker.specificValuePerProperty.Add(obj, "");
                }
                tracker.specificValuePerProperty[obj] = EditorGUILayout.TextField("Value to track", tracker.specificValuePerProperty[obj]);
            }
           

        }
    }
}
