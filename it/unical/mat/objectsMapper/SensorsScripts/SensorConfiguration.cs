using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode,Serializable]
public class SensorConfiguration : AbstractConfiguration
{
    [SerializeField]
    internal List<MyListStringIntPair> operationPerProperty;
    [SerializeField]
    internal List<MyListStringStringPair> specificValuePerProperty;
    #region Unity Messages
    void OnEnable()
    {
        base.Reset();
        if (operationPerProperty is null)
        {
            operationPerProperty = new List<MyListStringIntPair>();
        }
        if (specificValuePerProperty is null)
        {
            specificValuePerProperty = new List<MyListStringStringPair>();
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
            gameObject.AddComponent<MonoBehaviourSensorsManager>();//.hideFlags=HideFlags.HideInInspector;
        }
        GetComponent<MonoBehaviourSensorsManager>().AddConfiguration();
    }
    internal override void DeleteConfiguration()
    {
        if (saved)
        {
            MonoBehaviourSensorsManager monoBehaviourSensorsManager = GetComponent<MonoBehaviourSensorsManager>();
            if (monoBehaviourSensorsManager != null)
            {
                monoBehaviourSensorsManager.DeleteConfiguration();
            }
        }
    }
    internal override void CleanSpecificDataStructure()
    {
        operationPerProperty = new List<MyListStringIntPair>();
        specificValuePerProperty = new List<MyListStringStringPair>();
    }

    internal override void SpecificConfiguration(FieldOrProperty fieldOrProperty, MyListString property, GameObjectsTracker tracker)
    {
        MyListStringIntPair pair = new MyListStringIntPair();
        pair.Key = property;
        pair.Value = tracker.operationPerProperty[fieldOrProperty];
        operationPerProperty.Add(pair);
        if (tracker.specificValuePerProperty.ContainsKey(fieldOrProperty))
        {
            MyListStringStringPair pair2 = new MyListStringStringPair();
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

