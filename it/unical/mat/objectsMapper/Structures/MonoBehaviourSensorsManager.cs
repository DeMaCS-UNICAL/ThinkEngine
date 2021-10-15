using Structures.Mappers;
using System.Collections.Generic;
using UnityEngine;

namespace Structures
{
    class MonoBehaviourSensorsManager:MonoBehaviour
    {
        Dictionary<MyListString,int> propertiesIndex;
        Dictionary<int, InstantiationInformation> instantiationInformationForProperty;
        Dictionary<int, ISensors> monoBehaviourSensorsForProperty;
        internal bool ready;
        internal Dictionary<SensorConfiguration,List<MonoBehaviourSensor>> _configurations;
        internal Dictionary<SensorConfiguration,List<MonoBehaviourSensor>> Sensors
        {
            get
            {
                if (_configurations == null)
                {
                    _configurations = new Dictionary<SensorConfiguration, List<MonoBehaviourSensor>>();
                }
                return _configurations;
            }
        }

        void Start()
        {
            propertiesIndex = new Dictionary<MyListString, int>();
            instantiationInformationForProperty = new Dictionary<int, InstantiationInformation>();
            monoBehaviourSensorsForProperty = new Dictionary<int, ISensors>();
            foreach(SensorConfiguration configuration in GetComponents<SensorConfiguration>())
            {
                Sensors[configuration] = new List<MonoBehaviourSensor>();
                foreach(MyListString property in configuration.ToMapProperties)
                {
                    int propertyIndex = property.GetHashCode();
                    propertiesIndex[property]=propertyIndex;
                    InstantiationInformation information = new InstantiationInformation()
                    {
                        instantiateOn = gameObject,
                        currentObjectOfTheHierarchy = gameObject,
                        propertyHierarchy = new MyListString(property.myStrings),
                        residualPropertyHierarchy = new MyListString(property.myStrings),
                        firstPlaceholder=0,
                        configuration = configuration
                    };
                    instantiationInformationForProperty[propertyIndex]= information;
                    monoBehaviourSensorsForProperty[propertyIndex] = MapperManager.InstantiateSensors(information);
                    Sensors[configuration].AddRange(monoBehaviourSensorsForProperty[propertyIndex].GetSensorsList());
                }
            }
            ready = true;
        }
        void Update()
        {
            foreach(SensorConfiguration configuration in Sensors.Keys)
            {
                Sensors[configuration].Clear();
            }
            foreach(MyListString property in propertiesIndex.Keys)
            {
                int propertyIndex = propertiesIndex[property];
                ISensors sensors = monoBehaviourSensorsForProperty[propertyIndex];
                InformationRefresh(propertyIndex);
                monoBehaviourSensorsForProperty[propertyIndex] = MapperManager.ManageSensors(instantiationInformationForProperty[propertyIndex], sensors);
                Sensors[(SensorConfiguration) instantiationInformationForProperty[propertyIndex].configuration].AddRange(monoBehaviourSensorsForProperty[propertyIndex].GetSensorsList());
            }
        }

        private void InformationRefresh(int propertyIndex)
        {
            instantiationInformationForProperty[propertyIndex].currentObjectOfTheHierarchy = gameObject;
            instantiationInformationForProperty[propertyIndex].residualPropertyHierarchy = new MyListString(instantiationInformationForProperty[propertyIndex].propertyHierarchy.myStrings);
            instantiationInformationForProperty[propertyIndex].firstPlaceholder = 0;
        }

        internal bool ExistsConfigurationOtherThan(string name, SensorConfiguration newSensorConfiguration)
        {
            foreach (SensorConfiguration configuration in GetComponents<SensorConfiguration>())
            {
                if (configuration != newSensorConfiguration && configuration.ConfigurationName.Equals(name))
                {
                    return true;
                }
            }
            return false;
        }

        internal SensorConfiguration GetConfiguration(string confName)
        {
            SensorConfiguration[] configurations = GetComponents<SensorConfiguration>();
            if (configurations == null || configurations.Length == 0)
            {
                Destroy(this);
                return null;
            }
            foreach (SensorConfiguration configuration in configurations)
            {
                if (configuration.ConfigurationName.Equals(confName))
                {
                    return configuration;
                }
            }
            return null;
        }

        internal IEnumerable<string> GetAllConfigurationNames()
        {
            List<string> toReturn = new List<string>();
            SensorConfiguration[] configurations = GetComponents<SensorConfiguration>();
            if(configurations==null || configurations.Length == 0)
            {
                Destroy(this);
                return toReturn;
            }
            foreach (SensorConfiguration configuration in configurations)
            {
                toReturn.Add(configuration.ConfigurationName);
            }
            return toReturn;
        }
    }
}
