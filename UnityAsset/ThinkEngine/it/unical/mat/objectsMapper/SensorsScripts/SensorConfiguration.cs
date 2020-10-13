using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEditor;
using System.Collections;


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

    internal override void specificConfiguration(FieldOrProperty fieldOrProperty, MyListString property)
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

    internal override void ASPRep()
    {
        base.ASPRep();
        for (int i = 0; i < aspTemplate.Count; i++)
        {
            for (int j = 0; j < aspTemplate[i].Count; j++)
            {
                aspTemplate[i][j] = aspTemplate[i][j] + ".";
            }
        }
    }
}

