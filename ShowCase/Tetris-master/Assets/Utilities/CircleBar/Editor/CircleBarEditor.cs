using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(CircleBar))]
public class CircleBarEditor : Editor {
    SerializedProperty currentProgress;
    SerializedProperty currentValue;
    SerializedProperty valueFormat;
    SerializedProperty displayValue;
    SerializedProperty smoothProgress;

    void OnEnable() {
        currentProgress = serializedObject.FindProperty("_currentProgress");
        currentValue = serializedObject.FindProperty("_currentValue");
        valueFormat = serializedObject.FindProperty("valueFormat");
        displayValue = serializedObject.FindProperty("displayCurrentValue");
        smoothProgress = serializedObject.FindProperty("raiseSmooth");

        update();
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorGUILayout.Slider(currentProgress, 0, 1);
        EditorGUILayout.PropertyField(displayValue);
        EditorGUILayout.PropertyField(smoothProgress);

        if(displayValue.boolValue) {
            EditorGUILayout.PropertyField(currentValue);
            EditorGUILayout.PropertyField(valueFormat);
        }

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed) update();
    }

    private void update() {
        CircleBar trg = (CircleBar)target;
        Transform obj = trg.transform;

        Image progress = obj.GetChild(0).GetComponent<Image>();
        Text value = obj.GetChild(2).GetComponent<Text>();
        progress.fillAmount = currentProgress.floatValue;

        if(displayValue.boolValue) {
            if (!value.gameObject.activeInHierarchy) value.gameObject.SetActive(true);
            value.text = string.Format(valueFormat.stringValue, currentValue.floatValue);
        }
        else {
            if (value.gameObject.activeInHierarchy) value.gameObject.SetActive(false);
        }
    }
}
