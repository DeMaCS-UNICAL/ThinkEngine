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

[Serializable]
public class ActuatorConfigurationWindow : AbstractConfigurationWindow
{



    [MenuItem("Window/Actuator Configuration Window")]
    public static void Init()
    {
        //Debug.Log("going to show");
        EditorWindow.GetWindow(typeof(ActuatorConfigurationWindow));
        //Debug.Log("showed");
    }


    void OnEnable()
    {
        if (!Directory.Exists("Assets/Resources")){
            Directory.CreateDirectory("Assets/Resources");
        }
        if (AssetDatabase.LoadAssetAtPath("Assets/Resources/ActuatorsManager.asset", typeof(ActuatorsManager)) == null)
        {
            manager = ActuatorsManager.GetInstance();
        }
        else
        {
            manager = (ActuatorsManager)AssetDatabase.LoadAssetAtPath("Assets/Resources/ActuatorsManager.asset", typeof(ActuatorsManager));

        }
        tracker = new GameObjectsTracker();
    }

    private void OnFocus()
    {
        refreshAvailableGO();

    }

    private void OnLostFocus() { }

    void OnGUI()
    {
        draw("Actuator");
    }



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

    protected override string onSaving()
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
        /* ConfigurationPopup popup = new ConfigurationPopup();
         Vector2 pos = new Vector2((this.position.xMax + this.position.xMin) / 2, (this.position.yMax + this.position.yMin) / 2);
         Vector2 dim = new Vector2(100, 100);
         Rect popupRect = new Rect(pos,dim);
         popup.window = this;
         PopupWindow.Show(popupRect,popup);
         popup.wait();
         Debug.Log("Configuration name " + configurationName);
         return configurationName;*/
        return "";
    }

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
