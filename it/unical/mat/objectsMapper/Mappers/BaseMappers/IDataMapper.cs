using System;
using System.Collections.Generic;

namespace Mappers.BaseMappers
{
    interface IDataMapper
    {
        bool NeedsSpecifications(Type type);
        bool NeedsAggregates(Type type);
        bool IsTypeExpandable(Type type);
        bool Supports(Type type);
        bool IsFinal(Type t);
        Type GetAggregationTypes(Type type = null);
        int GetAggregationSpecificIndex(Type type);

        Dictionary<MyListString, KeyValuePair<Type, object>> RetrieveProperties(Type objectType, MyListString currentObjectPropertyHierarchy, object currentObject);
        ISensors InstantiateSensors(InstantiationInformation information);
        void UpdateSensor(Sensor sensor, object currentObject, MyListString residualPropertyHierarchy, int level);
        ISensors ManageSensors(InstantiationInformation information, ISensors sensors);
        IActuators InstantiateActuators(InstantiationInformation information);
        IActuators ManageActuators(InstantiationInformation information, IActuators actuators);
        string ActuatorBasicMap(MonoBehaviourActuator actuator, object currentObject, int hierarchyLevel, MyListString residualPropertyHierarchy, List<object> valuesForPlaceHolders); //The back translation from the external solver to C# syntax for an actuator mapper
        void SetPropertyValue(MonoBehaviourActuator actuator, MyListString propertyHierarchy, object currentObject, object valueToSet, int level);// Set the value of the property or field (propertyHierarchy) for the object actualObject
        string GetASPTemplate(ref InstantiationInformation information, List<string> variables);
    }
}