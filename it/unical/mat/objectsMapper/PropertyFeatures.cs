using System;
using System.Collections.Generic;
using UnityEngine;
using static ThinkEngine.Mappers.OperationContainer;

namespace ThinkEngine
{
    [Serializable]
    internal class PropertyFeatures : ISerializationCallbackReceiver
    {
        private static HashSet<int> usedNameProperty = new HashSet<int>();
        [SerializeField,HideInInspector]
        internal MyListString property;
        [SerializeField,HideInInspector]
        internal int windowWidth = 200;
        [SerializeField,HideInInspector]
        private string _propertyName;
        [SerializeField,HideInInspector]
        private int propertyNameHash;
        [SerializeField,HideInInspector]
        internal int operation;
        [SerializeField,HideInInspector]
        internal string specifValue;
        [SerializeField, HideInInspector]
        internal int counter;//for atLeast and count operations

        internal int GetPropertyIndex { get; }
        internal string PropertyName
        {
            get
            {
                return _propertyName;
            }
            set
            {
                ValidateNameAndAssign(value);
            }
        }

        internal PropertyFeatures(GameObject go, MyListString p)
        {
            property = p.GetClone();
            GeneratePropertyName(go.name);
        }

        private void ValidateNameAndAssign(string value)
        {
            if(_propertyName == value)
            {
                Debug.Log("SAME VALUE! "+_propertyName+" "+value);
                return;
            }
            if (usedNameProperty.Contains(value.GetHashCode()))
            {
                Debug.Log("THROWING!");
                throw new Exception("InvalidName");
            }
            if(_propertyName!=null && usedNameProperty.Contains(propertyNameHash))
            {
                usedNameProperty.Remove(propertyNameHash);
            }
            Debug.Log("setting _property_name to " + value);
            _propertyName = value;
            propertyNameHash = PropertyName.GetHashCode();
            usedNameProperty.Add(propertyNameHash);
        }

        private void GeneratePropertyName(string goName)
        {
            string prefix = goName;
            string suffix ="";
            int count = 0;
            Debug.Log(property);
            if (property.Count > 0)
            {
                count = property.Count - 1;
                suffix = "_" + property[count];
            }
            while (true)
            {
                try
                {
                    PropertyName = prefix + suffix;
                    break;
                }
                catch (Exception e)
                {
                    if (e.Message.Equals("InvalidName"))
                    {
                        if(count!= 0 && property.Count >= count)
                        {
                            count--;
                            suffix = "_" + property[count] + suffix;
                        }
                        else
                        {
                            suffix += "0";
                        }
                        continue;
                    }
                    else
                    {
                        throw e;
                    }
                }
            }
        }

        internal void Remove()
        {
            usedNameProperty.Remove(propertyNameHash);
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            usedNameProperty.Add(propertyNameHash);
        }
    }
}