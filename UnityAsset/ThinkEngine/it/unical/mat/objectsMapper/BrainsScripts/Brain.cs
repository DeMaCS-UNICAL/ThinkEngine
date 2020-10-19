using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;
using EmbASP4Unity.it.unical.mat.objectsMapper.Mappers;
using System.Threading;
using System.Timers;
using System.Reflection;
using System.Collections;
using EmbASP4Unity.it.unical.mat.objectsMapper.BrainsScripts;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.ComponentModel;
using UnityEditor;

[ExecuteInEditMode]

public class Brain :MonoBehaviour
{
    public readonly object toLock = new object();
    public bool enableBrain;
    public bool debug=true;
    public bool maintainFactFile;
    [SerializeField]
    internal List<SensorConfiguration> sensorsConfigurations;
    [SerializeField]
    internal List<ActuatorConfiguration> actuatorsConfigurations;
    private Thread executionThread;
    private SolverExectuor embasp;
    internal bool solverWaiting;
    internal string ASPFilePath;
    internal string ASPFileTemplatePath;
    private MethodInfo reasonerMethod;
    private object triggerClass;
    public string executeReasonerOn;
    internal string sensorsMapping;
    private Stopwatch watch;
    internal long factsMSTotal;
    internal int factsStep;
    internal long asTotalMS;
    internal int asSteps;

    void Reset()
    {
        triggerClass = Utility.triggerClass;
        if(sensorsConfigurations is null)
        {
            sensorsConfigurations = new List<SensorConfiguration>();
            actuatorsConfigurations = new List<ActuatorConfiguration>();
        }
        if (ASPFilePath is null)
        {
            ASPFilePath = @".\Assets\Resources\" + gameObject.name + ".asp";
            ASPFileTemplatePath = @".\Assets\Resources\" + gameObject.name + "Template.asp";
        }
        if(executeReasonerOn is null)
        {
            executeReasonerOn = "";
        }
    }
    void OnEnable()
    {
        Reset();
    }
    void Awake()
    {
        OnEnable();
    }
    void Start()
    {
        if (Application.isPlaying && enableBrain)
        {
            initBrain2();
        }
        
    }
   
    internal void generateFile()
    {
        using (FileStream fs = File.Create(ASPFileTemplatePath))
        {
            foreach (ActuatorConfiguration actuatorConf in actuatorsConfigurations)
            {
                Byte[] info = new UTF8Encoding(true).GetBytes(actuatorConf.getAspTemplate());
                fs.Write(info, 0, info.Length);
            }
            foreach (SensorConfiguration sensorConf in sensorsConfigurations)
            {
                Byte[] info = new UTF8Encoding(true).GetBytes(sensorConf.getAspTemplate());
                fs.Write(info, 0, info.Length);
            }
        }
    }

    void initBrain2()
    {
       
        embasp = new SolverExectuor(this);
        ////MyDebugger.MyDebug("creating sensors");
        prepareSensors();
        prepareActuators();
        if (!executeReasonerOn.Equals("When Sensors are ready"))
        {
            reasonerMethod = Utility.getTriggerMethod(executeReasonerOn);
            if (!(reasonerMethod is null))
            {
                StartCoroutine("pulseOn");
            }
        }
        executionThread = new Thread(() =>
        {
            Thread.CurrentThread.Name = "Solver executor";
            Thread.CurrentThread.IsBackground = true;
            embasp.Run();
        });
        executionThread.Start();
        watch = new Stopwatch();
        watch.Start();
    }

    internal void removeNullSensorConfigurations()
    {
        MyDebugger.MyDebug("removing null");
        sensorsConfigurations.RemoveAll(x => (x == null||x is null));
    }
    internal void removeNullActuatorConfigurations()
    {
        actuatorsConfigurations.RemoveAll(x => x == null);
    }

    private void prepareActuators()
    {
        foreach (ActuatorConfiguration actuatorConfiguration in actuatorsConfigurations)
        {
            GameObject currentGameObject = actuatorConfiguration.gameObject;
            MonoBehaviourActuatorsManager currentManager = currentGameObject.GetComponent<MonoBehaviourActuatorsManager>();
            if (currentManager is null)
            {
                currentManager = currentGameObject.AddComponent<MonoBehaviourActuatorsManager>();
            }
            currentManager.instantiateActuator(actuatorConfiguration, this);
            ////MyDebugger.MyDebug(conf.configurationName+" added");
        }
        
    }

    private void prepareSensors()
    {
        foreach (SensorConfiguration sensorConfiguration in sensorsConfigurations)
        {
            GameObject currentGameObject = sensorConfiguration.gameObject;
            MonoBehaviourSensorsManager currentManager = currentGameObject.GetComponent<MonoBehaviourSensorsManager>();
            if (currentManager is null)
            {
                currentManager = currentGameObject.AddComponent<MonoBehaviourSensorsManager>();
            }
            //MyDebugger.MyDebug("configuration of the manager of " + conf.gOName + " game object: " + currentManager.configurations);
            
            currentManager.instantiateSensor(sensorConfiguration);
        }
        SensorsManager sensorManager = FindObjectOfType<SensorsManager>();
        sensorManager.registerSensors(this, sensorsConfigurations);
    }
    private IEnumerator pulseOn()
    {
        while (true)
        {
            yield return new WaitUntil(() => (bool)reasonerMethod.Invoke(triggerClass, null));
            lock (toLock)
            {
                solverWaiting = false;
                MyDebugger.MyDebug("Pulsing in brain");
                Monitor.Pulse(toLock);
            }
        }
    }
    
    void Update()
    {
        MyDebugger.enabled = debug;
        if (reasonerMethod is null && SensorsManager.frameFromLastUpdate == 1)
        {
            lock (toLock)
            {
                solverWaiting = false;
                Monitor.Pulse(toLock);
            }
        }
    }

    void OnApplicationQuit()
    {
        if (embasp != null) {
            embasp.reason = false;
            ////MyDebugger.MyDebug("finalize");
            lock (toLock)
            {
                Monitor.Pulse(toLock);
            }
            finalize();
        }
    }


    public void finalize()
    {
        //Performance.writeOnFile("facts", factsMSTotal / factsStep, true);
        //Performance.writeOnFile("answer set", asTotalMS / asSteps);
    }

}

