using System.Collections.Generic;

namespace NewStructures
{
    interface ISensors
    {
        bool IsEmpty();
        List<NewMonoBehaviourSensor> GetSensorsList();
    }
}