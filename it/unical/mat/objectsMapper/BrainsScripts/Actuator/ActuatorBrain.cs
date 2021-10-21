using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Collections;
using ThinkEngine.it.unical.mat.objectsMapper.BrainsScripts;
using Planner;

[ExecuteAlways, RequireComponent(typeof(IndexTracker))]
public class ActuatorBrain :Brain
{
    #region Serialized Fieds
    
    [SerializeField, HideInInspector]
    private List<string> _chosenActuatorConfigurations;
    
    #endregion
    internal List<string> ChosenActuatorConfigurations
    {
        get
        {
            if (_chosenActuatorConfigurations == null)
            {
                _chosenActuatorConfigurations = new List<string>();
            }
            return _chosenActuatorConfigurations;
        }
    }

    #region Runtime Fields
    internal string objectsIndexes;
    internal bool actuatorsConfigurationsChanged;
    #endregion

    #region Unity Messages

    
    void OnApplicationQuit()
    {
        if (executor != null)
        {
            executor.reason = false;
            lock (toLock)
            {
                Monitor.Pulse(toLock);
            }
        }
    }
    #endregion
    internal override void GenerateFile()
    {
        base.GenerateFile();
        using (StreamWriter fs = File.AppendText(ASPFileTemplatePath))
        {
            fs.Write("%Actuators.\n");
            HashSet<string> seenActuatorConfNames = new HashSet<string>();
            foreach (ActuatorConfiguration actuatorConf in Utility.ActuatorsManager.GetCorrespondingConfigurations(ChosenActuatorConfigurations))
            {
                if (seenActuatorConfNames.Contains(actuatorConf.ConfigurationName))
                {
                    continue;
                }
                seenActuatorConfNames.Add(actuatorConf.ConfigurationName);
                foreach (MyListString property in actuatorConf.ToMapProperties)
                {
                    fs.Write(MapperManager.GetASPTemplate(actuatorConf.ConfigurationName, actuatorConf.gameObject, property));
                }
            }
        }
    }
    protected override IEnumerator Init()
    {
        base.Init();
        executor = new ActuatorExecutor(this);
        PrepareActuators();
        string GOname = gameObject.name;
        executionThread = new Thread(() =>
        {
            Thread.CurrentThread.Name = "Solver executor "+ GOname;
            Thread.CurrentThread.IsBackground = true;
            executor.Run();
        });
        executionThread.Start();
        return null;
    }
    internal void RemoveNullActuatorConfigurations()
    {
        ChosenActuatorConfigurations.RemoveAll(x => !Utility.ActuatorsManager.ExistsConfigurationWithName(x,this));
    }
    private void PrepareActuators()
    {
        Utility.ActuatorsManager.RegisterBrainActuatorConfigurations(this, ChosenActuatorConfigurations);
    }
    
    protected override bool SomeConfigurationAvailable()
    {
        return base.SomeConfigurationAvailable() && Utility.ActuatorsManager.IsSomeActiveInScene(ChosenActuatorConfigurations);
    }

}

