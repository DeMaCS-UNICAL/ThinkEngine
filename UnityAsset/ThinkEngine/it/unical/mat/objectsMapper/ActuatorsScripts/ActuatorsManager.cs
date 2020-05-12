using EmbASP4Unity.it.unical.mat.objectsMapper.BrainsScripts;
using EmbASP4Unity.it.unical.mat.objectsMapper.SensorsScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        private List<string> configuredGameObject;
        [SerializeField]
        private List<string> configurationsNames;
        [NonSerialized]
        public Dictionary<Brain,List<SimpleActuator>> instantiatedActuators;
        public static ActuatorsManager instance;
        [NonSerialized]
        public bool applyCoroutinStarted=false;

        public AbstractConfiguration findConfiguration(string s)
        {
            foreach (ActuatorConfiguration c in actuatorsConfs)
            {
                if (c.configurationName.Equals(s))
                {
                    return c;
                }
            }
            return null;
        }

        public List<AbstractConfiguration> getConfigurations()
        {
            return actuatorsConfs;
        }
        public List<string> getUsedNames()
        {
            return configurationsNames;
        }
        public static ActuatorsManager GetInstance()
        {
            if (instance == null)
            {
                if (!Directory.Exists("Assets/Resources"))
                {
                    Directory.CreateDirectory("Assets/Resources");
                }
                if (AssetDatabase.LoadAssetAtPath("Assets/Resources/ActuatorsManager.asset", typeof(ActuatorsManager)) == null)
                {
                    instance = new ActuatorsManager();
                }
                else
                {
                    instance = (ActuatorsManager)AssetDatabase.LoadAssetAtPath("Assets/Resources/ActuatorsManager.asset", typeof(ActuatorsManager));

                }
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

        public List<string> getConfiguredGameObject()
        {
            return configuredGameObject;
        }

        void OnEnable()
        {

            if (actuatorsConfs == null)
            {
                actuatorsConfs = new List<AbstractConfiguration>();
            }
            if (configuredGameObject == null)
            {
                configuredGameObject = new List<string>();
            }
            if(configurationsNames == null)
            {
                configurationsNames = new List<string>();
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

        public void delete(string v)
        {
            int i = 0;
            for (; i < actuatorsConfs.Count; i++)
            {
                if (actuatorsConfs[i].configurationName.Equals(v))
                {
                    break;
                }
            }
            if (i < actuatorsConfs.Count) {
                deleteGO(actuatorsConfs[i]);
                actuatorsConfs.RemoveAt(i);
            }
            configurationsNames.Remove(v);
        }

        public AbstractConfiguration newConfiguration(string n, string go)
        {
            return new ActuatorConfiguration(n,go);
        }

        private void deleteGO(AbstractConfiguration abstractConfiguration)
        {
            foreach (ActuatorConfiguration c in actuatorsConfs)
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

        public void addConfiguration(AbstractConfiguration abstractConfiguration)
        {
            delete(abstractConfiguration.configurationName);
            actuatorsConfs.Add(abstractConfiguration);
            if (!configurationsNames.Contains(abstractConfiguration.configurationName))
            {
                configurationsNames.Add(abstractConfiguration.configurationName);
            }
            if (!configuredGameObject.Contains(abstractConfiguration.gOName))
            {
                configuredGameObject.Add(abstractConfiguration.gOName);
            }
        }
    }
}
