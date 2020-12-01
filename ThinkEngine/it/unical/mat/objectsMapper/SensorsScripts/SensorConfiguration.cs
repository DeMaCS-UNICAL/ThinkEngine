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
    internal List<ListOfMyListStringIntPair> operationPerProperty;
    [SerializeField]
    internal List<ListOfMyListStringStringPair> specificValuePerProperty;
    #region Unity Messages
    void OnEnable()
    {
        base.Reset();
        if (operationPerProperty is null)
        {
            operationPerProperty = new List<ListOfMyListStringIntPair>();
        }
        if (specificValuePerProperty is null)
        {
            specificValuePerProperty = new List<ListOfMyListStringStringPair>();
        }
    }
    new void Reset()
    {
        OnEnable();
    }
    #endregion
    protected override void ConfigurationSaved(GameObjectsTracker tracker)
    {
        if(GetComponent<MonoBehaviourSensorsManager>() == null)
        {
            gameObject.AddComponent<MonoBehaviourSensorsManager>().hideFlags=HideFlags.HideInInspector;
        }
        GetComponent<MonoBehaviourSensorsManager>().AddConfiguration(this);
    }
    internal override void DeleteConfiguration()
    {
        if (saved)
        {
            MonoBehaviourSensorsManager monoBehaviourSensorsManager = GetComponent<MonoBehaviourSensorsManager>();
            if (monoBehaviourSensorsManager != null)
            {
                monoBehaviourSensorsManager.DeleteConfiguration(this);
            }
        }
    }
    internal override void CleanSpecificDataStructure()
    {
        operationPerProperty = new List<ListOfMyListStringIntPair>();
        specificValuePerProperty = new List<ListOfMyListStringStringPair>();
    }

    internal override void SpecificConfiguration(FieldOrProperty fieldOrProperty, MyListString property, GameObjectsTracker tracker)
    {
        ListOfMyListStringIntPair pair = new ListOfMyListStringIntPair();
        pair.Key = property;
        pair.Value = tracker.operationPerProperty[fieldOrProperty];
        operationPerProperty.Add(pair);
        if (tracker.specificValuePerProperty.ContainsKey(fieldOrProperty))
        {
            ListOfMyListStringStringPair pair2 = new ListOfMyListStringStringPair();
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

