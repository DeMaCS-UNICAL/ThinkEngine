using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ThinkEngine
{
    [Serializable]
    public class SerializableSensorType : ISerializationCallbackReceiver
    {
#if UNITY_EDITOR
        [SerializeField]
        private MonoScript scriptAsset;
#endif

        [SerializeField]
        [HideInInspector]
        private string typeName;

        public Type ScriptType { get; private set; }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (scriptAsset)
            {
                string assemblyQualifiedName = scriptAsset.GetClass()?.AssemblyQualifiedName;
                if (TypeIsValid(assemblyQualifiedName))
                {
                    typeName = scriptAsset.GetClass()?.AssemblyQualifiedName;
                }
                else
                {
                    Debug.LogError("MonoScript must refer to a Subclass of Sensor!");
                    scriptAsset = null;
                }
            }
            else
            {
                typeName = null;
            }
#endif
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (string.IsNullOrEmpty(typeName))
                ScriptType = null;
            else
                ScriptType = Type.GetType(typeName);
        }

        private bool TypeIsValid(string assemblyQualifiedName)
        {
            return Type.GetType(assemblyQualifiedName).IsSubclassOf(typeof(Sensor));
        }
    }
}


/*
using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
 
[Serializable]
public class ScriptReference : ISerializationCallbackReceiver
{
#if UNITY_EDITOR
    [SerializeField]
    MonoScript scriptAsset;
#endif

    [SerializeField]
    string typeName;

    public Type ScriptType { get; private set; }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        if (string.IsNullOrEmpty(typeName))
            ScriptType = null;
        else
            ScriptType = Type.GetType(typeName);
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
#if UNITY_EDITOR
        if (scriptAsset)
            typeName = scriptAsset.GetClass()?.AssemblyQualifiedName;
        else
            typeName = null;
#endif
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ScriptReference))]
    class ScriptReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.ObjectField(position, property.FindPropertyRelative(nameof(scriptAsset)), label);
        }
    }
#endif
}
*/
