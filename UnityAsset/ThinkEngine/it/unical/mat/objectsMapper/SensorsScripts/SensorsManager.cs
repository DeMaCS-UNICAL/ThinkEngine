using EmbASP4Unity.it.unical.mat.objectsMapper.BrainsScripts;
using System;
using System.Collections.Generic;
using System.IO;
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
        private List<string> configuredGameObject;
        [SerializeField]
        private List<string> configurationsNames;
        [NonSerialized]
        public Dictionary<Brain, List<IMonoBehaviourSensor>> instantiatedSensors;
        public static SensorsManager instance;

        public AbstractConfiguration findConfiguration(string s){
            foreach(SensorConfiguration c in sensConfs)
            {
                if (c.configurationName.Equals(s))
                {
                    return c;
                }
            }
            return null;
        }

        public List<string> getConfiguredGameObject()
        {
            return configuredGameObject;
        }
        public List<string> getUsedNames()
        {
            return configurationsNames;
        }
        public List<AbstractConfiguration> getConfigurations()
        {
            return sensConfs;
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
           // Debug.Log("instance " + instance);
            if (instance == null)
            {
                if (!Directory.Exists("Assets/Resources"))
                {
                    Directory.CreateDirectory("Assets/Resources");
                }
                if (AssetDatabase.LoadAssetAtPath("Assets/Resources/SensorsManager.asset", typeof(SensorsManager)) == null)
                {
                    instance = new SensorsManager();
                }
                else
                {
                    instance = (SensorsManager)AssetDatabase.LoadAssetAtPath("Assets/Resources/SensorsManager.asset", typeof(SensorsManager));
                }
            }
            //Debug.Log("instance after " + instance);
            //Debug.Log("confs: " + instance.sensConfs.Count);
            return instance;
        }

        public void registerSensors(Brain b, List<IMonoBehaviourSensor> instantiated)
        {
            if (instantiatedSensors == null)
            {
                instantiatedSensors = new Dictionary<Brain, List<IMonoBehaviourSensor>>();
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
        /*public void updateSensors(Brain brain)
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
                }

            }

        }*/
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
            if (configuredGameObject == null)
            {
                configuredGameObject = new List<string>();
            }
            if (configurationsNames == null)
            {
                configurationsNames = new List<string>();
            }
        }

        public void delete(string v)
        {
            int i = 0;
            for (; i < sensConfs.Count; i++)
            {
                if (sensConfs[i].configurationName.Equals(v))
                {
                    break;
                }
            }
            if (i < sensConfs.Count)
            {
                deleteGO(sensConfs[i]);
                sensConfs.RemoveAt(i);
            }
            configurationsNames.Remove(v);
        }

        private void deleteGO(AbstractConfiguration abstractConfiguration)
        {
            foreach(SensorConfiguration c in sensConfs)
            {
                if (!c.configurationName.Equals(abstractConfiguration.configurationName))
                {
                    if (c.gOName.Equals(abstractConfiguration.gOName))
                    {
                        return;
                    }
                }
            }
            configuredGameObject.Remove(abstractConfiguration.gOName);
        }

        public AbstractConfiguration newConfiguration(string n,string go)
        {
            return new SensorConfiguration(n,go);
        }

        public void addConfiguration(AbstractConfiguration abstractConfiguration)
        {
            //Debug.Log("checking if to delete " + abstractConfiguration.name);
            delete(abstractConfiguration.configurationName);
            sensConfs.Add(abstractConfiguration);
            if (!configurationsNames.Contains(abstractConfiguration.configurationName))
            {
                configurationsNames.Add(abstractConfiguration.configurationName);
            }
            if (!configuredGameObject.Contains(abstractConfiguration.gOName))
            {
                configuredGameObject.Add(abstractConfiguration.gOName);
            }
        }

        internal void addSensor(Brain brain, IMonoBehaviourSensor sensor)
        {
            instantiatedSensors[brain].Add(sensor);
        }
        internal void removeSensor(Brain brain, IMonoBehaviourSensor sensor)
        {
            instantiatedSensors[brain].Remove(sensor);
        }
    }
}
