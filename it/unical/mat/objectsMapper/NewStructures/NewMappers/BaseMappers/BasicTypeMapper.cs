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
    public void SetPropertyValue(object actualObject, MyListString propertyHierarchy, object value)
    {
        if (value == null)
        {
            throw new Exception("Value to set can not be null.");
        }
        RetrieveProperty(actualObject, propertyHierarchy, out object secondLastPropertyValue, out FieldOrProperty property);
        property.SetValue(secondLastPropertyValue, Convert.ChangeType(value, property.Type()));
    }
    private void RetrieveProperty(object actualObject, MyListString property, out object currentPropertyValue, out FieldOrProperty currentProperty)
    {
        if (actualObject == null || property.Count == 0 || !SupportedTypes.Contains(actualObject.GetType()))
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
    public void UpdateSensor(NewMonoBehaviourSensor sensor, object actualValue, MyListString property, int hierarchyLevel)
    {
        if (!Supports(actualValue.GetType()))
        {
            throw new Exception("Wrong mapper for property " + property);
        }

        List<object> values = ((BasicTypeInfoAndValue)sensor.propertyInfo[hierarchyLevel]).values;
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
        BasicTypeInfoAndValue infoAndValue = (BasicTypeInfoAndValue)sensor.propertyInfo[hierarchyLevel];
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
        if (!(instantiatedSensors is BasicTypeSensor))
        {
            throw new Exception("Wrong mapper for property " + information.propertyHierarchy);
        }
        if (((BasicTypeSensor)instantiatedSensors).sensor == null)
        {
            return InstantiateSensors(information);
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
    
    public void InstantiateActuators(object actualObject, MyListString propertyHierarchy)
    {
        throw new NotImplementedException();
    }
   
    public void ManageActuators(object actualObject, MyListString propertyHierarchy, List<MonoBehaviourActuatorHider.MonoBehaviourActuator> instantiatedActuators)
    {
        throw new NotImplementedException();
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
            append += ")";//in order to close the brachets of confName(gameObjectName, objectindex(X),
            prepend += "{" + information.firstPlaceholder + "}";
            information.prependMapping.Add(prepend);
            information.appendMapping.Insert(0, append);
            information.mappingDone = true;
        }
        return information.Mapping();
    }
    #region ABSTRACT METHODS

    public abstract Type GetAggregationTypes(Type type=null);
    public  string ActuatorBasicMap(object obj)
    {
        throw new NotImplementedException();
    }
    

    public abstract string BasicMap(object value);






    #endregion
}
