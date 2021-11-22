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
using ThinkEngine.it.unical.mat.objectsMapper.BrainsScripts.Actuator;
using BrainsScripts.Specialitations.ASP;

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

    protected override HashSet<string> SupportedFileExtensions 
    {
        get
        {
            return new HashSet<string> { "asp" };
        }
    }

    IActualActuatorBrain actuatorBrain;
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
    protected override string SpecificFileParts()
    {
        string toReturn = "%Actuators:\n";
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
                string actuatorMappingAsASP = MapperManager.GetASPTemplate(actuatorConf.ConfigurationName, actuatorConf.gameObject, property);
                toReturn += ActualActuatorEncoding(actuatorMappingAsASP);
            }
        }
        return toReturn;
    }

    internal string ActualActuatorEncoding(string actuatorMappingAsASP)
    {
        return actuatorBrain.ActualActuatorEncoding(actuatorMappingAsASP);
    }
    protected override void Start()
    {
        if (FileExtension.Equals("asp"))
        {
            actuatorBrain = new ASPActuatorBrain();
        }
        base.Start();
    }
    protected override IEnumerator Init()
    {
        if (actuatorBrain != null)
        {
            yield return StartCoroutine(base.Init());
            executor = new ASPActuatorExecutor(this);
            PrepareActuators();
            string GOname = gameObject.name;
            executionThread = new Thread(() =>
            {
                Thread.CurrentThread.Name = "Solver executor " + GOname;
                Thread.CurrentThread.IsBackground = true;
                executor.Run();
            });
            executionThread.Start();
        }
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

    internal override string ActualSensorEncoding(string sensorsAsASP)
    {
        return actuatorBrain.ActualSensorEncoding(sensorsAsASP);
    }
}

