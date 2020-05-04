using UnityEditor;

[CustomEditor(typeof(SwipeInput))]
public class SwipeInputEditor : Editor {
    SerializedProperty enableDetection;
    SerializedProperty minimumSwipeDistance;
    SerializedProperty mouseSwipeKey;

    void OnEnable() {
        enableDetection = serializedObject.FindProperty("detectionEnabled");
        minimumSwipeDistance = serializedObject.FindProperty("minDistance");
        mouseSwipeKey = serializedObject.FindProperty("mouseSwipeKey");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        EditorGUILayout.PropertyField(enableDetection);
        EditorGUILayout.Slider(minimumSwipeDistance, 0, 1);
        EditorGUILayout.PropertyField(mouseSwipeKey);

        serializedObject.ApplyModifiedProperties();
    }
}
