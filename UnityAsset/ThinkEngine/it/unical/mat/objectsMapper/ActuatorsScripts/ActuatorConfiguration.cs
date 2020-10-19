using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;

[ExecuteInEditMode]
public class ActuatorConfiguration : AbstractConfiguration
{
    public Brain assignedTo;
    private object triggerClass;
    internal MethodInfo applyMethod;

    new void OnEnable()
    {
        triggerClass = Utility.triggerClass;
        manager = FindObjectOfType<ActuatorsManager>();
        if (manager is null)
        {
            manager = gameObject.AddComponent<ActuatorsManager>();
            ((ActuatorsManager)manager).hideFlags = HideFlags.HideInInspector;
        }
        base.OnEnable();
    }
    internal override void ASPRep()
    {
        base.ASPRep();
        foreach(MyListString property in aspTemplate.Keys)
        {
            for (int j = 0; j < aspTemplate[property].Count; j++)
            {
                aspTemplate[property][j] = "setOnActuator(" + aspTemplate[property][j] + ")";
            }
        }
    }

    internal override string getAspTemplate()
    {
        string original = base.getAspTemplate();
        string toReturn = "";
        foreach(string line in original.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
        {
            toReturn += "setOnActuator(" + line + "):-"+Environment.NewLine;
        }
        return toReturn;
    }

    internal bool checkIfApply()
    {
        if(applyMethod is null)
        {
            return true;
        }
        return (bool) applyMethod.Invoke(triggerClass, null);
    }
}
