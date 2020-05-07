using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace EmbASP4Unity.it.unical.mat.objectsMapper.SensorsScripts
{
    [Serializable]
    public class SensorConfiguration : AbstractConfiguration
    {
        [SerializeField]
        internal List<StringIntPair> operationPerProperty;
        [SerializeField]
        internal List<StringStringPair> specificValuePerProperty;

        public SensorConfiguration(string s)
        {
            this.name = s;
            operationPerProperty = new List<StringIntPair>();
            specificValuePerProperty = new List<StringStringPair>();
        }

        internal SensorConfiguration()
        {
            operationPerProperty = new List<StringIntPair>();
            specificValuePerProperty = new List<StringStringPair>();
        }

        internal override void cleanSpecificDataStructure()
        {
            operationPerProperty = new List<StringIntPair>();
            specificValuePerProperty = new List<StringStringPair>();
        }

        internal override void specificConfiguration(FieldOrProperty fieldOrProperty, string s)
        {
            StringIntPair pair = new StringIntPair();
            pair.Key = s;
            pair.Value = tracker.operationPerProperty[fieldOrProperty];
            operationPerProperty.Add(pair);
            if (tracker.specificValuePerProperty.ContainsKey(fieldOrProperty))
            {
                StringStringPair pair2 = new StringStringPair();
                pair2.Key = s;
                pair2.Value = tracker.specificValuePerProperty[fieldOrProperty];
                specificValuePerProperty.Add(pair2);
            }
        }
    }
}
