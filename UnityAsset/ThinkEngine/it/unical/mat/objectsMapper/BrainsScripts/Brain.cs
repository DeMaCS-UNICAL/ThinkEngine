﻿using EmbASP4Unity.it.unical.mat.objectsMapper.ActuatorsScripts;
using EmbASP4Unity.it.unical.mat.objectsMapper.SensorsScripts;
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
    public bool debug;
    public bool maintainFactFile;
    public List<SensorConfiguration> sensorsConfigurations;
    public List<ActuatorConfiguration> actuatorsConfigurations;
    //private List<AdvancedSensor> sensors;
    //private List<SimpleActuator> actuators;
    private MappingManager mapper;
    private Thread executionThread;
    private SolverExectuor embasp;
    int count = 0;
    private static System.Timers.Timer timer;
    //public bool sensorsUpdated;
    internal bool solverWaiting;
    public string ASPFilePath;
    public string ASPFileTemplatePath;
    public string triggerClassPath;
    //private bool updateSensors;
    private bool actuatorsReady;
    public bool updateSensorsRepeteadly;
    public float sensorsUpdateFrequencyMS;
    public bool updateSensorsOnTrigger;
    public string updateSensorsOn="";
    private MethodInfo sensorsUpdateMethod;
    private MethodInfo reasonerMethod;
    private object triggerClass;
    public string executeReasonerOn;
    public string applyActuatorsCondition;
    private MethodInfo applyActuatorsMethod;
    private SensorsManager sensorManager;
    private ActuatorsManager actuatorsManager;
    internal long elapsedMS;
    private Stopwatch watch;
    internal long factsMSTotal;
    internal int factsStep;
    internal long asTotalMS;
    internal int asSteps;

    void Awake()
    {
        MyDebugger.enabled = debug;
        //Debug.unityLogger.logEnabled = false;
        triggerClassPath = @".\Assets\Scripts\Trigger.cs";
        if (!Directory.Exists(@"Assets\Scripts"))
        {
            Directory.CreateDirectory(@"Assets\Scripts");
        }
        if (ASPFilePath is null)
        {
            ASPFilePath = @".\Assets\Resources\" + gameObject.name + ".asp";
        }
        actuatorsManager = ActuatorsManager.GetInstance();
        sensorManager = SensorsManager.GetInstance();
        if (Application.isEditor && !File.Exists(triggerClassPath)) {
            using (FileStream fs = File.Create(triggerClassPath))
            {
                string triggerClassContent = "using System;\n";
                triggerClassContent+="using UnityEngine;\n\n" ;
                triggerClassContent += @"// every method of this class returning a bool value can be used to trigger the sensors update.";
                triggerClassContent += "\n public class Trigger:ScriptableObject{\n\n";
                triggerClassContent += "}";
                Byte[] info = new UTF8Encoding(true).GetBytes(triggerClassContent);
                fs.Write(info, 0, info.Length);
            }
            AssetDatabase.Refresh();
        }
        //MyDebugger.MyDebug("FINISH WITH AWAKE");
    }
    void Start()
    {
        //Debug.unityLogger.logEnabled = false;
        //MyDebugger.MyDebug("STARTING BRAIN");
        if (Application.isPlaying && enableBrain)
        {
            initBrain2();
        }
        
    }
    void Update()
    {
        if (!Application.isPlaying || !enableBrain)
        {
            return;
        }
        if (updateSensorsRepeteadly)
        {
            if (elapsedMS > sensorsUpdateFrequencyMS)
            {
                watch.Restart();
            }
            elapsedMS = watch.ElapsedMilliseconds;
            if (elapsedMS >= sensorsUpdateFrequencyMS)
            {
                Performance.updatingSensors = true;
            }
        }
    }
    internal bool actuatorsUpdateCondition()
    {
        if (applyActuatorsMethod != null)
        {
            return (bool)applyActuatorsMethod.Invoke(triggerClass, null);
        }else if (applyActuatorsCondition.Equals("Never"))
        {
            return false;
        }
        return true;
    }

    void Reset() {
        //executeRepeatedly = true;
        triggerClassPath = @".\Assets\Scripts\Trigger.cs";
        //ASPFilePath = @".\Assets\Resources\" + gameObject.name + ".asp";
        ASPFileTemplatePath = @".\Assets\Resources\" + gameObject.name + "_template.asp";
        // brainUpdateFrequency = 500;
    }

      

    void OnValidate() {
        triggerClassPath = @".\Assets\Scripts\Trigger.cs";
        //ASPFilePath = @".\Assets\Resources\" + gameObject.name + ".asp";
        ASPFileTemplatePath = @".\Assets\Resources\" + gameObject.name + "_template.asp";
    }

    internal void generateFile()
    {
        List<AdvancedSensor> sensors = new List<AdvancedSensor>();
        List<SimpleActuator> actuators = new List<SimpleActuator>();
        foreach (SensorConfiguration conf in sensorsConfigurations)
        {
            sensors.Add(new AdvancedSensor(conf));
            ////MyDebugger.MyDebug(conf.configurationName+" added");
        }
        foreach (ActuatorConfiguration conf in actuatorsConfigurations)
        {
            actuators.Add(new SimpleActuator(conf));
            ////MyDebugger.MyDebug(conf.configurationName+" added");
        }
        mapper = MappingManager.getInstance();
        IMapper actuatorMapper = mapper.getMapper(typeof(SimpleActuator));
        ASPAdvancedSensorMapper sensorMapper = (ASPAdvancedSensorMapper)mapper.getMapper(typeof(AdvancedSensor));
        using (FileStream fs = File.Create(ASPFileTemplatePath))
        {
            foreach (SimpleActuator act in actuators)
            {
                Byte[] info = new UTF8Encoding(true).GetBytes(actuatorMapper.Map(act));
                fs.Write(info, 0, info.Length);
            }
            foreach (AdvancedSensor sens in sensors)
            {
                Byte[] info = new UTF8Encoding(true).GetBytes(sensorMapper.getASPRepresentation(sens));
                fs.Write(info, 0, info.Length);
            }
        }
    }
    

    void initBrain2()
    {
        List<IMonoBehaviourSensor> sensors = new List<IMonoBehaviourSensor>();
        HashSet<MonoBehaviourSensorsManager> sensorsManagers = new HashSet<MonoBehaviourSensorsManager>();
        List<SimpleActuator> actuators = new List<SimpleActuator>();
        embasp = new SolverExectuor(this);
        triggerClass = ScriptableObject.CreateInstance("Trigger");
        MethodInfo[] methods = triggerClass.GetType().GetMethods();
        ////MyDebugger.MyDebug("creating sensors");
        prepareSensors(sensors, sensorsManagers, methods, triggerClass);
        prepareActuators(sensors, actuators, methods);
        if (!executeReasonerOn.Equals("When Sensors are ready"))
        {
            foreach (MethodInfo mI in methods)
            {
                if (mI.Name.Equals(executeReasonerOn))
                {
                    ////MyDebugger.MyDebug(mI.Name);
                    reasonerMethod = mI;
                    StartCoroutine("pulseOn");
                    break;
                }
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

    private void prepareActuators(List<IMonoBehaviourSensor> sensors, List<SimpleActuator> actuators, MethodInfo[] methods)
    {
        foreach (ActuatorConfiguration conf in actuatorsConfigurations)
        {
            actuators.Add(new SimpleActuator(conf));
            ////MyDebugger.MyDebug(conf.configurationName+" added");
        }
        actuatorsManager.registerActuators(this, actuators);

        if (!actuatorsManager.applyCoroutinStarted)
        {
            StartCoroutine(actuatorsManager.applyActuators());
            actuatorsManager.applyCoroutinStarted = true;
        }
        foreach (MethodInfo mI in methods)
        {
            if (mI.Name.Equals(applyActuatorsCondition))
            {
                MyDebugger.MyDebug("apply actuators on "+mI.Name);
                applyActuatorsMethod = mI;
            }
        }
    }

    private void prepareSensors(List<IMonoBehaviourSensor> sensors, HashSet<MonoBehaviourSensorsManager> sensorsManagers, MethodInfo[] methods, object triggerClass)
    {
        if (!updateSensorsOn.Equals(""))
        {

            foreach (MethodInfo mI in methods)
            {
                if (mI.Name.Equals(updateSensorsOn))
                {
                    ////MyDebugger.MyDebug(mI.Name);
                    sensorsUpdateMethod = mI;
                    break;
                    //StartCoroutine("UpdateSensorsOnTrigger");
                }
            }
        }
        foreach (SensorConfiguration conf in sensorsConfigurations)
        {
            MonoBehaviourSensorsManager currentManager = GameObject.Find(conf.gOName).GetComponent<MonoBehaviourSensorsManager>();
            if (currentManager is null)
            {
                currentManager = GameObject.Find(conf.gOName).AddComponent<MonoBehaviourSensorsManager>();
                currentManager.brain = this;
            }
            //MyDebugger.MyDebug("configuration of the manager of " + conf.gOName + " game object: " + currentManager.configurations);
            if (updateSensorsRepeteadly)
            {
                currentManager.executeRepeteadly = updateSensorsRepeteadly;
                currentManager.frequence = sensorsUpdateFrequencyMS;
            }
            else
            {
                currentManager.updateMethod = sensorsUpdateMethod;
                currentManager.triggerClass = triggerClass;
            }
            currentManager.configurations.Add(conf);
            sensorsManagers.Add(currentManager);
        }
        
        foreach (MonoBehaviourSensorsManager manager in sensorsManagers)
        {
            sensors.AddRange(manager.generateSensors());
            
        }
        sensorManager.registerSensors(this, sensors);
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

    public void setActuatorsReady(bool v)
    {
        actuatorsReady = v;
    }

    public bool areActuatorsReady()
    {
        return actuatorsReady;
    }

    public IEnumerable<SimpleActuator> getActuators()
    {
        return actuatorsManager.instantiatedActuators[this];
    }
    
    void OnApplicationQuit()
    {
        if (timer != null)
        {
            timer.Enabled = false;
        }
        if (embasp != null) {
            embasp.reason = false;
            ////MyDebugger.MyDebug("finalize");
            sensorManager.pulseExecutor(this);
            finalize();
        }
    }


    public void finalize()
    {
        //Performance.writeOnFile("facts", factsMSTotal / factsStep, true);
        //Performance.writeOnFile("answer set", asTotalMS / asSteps);
    }

}

