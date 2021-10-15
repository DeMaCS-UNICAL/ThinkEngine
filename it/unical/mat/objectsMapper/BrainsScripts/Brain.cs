using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Collections;
using ThinkEngine.it.unical.mat.objectsMapper.BrainsScripts;
using Structures;

[ExecuteAlways, RequireComponent(typeof(IndexTracker))]
public class Brain :MonoBehaviour
{
    #region Serialized Fieds
    public bool enableBrain=true;
    public bool debug=true;
    public bool maintainFactFile;
    [SerializeField, HideInInspector]
    private List<string> _chosenSensorConfigurations;
    [SerializeField, HideInInspector]
    private List<string> _chosenActuatorConfigurations;
    [SerializeField,HideInInspector]
    internal string ASPFilesPath;
    [SerializeField, HideInInspector]
    internal string ASPFilesPrefix;
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
    internal List<string> ChosenSensorConfigurations
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
    internal string ASPFileTemplatePath
    {
        get
        {
            if (_ASPFileTemplatePath == null || !_ASPFileTemplatePath.Equals("Template" + gameObject.name + ".asp"))
            {
                _ASPFileTemplatePath = @".\Assets\Resources\" + "Template"+gameObject.name + ".asp";
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
    internal string ExecuteReasonerOn
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

    void OnEnable()
    {
        originalName = gameObject.name;
    }
    void Start()
    {
        //Debug.Log("DLL success");
        Utility.LoadPrefabs();
        if (Application.isPlaying && enableBrain)
        {
            triggerClass = Utility.TriggerClass;
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
            HashSet<string> seenActuatorConfNames = new HashSet<string>();
            HashSet<string> seenSensorConfNames = new HashSet<string>();
            foreach (ActuatorConfiguration actuatorConf in Utility.ActuatorsManager.GetCorrespondingConfigurations(ChosenActuatorConfigurations))
            {
                if (seenActuatorConfNames.Contains(actuatorConf.ConfigurationName))
                {
                    continue;
                }
                seenActuatorConfNames.Add(actuatorConf.ConfigurationName);
                foreach (MyListString property in actuatorConf.ToMapProperties)
                {
                    info = new UTF8Encoding(true).GetBytes(MapperManager.GetASPTemplate(actuatorConf.ConfigurationName, actuatorConf.gameObject, property));
                    fs.Write(info, 0, info.Length);
                }
            }
            foreach (SensorConfiguration sensorConf in Utility.SensorsManager.GetConfigurations(ChosenSensorConfigurations))
            {
                if (seenSensorConfNames.Contains(sensorConf.ConfigurationName))
                {
                    continue;
                }
                seenSensorConfNames.Add(sensorConf.ConfigurationName);
                foreach (MyListString property in sensorConf.ToMapProperties)
                {
                    info = new UTF8Encoding(true).GetBytes(MapperManager.GetASPTemplate(sensorConf.ConfigurationName, sensorConf.gameObject, property, true));
                    fs.Write(info, 0, info.Length);
                }
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
        if (!ExecuteReasonerOn.Equals("When Sensors are ready"))
        {
            reasonerMethod = Utility.GetTriggerMethod(ExecuteReasonerOn);
            if (reasonerMethod != null)
            {
                StartCoroutine(PulseOn());
            }
        }
        string GOname = gameObject.name;
        executionThread = new Thread(() =>
        {
            Thread.CurrentThread.Name = "Solver executor "+ GOname;
            Thread.CurrentThread.IsBackground = true;
            embasp.Run();
        });
        executionThread.Start();
    }
    internal void RemoveNullSensorConfigurations()
    {
        ChosenSensorConfigurations.RemoveAll(x => !Utility.SensorsManager.ExistsConfigurationWithName(x));
    }
    internal void RemoveNullActuatorConfigurations()
    {
        ChosenActuatorConfigurations.RemoveAll(x => !Utility.ActuatorsManager.ExistsConfigurationWithName(x,this));
    }
    private void PrepareActuators()
    {
        Utility.ActuatorsManager.RegisterBrainActuatorConfigurations(this, ChosenActuatorConfigurations);
    }
    private void PrepareSensors()
    {
        Utility.SensorsManager.RegisterBrainsSensorConfigurations(this, ChosenSensorConfigurations);
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
        return Utility.SensorsManager.IsSomeActiveInScene(ChosenSensorConfigurations) && Utility.ActuatorsManager.IsSomeActiveInScene(ChosenActuatorConfigurations);
    }
}

