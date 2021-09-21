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
        Dictionary<int, NewSensorConfiguration> sensorConfigurationForProperty;
        Dictionary<int, Sensors> monoBehaviourSensorForProperty;
        void Start()
        {
            propertiesIndex = new Dictionary<MyListString, int>();
            sensorConfigurationForProperty = new Dictionary<int, NewSensorConfiguration>();
            monoBehaviourSensorForProperty = new Dictionary<int, Sensors>();
            foreach(NewSensorConfiguration configuration in GetComponents<NewSensorConfiguration>())
            {
                foreach(MyListString property in configuration.savedProperties)
                {
                    int propertyIndex = property.GetHashCode();
                    propertiesIndex[property]=propertyIndex;
                    sensorConfigurationForProperty[propertyIndex]= configuration;
                    monoBehaviourSensorForProperty[propertyIndex] = MapperManager.InstantiateSensors(gameObject,property, configuration);
                }
            }
        }
        void Update()
        {
            foreach(MyListString property in propertiesIndex.Keys)
            {
                int propertyIndex = propertiesIndex[property];
                Sensors sensors = monoBehaviourSensorForProperty[propertyIndex];
                monoBehaviourSensorForProperty[propertyIndex] = MapperManager.ManageSensors(gameObject, property, sensorConfigurationForProperty[propertyIndex], sensors);

            }
        }


        internal bool ExistsConfigurationOtherThan(string name, NewSensorConfiguration newSensorConfiguration)
        {
            foreach (NewSensorConfiguration configuration in GetComponents<NewSensorConfiguration>())
            {
                if (configuration != newSensorConfiguration && configuration.configurationName.Equals(name))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
