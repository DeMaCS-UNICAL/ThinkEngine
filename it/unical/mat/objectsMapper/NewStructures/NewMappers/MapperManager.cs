using NewMappers;
using NewMappers.IntermediateMappers;
using NewStructures;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

internal class MapperManager
{

    static Dictionary<Type, SensorDataMapper> metSensorMappers; //each type that is supported for sensors is associated with the actual data mapper 
                                                        //that is a derived class of DataMapper. The derived class register itself to this dictionary
    static Dictionary<Type, ActuatorDataMapper> metActuatorMappers; //each type that is supported for actuators is associated with the actual data mapper
                                                            //that is a derived class of DataMapper. The derived class register itself to this dictionary
    static List<SensorDataMapper> sensorMappers;
    static List<ActuatorDataMapper> actuatorMappers;
    static MapperManager()
    {
        metSensorMappers = new Dictionary<Type, SensorDataMapper>();
        metActuatorMappers = new Dictionary<Type, ActuatorDataMapper>();
        sensorMappers = new List<SensorDataMapper>();
        actuatorMappers = new List<ActuatorDataMapper>();
        RegisterMappers();
    }

    #region PUBLIC STATIC METHODS
    internal static SensorDataMapper GetMapper(Type type)
    {
        if (ExistsSensorMapper(type))
        {
            return metSensorMappers[type];
        }
        return null;
    }

