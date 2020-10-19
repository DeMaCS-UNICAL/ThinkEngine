using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEditor;
using System.Collections;

[ExecuteInEditMode,Serializable]
public class SensorConfiguration : AbstractConfiguration
{
    [SerializeField]
    internal List<ListOfStringIntPair> operationPerProperty;
    [SerializeField]
    internal List<ListOfStringStringPair> specificValuePerProperty;

    new void OnEnable()
    {
        manager = FindObjectOfType<SensorsManager>();
        if (manager is null)
        {
            manager = gameObject.AddComponent<SensorsManager>();
            ((SensorsManager)manager).hideFlags = HideFlags.HideInInspector;
        }
        base.OnEnable();
        if (operationPerProperty is null)
        {
            operationPerProperty = new List<ListOfStringIntPair>();
        }
        if (specificValuePerProperty is null)
        {
            specificValuePerProperty = new List<ListOfStringStringPair>();
        }
    }
    void Reset()
    {
        OnEnable();
    }
    internal override void cleanSpecificDataStructure()
    {
        operationPerProperty = new List<ListOfStringIntPair>();
        specificValuePerProperty = new List<ListOfStringStringPair>();
    }

    internal override void specificConfiguration(FieldOrProperty fieldOrProperty, MyListString property, GameObjectsTracker tracker)
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
        foreach(MyListString property in aspTemplate.Keys)
        {
            for (int j = 0; j < aspTemplate[property].Count; j++)
            {
                aspTemplate[property][j] = aspTemplate[property][j] + ".";
            }
        }
    }
    internal override string getAspTemplate()
    {
        string original = base.getAspTemplate();
        string toReturn = "";
        foreach (string line in original.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
        {
            toReturn += "%"+line + "." + Environment.NewLine;
        }
        return toReturn;
    }
    
}

