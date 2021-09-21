using System;
using UnityEngine;
using System.Reflection;

[ExecuteInEditMode, Serializable]
public class ActuatorConfiguration : AbstractConfiguration
{
    public Brain assignedTo;
    private object triggerClass;
    internal MethodInfo applyMethod;

    #region Unity Messages
    void OnEnable()
    {
        base.Reset();
        triggerClass = Utility.triggerClass;
    }
    new void Reset()
    {
        OnEnable();
    }
    #endregion
    protected override void ConfigurationSaved(GameObjectsTracker tracker)
    {
        if (GetComponent<MonoBehaviourActuatorsManager>() == null)
        {
            gameObject.AddComponent<MonoBehaviourActuatorsManager>();//.hideFlags = HideFlags.HideInInspector;
        }
        GetComponent<MonoBehaviourActuatorsManager>().AddConfiguration(this);
    }
    internal override void DeleteConfiguration()
    {
        if (saved)
        {
            MonoBehaviourActuatorsManager monoBehaviourActuatorsManager = GetComponent<MonoBehaviourActuatorsManager>();
            if (monoBehaviourActuatorsManager != null)
            {
                monoBehaviourActuatorsManager.DeleteConfiguration(this);
            }
        }
    }
    internal override void ASPRepresentation()
    {
        base.ASPRepresentation();
        foreach(MyListString property in aspTemplate.Keys)
        {
            for (int j = 0; j < aspTemplate[property].Count; j++)
            {
                aspTemplate[property][j] = "setOnActuator(" + aspTemplate[property][j] + ")";
            }
        }
    }
    internal override string GetAspTemplate()
    {
        string original = base.GetAspTemplate();
        string toReturn = "";
        foreach(string line in original.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
        {
            toReturn += line + ":-objectIndex("+ ASPMapperHelper.AspFormat(configurationName) + ",X)"+Environment.NewLine;
        }
        return toReturn;
    }
    internal bool CheckIfApply()
    {
        if(applyMethod is null)
        {
            return true;
        }
        return (bool) applyMethod.Invoke(triggerClass, null);
    }
}
