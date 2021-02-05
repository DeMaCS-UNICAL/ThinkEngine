//#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Threading;
using EmbASP4Unity.it.unical.mat.objectsMapper.EditorWindows;
using EmbASP4Unity.it.unical.mat.objectsMapper;
using System.IO;

[CustomEditor(typeof(ActuatorConfiguration))]
public class ActuatorConfigurationEditor : AbstractConfigurationEditor
{
    int chosenMethod;
    List<string> methodsToShow;
    new void Reset()
    {
        base.Reset();
        typeOfConfiguration = "Actuator";
        methodsToShow = Utility.triggerMethodsToShow;
        methodsToShow.Add("Always");
        if (((ActuatorConfiguration)target).applyMethod is null)
        {
            chosenMethod = methodsToShow.Count - 1;
        }
        else
        {
            chosenMethod = Utility.getTriggerMethodIndex(((ActuatorConfiguration)target).applyMethod.Name);
        }
    }
    protected override bool IsMappable(FieldOrProperty obj)
    {
        return tracker.IsBaseType(obj);
    }
    public override void OnInspectorGUI()
    {
        if (Application.isPlaying)
        {
            DrawDefaultInspector();
            return;
        }
        GUI.enabled = false;
        EditorGUILayout.TextField("Trigger Script Path", Utility.triggerClassPath);
        GUI.enabled = true;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Choose when to apply the reasoner actions");
        chosenMethod = EditorGUILayout.Popup(chosenMethod, methodsToShow.ToArray());
        EditorGUILayout.EndHorizontal();
        if (chosenMethod == methodsToShow.Count - 1)
        {
            ((ActuatorConfiguration)target).applyMethod = null;
        }
        else
        {
            ((ActuatorConfiguration)target).applyMethod = Utility.getTriggerMethod(chosenMethod);
        }
        serializedObject.ApplyModifiedProperties();
        base.OnInspectorGUI();

    }
    internal override bool ExistsConfigurationWithName(string configurationName)
    {
        return Utility.actuatorsManager.ExistsConfigurationWithName(configurationName);
    }
}
//#endif