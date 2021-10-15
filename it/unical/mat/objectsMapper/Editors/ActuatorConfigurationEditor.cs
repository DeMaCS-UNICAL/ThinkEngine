using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editors
{
    [CustomEditor(typeof(ActuatorConfiguration))]
    class ActuatorConfigurationEditor : AbstractConfigurationEditor
    {
        protected override void SpecificFields(MyListString property)
        {

        }

        void OnDestroy()
        {
            if (target == null && go != null)
            {
                DestroyImmediate(go.GetComponent<MonoBehaviourActuatorsManager>());
            }
        }
        int chosenMethod;
        List<string> methodsToShow;
        new void Reset()
        {
            base.Reset();
            methodsToShow = Utility.TriggerMethodsToShow;
            methodsToShow.Add("Always");
            if (((ActuatorConfiguration)target).applyMethod is null)
            {
                chosenMethod = methodsToShow.Count - 1;
            }
            else
            {
                chosenMethod = Utility.GetTriggerMethodIndex(((ActuatorConfiguration)target).applyMethod.Name);
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
            EditorGUILayout.TextField("Trigger Script Path", Utility.TriggerClassPath);
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
                ((ActuatorConfiguration)target).applyMethod = Utility.GetTriggerMethod(chosenMethod);
            }
            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }
    }
}
