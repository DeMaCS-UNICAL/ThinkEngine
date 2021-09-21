using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MonoBehaviourActuatorHider;

namespace NewMappers
{
    interface ActuatorMapper
    {
        string ActuatorBasicMap(object currentObject); //The back translation from the external solver to C# syntax for an actuator mapper
        void SetPropertyValue(object actualObject, MyListString propertyHierarchy, object value);// Set the value of the property or field (propertyHierarchy) for the object actualObject
        void InstantiateActuators(object actualObject, MyListString propertyHierarchy);
        void ManageActuators(object actualObject, MyListString propertyHierarchy, List<MonoBehaviourActuator> instantiatedActuators);

    }
}
