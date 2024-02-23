using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ThinkEngine
{
    [Serializable]
    public class SerializableSensorType 
    {

        [SerializeField]
        [HideInInspector]
        private string typeName;
        [SerializeField]
        [HideInInspector]
        private Type _ScriptType;
        public Type ScriptType {
            get { 
                if (_ScriptType == null)
                {
                    _ScriptType = Type.GetType(typeName);
                }
                return _ScriptType; 
            }
            private set { _ScriptType = value; } 
        }
#if UNITY_EDITOR
        public SerializableSensorType(MonoScript retrieved)
        {
            string type = retrieved.GetClass()?.AssemblyQualifiedName;
            Debug.Log("trying serialize "+type);
            if (TypeIsValid(type))
            {
                Debug.Log("is valid");
                //ScriptType = Type.GetType(type);
                typeName = type;
                Debug.Log("scripttype is "+ScriptType);
            }
            else
            {
                throw new Exception();
            }
        }
#endif
        private bool TypeIsValid(string assemblyQualifiedName)
        {
            return Type.GetType(assemblyQualifiedName).IsSubclassOf(typeof(Sensor));
        }
        /*
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (scriptAsset)
            {
                // string assemblyQualifiedName = scriptAsset.GetClass()?.AssemblyQualifiedName;
                if (TypeIsValid(scriptAsset.GetClass()))
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

        
        
        private bool TypeIsValid(Type type)
        {
            return type.IsSubclassOf(typeof(Sensor));
        }*/
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
