using System.Collections.Generic;

namespace Structures
{
    interface ISensors
    {
        bool IsEmpty();
        List<MonoBehaviourSensor> GetSensorsList();
    }
}