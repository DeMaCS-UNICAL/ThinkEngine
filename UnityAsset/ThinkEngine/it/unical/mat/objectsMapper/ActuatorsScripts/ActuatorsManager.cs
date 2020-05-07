using EmbASP4Unity.it.unical.mat.objectsMapper.BrainsScripts;
using EmbASP4Unity.it.unical.mat.objectsMapper.SensorsScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace EmbASP4Unity.it.unical.mat.objectsMapper.ActuatorsScripts
{
    [Serializable]
    public class ActuatorsManager : ScriptableObject,IManager
    {
        [SerializeField]
        private List<AbstractConfiguration> actuatorsConfs;
        [SerializeField]
        private List<ActuatorConfiguration> confsToSerialize;
        [SerializeField]
        private List<string> ConfiguredGameObject;
        [SerializeField]
        private List<string> ConfigurationsNames;
        [NonSerialized]
        public Dictionary<Brain,List<SimpleActuator>> instantiatedActuators;
        public static ActuatorsManager instance;
        [NonSerialized]
        public bool applyCoroutinStarted=false;

        public AbstractConfiguration findConfiguration(string s)
        {
            foreach (ActuatorConfiguration c in actuatorsConfs)
            {
                if (c.name.Equals(s))
                {
                    return c;
                }
            }
            return new ActuatorConfiguration(s);
        }

        public ref List<AbstractConfiguration> confs()
        {
            return ref actuatorsConfs;
        }
        public ref List<string> usedNames()
        {
            return ref ConfigurationsNames;
        }
        public static ActuatorsManager GetInstance()
        {
            if (instance == null)
            {
                instance = new ActuatorsManager();
            }
            return instance;
        }
        public void registerActuators(Brain b, List<SimpleActuator> instantiated)
        {
            if (instantiatedActuators == null)
            {
                instantiatedActuators = new Dictionary<Brain, List<SimpleActuator>>();
            }
            if (!instantiatedActuators.ContainsKey(b))
            {
                instantiatedActuators.Add(b, instantiated);
            }
            else
            {
                instantiatedActuators[b]= instantiated;
            }
        }

        public IEnumerator applyActuators()
        {
            while (true)
            {
                foreach (Brain brain in instantiatedActuators.Keys)
                {
                    if (brain.areActuatorsReady() && brain.actuatorsUpdateCondition())
                    {
                        foreach (SimpleActuator act in instantiatedActuators[brain])
                        {
                            Performance.updatingActuators = true;
                            act.UpdateProperties();
                        }
                        
                    }
                    brain.setActuatorsReady(false);
                }
                yield return null;
            }
            
        }

        public ref List<string> configuredGameObject()
        {
            return ref ConfiguredGameObject;
        }

        void OnEnable()
        {

            if (actuatorsConfs == null)
            {
                actuatorsConfs = new List<AbstractConfiguration>();
            }
            if (ConfiguredGameObject == null)
            {
                ConfiguredGameObject = new List<string>();
            }
            if(ConfigurationsNames == null)
            {
                ConfigurationsNames = new List<string>();
            }
            
            
        }

        public void OnBeforeSerialize()
        {
            confsToSerialize = new List<ActuatorConfiguration>();
            foreach (AbstractConfiguration conf in actuatorsConfs)
            {
                //Debug.Log("before serialization " + ((ActuatorConfiguration)conf));
                ActuatorConfiguration actuatorConf = (ActuatorConfiguration)conf;
                confsToSerialize.Add(actuatorConf);
                
            }
        }

        public void OnAfterDeserialize()
        {
            instance = this;
            actuatorsConfs = new List<AbstractConfiguration>();
            foreach (ActuatorConfiguration conf in confsToSerialize)
            {

                actuatorsConfs.Add(conf);

            }
        }

        
    }
}
