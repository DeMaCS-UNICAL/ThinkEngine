using NewStructures.NewMappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NewStructures
{
    class NewMonoBehaviourSensorsManager:MonoBehaviour
    {
        Dictionary<MyListString,int> propertiesIndex;
        Dictionary<int, InstantiationInformation> instantiationInformationForProperty;
        Dictionary<int, ISensors> monoBehaviourSensorsForProperty;
        internal bool ready;
        internal Dictionary<NewSensorConfiguration,List<NewMonoBehaviourSensor>> _configurations;
        internal Dictionary<NewSensorConfiguration,List<NewMonoBehaviourSensor>> Sensors
        {
            get
            {
                if (_configurations == null)
                {
                    _configurations = new Dictionary<NewSensorConfiguration, List<NewMonoBehaviourSensor>>();
                }
                return _configurations;
            }
        }

        void Start()
        {
            propertiesIndex = new Dictionary<MyListString, int>();
            instantiationInformationForProperty = new Dictionary<int, InstantiationInformation>();
            monoBehaviourSensorsForProperty = new Dictionary<int, ISensors>();
            foreach(NewSensorConfiguration configuration in GetComponents<NewSensorConfiguration>())
            {
                Sensors[configuration] = new List<NewMonoBehaviourSensor>();
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
            foreach(MyListString property in propertiesIndex.Keys)
            {
                int propertyIndex = propertiesIndex[property];
                ISensors sensors = monoBehaviourSensorsForProperty[propertyIndex];
                InformationRefresh(propertyIndex);
                monoBehaviourSensorsForProperty[propertyIndex] = MapperManager.ManageSensors(instantiationInformationForProperty[propertyIndex], sensors);
                Sensors[(NewSensorConfiguration) instantiationInformationForProperty[propertyIndex].configuration] = monoBehaviourSensorsForProperty[propertyIndex].GetSensorsList();
            }
        }

        private void InformationRefresh(int propertyIndex)
        {
            instantiationInformationForProperty[propertyIndex].currentObjectOfTheHierarchy = gameObject;
            instantiationInformationForProperty[propertyIndex].residualPropertyHierarchy = new MyListString(instantiationInformationForProperty[propertyIndex].propertyHierarchy.myStrings);
            instantiationInformationForProperty[propertyIndex].firstPlaceholder = 0;
        }

        internal bool ExistsConfigurationOtherThan(string name, NewSensorConfiguration newSensorConfiguration)
        {
            foreach (NewSensorConfiguration configuration in GetComponents<NewSensorConfiguration>())
            {
                if (configuration != newSensorConfiguration && configuration.ConfigurationName.Equals(name))
                {
                    return true;
                }
            }
            return false;
        }

        internal NewSensorConfiguration GetConfiguration(string confName)
        {
            NewSensorConfiguration[] configurations = GetComponents<NewSensorConfiguration>();
            if (configurations == null || configurations.Length == 0)
            {
                Destroy(this);
                return null;
            }
            foreach (NewSensorConfiguration configuration in configurations)
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
            NewSensorConfiguration[] configurations = GetComponents<NewSensorConfiguration>();
            if(configurations==null || configurations.Length == 0)
            {
                Destroy(this);
                return toReturn;
            }
            foreach (NewSensorConfiguration configuration in configurations)
            {
                toReturn.Add(configuration.ConfigurationName);
            }
            return toReturn;
        }
    }
}
