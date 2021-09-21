using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MonoBehaviourActuatorHider;
using static MonoBehaviourSensorHider;

interface DataMapper
{
    bool IsTypeExpandable();
    Dictionary<MyListString, KeyValuePair<Type, object>> RetrieveProperties(Type objectType, MyListString currentObjectPropertyHierarchy, object currentObject);
    bool Supports(Type type);
}