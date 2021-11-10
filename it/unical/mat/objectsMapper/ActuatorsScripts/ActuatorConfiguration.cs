using System;
using UnityEngine;
using System.Reflection;

[ExecuteInEditMode, Serializable]
public class ActuatorConfiguration : AbstractConfiguration
{
    private object triggerClass;
    [SerializeField, HideInInspector]
    private string _methodToApply;
    internal string MethodToApply
    {
        get
        {
            if (_methodToApply == null)
            {
                _methodToApply = "";
            }
            return _methodToApply;
        }
        set
        {
            _methodToApply = value;
        }
    }
    internal MethodInfo applyMethod;

    #region Unity Messages
    void OnEnable()
    {
        base.Reset();
        triggerClass = Utility.triggerClass;
        applyMethod = Utility.getTriggerMethod(MethodToApply);
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
            toReturn += line + ":-objectIndex("+ ASPMapperHelper.aspFormat(configurationName) + ",X)"+Environment.NewLine;
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
