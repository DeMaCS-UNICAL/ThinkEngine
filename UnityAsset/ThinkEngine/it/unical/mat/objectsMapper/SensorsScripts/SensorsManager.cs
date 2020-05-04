using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace EmbASP4Unity.it.unical.mat.objectsMapper.SensorsScripts
{
    [Serializable]
    public class SensorsManager : ScriptableObject,IManager
    {
        
        private List<AbstractConfiguration> sensConfs;
        [SerializeField]
        private List<SensorConfiguration> confsToSerialize;
        [SerializeField]
        private List<string> ConfiguredGameObject;

        public ref List<string> configuredGameObject()
        {
            return ref ConfiguredGameObject;
        }
        public ref List<AbstractConfiguration> confs()
        {
            return ref sensConfs;
        }

        public void OnAfterDeserialize()
        {
            sensConfs = new List<AbstractConfiguration>();
            foreach (SensorConfiguration conf in confsToSerialize)
            {
               
                sensConfs.Add(conf);
               
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
        }
    }
}
