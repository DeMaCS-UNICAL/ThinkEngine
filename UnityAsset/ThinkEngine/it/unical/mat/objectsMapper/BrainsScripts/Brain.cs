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
    internal List<SensorConfiguration> sensorsConfigurations;
    internal List<ActuatorConfiguration> actuatorsConfigurations;
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
    //public bool updateSensorsRepeteadly;
    //public float sensorsUpdateFrequencyMS;
    //public bool updateSensorsOnTrigger;
    //public string updateSensorsOn="";
    //private MethodInfo sensorsUpdateMethod;
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
        checkTriggerClass();
        actuatorsManager = ActuatorsManager.GetInstance();
        sensorManager = SensorsManager.GetInstance();
        //MyDebugger.MyDebug("FINISH WITH AWAKE");
    }

    private void checkTriggerClass()
    {
        triggerClassPath = @".\Assets\Scripts\Trigger.cs";
        if (!Directory.Exists(@"Assets\Scripts"))
        {
            Directory.CreateDirectory(@"Assets\Scripts");
        }
        if (ASPFilePath is null)
        {
            ASPFilePath = @".\Assets\Resources\" + gameObject.name + ".asp";
        }

        if (Application.isEditor && !File.Exists(triggerClassPath))
        {
            createTriggerScript();
        }
    }

    private void createTriggerScript()
    {
        using (FileStream fs = File.Create(triggerClassPath))
        {
            string triggerClassContent = "using System;\n";
            triggerClassContent += "using UnityEngine;\n\n";
            triggerClassContent += @"// every method of this class without parameters and that returns a bool value can be used to trigger the reasoner.";
            triggerClassContent += "\n public class Trigger:ScriptableObject{\n\n";
            triggerClassContent += "}";
            Byte[] info = new UTF8Encoding(true).GetBytes(triggerClassContent);
            fs.Write(info, 0, info.Length);
        }
        AssetDatabase.Refresh();
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
        triggerClassPath = @".\Assets\Scripts\Trigger.cs";
        ASPFileTemplatePath = @".\Assets\Resources\" + gameObject.name + "_template.asp";
    }

      

    void OnValidate() {
        triggerClassPath = @".\Assets\Scripts\Trigger.cs";
        ASPFileTemplatePath = @".\Assets\Resources\" + gameObject.name + "_template.asp";
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
        triggerClass = ScriptableObject.CreateInstance("Trigger");
        MethodInfo[] triggerMethods = triggerClass.GetType().GetMethods();
        ////MyDebugger.MyDebug("creating sensors");
        prepareSensors();
        prepareActuators(triggerMethods);
        if (!executeReasonerOn.Equals("When Sensors are ready"))
        {
            foreach (MethodInfo mI in triggerMethods)
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

    private void prepareActuators(MethodInfo[] triggerMethods)
    {
        foreach (ActuatorConfiguration actuatorConfiguration in actuatorsConfigurations)
        {
            GameObject currentGameObject = actuatorConfiguration.gameObject;
            MonoBehaviourActuatorsManager currentManager = currentGameObject.GetComponent<MonoBehaviourActuatorsManager>();
            if (currentManager is null)
            {
                currentManager = currentGameObject.AddComponent<MonoBehaviourActuatorsManager>();
            }
            currentManager.instantiateActuator(actuatorConfiguration);
            ////MyDebugger.MyDebug(conf.configurationName+" added");
        }

        foreach (MethodInfo mI in triggerMethods)
        {
            if (mI.Name.Equals(applyActuatorsCondition))
            {
                MyDebugger.MyDebug("apply actuators on "+mI.Name);
                applyActuatorsMethod = mI;
            }
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

