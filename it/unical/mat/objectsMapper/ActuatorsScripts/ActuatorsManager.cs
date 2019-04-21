using System;
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

        public ref List<AbstractConfiguration> confs()
        {
            return ref actuatorsConfs;
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
        }

        public void OnBeforeSerialize()
        {
            confsToSerialize = new List<ActuatorConfiguration>();
            foreach (AbstractConfiguration conf in actuatorsConfs)
            {
                //Debug.Log("before serialization " + ((SensorConfiguration)conf).operationPerProperty.Count);
                confsToSerialize.Add((ActuatorConfiguration)conf);
            }
        }

        public void OnAfterDeserialize()
        {
            actuatorsConfs = new List<AbstractConfiguration>();
            foreach (ActuatorConfiguration conf in confsToSerialize)
            {

                actuatorsConfs.Add(conf);

            }
        }
    }
}
