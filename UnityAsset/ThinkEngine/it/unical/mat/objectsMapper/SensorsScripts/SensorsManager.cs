using EmbASP4Unity.it.unical.mat.objectsMapper.BrainsScripts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using Debug = UnityEngine.Debug;

[ExecuteInEditMode]
public  class SensorsManager : MonoBehaviour
{
    private static Dictionary<Brain, List<string>> instantiatedSensors;
    private static Queue<KeyValuePair<Brain,object>> _requestedMappings;
    internal static int updateFrequencyInFrames=12;
    internal bool _configurationsChanged;
    internal static float avgFps = 0;
    internal static float bestAvgFps = 0;
    internal static int frameCount = 0;
    internal static int numberOfLiveSensor = 0;
    internal static int frameFromLastUpdate = 2;
    internal static float updateScaleFactor = 1;
    internal static float lastUpdateFPS;
    private static bool uncompletedTasks;
    internal static bool destroyed;
    private static Queue<KeyValuePair<Brain,object>> requestedMappings
    {
        get
        {
            if (_requestedMappings == null)
            {
                _requestedMappings = new Queue<KeyValuePair<Brain, object>>();
            }
            return _requestedMappings;
        }
    }
    internal bool configurationsChanged
    {
        get
        {
            return _configurationsChanged;
        }
        set
        {
            _configurationsChanged = value;
            if (_configurationsChanged)
            {
                notifyBrains();
            }
        }
    }

    private void notifyBrains()
    {
        foreach(Brain brain in Resources.FindObjectsOfTypeAll<Brain>())
        {
            brain.sensorsConfigurationsChanged = true;
        }
        configurationsChanged = false;
    }

