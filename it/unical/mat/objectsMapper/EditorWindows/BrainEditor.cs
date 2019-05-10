using EmbASP4Unity.it.unical.mat.objectsMapper.BrainsScripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace EmbASP4Unity.it.unical.mat.objectsMapper.EditorWindows
{
    [CustomEditor(typeof(Brain))]
    public class BrainEditor:Editor
    {
        List<string> methodsToShow = new List<string>();
        List<string> methodsToShowForReasoner = new List<string>();
        int sensorsUpdateIndex = 0;
        int reasoningExecutionUpdateIndex = 0;
        void OnEnable()
        {
            
            var triggerClass = ScriptableObject.CreateInstance("Trigger");
            MethodInfo[] methods = triggerClass.GetType().GetMethods();
            
            foreach (MethodInfo mI in methods)
            {
                if (mI.ReturnType == typeof(bool))
                {
                    methodsToShow.Add(mI.Name);
                    Debug.Log(mI.Name);
                }
            }
            methodsToShowForReasoner.Add("When Sensors are ready");
            methodsToShowForReasoner.AddRange(methodsToShow);
        }
        public override void OnInspectorGUI()
        {
            // base.OnInspectorGUI();
            Brain myScript = target as Brain;
            List<string> excludedProperties = new List<string>();
            excludedProperties.Add("triggerMethod");
            excludedProperties.Add("executeReasonerOn");
            if (!myScript.executeRepeatedly)
            {
                excludedProperties.Add("brainUpdateFrequency");
                excludedProperties.Add("startIn");
            }
            if (!myScript.executeOnTrigger)
            {
                excludedProperties.Add("triggerClassPath");
            }
            SerializedObject serialized = new SerializedObject(myScript);
            DrawPropertiesExcluding(serialized, excludedProperties.ToArray());
            serialized.ApplyModifiedProperties();
            if(myScript.executeOnTrigger && myScript.executeRepeatedly)
            {
                if (excludedProperties.Contains("triggerClassPath"))
                {
                    myScript.executeRepeatedly = false;
                }
                else
                {
                    myScript.executeOnTrigger = false;
                }
            }
            if (myScript.executeOnTrigger)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Choose a method to use as trigger for Sensors Update");
                sensorsUpdateIndex = EditorGUILayout.Popup(sensorsUpdateIndex, methodsToShow.ToArray());
                EditorGUILayout.EndHorizontal();
                myScript.triggerMethod = methodsToShow[sensorsUpdateIndex];
            }
            else
            {
                myScript.triggerMethod = "";
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Choose when to run the reasoner");
            reasoningExecutionUpdateIndex = EditorGUILayout.Popup(reasoningExecutionUpdateIndex, methodsToShowForReasoner.ToArray());
            EditorGUILayout.EndHorizontal();
            myScript.executeReasonerOn = methodsToShowForReasoner[reasoningExecutionUpdateIndex];


            Brain current = (Brain)target;
            if(GUILayout.Button("Generate ASP file", GUILayout.Width(300)))
            {
                current.generateFile();
            }
            
        }
    }
}
