#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Threading;
using ThinkEngine.it.unical.mat.objectsMapper.EditorWindows;
using ThinkEngine.it.unical.mat.objectsMapper;
using System.IO;

[CustomEditor(typeof(SensorConfiguration))]
public class SensorConfigurationEditor : AbstractConfigurationEditor
{
    new void Reset()
    {
        base.Reset();
        typeOfConfiguration = "Sensor";
    }
    public override void OnInspectorGUI()
    {
        if (Application.isPlaying)
        {
            DrawDefaultInspector();
            return;
        }
        try
        {
            if (!objectMode)
            {
                base.OnInspectorGUI();
            }
            else
            {
                DrawObjectProperties();
            }
        }catch(Exception e)
        {
            Debug.Log(e.StackTrace);
            MyReset(true);
        }
    }
    internal override void AddCustomFields(FieldOrProperty property)
    {
        if (tracker.objectsToggled[property])
        {
            if (!tracker.operationPerProperty.ContainsKey(property))
            {
                tracker.operationPerProperty.Add(property, 0);
            }
            tracker.operationPerProperty[property] = EditorGUILayout.Popup(tracker.operationPerProperty[property], Enum.GetNames(Operation.GetOperationsPerType(property.Type())));
            if(tracker.operationPerProperty[property]== Operation.SPECIFIC)//thus is a specific_value
            {
                if (!tracker.specificValuePerProperty.ContainsKey(property))
                {
                    tracker.specificValuePerProperty.Add(property, "");
                }
                tracker.specificValuePerProperty[property] = EditorGUILayout.TextField("Value to track", tracker.specificValuePerProperty[property]);
            }
        }
    }
    internal override bool ExistsConfigurationWithName(string configurationName)
    {
        return Utility.sensorsManager.ExistsConfigurationWithName(configurationName);
    }
}
#endif