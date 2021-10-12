using NewStructures;
using NewStructures.NewMappers;
using System;
using System.Collections.Generic;
using UnityEngine;

internal class NewMonoBehaviourActuatorsManager : MonoBehaviour
{
    Dictionary<MyListString, int> propertiesIndex;
    Dictionary<int, InstantiationInformation> instantiationInformationForProperty;
    Dictionary<int, IActuators> monoBehaviourActuatorsForProperty;
    internal bool ready;
    internal Dictionary<NewActuatorConfiguration,List<NewMonoBehaviourActuator>> _actuators;
    internal Dictionary<NewActuatorConfiguration,List<NewMonoBehaviourActuator>> Actuators
    {
        get
        {
            if (_actuators == null)
            {
                _actuators = new Dictionary<NewActuatorConfiguration, List<NewMonoBehaviourActuator>>();
            }
            return _actuators;
        }
    }

    void Start()
    {
        propertiesIndex = new Dictionary<MyListString, int>();
        instantiationInformationForProperty = new Dictionary<int, InstantiationInformation>();
        monoBehaviourActuatorsForProperty = new Dictionary<int, IActuators>();
        foreach (NewActuatorConfiguration configuration in GetComponents<NewActuatorConfiguration>())
        {
            Actuators[configuration] = new List<NewMonoBehaviourActuator>();
            foreach (MyListString property in configuration.ToMapProperties)
            {
                int propertyIndex = property.GetHashCode();
                propertiesIndex[property] = propertyIndex;
                InstantiationInformation information = new InstantiationInformation()
                {
                    instantiateOn = gameObject,
                    currentObjectOfTheHierarchy = gameObject,
                    propertyHierarchy = new MyListString(property.myStrings),
                    residualPropertyHierarchy = new MyListString(property.myStrings),
                    firstPlaceholder = 0,
                    configuration = configuration
                };
                instantiationInformationForProperty[propertyIndex] = information;
                monoBehaviourActuatorsForProperty[propertyIndex] = MapperManager.InstantiateActuators(information);
                Actuators[configuration].AddRange(monoBehaviourActuatorsForProperty[propertyIndex].GetActuatorsList());
            }
        }
        ready = true;
    }
    void Update()
    {
        foreach (MyListString property in propertiesIndex.Keys)
        {
            int propertyIndex = propertiesIndex[property];
            IActuators actuators = monoBehaviourActuatorsForProperty[propertyIndex];
            InformationRefresh(propertyIndex);
            monoBehaviourActuatorsForProperty[propertyIndex] = MapperManager.ManageActuators(instantiationInformationForProperty[propertyIndex], actuators);
            Actuators[(NewActuatorConfiguration) instantiationInformationForProperty[propertyIndex].configuration] = monoBehaviourActuatorsForProperty[propertyIndex].GetActuatorsList();
        }
    }

    private void InformationRefresh(int propertyIndex)
    {
        instantiationInformationForProperty[propertyIndex].currentObjectOfTheHierarchy = gameObject;
        instantiationInformationForProperty[propertyIndex].residualPropertyHierarchy = new MyListString(instantiationInformationForProperty[propertyIndex].propertyHierarchy.myStrings);
        instantiationInformationForProperty[propertyIndex].firstPlaceholder = 0;
    }
    internal bool ExistsConfigurationOtherThan(string name, NewActuatorConfiguration newActuatorConfiguration)
    {
        foreach (NewActuatorConfiguration configuration in GetComponents<NewActuatorConfiguration>())
        {
            if (configuration != newActuatorConfiguration && configuration.ConfigurationName.Equals(name))
            {
                return true;
            }
        }
        return false;
    }

    internal IEnumerable<string> GetAllConfigurationNames()
    {
        List<string> toReturn = new List<string>();
        NewActuatorConfiguration[] configurations = GetComponents<NewActuatorConfiguration>();
        if (configurations == null || configurations.Length==0)
        {
            Destroy(this);
            return toReturn;
        }
        foreach (NewActuatorConfiguration configuration in configurations)
        {
            toReturn.Add(configuration.ConfigurationName);
        }
        return toReturn;
    }

    internal NewActuatorConfiguration GetConfiguration(string confName)
    {
        NewActuatorConfiguration[] configurations = GetComponents<NewActuatorConfiguration>();
        if (configurations == null || configurations.Length==0)
        {
            Destroy(this);
            return null;
        }
        foreach (NewActuatorConfiguration configuration in configurations)
        {
            if (configuration.ConfigurationName.Equals(confName))
            {
                return configuration;
            }
        }
        return null;
    }
}