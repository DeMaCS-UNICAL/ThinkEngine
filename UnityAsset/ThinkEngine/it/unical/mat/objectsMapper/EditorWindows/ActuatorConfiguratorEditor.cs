using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Threading;
using EmbASP4Unity.it.unical.mat.objectsMapper.SensorsScripts;
using EmbASP4Unity.it.unical.mat.objectsMapper.EditorWindows;
using EmbASP4Unity.it.unical.mat.objectsMapper;
using EmbASP4Unity.it.unical.mat.objectsMapper.ActuatorsScripts;
using System.IO;

[CustomEditor(typeof(ActuatorConfigurator))]
public class ActuatorConfiguratorEditor : AbstractConfiguratorEditor
{



    


    void OnEnable()
    {
        base.OnEnable();
        typeOfConfigurator = "Actuator";
        
    }

   
    private void OnLostFocus() { }

    


    protected override void updateConfiguredObject()
    {
        updateConfiguredObject(ActuatorConfiguration.CreateInstance<ActuatorConfiguration>());
    }


    /*void OnDisable()
    {
        if (!Directory.Exists("Assets/Resources/Actuators"))
        {
            Directory.CreateDirectory("Assets/Resources/Actuators");
        }
        
        if (AssetDatabase.LoadAssetAtPath("Assets/Resources/ActuatorsManager.asset", typeof(ActuatorsManager)) == null)
        {
            AssetDatabase.CreateAsset((ActuatorsManager)manager, "Assets/Resources/ActuatorsManager.asset");
        }
        else
        {
            EditorUtility.SetDirty((ActuatorsManager)manager);
            AssetDatabase.SaveAssets();
        }
        foreach (AbstractConfiguration conf in ((ActuatorsManager)manager).confs())
        {
            ActuatorConfiguration actuatorConf = (ActuatorConfiguration)conf;
            if (AssetDatabase.LoadAssetAtPath("Assets/Resources/Actuators/" + actuatorConf.configurationName + ".asset", typeof(ActuatorConfiguration)) == null)
            {
                AssetDatabase.CreateAsset(actuatorConf, "Assets/Resources/Actuators/" + actuatorConf.configurationName + ".asset");
            }
            else
            {
                EditorUtility.SetDirty(actuatorConf);
                AssetDatabase.SaveAssets();
            }
        }
    }*/

    

    internal override void addCustomFields(FieldOrProperty obj)
    {
        if (tracker.ObjectsToggled[obj])
        {
            
        }
    }
    protected override bool isMappable(FieldOrProperty obj)
    {
        return tracker.IsBaseType(obj);
    }
}
