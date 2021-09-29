using newMappers;
using NewStructures;
using NewStructures.NewMappers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

internal class MapperManager
{

    static readonly Dictionary<Type, IDataMapper> metMappers; //each type that is supported for sensors is associated with the actual data mapper 
                                                        //that is a derived class of DataMapper. The derived class register itself to this dictionary
    static readonly List<Type> unsupportedTypes;
    static readonly List<IDataMapper> mappers;
    static MapperManager()
    {
        metMappers = new Dictionary<Type, IDataMapper>();
        mappers = new List<IDataMapper>();
        unsupportedTypes = new List<Type>();
        RegisterMappers();
    }

    #region PUBLIC STATIC METHODS

    internal static bool IsFinal(Type type)
    {
        if (ExistsMapper(type))
        {
            return metMappers[type].IsFinal(type);
        }
        return false;
    }
    internal static ISensors InstantiateSensors(InstantiationInformation information)
    {
        IDataMapper mapper = RetrieveAdditionalInformation(ref information,!information.mappingDone);
        return mapper?.InstantiateSensors(information);
    }
    internal static ISensors ManageSensors(InstantiationInformation information, ISensors sensors)
    {
        IDataMapper mapper = RetrieveAdditionalInformation(ref information, !information.mappingDone);
        return mapper?.ManageSensors(information, sensors);
    }
    private static IDataMapper RetrieveAdditionalInformation(ref InstantiationInformation information, bool generateMapping=false)
    {
        object currentObject = information.currentObjectOfTheHierarchy;
        IDataMapper mapper = RetrieveMapper(ref information.residualPropertyHierarchy, ref currentObject);
        if (mapper != null)
        {
            information.currentObjectOfTheHierarchy = currentObject;
            if (generateMapping)
            {
                GenerateMapping(ref information);
            }
        }
        else
        {
            information.prependMapping.Clear();
            information.appendMapping.Clear();
        }

        return mapper;
    }
    private static void GenerateMapping(ref InstantiationInformation information)
    {
        int upTo = information.propertyHierarchy.Count - information.residualPropertyHierarchy.Count;
        string prepend = "";
        string append = "";
        for (int i = information.prependMapping.Count; i < upTo; i++)
        {
            prepend += NewASPMapperHelper.AspFormat(information.propertyHierarchy[i]) + "(";
            append = ")"+append;
        }
        information.prependMapping.Add(prepend);
        information.appendMapping.Add(append);
    }
    
