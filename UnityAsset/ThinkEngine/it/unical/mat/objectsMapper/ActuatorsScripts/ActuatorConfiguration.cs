using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEditor;
using System.Collections;

public class ActuatorConfiguration : AbstractConfiguration
{

    void Awake()
    {
        base.Awake();
        manager = ActuatorsManager.GetInstance();
    }

    internal override void ASPRep()
    {
        base.ASPRep();
        for(int i=0; i < aspTemplate.Count; i++)
        {
            aspTemplate[i] = "setOnActuator(" + aspTemplate[i] + ")";
        }
    }
}
