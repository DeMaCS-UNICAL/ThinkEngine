using NewMappers;
using NewMappers.IntermediateMappers;
using NewStructures;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static NewMappers.NewOperationContainer;

internal abstract class BasicTypeMapper : SensorDataMapperWithAggregates,ActuatorMapper
{
    #region SUPPORT CLASSES
    internal class BasicTypeInfoAndValue : InfoAndValue
    {
        internal NewOperation operation;
        internal string specificValue;
        internal List<object> values;
        internal string mapping;
        internal BasicTypeInfoAndValue()
        {
            values = new List<object>();
            mapping = "";
        }
    }
    internal class BasicTypeSensor : Sensors
    {
        internal NewMonoBehaviourSensor sensor;
        internal BasicTypeSensor(NewMonoBehaviourSensor sensor)
        {
            this.sensor = sensor;
        }
    }
    #endregion
    #region ENUMS
    internal enum NumericOperations { Newest, Oldest, Specific_Value, Max, Min, Avg };
    internal enum BoolOperations { Newest, Oldest, Specific_Value, Conjunction, Disjunction };
    internal enum StringOperations { Newest, Oldest, Specific_Value };
    #endregion

    internal Type _convertingType;
    internal Type convertingType {
        get
        {
            return _convertingType;
        }
    }
    internal List<Type> _supportedTypes;
    internal List<Type> supportedTypes
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
    public bool Supports(Type type)
    {
        return supportedTypes.Contains(type);
    }
    public void SetPropertyValue(object actualObject, MyListString propertyHierarchy, object value)
    {
        if (value == null)
        {
            throw new Exception("Value to set can not be null.");
        }
        object secondLastPropertyValue;
        FieldOrProperty property;
        RetrieveProperty(actualObject, propertyHierarchy, out secondLastPropertyValue, out property);
        property.SetValue(secondLastPropertyValue, Convert.ChangeType(value, property.Type()));
    }
    private void RetrieveProperty(object actualObject, MyListString property, out object currentPropertyValue, out FieldOrProperty currentProperty)
    {
        if (actualObject == null || property.Count == 0 || !supportedTypes.Contains(actualObject.GetType()))
        {
            throw new Exception("Invalid arguments.");
        }
        currentPropertyValue = actualObject;
        int currentHierarchyPosition = 0;
        string currentPropertyName;
        currentProperty = null;
        while (property.Count > currentHierarchyPosition)
        {
            if (currentPropertyValue == null)
            {
                throw new Exception("Impossibile to retrieve property " + property + ". Null value at level " + currentHierarchyPosition + " of the hierarchy.");
            }
            currentPropertyName = property[currentHierarchyPosition++];
            MemberInfo[] members = currentPropertyValue.GetType().GetMember(currentPropertyName);
            if (members.Length > 0)
            {
                currentProperty = new FieldOrProperty(members[0]);
                if (property.Count > currentHierarchyPosition + 1)//at the end, i'm interested in the second-last value
                {
                    currentPropertyValue = currentProperty.GetValue(currentPropertyValue);
                }
            }
            else
            {
                throw new Exception("Impossibile to retrieve property " + property + ". Property in position " + (currentHierarchyPosition - 1) + " of the hierarchy does not exist.");
            }
        }
    }
    public void UpdateSensor(NewMonoBehaviourSensor sensor, object actualValue)
    {
        if (!Supports(actualValue.GetType()))
        {
            throw new Exception("Wrong mapper for property " + sensor.property);
        }

        List<object> values = ((BasicTypeInfoAndValue)sensor.propertyInfo).values;
        if (values.Count == 200)
        {
            values.RemoveAt(0);
        }
        values.Add(Convert.ChangeType(actualValue,convertingType));
    }
    public int GetAggregationSpecificIndex()
    {
        return 2;
    }
    public bool IsTypeExpandable()
    {
        return false;
    }
    public Sensors InstantiateSensors(GameObject gameobject, object actualObject, MyListString propertyHierarchy, MyListString property, NewSensorConfiguration configuration)
    {
        if(!supportedTypes.Contains(actualObject.GetType()) || property.Count != 1)
        {
            throw new Exception("Wrong mapper for property " + propertyHierarchy);
        }
        BasicTypeInfoAndValue additionalInfo = new BasicTypeInfoAndValue();
        int operationIndex = configuration.operationPerProperty[propertyHierarchy.GetHashCode()];
        additionalInfo.operation = OperationList()[operationIndex];
        if(operationIndex == GetAggregationSpecificIndex())
        {
            additionalInfo.specificValue = configuration.specificValuePerProperty[propertyHierarchy.GetHashCode()];
        }
        BasicTypeSensor sensor = new BasicTypeSensor(gameobject.AddComponent<NewMonoBehaviourSensor>());
        additionalInfo.values.Add(Convert.ChangeType( actualObject,convertingType));
        additionalInfo.mapping = GenerateMapping(propertyHierarchy);
        sensor.sensor.Configure(configuration.configurationName, propertyHierarchy, actualObject.GetType(),additionalInfo);
        return sensor;
    }
    public void InstantiateActuators(object actualObject, MyListString propertyHierarchy)
    {
        throw new NotImplementedException();
    }
    public Sensors ManageSensors(GameObject gameobject, object actualObject, MyListString propertyHierarchy, MyListString residualPropertyHierarchy, NewSensorConfiguration configuration, Sensors instantiatedSensors)
    {
        if(!(instantiatedSensors is BasicTypeSensor))
        {
            throw new Exception("Wrong mapper for property " + propertyHierarchy);
        }
        if (((BasicTypeSensor)instantiatedSensors).sensor == null)
        {
            return InstantiateSensors(gameobject, actualObject, propertyHierarchy, residualPropertyHierarchy, configuration);
        }
        return instantiatedSensors;
    }
    public void ManageActuators(object actualObject, MyListString propertyHierarchy, List<MonoBehaviourActuatorHider.MonoBehaviourActuator> instantiatedActuators)
    {
        throw new NotImplementedException();
    }
    public Dictionary<MyListString, KeyValuePair<Type, object>> RetrieveProperties(Type objectType, MyListString currentObjectPropertyHierarchy, object currentObject)
    {
        if (!supportedTypes.Contains(objectType))
        {
            throw new Exception("Type " + objectType + " is not supported by " + this.GetType());
        }
        return new Dictionary<MyListString, KeyValuePair<Type, object>>();
    }
    public bool NeedsSpecifications()
    {
        return true;
    }
    #region ABSTRACT METHODS

    public abstract Type GetAggregationTypes();
    public abstract string SensorBasicMap(NewMonoBehaviourSensor sensor);
    public abstract string ActuatorBasicMap(object obj);
    protected abstract string GenerateMapping(MyListString propertyHierarchy);

    public abstract string BasicMap(object value);



    #endregion
}
