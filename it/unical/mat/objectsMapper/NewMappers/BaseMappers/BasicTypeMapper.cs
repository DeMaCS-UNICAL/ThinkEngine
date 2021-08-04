using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


internal abstract class BasicTypeMapper : DataMapper
{
    internal override void SetPropertyValue(object actualObject, MyListString property, object value)
    {
        throw new NotImplementedException();
    }

    internal override void InstantiateSensors(object actualObject, MyListString propertyHierarchy)
    {
        throw new NotImplementedException();
    }

    internal override void InstantiateActuators(object actualObject, MyListString propertyHierarchy)
    {
        throw new NotImplementedException();
    }

    internal override void ManageSensors(object actualObject, MyListString propertyHierarchy, List<MonoBehaviourSensorHider.MonoBehaviourSensor> instantiatedSensors)
    {
        throw new NotImplementedException();
    }

    internal override void ManageActuators(object actualObject, MyListString propertyHierarchy, List<MonoBehaviourActuatorHider.MonoBehaviourActuator> instantiatedActuators)
    {
        throw new NotImplementedException();
    }
}
