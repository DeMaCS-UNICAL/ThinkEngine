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
 
    public class SensorConfiguration : AbstractConfiguration
    {
        internal List<ListOfStringIntPair> operationPerProperty;
        internal List<ListOfStringStringPair> specificValuePerProperty;

        void Awake()
        {
            base.Awake();
            manager = SensorsManager.GetInstance();
            if (operationPerProperty is null)
            {
                operationPerProperty = new List<ListOfStringIntPair>();
            }
            if (specificValuePerProperty is null)
            {
                specificValuePerProperty = new List<ListOfStringStringPair>();
            }
        }

        internal override void cleanSpecificDataStructure()
        {
            operationPerProperty = new List<ListOfStringIntPair>();
            specificValuePerProperty = new List<ListOfStringStringPair>();
        }

        internal override void specificConfiguration(FieldOrProperty fieldOrProperty, List<string> property)
        {
            ListOfStringIntPair pair = new ListOfStringIntPair();
            pair.Key = property;
            pair.Value = tracker.operationPerProperty[fieldOrProperty];
            operationPerProperty.Add(pair);
            if (tracker.specificValuePerProperty.ContainsKey(fieldOrProperty))
            {
                ListOfStringStringPair pair2 = new ListOfStringStringPair();
                pair2.Key = property;
                pair2.Value = tracker.specificValuePerProperty[fieldOrProperty];
                specificValuePerProperty.Add(pair2);
            }
        }
    }
}
