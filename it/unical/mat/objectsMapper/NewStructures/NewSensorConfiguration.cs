﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NewStructures
{
    [ExecuteInEditMode,Serializable,RequireComponent(typeof(NewMonoBehaviourSensorsManager))]
    class NewSensorConfiguration : NewAbstractConfiguration, ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector]
        internal List<int> operationPerPropertyIndexes;
        [SerializeField, HideInInspector]
        internal List<int> operationPerPropertyOperations;
        [SerializeField, HideInInspector]
        internal List<int> specificValuePerPropertyIndexes;
        [SerializeField, HideInInspector]
        internal List<string> specificValuePerPropertyValues;
        internal Dictionary<int,int> _operationPerProperty;
        internal Dictionary<int,int> OperationPerProperty
        {
            get
            {
                if (_operationPerProperty == null)
                {
                    _operationPerProperty = new Dictionary<int, int>();
                }
                return _operationPerProperty;
            }
            set
            {
                _operationPerProperty = value;
            }
        }
        [SerializeField,HideInInspector]
        internal Dictionary<int,string> _specificValuePerProperty;
        internal Dictionary<int,string> SpecificValuePerProperty
        {
            get
            {
                if (_specificValuePerProperty == null)
                {
                    _specificValuePerProperty = new Dictionary<int, string>();
                }
                return _specificValuePerProperty;
            }
            set
            {
                _specificValuePerProperty = value;
            }
        }
        internal override string ConfigurationName {
            set {
                if (!Utility.sensorsManager.IsConfigurationNameValid(value, this))
                {
                    throw new Exception("The chosen configuration name cannot be used.");
                }
                _configurationName = value;
            } 
        }
        
        internal override void Clear()
        {
            base.Clear();
            OperationPerProperty = new Dictionary<int, int>();
            SpecificValuePerProperty = new Dictionary<int, string>();
        }
        internal override string GetAutoConfigurationName()
        {
            string name;
            string toAppend = "";
            int count = 0;
            int[,] test = new int[2, 2];
            Debug.Log(test.GetType().Equals(typeof(Array)));
            Debug.Log(test is Array);
            do
            {
                name = char.ToLower(gameObject.name[0]).ToString() + gameObject.name.Substring(1) + "Sensor"+ toAppend;
                toAppend += count;
                count++;
            }
            while (!Utility.sensorsManager.IsConfigurationNameValid(name,this));
            return name;
        }
        internal void SetOperationPerProperty(MyListString property, int operation)
        {
            if (!SavedProperties.Contains(property))
            {
                throw new Exception("Property not selected");
            }
            OperationPerProperty[property.GetHashCode()] = operation;
        }
        internal void SetSpecificValuePerProperty(MyListString property, string value)
        {
            if (!SavedProperties.Contains(property))
            {
                throw new Exception("Property not selected");
            }
            SpecificValuePerProperty[property.GetHashCode()] = value;

        }
        internal override bool IsSensor()
        {
            return true;
        }
        protected override void PropertySelected(MyListString property)
        {
            OperationPerProperty[property.GetHashCode()] =0;
            SpecificValuePerProperty[property.GetHashCode()] ="";
        }
        protected override void PropertyDeleted(MyListString property)
        {
            OperationPerProperty.Remove(property.GetHashCode());
            SpecificValuePerProperty.Remove(property.GetHashCode());
        }
       
        public void OnBeforeSerialize()
        {
            operationPerPropertyIndexes = new List<int>();
            operationPerPropertyOperations = new List<int>();
            specificValuePerPropertyIndexes = new List<int>();
            specificValuePerPropertyValues = new List<string>();
            foreach(int key in OperationPerProperty.Keys)
            {
                operationPerPropertyIndexes.Add(key);
                operationPerPropertyOperations.Add(OperationPerProperty[key]);
            }
            foreach (int key in SpecificValuePerProperty.Keys)
            {
                specificValuePerPropertyIndexes.Add(key);
                specificValuePerPropertyValues.Add(SpecificValuePerProperty[key]);
            }
        }

        public void OnAfterDeserialize()
        {
            OperationPerProperty = new Dictionary<int, int>();
            SpecificValuePerProperty = new Dictionary<int, string>();
            for(int i=0; i < operationPerPropertyIndexes.Count; i++)
            {
                OperationPerProperty.Add(operationPerPropertyIndexes[i], operationPerPropertyOperations[i]);
            }
            for (int i = 0; i < specificValuePerPropertyIndexes.Count; i++)
            {
                SpecificValuePerProperty.Add(specificValuePerPropertyIndexes[i], specificValuePerPropertyValues[i]);
            }
        }

        internal override bool IsAValidName(string temporaryName)
        {
            return Utility.sensorsManager.IsConfigurationNameValid(temporaryName, this);
        }
    }
}
