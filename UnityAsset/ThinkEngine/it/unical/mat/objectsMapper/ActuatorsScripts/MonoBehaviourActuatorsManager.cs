using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MonoBehaviourActuatorsManager:MonoBehaviour
{
    private int step = 0;
    internal bool ready;
    public List<ActuatorConfiguration> configurations;
    private int actuatorsAddedd = 0;

    void Awake()
    {
        ////MyDebugger.MyDebug("Awakening");
        //Debug.unityLogger.logEnabled = true;
        configurations = new List<ActuatorConfiguration>();
    }
    public void instantiateActuator(ActuatorConfiguration actuatorConfiguration)
    {
        if (!configurations.Contains(actuatorConfiguration))
        {
            if (actuatorConfiguration.gameObject.Equals(gameObject))
            {
                generateActuator(actuatorConfiguration);
                configurations.Add(actuatorConfiguration);
            }
        }
    }

    private void generateActuator(ActuatorConfiguration actuatorConfiguration)
    {
        MonoBehaviourActuator newActuator = gameObject.AddComponent<MonoBehaviourActuator>();
        foreach (List<string> currentProperty in actuatorConfiguration.properties)
        {
            newActuator.property = currentProperty;
            newActuator.actuatorName = actuatorConfiguration.configurationName;
        }
    }
}
