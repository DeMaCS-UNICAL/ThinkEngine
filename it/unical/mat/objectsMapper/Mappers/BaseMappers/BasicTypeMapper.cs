using Mappers;
using Mappers.BaseMappers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Mappers.OperationContainer;

internal abstract class BasicTypeMapper : IDataMapper
{
    #region SUPPORT CLASSES
    private class BasicTypeInfoAndValue : IInfoAndValue
    {
        internal Operation operation;
        internal string specificValue;
        internal List<object> values;
        internal BasicTypeInfoAndValue()
        {
            values = new List<object>();
        }

        public object GetValuesForPlaceholders()
        {
            if (values[0] != null)
            {
                return GetMapper(values[0].GetType()).BasicMap(operation(values));
            }
            return null;
        }
    }


    private class BasicTypeSensor : ISensors
    {
        internal Sensor sensor;
        internal BasicTypeSensor(Sensor sensor)
        {
            this.sensor = sensor;
        }

        public List<Sensor> GetSensorsList()
        {
            return new List<Sensor> { sensor };
        }

        public bool IsEmpty()
        {
            return sensor == null;
        }
    }
    private class BasicTypeActuator : IActuators
    {
        internal MonoBehaviourActuator actuator;
        internal BasicTypeActuator(MonoBehaviourActuator actuator)
        {
            this.actuator = actuator;
        }

        public bool IsEmpty()
        {
            return actuator == null;
        }
        public List<MonoBehaviourActuator> GetActuatorsList()
        {
            return new List<MonoBehaviourActuator> { actuator };
        }
    }
    #endregion
    #region ENUMS
    internal enum NumericOperations { Newest, Oldest, Specific_Value, Max, Min, Avg };
    internal enum BoolOperations { Newest, Oldest, Specific_Value, Conjunction, Disjunction };
    internal enum StringOperations { Newest, Oldest, Specific_Value };
    #endregion

    private static BasicTypeMapper GetMapper(Type type)
    {
        IDataMapper mapper = MapperManager.GetMapper(type);
        if(mapper is BasicTypeMapper mapper1)
        {
            return mapper1;
        }
        return null;
    }
    internal Type _convertingType;
    internal Type ConvertingType {
        get
        {
            return _convertingType;
        }
    }
    internal List<Type> _supportedTypes;
    internal List<Type> SupportedTypes
    {
        get
        {
            return _supportedTypes;
        }
    }
    internal List<Operation> _operationList;
    public List<Operation> OperationList()
    {
        return _operationList;
    }
    internal static object Oldest(IList values)
    {
        if (values.Count == 0)
        {
            return null;
        }
        return values[0];
    }
    internal static object Newest(IList values)
    {
        if (values.Count == 0)
        {
            return null;
        }
        return values[values.Count - 1];
    }
    internal static object Specific_Value(IList values)
    {
        throw new NotSupportedException();
    }

    public bool IsFinal(Type t)
    {
        return true;
    }
    public bool Supports(Type type)
    {
        return SupportedTypes.Contains(type);
    }
    public void UpdateSensor(Sensor sensor, object actualValue, MyListString property, int hierarchyLevel)
    {
        if (actualValue == null)
        {
            return;
        }
        if (!Supports(actualValue.GetType()))
        {
            Debug.LogError("Wrong mapper for property " + property);
        }

        List<object> values = ((BasicTypeInfoAndValue)sensor.PropertyInfo[hierarchyLevel]).values;
        if (values.Count == 200)
        {
            values.RemoveAt(0);
        }
        values.Add(Convert.ChangeType(actualValue,ConvertingType));
    }

