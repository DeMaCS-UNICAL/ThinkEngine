﻿using System;
using System.Collections.Generic;
using ThinkEngine.Mappers;
using UnityEngine;

namespace ThinkEngine
{
    [ExecuteInEditMode, Serializable, RequireComponent(typeof(IndexTracker)), RequireComponent(typeof(MonoBehaviourSensorsManager))]
    class SensorConfiguration : AbstractConfiguration//, ISerializationCallbackReceiver
    {
        public bool isInvariant;
        public bool isFixedSize;

        internal override string ConfigurationName
        {
            set
            {
                if (!Utility.SensorsManager.IsConfigurationNameValid(value, this))
                {
                    throw new Exception("The chosen configuration name cannot be used.");
                }
                string old = _configurationName;
                _configurationName = value;
                if (!old.Equals(_configurationName))
                {
                    SensorsManager.ConfigurationsChanged = true;
                }
            }
        }

        internal override string GetAutoConfigurationName()
        {
            string name;
            string toAppend = "";
            int count = 0;
            do
            {
                name = ASPMapperHelper.AspFormat(gameObject.name) + "Sensor" + toAppend;
                toAppend += count;
                count++;
            }
            while (!Utility.SensorsManager.IsConfigurationNameValid(name, this));
            return name;
        }
        internal void SetOperationPerProperty(MyListString property, int operation)
        {
            if (!SavedProperties.Contains(property))
            {
                throw new Exception("Property not selected");
            }
            PropertyFeatures.Find(x => x.property.Equals(property)).operation = operation;
        }
        internal void SetSpecificValuePerProperty(MyListString property, string value)
        {
            if (!SavedProperties.Contains(property))
            {
                throw new Exception("Property not selected");
            }
            PropertyFeatures.Find(x => x.property.Equals(property)).specifValue = value;

        }

        internal void SetCounterPerProperty(MyListString actualProperty, int newCounter)
        {
            if (!SavedProperties.Contains(actualProperty))
            {
                throw new Exception("Property not selected");
            }
            PropertyFeatures.Find(x => x.property.Equals(actualProperty)).counter = newCounter;
        }

        internal override bool IsSensor()
        {
            return true;
        }
        /*
protected override void PropertySelected(MyListString property)
{
   propertyFeatures.Find(x => x.property.Equals(property)).operation = 0;
   propertyFeatures.Find(x => x.property.Equals(property)).specifValue = "";
}
protected override void PropertyDeleted(MyListString property)
{

}

public void OnBeforeSerialize()
{
   operationPerPropertyIndexes = new List<int>();
   operationPerPropertyOperations = new List<int>();
   specificValuePerPropertyIndexes = new List<int>();
   specificValuePerPropertyValues = new List<string>();
   foreach (int key in OperationPerProperty.Keys)
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
   for (int i = 0; i < operationPerPropertyIndexes.Count; i++)
   {
       OperationPerProperty.Add(operationPerPropertyIndexes[i], operationPerPropertyOperations[i]);
   }
   for (int i = 0; i < specificValuePerPropertyIndexes.Count; i++)
   {
       SpecificValuePerProperty.Add(specificValuePerPropertyIndexes[i], specificValuePerPropertyValues[i]);
   }
}
*/
        internal override bool IsAValidName(string temporaryName)
        {
            return temporaryName.Equals(ConfigurationName) || Utility.SensorsManager.IsConfigurationNameValid(temporaryName, this);
        }
    }
}