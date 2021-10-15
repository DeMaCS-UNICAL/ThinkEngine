using System.Collections.Generic;

internal interface IActuators
{
    bool IsEmpty();
    List<MonoBehaviourActuator> GetActuatorsList();
}