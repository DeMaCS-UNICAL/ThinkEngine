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

[ExecuteAlways]

public class Brain :MonoBehaviour
{
    public readonly object toLock = new object();
    public bool enableBrain;
    public bool debug=true;
    public bool maintainFactFile;
    public List<string> chosenSensorConfigurations;
    public List<string> chosenActuatorConfigurations;
    private Thread executionThread;
    internal SolverExectuor embasp;
    internal bool solverWaiting;
    [SerializeField,HideInInspector]
    internal string ASPFilePath;
    [SerializeField,HideInInspector]
    internal string ASPFileTemplatePath;
    internal MethodInfo reasonerMethod;
    private object triggerClass;
    public string executeReasonerOn;
    internal string sensorsMapping;
    internal string objectsIndexes;
    private Stopwatch watch;
    internal long factsMSTotal;
    internal int factsStep;
    internal long asTotalMS;
    internal int asSteps;
    internal string dataPath;
    internal bool sensorsConfigurationsChanged;
    internal bool actuatorsConfigurationsChanged;
    [SerializeField]
    internal bool prefabBrain;
    [SerializeField]
    internal bool specificASPFile;
    [SerializeField]
    internal bool globalASPFile;
    private string originalName;
    internal bool missingData;

    void Reset()
    {
        if (GetComponent<IndexTracker>() == null)
        {
            gameObject.AddComponent<IndexTracker>();
        }
        MyDebugger.enabled = debug;
        triggerClass = Utility.triggerClass;
        dataPath = Environment.CurrentDirectory;
        if (chosenActuatorConfigurations == null)
        {
            chosenActuatorConfigurations = new List<string>();
            chosenSensorConfigurations = new List<string>();
        }
        if (ASPFileTemplatePath is null)
        {
            ASPFileTemplatePath = @".\Assets\Resources\" + gameObject.name + "Template.asp";
        }
        if(executeReasonerOn is null)
        {
            executeReasonerOn = "";
        }
        if (!File.Exists(ASPFileTemplatePath))
        {
            if (!Directory.Exists(@".\Assets\Resources"))
            {
                Directory.CreateDirectory(@".\Assets\Resources");
            }
            File.Create(ASPFileTemplatePath);
        }
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
            StartCoroutine("initBrain2");
        }

    }
   
    internal void generateFile()
    {
        using (FileStream fs = File.Create(ASPFileTemplatePath))
        {
            Byte[] info = new UTF8Encoding(true).GetBytes("%For runtime instantiated GameObject, only the prefab mapping is provided. Use that one substituting the gameobject name accordingly\n");
            fs.Write(info, 0, info.Length);

            foreach (ActuatorConfiguration actuatorConf in Utility.actuatorsManager.getConfigurations(chosenActuatorConfigurations))
            {
                info = new UTF8Encoding(true).GetBytes(actuatorConf.GetAspTemplate());
                fs.Write(info, 0, info.Length);
            }
            foreach (SensorConfiguration sensorConf in Utility.sensorsManager.getConfigurations(chosenSensorConfigurations))
            {
                info = new UTF8Encoding(true).GetBytes(sensorConf.GetAspTemplate());
                fs.Write(info, 0, info.Length);
            }
        }
    }

    IEnumerator initBrain2()
    {
        if(specificASPFile && originalName.Equals(gameObject.name))
        {
            yield return new WaitUntil( () => !originalName.Equals(gameObject.name));
        }
        embasp = new SolverExectuor(this);
        prepareSensors();
        prepareActuators();
        if (!someConfigurationAvailable())
        {
            missingData = true;
        }
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
            //MyDebugger.MyDebug("starting thread");
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
        chosenSensorConfigurations.RemoveAll(x => !Utility.sensorsManager.existsConfigurationWithName(x));
    }
    internal void removeNullActuatorConfigurations()
    {
        chosenActuatorConfigurations.RemoveAll(x => !Utility.actuatorsManager.existsConfigurationWithName(x,this));
    }

    private void prepareActuators()
    {
        Utility.actuatorsManager.registerActuators(this, chosenActuatorConfigurations);
    }

    private void prepareSensors()
    {
        Utility.sensorsManager.registerSensors(this, chosenSensorConfigurations);
    }
    private IEnumerator pulseOn()
    {
        while (true)
        {
            yield return new WaitUntil(() => solverWaiting && someConfigurationAvailable() && (bool)reasonerMethod.Invoke(triggerClass, null));
            lock (toLock)
            {
                solverWaiting = false;
                Monitor.Pulse(toLock);
            }
        }
    }
    void Update()
    {
        if (reasonerMethod == null)
        {
            lock (toLock)
            {
                if (!someConfigurationAvailable())
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

    private bool someConfigurationAvailable()
    {
        return Utility.sensorsManager.isSomeActiveInScene(chosenSensorConfigurations) && Utility.actuatorsManager.isSomeActiveInScene(chosenActuatorConfigurations);
    }

    void LateUpdate()
    {
        MyDebugger.enabled = debug;
        /*if (reasonerMethod is null && SensorsManager.frameFromLastUpdate == -1)
        {
            lock (toLock)
            {
                solverWaiting = false;
                Monitor.Pulse(toLock);
            }
        }*/
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