    public ISensors InstantiateSensors(InstantiationInformation information)
    {
        if (!SupportedTypes.Contains(information.currentType) || information.residualPropertyHierarchy.Count > 1)
        {
            Debug.LogError("Wrong mapper for property " + information.propertyHierarchy);
        }
        BasicTypeInfoAndValue additionalInfo = new BasicTypeInfoAndValue();
        /*if (!((SensorConfiguration)information.configuration).OperationPerProperty.ContainsKey(information.propertyHierarchy.GetHashCode()))
        {
            Debug.Log(information.propertyHierarchy);
        }*/
        int operationIndex = ((SensorConfiguration)information.configuration).OperationPerProperty[information.propertyHierarchy.GetHashCode()];
        additionalInfo.operation = OperationList()[operationIndex];
        if (operationIndex == GetAggregationSpecificIndex())
        {
            additionalInfo.specificValue = ((SensorConfiguration)information.configuration).SpecificValuePerProperty[information.propertyHierarchy.GetHashCode()];
        }
        BasicTypeSensor sensor = new BasicTypeSensor(new Sensor());
        additionalInfo.values.Add(Convert.ChangeType(information.currentObjectOfTheHierarchy, ConvertingType));
        information.hierarchyInfo.Add(additionalInfo);
        sensor.sensor.Configure(information, GenerateMapping(information));
        return sensor;
    }
    public ISensors ManageSensors(InstantiationInformation information, ISensors instantiatedSensors)
    {
        if (instantiatedSensors == null)
        {
            return InstantiateSensors(information);
        }
        if (!(instantiatedSensors is BasicTypeSensor))
        {
            Debug.LogError("Wrong mapper for property " + information.propertyHierarchy);
        }
        return instantiatedSensors;
    }
    public int GetAggregationSpecificIndex(Type type=null)
    {
        return 2;
    }
    public bool IsTypeExpandable(Type type)
    {
        return false;
    }
    
    public IActuators InstantiateActuators(InstantiationInformation information)
    {
        if (!SupportedTypes.Contains(information.currentObjectOfTheHierarchy.GetType()) || information.residualPropertyHierarchy.Count > 1)
        {
            Debug.LogError("Wrong mapper for property " + information.propertyHierarchy);
        }
        BasicTypeActuator actuator = new BasicTypeActuator(information.instantiateOn.AddComponent<MonoBehaviourActuator>());
        actuator.actuator.Configure(information, GenerateMapping(information));
        return actuator;
    }

    public IActuators ManageActuators(InstantiationInformation information, IActuators actuators)
    {
        if (actuators == null)
        {
            return InstantiateActuators(information);
        }
        if (!(actuators is BasicTypeActuator))
        {
            Debug.LogError("Wrong mapper for property " + information.propertyHierarchy);
        }
        return actuators;
    }
    public string ActuatorBasicMap(MonoBehaviourActuator actuator, object currentObject, int hierarchyLevel, MyListString residualPropertyHierarchy, List<object> valuesForPlaceholders)
    {
        if (!SupportedTypes.Contains(currentObject.GetType()) || residualPropertyHierarchy.Count > 1)
        {
            Debug.LogError("Type " + currentObject.GetType() + " is not supported as Sensor");
        }
        valuesForPlaceholders.Add(currentObject);
        return string.Format(actuator.Mapping, valuesForPlaceholders.ToArray()); 
    }

    public string GetASPTemplate(ref InstantiationInformation information, List<string> variables)
    {
        variables.Add("Value");
        string mapping = GenerateMapping(information);
        return string.Format(mapping, variables.ToArray());
    }
    public void SetPropertyValue(MonoBehaviourActuator actuator, MyListString propertyHierarchy, object currentObject, object valueToSet, int level)
    {
        throw new NotSupportedException();
    }
    internal object GetConvertedValue(object valueToSet)
    {
        return Convert.ChangeType(valueToSet, ConvertingType);
    }
    public Dictionary<MyListString, KeyValuePair<Type, object>> RetrieveProperties(Type objectType, MyListString currentObjectPropertyHierarchy, object currentObject)
    {
        if (!SupportedTypes.Contains(objectType))
        {
            Debug.LogError("Type " + objectType + " is not supported by " + this.GetType());
        }
        return new Dictionary<MyListString, KeyValuePair<Type, object>>();
    }
    public bool NeedsSpecifications(Type type)
    {
        return true;
    }
    public bool NeedsAggregates(Type type)
    {
        return true;
    }
    protected string GenerateMapping(InstantiationInformation information)
    {
        if (!information.mappingDone)
        {
            string prepend = "";
            string append = "";
            for (int i = 0; i < information.residualPropertyHierarchy.Count; i++)
            {
                prepend += NewASPMapperHelper.AspFormat(information.residualPropertyHierarchy[i]) + "(";
                append = ")" + append;

            }
            prepend += "{" + information.firstPlaceholder + "}";
            information.prependMapping.Add(prepend);
            information.appendMapping.Insert(0, append);
            information.mappingDone = true;
        }
        return information.Mapping();
    }
    #region ABSTRACT METHODS

    public abstract Type GetAggregationTypes(Type type=null);
   
    

    public abstract string BasicMap(object value);









    #endregion
}
