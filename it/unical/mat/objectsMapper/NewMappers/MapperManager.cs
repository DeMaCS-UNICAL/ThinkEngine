using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


internal class MapperManager
{
    private static MapperManager _instance;
    private static MapperManager instance { 
        get
        {
            if (_instance is null)
            {
                _instance = new MapperManager();
                RegisterMappers();
            }
            return _instance;
        } 
    }
    static List<DataMapper> mappers = MapperCollector.mappers; //is a collection of all the mapper already existing

    static Dictionary<Type, DataMapper> sensorMappers; //each type that is supported for sensors is associated with the actual data mapper 
                                                        //that is a derived class of DataMapper. The derived class register itself to this dictionary
    static Dictionary<Type, DataMapper> actuatorMappers; //each type that is supported for actuators is associated with the actual data mapper 
                                                         //that is a derived class of DataMapper. The derived class register itself to this dictionary

    #region PUBLIC STATIC METHODS

    internal static bool IsSensorMappable(Type currentType)
    {
        return sensorMappers.ContainsKey(currentType);
    }
    internal static bool IsActuatorMappable(Type currentType)
    {
        return actuatorMappers.ContainsKey(currentType);
    }
    internal static string GetSensorBasicMap(object currentObject)
    {
        if (!IsSensorMappable(currentObject.GetType()))
        {
            throw new Exception("Type " + currentObject.GetType() + " is not supported as Sensor");
        }
        return sensorMappers[currentObject.GetType()].SensorBasicMap(currentObject);
    }
    internal static string GetActuatorBasicMap(object currentObject)
    {
        if (!IsActuatorMappable(currentObject.GetType()))
        {
            throw new Exception("Type " + currentObject.GetType() + " is not supported as Actuator");
        }
        return actuatorMappers[currentObject.GetType()].ActuatorBasicMap(currentObject);
    }
    internal static FieldOrProperty RetrieveProperty(object actualObject, MyListString propertyHierarchy) //we assume that the first entry of propertyHierarchy is a direct property of actual object
    {
        FieldOrProperty toReturn = null;
        int pos = 0;
        string nextName;
        object nextObject=actualObject;
        while (nextObject != null && pos < propertyHierarchy.Count - 1)
        {
            nextName = propertyHierarchy[pos++];
            MemberInfo[] members = nextObject.GetType().GetMember(nextName, Utility.BindingAttr);
            if (members.Length == 0)
            {
                return null;
            }
            else
            {
                toReturn = new FieldOrProperty( members[0]);
                nextObject = toReturn.GetValue(nextObject);
            }
        }
        return toReturn;
    }

    internal static FieldOrProperty RetrievePropertyForType(Type actualObject, MyListString propertyHierarchy)
    {
        FieldOrProperty toReturn = null;
        int pos = 0;
        string nextName;
        Type nextType = actualObject;
        while (nextType != null && pos < propertyHierarchy.Count - 1)
        {
            nextName = propertyHierarchy[pos++];
            MemberInfo[] members = nextType.GetMember(nextName, Utility.BindingAttr);
            if (members.Length == 0)
            {
                return null;
            }
            else
            {
                toReturn = new FieldOrProperty(members[0]);
                nextType = toReturn.Type();
            }
        }
        return toReturn;
    }

    internal static KeyValuePair<object,Type> RetrievePropertyValue(object actualObject, MyListString property)
    {
        FieldOrProperty fieldOrProperty = RetrieveProperty(actualObject, property);
        if (fieldOrProperty != null)
        {
            Type fieldOrPropertyType = fieldOrProperty.Type();
            return new KeyValuePair<object, Type>(fieldOrProperty,fieldOrPropertyType);
        }
        throw new Exception("Property or field " + property + " does not exists in the hierarchy of the object " + actualObject);
    }
    internal static void SetPropertyValue(object actualObject, MyListString property, object value)
    {
        if (!IsActuatorMappable(value.GetType()))
        {
            throw new Exception("Type " + value.GetType() + " is not supported as Sensor");
        }
        actuatorMappers[value.GetType()].SetPropertyValue(actualObject, property, value);
    }
    internal static void RegisterMappers()
    {
        foreach (DataMapper mapper in mappers)
        {
            mapper.Register();
        }
    }
    internal static void RegisterSensorMapper(DataMapper mapper)
    {
        foreach (Type t in mapper.supportedTypes)
        {
            if (!sensorMappers.ContainsKey(t))
            {
                sensorMappers.Add(t, mapper);
            }
        }
    }
    internal static void RegisterActuatorMapper(DataMapper mapper)
    {
        foreach (Type t in mapper.supportedTypes)
        {
            if (!actuatorMappers.ContainsKey(t))
            {
                actuatorMappers.Add(t, mapper);
            }
        }
    }
    #endregion

}

