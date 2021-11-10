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
        chosenMethod = Utility.getTriggerMethodIndex(((ActuatorConfiguration)target).MethodToApply);
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
        ((ActuatorConfiguration)target).MethodToApply = methodsToShow[chosenMethod];
        if (GUI.changed)
        {
            EditorUtility.SetDirty((ActuatorConfiguration)target);
        }
        base.OnInspectorGUI();

    }
    internal override bool ExistsConfigurationWithName(string configurationName)
    {
        return Utility.actuatorsManager.ExistsConfigurationWithName(configurationName);
    }
}
#endif