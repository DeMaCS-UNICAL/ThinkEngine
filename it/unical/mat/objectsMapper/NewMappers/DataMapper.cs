using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MonoBehaviourActuatorHider;
using static MonoBehaviourSensorHider;

internal abstract class DataMapper
{
    internal abstract List<Type> supportedTypes { get; } //List of the Types supported by the implementing mapper
    internal abstract string SensorBasicMap(object currentObject);//The translation from the C# syntax to the external solver one for a sensor mapper
    internal abstract string ActuatorBasicMap(object currentObject); //The back translation from the external solver to C# syntax for an actuator mapper
    internal abstract void Register(); // This should invoke one or both of 
                                       //MapperManager.RegisterSensorMapper(this); if the inherited class is a sensor mapper
                                       //MapperManager.RegisterActuatorMapper(this); if the inherited class is an actuator mapper
    internal abstract void SetPropertyValue(object actualObject, MyListString propertyHierarchy, object value);// Set the value of the property or field (propertyHierarchy) for the object actualObject
    internal abstract void InstantiateSensors(object actualObject, MyListString propertyHierarchy);
    internal abstract void InstantiateActuators(object actualObject, MyListString propertyHierarchy);
    internal abstract void ManageSensors(object actualObject, MyListString propertyHierarchy, List<MonoBehaviourSensor> instantiatedSensors);
    internal abstract void ManageActuators(object actualObject, MyListString propertyHierarchy, List<MonoBehaviourActuator> instantiatedActuators);
}