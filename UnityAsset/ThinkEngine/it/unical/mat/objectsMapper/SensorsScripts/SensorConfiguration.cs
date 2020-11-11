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
        Reset();
    }
    new void Reset()
    {
        base.Reset();
        if (operationPerProperty is null)
        {
            MyDebugger.MyDebug("Warning! Specific Configurations are not serialized");
            operationPerProperty = new List<ListOfStringIntPair>();
        }
        if (specificValuePerProperty is null)
        {
            specificValuePerProperty = new List<ListOfStringStringPair>();
        }
    }
    internal override void ConfigurationSaved(GameObjectsTracker tracker)
    {
        if(GetComponent<MonoBehaviourSensorsManager>() == null)
        {
            gameObject.AddComponent<MonoBehaviourSensorsManager>();
        }
        GetComponent<MonoBehaviourSensorsManager>().addConfiguration(this);
    }
    
    internal override void DeleteConfiguration()
    {
        if (saved)
        {
            GetComponent<MonoBehaviourSensorsManager>().deleteConfiguration(this);
        }
    }
    internal override void CleanSpecificDataStructure()
    {
        operationPerProperty = new List<ListOfStringIntPair>();
        specificValuePerProperty = new List<ListOfStringStringPair>();
    }

    internal override void SpecificConfiguration(FieldOrProperty fieldOrProperty, MyListString property, GameObjectsTracker tracker)
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

    internal override void ASPRepresentation()
    {
        base.ASPRepresentation();
        foreach(MyListString property in aspTemplate.Keys)
        {
            for (int j = 0; j < aspTemplate[property].Count; j++)
            {
                aspTemplate[property][j] = aspTemplate[property][j] + ".";
            }
        }
    }
    internal override string GetAspTemplate()
    {
        string original = base.GetAspTemplate();
        string toReturn = "";
        foreach (string line in original.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
        {
            toReturn += "%"+line + Environment.NewLine;
        }
        return toReturn;
    }
    
}

