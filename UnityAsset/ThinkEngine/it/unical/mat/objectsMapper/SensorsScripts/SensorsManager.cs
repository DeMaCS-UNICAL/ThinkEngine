using EmbASP4Unity.it.unical.mat.objectsMapper.BrainsScripts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

[ExecuteInEditMode]
public  class SensorsManager : MonoBehaviour,IManager
{
    [SerializeField]
    private List<string> configuredGameObject;
    [SerializeField]
    private List<string> configurationsNames;
    private static Dictionary<Brain, List<SensorConfiguration>> instantiatedSensors;
    private static Queue<Brain> requestedMappings;
    private static object waitOn = new object();

    internal static int updateFrequencyInFrames;
    internal static int avgFps=0;
    internal static int frameCount=0;
    internal static int numberOfLiveSensor = 0;
    internal static int frameFromLastUpdate = 0;
    internal static float updateScaleFactor = 1;

    void Reset()
    {
        if (FindObjectsOfType<SensorsManager>().Length > 1)
        {
            try
            {
                throw new Exception("Only one SensorsManager can be instantiated");
            }
            finally
            {
                Destroy(this);
            }
        }
        if (requestedMappings == null)
        {
            requestedMappings = new Queue<Brain>();
        }
        if (configuredGameObject == null)
        {
            configuredGameObject = new List<string>();
        }
        if (configurationsNames == null)
        {
            configurationsNames = new List<string>();
        }
    }
    
    void OnEnable()
    {
        Reset();
    }
    void Update()
    {
        if (Application.isPlaying)
        {
            returnSensorsMappings();// here is safe since the sensors are updated in the LateUpdate
            frameFromLastUpdate++;
            frameCount++;
            int currentFps = (int)(1f / Time.unscaledDeltaTime);
            avgFps = (avgFps * (frameCount - 1) + currentFps) / frameCount;
            if (frameFromLastUpdate > updateFrequencyInFrames)
            {
                updateScaleFactor = (avgFps - currentFps) / (float)avgFps * 10;
                frameFromLastUpdate = 1;
                numberOfLiveSensor = FindObjectsOfType<MonoBehaviourSensor>().Length;
                updateFrequencyInFrames = numberOfLiveSensor / avgFps;
                updateFrequencyInFrames += (int)Math.Ceiling(updateFrequencyInFrames * updateScaleFactor);
            }
        }
    }

    public List<string> getConfiguredGameObject()
    {
        return configuredGameObject;
    }
    public List<string> getUsedNames()
    {
        return configurationsNames;
    }
    
    internal static void requestSensorsMapping(Brain brain)
    {
        lock (waitOn)
        {
            requestedMappings.Enqueue(brain);
            Monitor.Wait(waitOn);
        }
    }

    internal static void returnSensorsMappings()
    {
        lock (waitOn)
        {
            while (requestedMappings.Count > 0)
            {
                Brain brain = requestedMappings.Dequeue();
                string mapping = "";
                List<MonoBehaviourSensor> sensors = new List<MonoBehaviourSensor>();
                foreach (SensorConfiguration sensorConf in instantiatedSensors[brain])
                {
                    sensors.AddRange(sensorConf.gameObject.GetComponent<MonoBehaviourSensorsManager>().configurations[sensorConf]);
                }
                Stopwatch watch = new Stopwatch();
                watch.Start();
                foreach (MonoBehaviourSensor sensor in sensors)
                {
                    mapping += sensor.Map();
                }
                watch.Stop();
                brain.factsStep++;
                brain.factsMSTotal += watch.ElapsedMilliseconds;
                brain.sensorsMapping = mapping;
            }
        }
        Monitor.PulseAll(waitOn);
    }

    public void registerSensors(Brain brain, List<SensorConfiguration> instantiated)
    {
        if (instantiatedSensors == null)
        {
            instantiatedSensors = new Dictionary<Brain, List<SensorConfiguration>>();
        }
        if (!instantiatedSensors.ContainsKey(brain))
        {
            instantiatedSensors.Add(brain, instantiated);
        }
        else
        {
            instantiatedSensors[brain] = instantiated;
        }
    }

    public void deleteConfiguration(AbstractConfiguration abstractConfiguration)
    {
        if (configurationsNames.Contains(abstractConfiguration.configurationName))
        {
            int toDelete = configurationsNames.IndexOf(abstractConfiguration.configurationName);
            configurationsNames.RemoveAt(toDelete);
            configuredGameObject.RemoveAt(toDelete);
        }
    }

    public void addConfiguration(AbstractConfiguration abstractConfiguration)
    {
        if (!configurationsNames.Contains(abstractConfiguration.configurationName))
        {
            configurationsNames.Add(abstractConfiguration.configurationName);
            configuredGameObject.Add(abstractConfiguration.gameObject.name);
        }
    }
    
    public bool existsConfigurationWithName(string name)
    {
        return configurationsNames.Contains(name);
    }
}

