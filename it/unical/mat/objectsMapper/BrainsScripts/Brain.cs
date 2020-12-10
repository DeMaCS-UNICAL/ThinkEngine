using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Collections;
using EmbASP4Unity.it.unical.mat.objectsMapper.BrainsScripts;

[ExecuteAlways]
public class Brain :MonoBehaviour
{
    #region Serialized Fieds
    public bool enableBrain;
    public bool debug=true;
    public bool maintainFactFile;
    [SerializeField, HideInInspector]
    private List<string> _chosenSensorConfigurations;
    [SerializeField, HideInInspector]
    private List<string> _chosenActuatorConfigurations;
    [SerializeField,HideInInspector]
    internal string ASPFilePath;
    [SerializeField,HideInInspector]
    private string _ASPFileTemplatePath;
    [SerializeField, HideInInspector]
    private string _executeReasonerOn;
    [SerializeField, HideInInspector]
    internal bool prefabBrain;
    [SerializeField, HideInInspector]
    internal bool specificASPFile;
    [SerializeField, HideInInspector]
    internal bool globalASPFile;
    #endregion
    private object triggerClass;
    internal List<string> chosenSensorConfigurations
    {
        get
        {
            if (_chosenSensorConfigurations == null)
            {
                _chosenSensorConfigurations = new List<string>();
            }
            return _chosenSensorConfigurations;
        }
    }
    internal List<string> chosenActuatorConfigurations
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
    internal string ASPFileTemplatePath
    {
        get
        {
            if (_ASPFileTemplatePath == null)
            {
                _ASPFileTemplatePath = @".\Assets\Resources\" + gameObject.name + "Template.asp";
                if (!File.Exists(_ASPFileTemplatePath))
                {
                    if (!Directory.Exists(@".\Assets\Resources"))
                    {
                        Directory.CreateDirectory(@".\Assets\Resources");
                    }
                    File.Create(_ASPFileTemplatePath);
                }
            }
            return _ASPFileTemplatePath;
        }
    }
    internal string executeReasonerOn
    {
        get
        {
            if (_executeReasonerOn == null)
            {
                _executeReasonerOn = "";
            }
            return _executeReasonerOn;
        }
        set
        {
            _executeReasonerOn = value;
        }
    }

    #region Runtime Fields
    internal readonly object toLock = new object();
    internal string sensorsMapping;
    internal string objectsIndexes;
    internal bool sensorsConfigurationsChanged;
    internal bool actuatorsConfigurationsChanged;
    private string originalName;
    internal MethodInfo reasonerMethod;
    internal bool missingData;
    private Thread executionThread;
    internal SolverExectuor embasp;
    internal bool solverWaiting;
    #endregion

    #region Unity Messages
    void Reset()
    {
        if (GetComponent<IndexTracker>() == null)
        {
            gameObject.AddComponent<IndexTracker>();
        }
        triggerClass = Utility.triggerClass;
    }
    void OnEnable()
    {
        originalName = gameObject.name;
        Reset();
    }
    void Start()
    {
        Utility.loadPrefabs();
        if (Application.isPlaying && enableBrain)
        {
            StartCoroutine(InitBrain2());
        }
    }
    void Update()
    {
        if (reasonerMethod == null)
        {
            lock (toLock)
            {
                if (!SomeConfigurationAvailable())
                {
                    missingData = true;
                    return;
                }
                if (solverWaiting)
                {
                    solverWaiting = false;
                    Monitor.Pulse(toLock);
                }
            }
        }
    }
    void OnApplicationQuit()
    {
        if (embasp != null)
        {
            embasp.reason = false;
            lock (toLock)
            {
                Monitor.Pulse(toLock);
            }
        }
    }
    #endregion
    internal void GenerateFile()
    {
        using (FileStream fs = File.Create(ASPFileTemplatePath))
        {
            Byte[] info = new UTF8Encoding(true).GetBytes("%For runtime instantiated GameObject, only the prefab mapping is provided. Use that one substituting the gameobject name accordingly\n");
            fs.Write(info, 0, info.Length);

            foreach (ActuatorConfiguration actuatorConf in Utility.actuatorsManager.GetCorrespondingConfigurations(chosenActuatorConfigurations))
            {
                info = new UTF8Encoding(true).GetBytes(actuatorConf.GetAspTemplate());
                fs.Write(info, 0, info.Length);
            }
            foreach (SensorConfiguration sensorConf in Utility.sensorsManager.GetConfigurations(chosenSensorConfigurations))
            {
                info = new UTF8Encoding(true).GetBytes(sensorConf.GetAspTemplate());
                fs.Write(info, 0, info.Length);
            }
        }
    }
    IEnumerator InitBrain2()
    {
        if(specificASPFile && originalName.Equals(gameObject.name))
        {
            yield return new WaitUntil( () => !originalName.Equals(gameObject.name));
        }
        embasp = new SolverExectuor(this);
        PrepareSensors();
        PrepareActuators();
        if (!SomeConfigurationAvailable())
        {
            missingData = true;
        }
        if (!executeReasonerOn.Equals("When Sensors are ready"))
        {
            reasonerMethod = Utility.getTriggerMethod(executeReasonerOn);
            if (reasonerMethod != null)
            {
                StartCoroutine(PulseOn());
            }
        }
        executionThread = new Thread(() =>
        {
            Thread.CurrentThread.Name = "Solver executor";
            Thread.CurrentThread.IsBackground = true;
            embasp.Run();
        });
        executionThread.Start();
    }
    internal void RemoveNullSensorConfigurations()
    {
        chosenSensorConfigurations.RemoveAll(x => !Utility.sensorsManager.ExistsConfigurationWithName(x));
    }
    internal void RemoveNullActuatorConfigurations()
    {
        chosenActuatorConfigurations.RemoveAll(x => !Utility.actuatorsManager.ExistsConfigurationWithName(x,this));
    }
    private void PrepareActuators()
    {
        Utility.actuatorsManager.RegisterBrainActuatorConfigurations(this, chosenActuatorConfigurations);
    }
    private void PrepareSensors()
    {
        Utility.sensorsManager.RegisterBrainsSensorConfigurations(this, chosenSensorConfigurations);
    }
    private IEnumerator PulseOn()
    {
        while (true)
        {
            yield return new WaitUntil(() => solverWaiting && SomeConfigurationAvailable() && (bool)reasonerMethod.Invoke(triggerClass, null));
            lock (toLock)
            {
                solverWaiting = false;
                Monitor.Pulse(toLock);
            }
        }
    }
    private bool SomeConfigurationAvailable()
    {
        return Utility.sensorsManager.IsSomeActiveInScene(chosenSensorConfigurations) && Utility.actuatorsManager.IsSomeActiveInScene(chosenActuatorConfigurations);
    }
}

