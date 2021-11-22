using System;
using System.Reflection;
using UnityEngine;

[ExecuteInEditMode, Serializable, RequireComponent(typeof(MonoBehaviourActuatorsManager))]
class ActuatorConfiguration : AbstractConfiguration
{
    private object _triggerClass;
    private object TriggerClass
    {
        get
        {
            if (_triggerClass == null)
            {
                _triggerClass = Utility.TriggerClass;
            }
            return _triggerClass;
        }
    }
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

    void OnEnable()
    {
        applyMethod = Utility.GetTriggerMethod(MethodToApply);
    }
    protected override void PropertyDeleted(MyListString property)
    {
            
    }

    protected override void PropertySelected(MyListString property)
    {
            
    }
    internal override string ConfigurationName
    {
        set
        {
            if (!Utility.ActuatorsManager.IsConfigurationNameValid(value, this))
            {
                throw new Exception("The chosen configuration name cannot be used.");
            }
            _configurationName = value;
        }
    }
    internal override string GetAutoConfigurationName()
    {
        string name;
        string toAppend = "";
        int count = 0;
        do
        {
            name = char.ToLower(gameObject.name[0]).ToString() + gameObject.name.Substring(1) + "Actuator" + toAppend;
            toAppend += count;
            count++;
        }
        while (!Utility.ActuatorsManager.IsConfigurationNameValid(name, this));
        return name;
    }

    internal override bool IsAValidName(string temporaryName)
    {
        return temporaryName.Equals(ConfigurationName) || Utility.ActuatorsManager.IsConfigurationNameValid(temporaryName, this);
    }

    internal override bool IsSensor()
    {
        return false;
    }

    internal bool CheckIfApply()
    {
        if (applyMethod is null)
        {
            return true;
        }
        return (bool)applyMethod.Invoke(TriggerClass, null);
    }
}

