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
    internal bool ready = false;
    void Awake()
    {
        Reset();
    }
    void OnEnable()
    {
        Reset();
    }
    void Reset()
    {
        configurations = new Dictionary<ActuatorConfiguration, List<MonoBehaviourActuator>>();
        ActuatorsManager actuatorsManager = Utility.actuatorsManager;
        actuatorsManager.configurationsChanged = true;
    }
    void Start()
    {
        if (Application.isPlaying)
        {
            foreach (ActuatorConfiguration configuration in GetComponents<ActuatorConfiguration>())
            {
                if (configuration.saved)
                {
                    instantiateActuator(configuration);
                }
            }
            ready = true;
        }
    }
    public void instantiateActuator(ActuatorConfiguration actuatorConfiguration)
    {
        if (!configurations.Keys.Contains(actuatorConfiguration))
        {
            if (actuatorConfiguration.gameObject.Equals(gameObject))
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
    internal IEnumerable<string> GetAllConfigurationNames()
    {
        List<string> toReturn = new List<string>();
        foreach (ActuatorConfiguration configuration in GetComponents<ActuatorConfiguration>())
        {
            toReturn.Add(configuration.configurationName);
        }
        return toReturn;
    }
    internal ActuatorConfiguration getConfiguration(string name)
    {
        foreach (ActuatorConfiguration configuration in GetComponents<ActuatorConfiguration>())
        {
            if (configuration.saved && configuration.configurationName.Equals(name))
            {
                return configuration;
            }
        }
        return null;
    }
    internal void addConfiguration(ActuatorConfiguration configuration)
    {
        Utility.actuatorsManager.configurationsChanged = true;
    }
    internal void deleteConfiguration(ActuatorConfiguration configuration)
    {
        Utility.actuatorsManager.configurationsChanged = true;
        if (GetComponents<ActuatorConfiguration>() == null)
        {
            DestroyImmediate(this);
        }
    }
}

