using Planner;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ThinkEngine.it.unical.mat.objectsMapper.BrainsScripts;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Debug = UnityEngine.Debug;

[ExecuteInEditMode]
public  class SensorsManager : MonoBehaviour
{
    public int MIN_AVG_FPS = 60;
    public int MIN_CURRENT_FPS = 20;
    public int MAX_AVG_FPS = 70;
    public int MAX_CURRENT_FPS = 40;
    private int MAX_MS = 5;
    private float currentFps;
    private static Dictionary<Brain, List<string>> _instantiatedSensors;
    private static ConcurrentQueue<KeyValuePair<Brain,object>> _requestedMappings;
    internal static bool _configurationsChanged;
    internal float avgFps = 0;
    internal static float bestAvgFps = 0;
    internal static int frameCount = 0;
    internal static bool destroyed;
    internal static MonoBehaviourSensorsManager[] monoBehaviourMenagers;
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
            if (destroyed)
            {
                throw new Exception("application is closing");
            }
            if (_requestedMappings == null)
            {
                _requestedMappings = new ConcurrentQueue<KeyValuePair<Brain, object>>();
            }
            //Debug.Log(_requestedMappings.Count);
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
       // Debug.Log("destroing");
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
            frameCount++;
            currentFps = 1f / Time.unscaledDeltaTime;
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
            destroyed = false;
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
            Stopwatch localWatch = new Stopwatch();
            watch.Start();
            yield return new WaitUntil(()=>Executor.CanRead(false));
            monoBehaviourMenagers = RetrieveSensorsManagers();
            foreach (MonoBehaviourSensorsManager manager in monoBehaviourMenagers)
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
            Executor.CanRead(true);
            yield return null;
        }
    }


    void OnApplicationQuit()
    {
        Executor.ShutDownAll();
        destroyed = true;
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
    internal static void ReturnSensorsMappings(Brain brain)
    {
        ConcurrentBag<string> mapping = new ConcurrentBag<string>(); ;
        List<MonoBehaviourSensor> sensors = RetrieveBrainsSensors(brain);
        Parallel.ForEach(sensors, sensor =>
        {
            if (sensor != null)
            {
                mapping.Add(brain.ActualSensorEncoding(sensor.Map()));
            }
        });
        brain.sensorsMapping = string.Join("",mapping);
        
    }
    private static List<MonoBehaviourSensor> RetrieveBrainsSensors(Brain brain)
    {
        List<MonoBehaviourSensor> sensors = new List<MonoBehaviourSensor>();
        foreach (string sensorConf in InstantiatedSensors[brain])
        {
            foreach (MonoBehaviourSensorsManager monobehaviourManager in monoBehaviourMenagers)
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

