using System.Collections.Generic;

internal interface IActuators
{
    bool IsEmpty();
    List<NewMonoBehaviourActuator> GetActuatorsList();
}