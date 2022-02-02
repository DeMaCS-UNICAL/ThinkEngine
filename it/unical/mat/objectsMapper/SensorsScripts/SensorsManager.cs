using Planner;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Debug = UnityEngine.Debug;

[ExecuteInEditMode]
public  class SensorsManager : MonoBehaviour
{
    private const int MAX_SENSORS_PER_FRAME = 20;
    private const int MIN_AVG_FPS = 60;
    private const int MIN_CURRENT_FPS = 20;
    private const int MAX_AVG_FPS = 70;
    private const int MAX_CURRENT_FPS = 40;
    private static int MAX_MS = 5;
    private static Dictionary<Brain, List<string>> _instantiatedSensors;
    private static ConcurrentQueue<KeyValuePair<Brain,object>> _requestedMappings;
    public static int updateFrequencyInFrames=12;
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
    internal static float currentFps;
    private static Dictionary<Brain, List<string>> InstantiatedSensors
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
    internal bool IsConfigurationNameValid(string name, SensorConfiguration newSensorConfiguration)
    {
#if UNITY_EDITOR
        
        if (name.Equals(""))
        {
            return false;
        }
        foreach (MonoBehaviourSensorsManager manager in Resources.FindObjectsOfTypeAll<MonoBehaviourSensorsManager>())
        {
            if (PrefabStageUtility.GetPrefabStage(newSensorConfiguration.gameObject) != null)
            {
                GameObject managerPrefab = PrefabUtility.GetCorrespondingObjectFromSource(manager.gameObject);
                if (managerPrefab != null && newSensorConfiguration.gameObject.Equals(managerPrefab))
                {
                    continue;
                }
            }
            if (manager!=null && manager.ExistsConfigurationOtherThan(name, newSensorConfiguration))
            {
                return false;
            }
        }
#endif
        return true;
    }


    private static ConcurrentQueue<KeyValuePair<Brain,object>> RequestedMappings
    {
        get
        {
            if (_requestedMappings == null)
            {
                _requestedMappings = new ConcurrentQueue<KeyValuePair<Brain, object>>();
            }
            return _requestedMappings;
        }
    }
    internal static bool ConfigurationsChanged
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
    void Update()
    {
        if (Application.isPlaying)
        {
            frameFromLastUpdate++;
            frameCount++;
            float currentFps = 1f / Time.unscaledDeltaTime;
            avgFps = (avgFps * (frameCount - 1) + currentFps) / frameCount;
            if((avgFps< MIN_AVG_FPS || currentFps < MIN_CURRENT_FPS) && MAX_MS>1)
            {
                MAX_MS--;
            }
            else if(avgFps> MAX_AVG_FPS && currentFps> MAX_CURRENT_FPS)
            {
                MAX_MS++;
            }
        }
    }
    void Start()
    {
        if (Application.isPlaying)
        {
            Reset();
            StartCoroutine(SensorsUpdate());
        }
    }
    MonoBehaviourSensorsManager[] RetrieveSensorsManagers()
    {
        return FindObjectsOfType<MonoBehaviourSensorsManager>();
    }
    IEnumerator SensorsUpdate()
    {
        Stopwatch watch = new Stopwatch();
        while (true)
        {
            MonoBehaviourSensorsManager[] managers = RetrieveSensorsManagers();
            foreach (MonoBehaviourSensorsManager manager in managers)
            {
                watch.Start();
                if (manager != null)
                {
                    manager.ManageSensors();
                    if (watch.ElapsedMilliseconds > MAX_MS)
                    {
                        watch.Reset();
                        yield return null;
                        watch.Start();
                    }
                    foreach (List<MonoBehaviourSensor> sensorsList in manager.Sensors.Values)
                    {
                        foreach (MonoBehaviourSensor sensor in sensorsList)
                        {
                            if (sensor != null)
                            {
                                sensor.UpdateValue();
                            }
                            if (watch.ElapsedMilliseconds > MAX_MS)
                            {
                                watch.Reset();
                                yield return null;
                                watch.Start();
                            }
                        }
                    }
                }
            }
            watch.Reset();
            yield return null;
            StartCoroutine(ReturnSensorsMappings());
        }
    }


    void OnApplicationQuit()
    {
        destroyed = true;
        while (RequestedMappings.TryDequeue(out KeyValuePair<Brain, object> pair)) 
        { 
            Brain brain = pair.Key;
            object toLock = pair.Value;
            if (brain.executor != null)
            {
                brain.executor.reason = false;
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
        foreach (ActuatorBrain brain in Resources.FindObjectsOfTypeAll<ActuatorBrain>())
        {
            brain.sensorsConfigurationsChanged = true;
        }
        ConfigurationsChanged = false;
    }
    internal List<SensorConfiguration> GetConfigurations(List<string> chosenSensorConfigurations)
    {
        List<SensorConfiguration> toReturn = new List<SensorConfiguration>();
        foreach (string confName in chosenSensorConfigurations)
        {
            foreach (MonoBehaviourSensorsManager manager in Resources.FindObjectsOfTypeAll<MonoBehaviourSensorsManager>())
            {
#if UNITY_EDITOR
                if (PrefabStageUtility.GetPrefabStage(manager.gameObject)!=null)
                {
                    continue;
                }
#endif
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
        if (configurationNames.Count == 0)
        {
            return true;
        }
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
            RequestedMappings.Enqueue(new KeyValuePair<Brain,object>(brain,toLock));
            Monitor.Wait(toLock);
        }
    }
    internal static IEnumerator ReturnSensorsMappings()
    {
        int count = 0;
        while (RequestedMappings.TryDequeue(out KeyValuePair<Brain, object> currentPair) && count<5)
        {
            count++;
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
                    if (watch.ElapsedMilliseconds > MAX_MS)
                    {
                        watch.Reset();
                        yield return null;
                        watch.Start();
                    }
                    if (sensor != null)
                    {
                        //UnityEngine.Debug.Log(sensor +" "+ sensor.Map());
                        mapping += brain.ActualSensorEncoding(sensor.Map());
                    }
                }
                brain.sensorsMapping = mapping;
                Monitor.Pulse(toLock);
            }
        }
        if (RequestedMappings.Count > 0)
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
        foreach (string sensorConf in InstantiatedSensors[brain])
        {
            foreach (MonoBehaviourSensorsManager monobehaviourManager in FindObjectsOfType<MonoBehaviourSensorsManager>())
            {
                if (monobehaviourManager== null || !monobehaviourManager.ready)
                {
                    continue;
                }
                SensorConfiguration currentConfiguration = monobehaviourManager.GetConfiguration(sensorConf);
                if (currentConfiguration != null)
                {
                    sensors.AddRange(monobehaviourManager.Sensors[currentConfiguration]);
                }
            }
        }
        return sensors;
    }
    public void RegisterBrainsSensorConfigurations(Brain brain, List<string> instantiated)
    {
       if (!InstantiatedSensors.ContainsKey(brain))
        {
            InstantiatedSensors.Add(brain, instantiated);
        }
        else
        {
            InstantiatedSensors[brain] = instantiated;
        }
    }
#endregion
}

