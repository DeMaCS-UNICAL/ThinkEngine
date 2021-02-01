using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static MonoBehaviourActuatorHider;

[ExecuteInEditMode]
internal class MonoBehaviourActuatorsManager : MonoBehaviour
{
    internal Dictionary<ActuatorConfiguration, List<MonoBehaviourActuator>> _actuators;
    internal bool ready = false;
    bool destroyed;

    internal Dictionary<ActuatorConfiguration, List<MonoBehaviourActuator>> actuators
    {
        get
        {
            if (_actuators == null)
            {
                _actuators = new Dictionary<ActuatorConfiguration, List<MonoBehaviourActuator>>();
            }
            return _actuators;
        }
    }
    #region Unity Messages
    void OnEnable()
    {
        Reset();
    }
    void Reset()
    {
        //hideFlags = HideFlags.HideInInspector;
        ActuatorsManager.configurationsChanged = true;
    }
    void Start()
    {
        if (Application.isPlaying)
        {
            ActuatorConfiguration[] configurations = GetComponents<ActuatorConfiguration>();
            if (configurations == null)
            {
                Destroy(this);
                return;
            }
            foreach (ActuatorConfiguration configuration in configurations)
            {
                if (configuration.saved)
                {
                    InstantiateActuator(configuration);
                }
            }
            ready = true;
        }
    }
    void OnDestroy()
    {
        destroyed = true;
    }
    void OnDisable()
    {
        destroyed = true;
    }
    void OnApplicationQuit()
    {
        destroyed = true;
    }
    #endregion
    public void InstantiateActuator(ActuatorConfiguration actuatorConfiguration)
    {
        if (!actuators.Keys.Contains(actuatorConfiguration))
        {
            if (actuatorConfiguration.gameObject.Equals(gameObject))
            {
                actuators.Add(actuatorConfiguration, GenerateActuator(actuatorConfiguration));
            }
        }
    }
    private List<MonoBehaviourActuator> GenerateActuator(ActuatorConfiguration actuatorConfiguration)
    {
        List<MonoBehaviourActuator> generatedActuators = new List<MonoBehaviourActuator>();
        foreach (MyListString currentProperty in actuatorConfiguration.properties)
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
        ActuatorConfiguration[] configurations = GetComponents<ActuatorConfiguration>();
        if (configurations == null)
        {
            Destroy(this);
            return toReturn;
        }
        foreach (ActuatorConfiguration configuration in configurations)
        {
            toReturn.Add(configuration.configurationName);
        }
        return toReturn;
    }
    internal ActuatorConfiguration GetConfiguration(string name)
    {
        ActuatorConfiguration[] configurations = GetComponents<ActuatorConfiguration>();
        if (configurations == null)
        {
            Destroy(this);
            return null;
        }
        foreach (ActuatorConfiguration configuration in configurations)
        {
            if (configuration.saved && configuration.configurationName.Equals(name))
            {
                return configuration;
            }
        }
        return null;
    }
    internal void AddConfiguration(ActuatorConfiguration configuration)
    {
        ActuatorsManager.configurationsChanged = true;
    }
    internal void DeleteConfiguration(ActuatorConfiguration configuration)
    {
        if (destroyed)
        {
            return;
        }
        ActuatorsManager.configurationsChanged = true;
        if (GetComponents<ActuatorConfiguration>().Length == 1)
        {
            if (Application.isPlaying)
            {
                Destroy(this);
            }
            else
            {
                DestroyImmediate(this);
            }
        }
    }
}

