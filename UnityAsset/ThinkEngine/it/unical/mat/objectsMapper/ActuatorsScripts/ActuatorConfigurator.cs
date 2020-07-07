using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using System.IO;
using EmbASP4Unity.it.unical.mat.objectsMapper.ActuatorsScripts;
using EmbASP4Unity.it.unical.mat.objectsMapper;

[ExecuteInEditMode]
    public class ActuatorConfigurator:AbstractConfigurator
    {
        void Awake()
        {
            manager = ActuatorsManager.GetInstance();
            base.Awake();
        }
        void Update()
        {
            if (manager is null)
            {
                manager = ActuatorsManager.GetInstance();
            }
        }
        internal override string onSaving()
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
                foreach (AbstractConfiguration conf in ((ActuatorsManager)manager).getConfigurations())
                {
                    ActuatorConfiguration actuatorConf = (ActuatorConfiguration)conf;
                    if (AssetDatabase.LoadAssetAtPath("Assets/Resources/Actuators/" + actuatorConf.configurationName + ".asset", typeof(ActuatorConfiguration)) == null)
                    {
                        AssetDatabase.CreateAsset(actuatorConf, "Assets/Resources/Actuators/" + actuatorConf.configurationName + ".asset");
                    }
                    else
                    {
                        EditorUtility.SetDirty(actuatorConf);
                    }
                }
                AssetDatabase.SaveAssets();
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
        internal override void onDeleting(AbstractConfiguration c)
        {
            EditorUtility.SetDirty((ActuatorsManager)manager);
            AssetDatabase.SaveAssets();
            ActuatorConfiguration actuatorConf = (ActuatorConfiguration)c;
            if (!(AssetDatabase.LoadAssetAtPath("Assets/Resources/Actuators/" + actuatorConf.configurationName + ".asset", typeof(ActuatorConfiguration)) is null))
            {
                AssetDatabase.DeleteAsset("Assets/Resources/Actuators/" + actuatorConf.configurationName + ".asset");
            }
        }
    }

