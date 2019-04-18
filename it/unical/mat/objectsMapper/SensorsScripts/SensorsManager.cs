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
               // Debug.Log("after serialization SensorConfiguration" + conf.operationPerProperty.Count);
                sensConfs.Add(conf);
               /* Debug.Log("after serialization AbstractConfiguration" + ((SensorConfiguration)sensConfs[sensConfs.Count-1]).operationPerProperty.Count);
               / foreach(StringIntPair pair in conf.operationPerProperty)
                {
                    Debug.Log(pair.Key + " " + pair.Value);
                }*/
            }
        }

        public void OnBeforeSerialize()
        {
            confsToSerialize = new List<SensorConfiguration>();
            foreach (AbstractConfiguration conf in sensConfs)
            {
                //Debug.Log("before serialization " + ((SensorConfiguration)conf).operationPerProperty.Count);
                confsToSerialize.Add((SensorConfiguration)conf);
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
