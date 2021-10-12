using NewStructures;
using NewStructures.NewMappers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NewMappers.NewOperationContainer;

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
    void UpdateSensor(NewMonoBehaviourSensor sensor, object currentObject, MyListString residualPropertyHierarchy, int level);
    ISensors ManageSensors(InstantiationInformation information, ISensors sensors);
    string SensorBasicMap(NewMonoBehaviourSensor sensor, object currentObject, int hierarchyLevel, MyListString residualPropertyHierarchy, List<object> valuesForPlaceholders);//The translation from the C# syntax to the external solver one for a sensor mapper
    IActuators InstantiateActuators(InstantiationInformation information);
    IActuators ManageActuators(InstantiationInformation information, IActuators actuators);
    string ActuatorBasicMap(NewMonoBehaviourActuator actuator, object currentObject, int hierarchyLevel, MyListString residualPropertyHierarchy, List<object> valuesForPlaceHolders); //The back translation from the external solver to C# syntax for an actuator mapper
    void SetPropertyValue(NewMonoBehaviourActuator actuator, MyListString propertyHierarchy, object currentObject, object valueToSet, int level);// Set the value of the property or field (propertyHierarchy) for the object actualObject
    string GetASPTemplate(ref InstantiationInformation information, List<string> variables);
}