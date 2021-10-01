using newMappers;
using NewMappers;
using NewStructures;
using NewStructures.NewMappers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static NewMappers.NewOperationContainer;

internal abstract class BasicTypeMapper : IDataMapper
{
    #region SUPPORT CLASSES
    private class BasicTypeInfoAndValue : IInfoAndValue
    {
        internal NewOperation operation;
        internal string specificValue;
        internal List<object> values;
        internal BasicTypeInfoAndValue()
        {
            values = new List<object>();
        }
    }
    private class BasicTypeSensor : ISensors
    {
        internal NewMonoBehaviourSensor sensor;
        internal BasicTypeSensor(NewMonoBehaviourSensor sensor)
        {
            this.sensor = sensor;
        }

        public bool IsEmpty()
        {
            return sensor == null;
        }
    }
    private class BasicTypeActuator : IActuators
    {
        internal NewMonoBehaviourActuator actuator;
        internal BasicTypeActuator(NewMonoBehaviourActuator actuator)
        {
            this.actuator = actuator;
        }

        public bool IsEmpty()
        {
            return actuator == null;
        }
    }
    #endregion
    #region ENUMS
    internal enum NumericOperations { Newest, Oldest, Specific_Value, Max, Min, Avg };
    internal enum BoolOperations { Newest, Oldest, Specific_Value, Conjunction, Disjunction };
    internal enum StringOperations { Newest, Oldest, Specific_Value };
    #endregion

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
    internal List<NewOperation> _operationList;
    public List<NewOperation> OperationList()
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
        throw new NotImplementedException();
    }

    public bool IsFinal(Type t)
    {
        return true;
    }
    public bool Supports(Type type)
    {
        return SupportedTypes.Contains(type);
    }
    public void UpdateSensor(NewMonoBehaviourSensor sensor, object actualValue, MyListString property, int hierarchyLevel)
    {
        if (!Supports(actualValue.GetType()))
        {
            throw new Exception("Wrong mapper for property " + property);
        }

        List<object> values = ((BasicTypeInfoAndValue)sensor.PropertyInfo[hierarchyLevel]).values;
        if (values.Count == 200)
        {
            values.RemoveAt(0);
        }
        values.Add(Convert.ChangeType(actualValue,ConvertingType));
    }

    public string SensorBasicMap(NewMonoBehaviourSensor sensor, object currentObject, int hierarchyLevel, MyListString residualPropertyHierarchy, List<object> valuesForPlaceholders)
    {
        if (!SupportedTypes.Contains(currentObject.GetType()) || residualPropertyHierarchy.Count>1)
        {
            throw new Exception("Type " + currentObject.GetType() + " is not supported as Sensor");
        }
        BasicTypeInfoAndValue infoAndValue = (BasicTypeInfoAndValue)sensor.PropertyInfo[hierarchyLevel];
        string value = BasicMap(infoAndValue.operation(infoAndValue.values));
        valuesForPlaceholders.Add(value);
        return string.Format(sensor.Mapping, valuesForPlaceholders.ToArray());
    }
    public ISensors InstantiateSensors(InstantiationInformation information)
    {
        if (!SupportedTypes.Contains(information.currentObjectOfTheHierarchy.GetType()) || information.residualPropertyHierarchy.Count > 1)
        {
            throw new Exception("Wrong mapper for property " + information.propertyHierarchy);
        }
        
        BasicTypeInfoAndValue additionalInfo = new BasicTypeInfoAndValue();
        int operationIndex = ((NewSensorConfiguration)information.configuration).OperationPerProperty[information.propertyHierarchy.GetHashCode()];
        additionalInfo.operation = OperationList()[operationIndex];
        if (operationIndex == GetAggregationSpecificIndex())
        {
            additionalInfo.specificValue = ((NewSensorConfiguration)information.configuration).SpecificValuePerProperty[information.propertyHierarchy.GetHashCode()];
        }
        BasicTypeSensor sensor = new BasicTypeSensor(information.instantiateOn.AddComponent<NewMonoBehaviourSensor>());
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
            throw new Exception("Wrong mapper for property " + information.propertyHierarchy);
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
            throw new Exception("Wrong mapper for property " + information.propertyHierarchy);
        }
        BasicTypeActuator actuator = new BasicTypeActuator(information.instantiateOn.AddComponent<NewMonoBehaviourActuator>());
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
            throw new Exception("Wrong mapper for property " + information.propertyHierarchy);
        }
        return actuators;
    }
    public string ActuatorBasicMap(NewMonoBehaviourActuator actuator, object currentObject, int hierarchyLevel, MyListString residualPropertyHierarchy, List<object> valuesForPlaceholders)
    {
        if (!SupportedTypes.Contains(currentObject.GetType()) || residualPropertyHierarchy.Count > 1)
        {
            throw new Exception("Type " + currentObject.GetType() + " is not supported as Sensor");
        }
        valuesForPlaceholders.Add(currentObject);
        return string.Format(actuator.Mapping, valuesForPlaceholders.ToArray()); 
    }
    public void SetPropertyValue(NewMonoBehaviourActuator actuator, MyListString propertyHierarchy, ref object currentObject, object valueToSet, int level)
    {
        throw new NotImplementedException();
    }
    internal object GetConvertedValue(object valueToSet)
    {
        return Convert.ChangeType(valueToSet, ConvertingType);
    }
    public Dictionary<MyListString, KeyValuePair<Type, object>> RetrieveProperties(Type objectType, MyListString currentObjectPropertyHierarchy, object currentObject)
    {
        if (!SupportedTypes.Contains(objectType))
        {
            throw new Exception("Type " + objectType + " is not supported by " + this.GetType());
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
