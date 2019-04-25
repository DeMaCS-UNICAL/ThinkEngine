using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Threading;
using EmbASP4Unity.it.unical.mat.objectsMapper.SensorsScripts;
using EmbASP4Unity.it.unical.mat.objectsMapper.EditorWindows;
using EmbASP4Unity.it.unical.mat.objectsMapper;

[Serializable]
public class SensorConfigurationWindow : AbstractConfigurationWindow
{

    

    [MenuItem("Window/Sensor Configuration Window")]
    public static void Init()
    {
        //Debug.Log("going to show");
        EditorWindow.GetWindow(typeof(SensorConfigurationWindow));
        //Debug.Log("showed");
    }


    void OnEnable()
    {
        if (AssetDatabase.LoadAssetAtPath("Assets/Resources/SensorsManager.asset", typeof(SensorsManager)) == null)
        {
            //Debug.Log("manager not found");
            manager = CreateInstance<SensorsManager>();
        }
        else
        {
            manager = (SensorsManager)AssetDatabase.LoadAssetAtPath("Assets/Resources/SensorsManager.asset", typeof(SensorsManager));
            //Debug.Log("manager found");
        }
        tracker = new GameObjectsTracker();
    }

    private void OnFocus()
    {
        refreshAvailableGO();
        
    }

    void OnGUI()
    {
        if (!objectMode)
        {
            draw("Sensor");
        }
        else
        {
            drawObjectProperties();
        }
    }

    

    protected override void updateConfiguredObject()
    {
        
        updateConfiguredObject( SensorConfiguration.CreateInstance< SensorConfiguration>());
    }
    

    void OnDisable()
    {
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
    }

    protected override string onSaving()
    {
        throw new NotImplementedException();
    }

   
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
