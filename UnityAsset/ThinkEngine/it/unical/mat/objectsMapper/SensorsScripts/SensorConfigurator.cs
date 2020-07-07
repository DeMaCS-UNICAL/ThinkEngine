using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using System.IO;
using EmbASP4Unity.it.unical.mat.objectsMapper;
using EmbASP4Unity.it.unical.mat.objectsMapper.SensorsScripts;

[ExecuteInEditMode]
    public class SensorConfigurator:AbstractConfigurator
    {

        void Awake()
        {
            manager = SensorsManager.GetInstance();
            base.Awake();
        }
        void Update()
        {
            if (manager is null)
            {
                manager = SensorsManager.GetInstance();
            }
        }
        internal override string onSaving()
        {
            if (!Directory.Exists("Assets/Resources/Sensors"))
            {
                Directory.CreateDirectory("Assets/Resources/Sensors");
            }
            if (AssetDatabase.LoadAssetAtPath("Assets/Resources/SensorsManager.asset", typeof(SensorsManager)) == null)
            {
                AssetDatabase.CreateAsset((SensorsManager)manager, "Assets/Resources/SensorsManager.asset");
            }
            else
            {
                EditorUtility.SetDirty((SensorsManager)manager);
                foreach (AbstractConfiguration conf in ((SensorsManager)manager).getConfigurations())
                {
                    SensorConfiguration sensorConf = (SensorConfiguration)conf;
                    if (AssetDatabase.LoadAssetAtPath("Assets/Resources/Sensors/" + sensorConf.configurationName + ".asset", typeof(SensorConfiguration)) == null)
                    {
                        AssetDatabase.CreateAsset(sensorConf, "Assets/Resources/Sensors/" + sensorConf.configurationName + ".asset");
                       
                    }
                    else
                    {
                        EditorUtility.SetDirty(sensorConf);
                    }
                }
                AssetDatabase.SaveAssets();
            }
            
            return "";
        }
        internal override void onDeleting(AbstractConfiguration c)
        {
            EditorUtility.SetDirty((SensorsManager)manager);
            AssetDatabase.SaveAssets();
            SensorConfiguration sensorConf = (SensorConfiguration)c;
            if (!(AssetDatabase.LoadAssetAtPath("Assets/Resources/Sensors/" + sensorConf.configurationName + ".asset", typeof(SensorConfiguration)) is null))
            {
                AssetDatabase.DeleteAsset("Assets/Resources/Sensors/" + sensorConf.configurationName + ".asset");
            }
        }
    }

