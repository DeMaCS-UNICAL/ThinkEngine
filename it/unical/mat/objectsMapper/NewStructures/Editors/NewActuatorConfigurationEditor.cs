using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace NewStructures.Editors
{
    [CustomEditor(typeof(NewActuatorConfiguration))]
    class NewActuatorConfigurationEditor : NewAbstractConfigurationEditor
    {
        protected override void SpecificFields(MyListString property)
        {

        }

        void OnDestroy()
        {
            if (target == null && go != null)
            {
                DestroyImmediate(go.GetComponent<NewMonoBehaviourActuatorsManager>());
            }
        }
        int chosenMethod;
        List<string> methodsToShow;
        new void Reset()
        {
            base.Reset();
            methodsToShow = Utility.triggerMethodsToShow;
            methodsToShow.Add("Always");
            if (((NewActuatorConfiguration)target).applyMethod is null)
            {
                chosenMethod = methodsToShow.Count - 1;
            }
            else
            {
                chosenMethod = Utility.getTriggerMethodIndex(((NewActuatorConfiguration)target).applyMethod.Name);
            }
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
                ((NewActuatorConfiguration)target).applyMethod = null;
            }
            else
            {
                ((NewActuatorConfiguration)target).applyMethod = Utility.getTriggerMethod(chosenMethod);
            }
            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }
    }
}
