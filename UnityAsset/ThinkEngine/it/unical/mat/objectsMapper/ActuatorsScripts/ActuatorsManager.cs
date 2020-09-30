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
    public class ActuatorsManager : IManager
    {
        private List<string> configuredGameObject;
        private List<string> configurationsNames;
        public Dictionary<Brain,List<SimpleActuator>> instantiatedActuators;
        public static ActuatorsManager instance;
        public bool applyCoroutinStarted=false;
        
        
        public List<string> getUsedNames()
        {
            return configurationsNames;
        }
        public static ActuatorsManager GetInstance()
        {
            if (instance == null)
            {
                instance = new ActuatorsManager();
                instance.configuredGameObject = new List<string>();
                instance.configurationsNames = new List<string>();
                //MyDebugger.MyDebug("instance after " + instance);
                //MyDebugger.MyDebug("confs: " + instance.sensConfs.Count);
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

        

        public void delete(string v)
        {
            int elementPosition = configurationsNames.IndexOf(v);
            if (elementPosition != -1)
            {
                configurationsNames.RemoveAt(elementPosition);
                configuredGameObject.RemoveAt(elementPosition);
            }
        }


        public void addConfiguration(AbstractConfiguration abstractConfiguration)
        {
            if (!configurationsNames.Contains(abstractConfiguration.configurationName))
            {
                configurationsNames.Add(abstractConfiguration.configurationName);
                configuredGameObject.Add(abstractConfiguration.gameObject.name);
            }
        }

        public bool existsConfigurationWithName(string name)
        {
            return configurationsNames.Contains(name);
        }
    }
}