    internal static Sensors InstantiateSensors(GameObject gameobject, MyListString propertyHierarchy, NewSensorConfiguration configuration)
    {
        MyListString residualPropertyHierarchy = propertyHierarchy.GetClone();
        object currentObject = gameobject;
        bool found = false ;
        Type currentType = RetrieveMapper(ref residualPropertyHierarchy, ref currentObject, out found);
        if (found)
        {
            return metSensorMappers[currentType].InstantiateSensors(gameobject, currentObject, propertyHierarchy, residualPropertyHierarchy, configuration);
        }
        return null;
    }
    internal static Type RetrieveMapper(ref MyListString residualPropertyHierarchy, ref object currentObject, out bool found)
    {
        Type currentType;
        while (residualPropertyHierarchy.Count > 0)
        {
            string currentProperty = residualPropertyHierarchy[0];
            currentObject = RetrieveProperty(currentObject, currentProperty, out currentType);
            if (currentType!= null && ExistsSensorMapper(currentType))
            {
                found = true;
                return currentType;
            }
            else
            {
                if (currentObject == null)
                {
                    found = false;
                    return currentType; 
                }
            }
            residualPropertyHierarchy.RemoveAt(0);
        }
        found = false;
        return null;
    }
    internal static object RetrieveProperty(object currentObject, string currentProperty, out Type currentType)
    {
        MemberInfo[] members = currentObject.GetType().GetMember(currentProperty,Utility.BindingAttr);
        if (members.Length > 0)
        {
            FieldOrProperty fieldOrProperty = new FieldOrProperty(members[0]);
            currentObject = fieldOrProperty.GetValue(currentObject);
            currentType = fieldOrProperty.Type();
            return currentObject;
        }
        if (currentObject is GameObject)
        {
            foreach (Component component in ((GameObject)currentObject).GetComponents<Component>())
            {
                if (component != null)
                {
                    if (component.GetType().Name.Equals(currentProperty))
                    {
                        currentObject = component;
                        currentType = component.GetType();
                        return currentObject;
                    }
                }
            }
        }
        currentType = null;
        return null;
    }
    internal static Sensors ManageSensors(GameObject gameobject, MyListString propertyHierarchy, NewSensorConfiguration configuration,Sensors sensors)
    {
        MyListString residualPropertyHierarchy = propertyHierarchy.GetClone();
        object currentObject = gameobject;
        bool found = false;
        Type currentType = RetrieveMapper(ref residualPropertyHierarchy, ref currentObject, out found);
        if (found)
        {
            return metSensorMappers[currentType].ManageSensors(gameobject, currentObject, propertyHierarchy, residualPropertyHierarchy, configuration, sensors);
        }
        return null;
    }
    internal static string GetSensorBasicMap(NewMonoBehaviourSensor sensor)
    {
        if (!ExistsSensorMapper(sensor.currentPropertyType))
        {
            throw new Exception("Type " + sensor.currentPropertyType + " is not supported as Sensor");
        }
        return metSensorMappers[sensor.currentPropertyType].SensorBasicMap(sensor);
    }
    internal static void UpdateSensor(NewMonoBehaviourSensor sensor)
    {
        MyListString residualPropertyHierarchy = sensor.property.GetClone();
        object currentObject = sensor.gameObject;
        bool found = false;
        Type currentType = RetrieveMapper(ref residualPropertyHierarchy, ref currentObject, out found);
        if (found)
        {
            metSensorMappers[currentType].UpdateSensor(sensor,currentObject);
        }
    }
    internal static string GetActuatorBasicMap(object currentObject)
    {
        if (!ExistsActuatorMapper(currentObject.GetType()))
        {
            throw new Exception("Type " + currentObject.GetType() + " is not supported as Actuator");
        }
        return metActuatorMappers[currentObject.GetType()].ActuatorBasicMap(currentObject);
    }
    internal static Dictionary<MyListString,KeyValuePair<Type,object>> RetrieveSensorProperties(Type objectType, MyListString currentObjectPropertyHierarchy, object currentObject)
    {
        if (ExistsSensorMapper(objectType))
        {
            return metSensorMappers[objectType].RetrieveProperties(objectType, currentObjectPropertyHierarchy, currentObject);
        }
        else
        {
            return RetrieveProperties(objectType, currentObjectPropertyHierarchy, currentObject);
        }
    }
    internal static Dictionary<MyListString, KeyValuePair<Type, object>> RetrieveActuatorProperties(Type objectType, MyListString currentObjectPropertyHierarchy, object currentObject)
    {
        if (ExistsActuatorMapper(objectType))
        {
            return metActuatorMappers[currentObject.GetType()].RetrieveProperties(objectType, currentObjectPropertyHierarchy, currentObject);
        }
        else
        {
            return RetrieveProperties(objectType, currentObjectPropertyHierarchy, currentObject);
        }
    }
    internal static bool NeedsAggregates(Type type)
    {
        if (ExistsSensorMapper(type))
        {
            return metSensorMappers[type] is NeedingAggregatesMapper;
        }
        return false;
    }
    internal static bool NeedsSpecifications(Type type)
    {
        if (ExistsSensorMapper(type))
        {
            return metSensorMappers[type].NeedsSpecifications();
        }
        return true;
    }
    internal static Type GetAggregationTypes(Type type)
    {
        if (ExistsSensorMapper(type))
        {
            if (metSensorMappers[type] is NeedingAggregatesMapper)
            {
                return ((NeedingAggregatesMapper) metSensorMappers[type]).GetAggregationTypes();
            }
        }
        return null;
    }
    internal static int GetAggregationSpecificIndex(Type type)
    {
        if (ExistsSensorMapper(type))
        {
            if (metSensorMappers[type] is NeedingAggregatesMapper)
            {
                return ((NeedingAggregatesMapper)metSensorMappers[type]).GetAggregationSpecificIndex();
            }
        }
        return -1;
    }
    private static Dictionary<MyListString, KeyValuePair<Type, object>> RetrieveProperties(Type objectType, MyListString currentObjectPropertyHierarchy, object currentObject)
    {
        if(currentObject != null && !currentObject.GetType().Equals( objectType))
        {
            throw new Exception("Actual object type differs from declared one.");
        }
        Dictionary<MyListString, KeyValuePair<Type, object>> toReturn = new Dictionary<MyListString, KeyValuePair<Type, object>>();
        foreach (MemberInfo member in objectType.GetMembers(Utility.BindingAttr))
        {
            bool isField = member.MemberType.Equals(MemberTypes.Field);
            bool isProperty = member.MemberType.Equals(MemberTypes.Property);
            if (member != null && (isField || isProperty))
            {
                MyListString toAdd = new MyListString(currentObjectPropertyHierarchy.myStrings);
                toAdd.Add(member.Name);
                FieldOrProperty fieldOrProperty = new FieldOrProperty(member);
                object propertyValue;
                try
                {
                    propertyValue = currentObject != null ? fieldOrProperty.GetValue(currentObject) : null;
                }
                catch
                {
                    propertyValue = null;
                }
                KeyValuePair<Type, object> propertyPair = new KeyValuePair<Type, object>(fieldOrProperty.Type(), propertyValue);
                toReturn.Add(toAdd, propertyPair);
            }
        }
        if(currentObject is GameObject)
        {
            foreach(Component component in ((GameObject)currentObject).GetComponents(typeof(Component))){
                if(component != null)
                {
                    MyListString toAdd = new MyListString(currentObjectPropertyHierarchy.myStrings);
                    toAdd.Add(component.GetType().Name);
                    if (toReturn.ContainsKey(toAdd))
                    {
                        continue;
                    }
                    KeyValuePair<Type, object> propertyPair = new KeyValuePair<Type, object>(component.GetType(), component);
                    toReturn.Add(toAdd, propertyPair);
                }
            }
        }
        return toReturn;
    }
    internal static void RegisterMappers()
    {
        foreach (Type mapper in typeof(DataMapper).Assembly.GetTypes())
        {
            if(mapper.IsSubclassOf(typeof(SensorDataMapper)))
            {
                sensorMappers.Add((SensorDataMapper)mapper);
            }
            else if(mapper.IsSubclassOf(typeof(ActuatorDataMapper)))
            {
                actuatorMappers.Add((ActuatorDataMapper)mapper);
            }
        }
    }
    internal static bool IsTypeExpandable(Type type)
    {
        if (ExistsSensorMapper(type))
        {
            return metSensorMappers[type].IsTypeExpandable();
        }
        if (ExistsActuatorMapper(type))
        {
            return metActuatorMappers[type].IsTypeExpandable();
        }
        return true;
    }
    private static bool ExistsActuatorMapper(Type type)
    {
        return metActuatorMappers.ContainsKey(type) || IsSupportedByActuatorMapper(type);
    }
    private static bool ExistsSensorMapper(Type type)
    {
        return metSensorMappers.ContainsKey(type) || IsSupportedBySensorMapper(type);
    }
    private static bool IsSupportedByActuatorMapper(Type type)
    {
        foreach(ActuatorDataMapper mapper in actuatorMappers)
        {
            if (mapper.Supports(type))
            {
                metActuatorMappers.Add(type, mapper);
                return true;
            }
        }
        return false;
    }
    private static bool IsSupportedBySensorMapper(Type type)
    {
        foreach (SensorDataMapper mapper in sensorMappers)
        {
            if (mapper.Supports(type))
            {
                metSensorMappers.Add(type, mapper);
                return true;
            }
        }
        return false;
    }
    #endregion

    #region UNIMPLEMENTED
    internal static void SetPropertyValue(object actualObject, MyListString property, object value)
    {
        if (!ExistsActuatorMapper(value.GetType()))
        {
            throw new Exception("Type " + value.GetType() + " is not supported as Actuator");
        }
        //metActuatorMappers[value.GetType()].SetPropertyValue(actualObject, property, value);
    }
    #endregion
}