    internal static IDataMapper RetrieveMapper(ref MyListString residualPropertyHierarchy, ref object currentObject)
    {
        if(residualPropertyHierarchy.Count==0 && currentObject != null)
        {
            if (ExistsMapper(currentObject.GetType()))
            {
                return metMappers[currentObject.GetType()];
            }
        }
        while (residualPropertyHierarchy.Count > 0)
        {
            string currentProperty = residualPropertyHierarchy[0];
            currentObject = RetrieveProperty(currentObject, currentProperty, out Type currentType);
            if (currentType != null && ExistsMapper(currentType))
            {
                return metMappers[currentType];
            }
            else
            {
                if (currentObject == null)
                {
                    return null;
                }
            }
            residualPropertyHierarchy.RemoveAt(0);
        }
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
        if (currentObject is GameObject @object)
        {
            foreach (Component component in @object.GetComponents<Component>())
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
    
    internal static string GetSensorBasicMap(NewMonoBehaviourSensor sensor, object currentObject, MyListString residualPropertyHierarchy, List<object> values, int level)
    {
        IDataMapper mapper = RetrieveMapper(ref residualPropertyHierarchy, ref currentObject);
        if (mapper != null)
        {
            return mapper.SensorBasicMap(sensor, currentObject, level, residualPropertyHierarchy, values);
        }
        else
        {
            return "";
        }
    }
    internal static void UpdateSensor(NewMonoBehaviourSensor sensor, object currentObject, MyListString residualPropertyHierarchy, int level)
    {
        IDataMapper mapper = RetrieveMapper(ref residualPropertyHierarchy, ref currentObject);
        if (mapper!=null)
        {
            mapper.UpdateSensor(sensor,currentObject,residualPropertyHierarchy, level);
        }
    }
    internal static string GetActuatorBasicMap(object currentObject)
    {
        if (!ExistsMapper(currentObject.GetType()))
        {
            throw new Exception("Type " + currentObject.GetType() + " is not supported as Actuator");
        }
        return metMappers[currentObject.GetType()].ActuatorBasicMap(currentObject);
    }
    internal static Dictionary<MyListString,KeyValuePair<Type,object>> RetrieveProperties(Type objectType, MyListString currentObjectPropertyHierarchy, object currentObject)
    {
        if (ExistsMapper(objectType))
        {
            return metMappers[objectType].RetrieveProperties(objectType, currentObjectPropertyHierarchy, currentObject);
        }
        else
        {
            return RetrieveGeneralProperties(objectType, currentObjectPropertyHierarchy, currentObject);
        }
    }
    internal static bool NeedsAggregates(Type type)
    {
        if (ExistsMapper(type))
        {
            return metMappers[type].NeedsAggregates(type);
        }
        return false;
    }
    internal static bool NeedsSpecifications(Type type)
    {
        if (ExistsMapper(type))
        {
            return metMappers[type].NeedsSpecifications(type);
        }
        return true;
    }
    internal static Type GetAggregationTypes(Type type)
    {
        if (ExistsMapper(type))
        {
            if (metMappers[type].NeedsAggregates(type))
            {
                return metMappers[type].GetAggregationTypes(type);
            }
        }
        return null;
    }
    internal static int GetAggregationSpecificIndex(Type type)
    {
        if (ExistsMapper(type))
        {
            if (metMappers[type].NeedsAggregates(type))
            {
                return metMappers[type].GetAggregationSpecificIndex(type);
            }
        }
        return -1;
    }
    private static Dictionary<MyListString, KeyValuePair<Type, object>> RetrieveGeneralProperties(Type objectType, MyListString currentObjectPropertyHierarchy, object currentObject)
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
        if(currentObject is GameObject @object)
        {
            foreach(Component component in @object.GetComponents(typeof(Component))){
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
        foreach (Type type in typeof(IDataMapper).Assembly.GetTypes())
        {
            CheckAndRegister(type,mappers);
        }
    }

    private static void CheckAndRegister(Type derivedType, IList mappers)
    {
        if (typeof(IDataMapper).IsAssignableFrom(derivedType))
        {
            PropertyInfo instanceProperty = derivedType.GetProperty("Instance", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            if (instanceProperty != null)
            {
                object instance = instanceProperty.GetValue(null);
                if (instance != null)
                {
                    mappers.Add(instance);
                }
            }
        }
    }

    internal static bool IsTypeExpandable(Type type)
    {
        if (ExistsMapper(type))
        {
            return metMappers[type].IsTypeExpandable(type);
        }
        return true;
    }
    private static bool ExistsMapper(Type type)
    {
        return !unsupportedTypes.Contains(type) && ( metMappers.ContainsKey(type) || IsSupportedByMapper(type));
    }
    private static bool IsSupportedByMapper(Type type)
    {
        foreach (IDataMapper mapper in mappers)
        {
            if (mapper.Supports(type))
            {
                metMappers.Add(type, mapper);
                return true;
            }
        }
        unsupportedTypes.Add(type);
        return false;
    }
    #endregion

    #region UNIMPLEMENTED
    internal static void SetPropertyValue(object actualObject, MyListString property, object value)
    {
        if (!ExistsMapper(value.GetType()))
        {
            throw new Exception("Type " + value.GetType() + " is not supported as Actuator");
        }
        //metActuatorMappers[value.GetType()].SetPropertyValue(actualObject, property, value);
    }
    #endregion
}

