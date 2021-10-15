using System.Collections.Generic;


interface ISensors
{
    bool IsEmpty();
    List<MonoBehaviourSensor> GetSensorsList();
}
