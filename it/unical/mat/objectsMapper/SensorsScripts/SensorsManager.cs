using EmbASP4Unity.it.unical.mat.objectsMapper.BrainsScripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace EmbASP4Unity.it.unical.mat.objectsMapper.SensorsScripts
{
    [Serializable]
    public class SensorsManager : ScriptableObject, IManager
    {
        
        private List<AbstractConfiguration> sensConfs;
        [SerializeField]
        private List<SensorConfiguration> confsToSerialize;
        [SerializeField]
        private List<string> ConfiguredGameObject;
        [SerializeField]
        private List<string> ConfigurationsNames;
        [NonSerialized]
        public Dictionary<Brain, List<AdvancedSensor>> instantiatedSensors;
        public static SensorsManager instance;

        public ref List<string> configuredGameObject()
        {
            return ref ConfiguredGameObject;
        }
        public ref List<string> usedNames()
        {
            return ref ConfigurationsNames;
        }
        public ref List<AbstractConfiguration> confs()
        {
            return ref sensConfs;
        }

        public void OnAfterDeserialize()
        {
            instance = this;
            sensConfs = new List<AbstractConfiguration>();
            foreach (SensorConfiguration conf in confsToSerialize)
            {
               
                sensConfs.Add(conf);
               
            }
        }

        internal static SensorsManager GetInstance()
        {
            if (instance == null)
            {
                instance = new SensorsManager();
            }
            return instance;
        }

        public void registerSensors(Brain b, List<AdvancedSensor> instantiated)
        {
            if (instantiatedSensors == null)
            {
                instantiatedSensors = new Dictionary<Brain, List<AdvancedSensor>>();
            }
            if (!instantiatedSensors.ContainsKey(b))
            {
                instantiatedSensors.Add(b, instantiated);
            }
            else
            {
                instantiatedSensors[b] = instantiated;
            }
        }
        public void updateSensors(Brain brain)
        {
            Performance.updatingSensors = true;
            foreach (AdvancedSensor sens in instantiatedSensors[brain])
            {
                sens.UpdateProperties();
                /*if (sens.matrixProperties.Count() > 0)
                {
                    Debug.Log(sens.matrixProperties.Count() + " " + sens.matrixProperties.First().Key + " " + sens.matrixProperties.First().Value);
                }
                if (sens.listProperties.Count > 0)
                {
                    Debug.Log(sens.listProperties.Count() + " " + sens.listProperties.First().Key + " " + sens.listProperties.First().Value);
                }*/

            }

        }
        public void OnBeforeSerialize()
        {
            confsToSerialize = new List<SensorConfiguration>();
            foreach (AbstractConfiguration conf in sensConfs)
            {
                //Debug.Log("before serialization " + ((SensorConfiguration)conf).operationPerProperty.Count);
                SensorConfiguration sensorConf = (SensorConfiguration)conf;
                confsToSerialize.Add(sensorConf);
            }
        }


        void OnEnable()
        {
            
            if (sensConfs == null)
            {
                sensConfs = new List<AbstractConfiguration>();
            }
            if (ConfiguredGameObject == null)
            {
                ConfiguredGameObject = new List<string>();
            }
            if (ConfigurationsNames == null)
            {
                ConfigurationsNames = new List<string>();
            }
        }
    }
}
