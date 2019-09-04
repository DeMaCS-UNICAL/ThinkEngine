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
        List<string> methodsToShowForSensors = new List<string>();
        List<string> methodsToShowForReasoner = new List<string>();
        int sensorsUpdateIndex = 0;
        int reasonerExecutionIndex = 0;
        private static int colliderIndex;
        private Dictionary<string, bool> sensorsColliderToggle;
        private Dictionary<string, bool> reasonerColliderToggle;
        private bool showSensorsColliders=true;
        private List<string> collidersGO;
        private int sensorsUpdateType;
        private int reasonerExecutionType;
        private bool showReasonerColliders=true;

        void OnEnable()
        {
            
            var triggerClass = ScriptableObject.CreateInstance("Trigger");
            MethodInfo[] methods = triggerClass.GetType().GetMethods();
            foreach (MethodInfo mI in methods)
            {
                if (mI.ReturnType == typeof(bool))
                {
                    methodsToShowForSensors.Add(mI.Name);
                    Debug.Log(mI.Name);
                }
            }
            methodsToShowForReasoner.AddRange(methodsToShowForSensors);
            Brain myScript = target as Brain;
            if (!myScript.sensorsTriggerMethod.Equals(""))
            {
                for(int i=0; i < methodsToShowForSensors.Count; i++)
                {
                    if (methodsToShowForSensors[i].Equals(myScript.sensorsTriggerMethod))
                    {
                        sensorsUpdateIndex = i;
                        break;
                    }
                }
            }
            if (!myScript.reasonerTriggerMethod.Equals(""))
            {
                for (int i = 0; i < methodsToShowForSensors.Count; i++)
                {
                    if (methodsToShowForSensors[i].Equals(myScript.reasonerTriggerMethod))
                    {
                        reasonerExecutionIndex = i;
                        break;
                    }
                }
            }
            if (myScript.updateSensorsRepeatedly)
            {
                sensorsUpdateType = 0;
            }else if (myScript.updateSensorsOnTrigger)
            {
                sensorsUpdateType = 1;
            }else if (myScript.updateSensorsOnCollision)
            {
                sensorsUpdateType = 2;
            }
            if (myScript.executeReasonerAsSoonAsPossible)
            {
                reasonerExecutionType = 0;
            }
            else if (myScript.executeReasonerOnTrigger)
            {
                reasonerExecutionType = 1;
            }
            else if (myScript.executeReasonerOnCollision)
            {
                reasonerExecutionType = 2;
            }
            initColliders(myScript);
        }

        private void initColliders(Brain brain)
        {
            Collider2D[] bodies = GameObject.FindObjectsOfType<Collider2D>();
            collidersGO = new List<string>();
            sensorsColliderToggle = new Dictionary<string, bool>();
            reasonerColliderToggle = new Dictionary<string, bool>();

            foreach (Collider2D b in bodies)
            {
                if (b.isTrigger)
                {
                    string temp = b.gameObject.name;
                    collidersGO.Add(temp);
                    if (brain.sensorsCollidersGONames.Contains(temp))
                    {
                        sensorsColliderToggle.Add(temp, true);
                    }
                    else
                    {
                        sensorsColliderToggle.Add(temp, false);
                    }
                    if (brain.reasonerCollidersGONames.Contains(temp))
                    {
                        reasonerColliderToggle.Add(temp, true);
                    }
                    else
                    {
                        reasonerColliderToggle.Add(temp, false);
                    }
                }
            }
        }

        public override void OnInspectorGUI()
        {
            // base.OnInspectorGUI();
            
            Brain myScript = target as Brain;
            List<string> excludedProperties = new List<string>();
            excludedProperties.Add("sensorsTriggerMethod");
            excludedProperties.Add("reasonerTriggerMethod");
            excludedProperties.Add("updateSensorsRepeatedly");
            excludedProperties.Add("updateSensorsOnTrigger");
            excludedProperties.Add("updateSensorsOnCollision");
            excludedProperties.Add("executeReasonerOnTrigger");
            excludedProperties.Add("executeReasonerOnCollision");
            //excludedProperties.Add("sensorsCollidersGONames");
            //excludedProperties.Add("reasonerCollidersGONames");
            excludedProperties.Add("executeReasonerAsSoonAsPossible");

            if (!myScript.updateSensorsRepeatedly)
            {
                excludedProperties.Add("brainUpdateFrequency");
                excludedProperties.Add("startIn");
            }
            if (!myScript.updateSensorsOnTrigger)
            {
                excludedProperties.Add("triggerClassPath");
            }
            
            SerializedObject serialized = new SerializedObject(myScript);
            DrawPropertiesExcluding(serialized, excludedProperties.ToArray());
            if (GUILayout.Button("Generate ASP file", GUILayout.Width(300)))
            {
                myScript.generateFile();
            }
            string[] sensorsUpdateOptions = {"Periodically","On Condition","On Collision"};
            string[] reasonerExecutionOptions = {"Sensors Ready","On Condition","On Collision"};
            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            EditorGUILayout.LabelField("Choose when update Sensors");
            sensorsUpdateType = GUILayout.SelectionGrid(sensorsUpdateType, sensorsUpdateOptions, 3,GUILayout.Width(300));
            EditorGUILayout.EndHorizontal();
            switch (sensorsUpdateType)
            {
                case 0:
                    myScript.updateSensorsRepeatedly = true;
                    myScript.updateSensorsOnTrigger = false;
                    myScript.updateSensorsOnCollision = false;
                    break;
                case 1:
                    myScript.updateSensorsRepeatedly = false;
                    myScript.updateSensorsOnTrigger = true;
                    myScript.updateSensorsOnCollision = false;
                    break;
                case 2:
                    myScript.updateSensorsRepeatedly = false;
                    myScript.updateSensorsOnTrigger = false;
                    myScript.updateSensorsOnCollision = true;
                    break;
            }
            

            if (sensorsUpdateType == 2)
            {
                sensorsColliders(myScript);
            }
            if (sensorsUpdateType == 1)
            {
                sensorsUpdateTrigger(myScript);
            }
            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            EditorGUILayout.LabelField("Choose when execute Reasoner");
            reasonerExecutionType = GUILayout.SelectionGrid(reasonerExecutionType, reasonerExecutionOptions, 3, GUILayout.Width(300));
            EditorGUILayout.EndHorizontal();
            switch (reasonerExecutionType)
            {
                case 0:
                    myScript.executeReasonerAsSoonAsPossible = true;
                    myScript.executeReasonerOnTrigger = false;
                    myScript.executeReasonerOnCollision = false;
                    break;
                case 1:
                    myScript.executeReasonerAsSoonAsPossible = false;
                    myScript.executeReasonerOnTrigger = true;
                    myScript.executeReasonerOnCollision = false;
                    break;
                case 2:
                    myScript.executeReasonerAsSoonAsPossible = false;
                    myScript.executeReasonerOnTrigger = false;
                    myScript.executeReasonerOnCollision = true;
                    break;
            }
            
            if (reasonerExecutionType == 1)
            {
                reasonerTrigger(myScript);
            }
            if (reasonerExecutionType == 2)
            {
                reasonerColliders(myScript);
            }


    
            serialized.ApplyModifiedProperties();
        }

        private void reasonerColliders(Brain myScript)
        {
            showReasonerColliders = EditorGUILayout.Foldout(showReasonerColliders, "Show colliders for the Reasoner Execution");
            if (showReasonerColliders)
            {
                foreach (string gO in collidersGO)
                {
                    reasonerColliderToggle[gO] = EditorGUILayout.ToggleLeft(gO, reasonerColliderToggle[gO]);
                    if (reasonerColliderToggle[gO] && !myScript.reasonerCollidersGONames.Contains(gO))
                    {
                        myScript.reasonerCollidersGONames.Add(gO);
                    }
                    else
                    {
                        if (!reasonerColliderToggle[gO] && myScript.reasonerCollidersGONames.Contains(gO))
                        {
                            myScript.reasonerCollidersGONames.Remove(gO);
                        }
                    }
                }
            }
        }

        private void sensorsColliders(Brain myScript)
        {
            showSensorsColliders = EditorGUILayout.Foldout(showSensorsColliders, "Show colliders for Sensors Update");
            if (showSensorsColliders)
            {
                foreach (string gO in collidersGO)
                {
                    sensorsColliderToggle[gO] = EditorGUILayout.ToggleLeft(gO, sensorsColliderToggle[gO]);
                    if (sensorsColliderToggle[gO] && !myScript.sensorsCollidersGONames.Contains(gO))
                    {
                        myScript.sensorsCollidersGONames.Add(gO);
                    }
                    else
                    {
                        if (!sensorsColliderToggle[gO] && myScript.sensorsCollidersGONames.Contains(gO))
                        {
                            myScript.sensorsCollidersGONames.Remove(gO);
                        }
                    }
                }
            }
        }

        private void sensorsUpdateTrigger(Brain myScript)
        {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Choose a method to use as trigger for Sensors Update");
                sensorsUpdateIndex = EditorGUILayout.Popup(sensorsUpdateIndex, methodsToShowForSensors.ToArray());
                EditorGUILayout.EndHorizontal();
                myScript.sensorsTriggerMethod = methodsToShowForSensors[sensorsUpdateIndex];
        }

        private void reasonerTrigger(Brain myScript)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Choose a method to use as trigger for the Reasoner Execution");
            reasonerExecutionIndex = EditorGUILayout.Popup(reasonerExecutionIndex, methodsToShowForReasoner.ToArray());
            EditorGUILayout.EndHorizontal();
            myScript.reasonerTriggerMethod =  methodsToShowForReasoner[reasonerExecutionIndex];
        }
    }
}