   void OnDestroy()
    {
        destroyed = true;
    }

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
                if (Application.isPlaying)
                {
                    Destroy(this);
                }
                else
                {
                    DestroyImmediate(this);
                }
            }
        }
    }
    
    void Start()
    {
        if (Application.isPlaying)
        {
            Reset();
        }
    }
    void Update()
    {
        if (Application.isPlaying)
        {
            frameFromLastUpdate++;
            frameCount++;
            float currentFps = 1f / Time.unscaledDeltaTime;
            avgFps = (avgFps * (frameCount - 1) + currentFps) / frameCount;
            if (bestAvgFps < avgFps)
            {
                bestAvgFps = avgFps;
            }
            if (frameFromLastUpdate > updateFrequencyInFrames)
            {
                frameFromLastUpdate = -1;
                Performance.updatedSensors = true;
                lastUpdateFPS = currentFps;
            }
            updateScaleFactor = (bestAvgFps - avgFps) / bestAvgFps * 10;
            numberOfLiveSensor = FindObjectsOfType<MonoBehaviourSensor>().Length;
            updateFrequencyInFrames = (int)(numberOfLiveSensor / lastUpdateFPS);
            updateFrequencyInFrames += (int)Math.Ceiling(updateFrequencyInFrames * updateScaleFactor);
            updateFrequencyInFrames = Math.Max(1, updateFrequencyInFrames);
            returnSensorsMappings();// here is safe since the sensors are updated in the LateUpdate
        }

    }
    
    internal List<SensorConfiguration> getConfigurations(List<string> chosenSensorConfigurations)
    {
        List<SensorConfiguration> toReturn = new List<SensorConfiguration>();
        foreach (string confName in chosenSensorConfigurations)
        {
            foreach (MonoBehaviourSensorsManager manager in Resources.FindObjectsOfTypeAll<MonoBehaviourSensorsManager>())
            {
                if (PrefabStageUtility.GetPrefabStage(manager.gameObject)!=null)
                {
                    continue;
                }
                SensorConfiguration currentConfiguration = manager.getConfiguration(confName);
                if (currentConfiguration != null)
                {
                    toReturn.Add(currentConfiguration);
                }
            }
        }
        return toReturn;
    }
    internal bool isSomeActiveInScene(List<string> configurationNames)
    {
        foreach(string configurationName in configurationNames)
        {
            foreach(MonoBehaviourSensorsManager manager in FindObjectsOfType<MonoBehaviourSensorsManager>())
            {
                if (manager.getConfiguration(configurationName) != null)
                {
                    return true;
                }
            }
        }
        return false;
    }

    internal static void requestSensorsMapping(Brain brain)
    {
        object toLock = new object();
        lock (toLock)
        {
            requestedMappings.Enqueue(new KeyValuePair<Brain,object>(brain,toLock));
            //MyDebugger.MyDebug("requesting map");
            Monitor.Wait(toLock);
        }
    }

    internal static void returnSensorsMappings()
    {
        if (frameFromLastUpdate !=-1 && !uncompletedTasks)
        {
            return;
        }
        int count = 0;
        while (requestedMappings.Count > 0 && count<5)
        {
            count++;
            KeyValuePair<Brain,object> currentPair = requestedMappings.Dequeue();
            Brain brain = currentPair.Key;
            object toLock = currentPair.Value;
            lock (toLock)
            {
                string mapping = "";
                List<MonoBehaviourSensor> sensors = new List<MonoBehaviourSensor>();
                foreach (string sensorConf in instantiatedSensors[brain])
                {
                    foreach (MonoBehaviourSensorsManager monobehaviourManager in FindObjectsOfType<MonoBehaviourSensorsManager>())
                    {
                        //Debug.Log("manager in: " + monobehaviourManager.gameObject.name);
                        if (!monobehaviourManager.ready)
                        {
                            continue;
                        }
                        SensorConfiguration currentConfiguration = monobehaviourManager.getConfiguration(sensorConf);
                        if (currentConfiguration!=null)
                        {
                            //Debug.Log("has configuration " + currentConfiguration.configurationName +" with "+ monobehaviourManager.configurations[currentConfiguration].Count+" sensors");
                            sensors.AddRange(monobehaviourManager.configurations[currentConfiguration]);
                        }
                    }
                }
                Stopwatch watch = new Stopwatch();
                watch.Start();
                foreach (MonoBehaviourSensor sensor in sensors)
                {
                    //MyDebugger.MyDebug("asked " + sensor.sensorName + " map");
                    mapping += sensor.Map();
                }
                watch.Stop();
                brain.factsStep++;
                brain.factsMSTotal += watch.ElapsedMilliseconds;
                brain.sensorsMapping = mapping;
                Monitor.Pulse(toLock);
            }
        }
        if (requestedMappings.Count > 0)
        {
            uncompletedTasks = true;
        }
        else
        {
            uncompletedTasks = false;
        }
    }

    internal IEnumerable<string> configurationNames()
    {
        List<string> availableConfigurationNames = new List<string>();
        foreach(MonoBehaviourSensorsManager manager in Resources.FindObjectsOfTypeAll<MonoBehaviourSensorsManager>())
        {
            availableConfigurationNames.AddRange(manager.GetAllConfigurationNames());
        }
        return availableConfigurationNames;
    }

    public void registerSensors(Brain brain, List<string> instantiated)
    {
        if (instantiatedSensors == null)
        {
            instantiatedSensors = new Dictionary<Brain, List<string>>();
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
    
    public bool existsConfigurationWithName(string name)
    {
        foreach (MonoBehaviourSensorsManager manager in Resources.FindObjectsOfTypeAll<MonoBehaviourSensorsManager>())
        {
            if (manager.getConfiguration(name)!=null)
            {
                return true;
            }
        }
        return false;
    }
    void OnApplicationQuit()
    {
        while (requestedMappings.Count > 0)
        {
            KeyValuePair<Brain,object> pair = requestedMappings.Dequeue();
            Brain brain = pair.Key;
            object toLock = pair.Value;
            if (brain.embasp != null)
            {
                brain.embasp.reason = false;
                ////MyDebugger.MyDebug("finalize");
                lock (toLock)
                {
                    Monitor.Pulse(toLock);
                }
            }
        }
    }
}

