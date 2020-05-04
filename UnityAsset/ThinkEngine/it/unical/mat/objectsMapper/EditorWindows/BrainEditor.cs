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
        List<string> excludedProperties=new List<string>();
        List<string> methodsToShow = new List<string>();
        List<string> methodsToShowForReasoner = new List<string>();
        public int sensorsUpdateIndex = 0;
        public int reasoningExecutionIndex = 0;
        public int applyActuatorsIndex;
        private List<string> methodsToShowForActuators=new List<string>();
        private Brain myScript;

        void OnEnable()
        {
            myScript = target as Brain;
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
            methodsToShowForActuators.Add("Always");
            methodsToShowForActuators.Add("Never");
            methodsToShowForActuators.AddRange(methodsToShow);
            for(int i=0; i < methodsToShow.Count; i++)
            {
                if (myScript.updateSensorsOn.Equals(methodsToShow[i]))
                {
                    reasoningExecutionIndex = i;
                    break;
                }
            }
            for (int i = 0; i < methodsToShowForReasoner.Count; i++)
            {
                if (myScript.executeReasonerOn.Equals(methodsToShowForReasoner[i]))
                {
                    sensorsUpdateIndex = i;
                    break;
                }
            }
            for (int i = 0; i < methodsToShowForActuators.Count; i++)
            {
                if (myScript.applyActuatorsCondition.Equals(methodsToShowForActuators[i]))
                {
                    applyActuatorsIndex = i;
                    break;
                }
            }
        }
        public override void OnInspectorGUI()
        {
            // base.OnInspectorGUI();
            if (myScript.executeOnTrigger && myScript.executeRepeatedly)
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
            if (!(myScript.executeOnTrigger || myScript.executeRepeatedly))
            {
                if (excludedProperties.Contains("triggerClassPath"))
                {
                    myScript.executeOnTrigger = true;
                }
                else
                {
                    myScript.executeRepeatedly = true;
                }
            }
            excludedProperties = new List<string>();
            excludedProperties.Add("updateSensorsOn");
            excludedProperties.Add("executeReasonerOn");
            excludedProperties.Add("applyActuatorsCondition");
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
            
            if (myScript.executeOnTrigger)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Choose a method to use as trigger for Sensors Update");
                sensorsUpdateIndex = EditorGUILayout.Popup(sensorsUpdateIndex, methodsToShow.ToArray());
                EditorGUILayout.EndHorizontal();
                myScript.updateSensorsOn = methodsToShow[sensorsUpdateIndex];
            }
            else
            {
                myScript.updateSensorsOn = "";
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Choose when to run the reasoner");
            reasoningExecutionIndex = EditorGUILayout.Popup(reasoningExecutionIndex, methodsToShowForReasoner.ToArray());
            EditorGUILayout.EndHorizontal();
            myScript.executeReasonerOn = methodsToShowForReasoner[reasoningExecutionIndex];

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Choose when to apply Actuators actions");
            applyActuatorsIndex = EditorGUILayout.Popup(applyActuatorsIndex, methodsToShowForActuators.ToArray());
            EditorGUILayout.EndHorizontal();
            myScript.applyActuatorsCondition = methodsToShowForActuators[applyActuatorsIndex];

            serialized.ApplyModifiedProperties();//CHECK SERIALIZATION

            Brain current = (Brain)target;
            if(GUILayout.Button("Generate ASP file template", GUILayout.Width(300)))
            {
                current.generateFile();
            }
            EditorGUILayout.LabelField("Warning! Generating a new file will delete the previouse template.");

        }
    }
}
