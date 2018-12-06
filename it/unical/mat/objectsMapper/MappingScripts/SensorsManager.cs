using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace EmbASP4Unity.it.unical.mat.objectsMapper.MappingScripts
{
    [Serializable]
    class SensorsManager : ScriptableObject
    {
        [SerializeField]
        public List<SensorConfiguration> sensConfs;
        [SerializeField]
        public List<string> configuredGameObject;


        void OnEnable()
        {
            
            if (sensConfs == null)
            {
                sensConfs = new List<SensorConfiguration>();
            }
            if (configuredGameObject == null)
            {
                configuredGameObject = new List<string>();
            }
        }
    }
}
