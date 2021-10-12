using NewStructures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

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

    internal bool IsConfigurationNameValid(string name, NewSensorConfiguration newSensorConfiguration)
    {
        if (name.Equals(""))
        {
            return false;
        }
        foreach (NewMonoBehaviourSensorsManager manager in Resources.FindObjectsOfTypeAll<NewMonoBehaviourSensorsManager>())
        {
            if (manager.ExistsConfigurationOtherThan(name, newSensorConfiguration))
            {
                return false;
            }
        }
        return true;
    }


    private static Queue<KeyValuePair<Brain,object>> RequestedMappings
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
            numberOfLiveSensor = FindObjectsOfType<NewMonoBehaviourSensor>().Length;
            updateFrequencyInFrames = (int)(numberOfLiveSensor / lastUpdateFPS);
            updateFrequencyInFrames += (int)Math.Ceiling(updateFrequencyInFrames * updateScaleFactor);
            updateFrequencyInFrames = Math.Max(1, updateFrequencyInFrames);
            ReturnSensorsMappings();// here is safe since the sensors are updated in the LateUpdate
        }
    }
    void OnApplicationQuit()
    {
        destroyed = true;
        while (RequestedMappings.Count > 0)
        {
            KeyValuePair<Brain,object> pair = RequestedMappings.Dequeue();
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
        foreach(NewMonoBehaviourSensorsManager manager in Resources.FindObjectsOfTypeAll<NewMonoBehaviourSensorsManager>())
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
        ConfigurationsChanged = false;
    }
    internal List<NewSensorConfiguration> GetConfigurations(List<string> chosenSensorConfigurations)
    {
        List<NewSensorConfiguration> toReturn = new List<NewSensorConfiguration>();
        foreach (string confName in chosenSensorConfigurations)
        {
            foreach (NewMonoBehaviourSensorsManager manager in Resources.FindObjectsOfTypeAll<NewMonoBehaviourSensorsManager>())
            {
#if UNITY_EDITOR
                if (UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetPrefabStage(manager.gameObject)!=null)
                {
                    continue;
                }
#endif
                NewSensorConfiguration currentConfiguration = manager.GetConfiguration(confName);
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
        foreach (NewMonoBehaviourSensorsManager manager in Resources.FindObjectsOfTypeAll<NewMonoBehaviourSensorsManager>())
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
            foreach(NewMonoBehaviourSensorsManager manager in FindObjectsOfType<NewMonoBehaviourSensorsManager>())
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
    internal static void ReturnSensorsMappings()
    {
        if (frameFromLastUpdate !=-1 && !uncompletedTasks)
        {
            return;
        }
        int count = 0;
        while (RequestedMappings.Count > 0 && count<5)
        {
            count++;
            KeyValuePair<Brain,object> currentPair = RequestedMappings.Dequeue();
            Brain brain = currentPair.Key;
            object toLock = currentPair.Value;
            lock (toLock)
            {
                string mapping = "";
                List<NewMonoBehaviourSensor> sensors = RetrieveBrainsSensors(brain);
                UnityEngine.Debug.Log("there are " + sensors.Count + " sensors ");
                Stopwatch watch = new Stopwatch();
                watch.Start();
                foreach (NewMonoBehaviourSensor sensor in sensors)
                {
                    //UnityEngine.Debug.Log(sensor +" "+ sensor.Map());
                    mapping += sensor.Map();
                }
                watch.Stop();
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
    private static List<NewMonoBehaviourSensor> RetrieveBrainsSensors(Brain brain)
    {
        List<NewMonoBehaviourSensor> sensors = new List<NewMonoBehaviourSensor>();
        foreach (string sensorConf in InstantiatedSensors[brain])
        {
            foreach (NewMonoBehaviourSensorsManager monobehaviourManager in FindObjectsOfType<NewMonoBehaviourSensorsManager>())
            {
                if (!monobehaviourManager.ready)
                {
                    continue;
                }
                NewSensorConfiguration currentConfiguration = monobehaviourManager.GetConfiguration(sensorConf);
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

