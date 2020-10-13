using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Threading;
using EmbASP4Unity.it.unical.mat.objectsMapper.EditorWindows;
using EmbASP4Unity.it.unical.mat.objectsMapper;
using System.IO;

[CustomEditor(typeof(ActuatorConfiguration))]
public class ActuatorConfiguratorEditor : AbstractConfigurationEditor
{



    


    void OnEnable()
    {
        base.OnEnable();
        typeOfConfigurator = "Actuator";
        
    }
    

    

    protected override bool isMappable(FieldOrProperty obj)
    {
        return tracker.IsBaseType(obj);
    }
}
