﻿using EmbASP4Unity.it.unical.mat.objectsMapper.BrainsScripts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
        private static Dictionary<Brain, List<IMonoBehaviourSensor>> instantiatedSensors;
        private static Dictionary<Brain, int> sensorsUpdated;

        public static SensorsManager instance;
        private static Dictionary<Brain, object> to_lock;

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

        internal static string GetSensorsMapping(Brain brain)
        {
            lock (getLock(brain))
            {
                string mapping = "";
                foreach(IMonoBehaviourSensor sensor in instantiatedSensors[brain])
                {
                    mapping += sensor.Map();
                }
                return mapping;
            }
        }

        public void registerSensors(Brain brain, List<IMonoBehaviourSensor> instantiated)
        {
            
            lock (getLock(brain))
            {
                if (instantiatedSensors == null)
                {
                    instantiatedSensors = new Dictionary<Brain, List<IMonoBehaviourSensor>>();
                }
                if (!instantiatedSensors.ContainsKey(brain))
                {
                    instantiatedSensors.Add(brain, instantiated);
                }
                else
                {
                    instantiatedSensors[brain] = instantiated;
                }
            }
        }

        public static IEnumerable<IMonoBehaviourSensor> GetSensors(Brain brain)
        {
            lock (getLock(brain))
            {
                if (sensorsUpdatedCount(brain) != instantiatedSensorsCount(brain))
                {
                    //MyDebug("I'm waiting since " + sensorManager.sensorsUpdatedCount(this) + "<>" + sensorManager.instantiatedSensorsCount(this));
                    MyDebug(Thread.CurrentThread.Name+" is going to wait", brain.debug);
                    Monitor.Wait(SensorsManager.getLock(brain));
                    //MyDebug("I'm going to execute");

                }
                return getInstantiatedSensors(brain);
            }
        }

        private static object getLock(Brain brain)
        {
            if (to_lock is null)
            {
                to_lock = new Dictionary<Brain, object>();
            }
            if (!to_lock.ContainsKey(brain))
            {
                to_lock.Add(brain, new object());
            }
            return to_lock[brain];
        }

        public static int sensorsUpdatedCount(Brain brain)
        {
            if(sensorsUpdated is null || !sensorsUpdated.ContainsKey(brain))
            {
                return 0;
            }
            return sensorsUpdated[brain];
        }
        public static int instantiatedSensorsCount(Brain brain)
        {
            if (instantiatedSensors is null || !instantiatedSensors.ContainsKey(brain))
            {
                return 0;
            }
            return instantiatedSensors[brain].Count;
        }

        public static void AddUpdatedSensor(Brain brain)
        {
            lock (getLock(brain))
            {
                if (sensorsUpdated is null)
                {
                    sensorsUpdated = new Dictionary<Brain, int>();
                }
                if (!sensorsUpdated.ContainsKey(brain))
                {
                    sensorsUpdated.Add(brain, 0);
                }
                if (sensorsUpdated[brain] == instantiatedSensors[brain].Count)
                {
                    sensorsUpdated[brain] = 0;
                }
                sensorsUpdated[brain]++;
                if (sensorsUpdated[brain] == instantiatedSensors[brain].Count)
                {
                    Monitor.Pulse(getLock(brain));
                }
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

        internal static IEnumerable<IMonoBehaviourSensor> getInstantiatedSensors(Brain brain)
        {
            if(!(instantiatedSensors is null) && instantiatedSensors.ContainsKey(brain))
            {
                return instantiatedSensors[brain];
            }
            return new List<IMonoBehaviourSensor>();
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
            lock (getLock(brain))
            {
                if (!instantiatedSensors.ContainsKey(brain))
                {
                    instantiatedSensors.Add(brain, new List<IMonoBehaviourSensor>());
                }
                instantiatedSensors[brain].Add(sensor);
            }
        }
        internal void removeSensor(Brain brain, IMonoBehaviourSensor sensor)
        {
            lock (getLock(brain))
            {
                instantiatedSensors[brain].Remove(sensor);
            }
        }
        private static void MyDebug(string v, bool debug)
        {
            if (debug)
            {
                Debug.Log(v);
            }
        }
    }

}
