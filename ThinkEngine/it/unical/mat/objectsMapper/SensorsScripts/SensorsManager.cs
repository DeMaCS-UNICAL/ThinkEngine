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
using static MonoBehaviourSensorHider;

[ExecuteInEditMode]
public  class SensorsManager : MonoBehaviour
{
    private static Dictionary<Brain, List<string>> _instantiatedSensors;
    private static Queue<KeyValuePair<Brain,object>> _requestedMappings;
    internal static int updateFrequencyInFrames=12;
    internal static bool _configurationsChanged;
    internal static float avgFps = 0;
    internal static float bestAvgFps = 0;
    internal static int frameCount = 0;
    internal static int numberOfLiveSensor = 0;
    internal static int frameFromLastUpdate = 2;
    internal static float updateScaleFactor = 1;
    internal static float lastUpdateFPS;
    private static bool uncompletedTasks;
    internal static bool destroyed;
    private static Dictionary<Brain, List<string>> instantiatedSensors
    {
        get
        {
            if (_instantiatedSensors == null)
            {
                _instantiatedSensors = new Dictionary<Brain, List<string>>();
            }
            return _instantiatedSensors;
        }
    }
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
    internal static bool configurationsChanged
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
                NotifyBrains();
            }
        }
    }
    #region Unity Messages
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
            ReturnSensorsMappings();// here is safe since the sensors are updated in the LateUpdate
        }
    }
    void OnApplicationQuit()
    {
        destroyed = true;
        while (requestedMappings.Count > 0)
        {
            KeyValuePair<Brain,object> pair = requestedMappings.Dequeue();
            Brain brain = pair.Key;
            object toLock = pair.Value;
            if (brain.embasp != null)
            {
                brain.embasp.reason = false;
                lock (toLock)
                {
                    Monitor.Pulse(toLock);
                }
            }
        }
    }
    internal IEnumerable<string> ConfigurationNames()
    {
        List<string> availableConfigurationNames = new List<string>();
        foreach(MonoBehaviourSensorsManager manager in Resources.FindObjectsOfTypeAll<MonoBehaviourSensorsManager>())
        {
            availableConfigurationNames.AddRange(manager.GetAllConfigurationNames());
        }
        return availableConfigurationNames;
    }
    #endregion
    #region Design-time methods
    private static void NotifyBrains()
    {
        foreach (Brain brain in Resources.FindObjectsOfTypeAll<Brain>())
        {
            brain.sensorsConfigurationsChanged = true;
        }
        configurationsChanged = false;
    }
    internal List<SensorConfiguration> GetConfigurations(List<string> chosenSensorConfigurations)
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
                SensorConfiguration currentConfiguration = manager.GetConfiguration(confName);
                if (currentConfiguration != null)
                {
                    toReturn.Add(currentConfiguration);
                }
            }
        }
        return toReturn;
    }
    public bool ExistsConfigurationWithName(string name)
    {
        foreach (MonoBehaviourSensorsManager manager in Resources.FindObjectsOfTypeAll<MonoBehaviourSensorsManager>())
        {
            if (manager.GetConfiguration(name)!=null)
            {
                return true;
            }
        }
        return false;
    }
    #endregion
    #region Run-time methods
    internal bool IsSomeActiveInScene(List<string> configurationNames)
    {
        foreach(string configurationName in configurationNames)
        {
            foreach(MonoBehaviourSensorsManager manager in FindObjectsOfType<MonoBehaviourSensorsManager>())
            {
                if (manager.GetConfiguration(configurationName) != null)
                {
                    return true;
                }
            }
        }
        return false;
    }
    internal static void RequestSensorsMapping(Brain brain)
    {
        object toLock = new object();
        lock (toLock)
        {
            requestedMappings.Enqueue(new KeyValuePair<Brain,object>(brain,toLock));
            Monitor.Wait(toLock);
        }
    }
    internal static void ReturnSensorsMappings()
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
                List<MonoBehaviourSensor> sensors = RetrieveBrainsSensors(brain);
                Stopwatch watch = new Stopwatch();
                watch.Start();
                foreach (MonoBehaviourSensor sensor in sensors)
                {
                    mapping += sensor.Map();
                }
                watch.Stop();
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
    private static List<MonoBehaviourSensor> RetrieveBrainsSensors(Brain brain)
    {
        List<MonoBehaviourSensor> sensors = new List<MonoBehaviourSensor>();
        foreach (string sensorConf in instantiatedSensors[brain])
        {
            foreach (MonoBehaviourSensorsManager monobehaviourManager in FindObjectsOfType<MonoBehaviourSensorsManager>())
            {
                if (!monobehaviourManager.ready)
                {
                    continue;
                }
                SensorConfiguration currentConfiguration = monobehaviourManager.GetConfiguration(sensorConf);
                if (currentConfiguration != null)
                {
                    sensors.AddRange(monobehaviourManager.configurations[currentConfiguration]);
                }
            }
        }
        return sensors;
    }
    public void RegisterBrainsSensorConfigurations(Brain brain, List<string> instantiated)
    {
       if (!instantiatedSensors.ContainsKey(brain))
        {
            instantiatedSensors.Add(brain, instantiated);
        }
        else
        {
            instantiatedSensors[brain] = instantiated;
        }
    }
    #endregion
}

