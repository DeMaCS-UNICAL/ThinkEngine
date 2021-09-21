using NewStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NewMappers
{
    internal interface SensorMapper
    {
        string SensorBasicMap(NewMonoBehaviourSensor sensor);//The translation from the C# syntax to the external solver one for a sensor mapper
        void UpdateSensor(NewMonoBehaviourSensor sensor, object currentObject);
        Sensors InstantiateSensors(GameObject gameobject,object actualObject, MyListString propertyHierarchy, MyListString property, NewSensorConfiguration configuration);
        Sensors ManageSensors(GameObject gameobject, object currentObject, MyListString propertyHierarchy, MyListString residualPropertyHierarchy, NewSensorConfiguration configuration, Sensors sensors);


    }
}
