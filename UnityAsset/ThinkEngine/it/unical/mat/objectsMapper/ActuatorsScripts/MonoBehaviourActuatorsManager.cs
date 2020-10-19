using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MonoBehaviourActuatorsManager:MonoBehaviour
{
    public Dictionary<ActuatorConfiguration,List<MonoBehaviourActuator>> configurations;

    void Awake()
    {
        configurations = new Dictionary<ActuatorConfiguration, List<MonoBehaviourActuator>>();
    }
    public void instantiateActuator(ActuatorConfiguration actuatorConfiguration, Brain invoker)
    {
        if (!configurations.Keys.Contains(actuatorConfiguration) && actuatorConfiguration.assignedTo is null)
        {
            if (actuatorConfiguration.gameObject.Equals(gameObject) && actuatorConfiguration.assignedTo.Equals(invoker))
            {
                configurations.Add(actuatorConfiguration, generateActuator(actuatorConfiguration));
            }
        }
    }

    private List<MonoBehaviourActuator> generateActuator(ActuatorConfiguration actuatorConfiguration)
    {
        List<MonoBehaviourActuator> generatedActuators = new List<MonoBehaviourActuator>();
        foreach(MyListString currentProperty in actuatorConfiguration.properties)
        {
            MonoBehaviourActuator newActuator = gameObject.AddComponent<MonoBehaviourActuator>();
            newActuator.property = currentProperty;
            newActuator.actuatorName = actuatorConfiguration.configurationName;
            generatedActuators.Add(newActuator);
        }
        return generatedActuators;
    }
}
