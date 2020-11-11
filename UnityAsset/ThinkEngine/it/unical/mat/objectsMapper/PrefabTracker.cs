using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;


internal static class PrefabTracker
{
    internal static List<GameObject> GetPrefabs()
    {
        if (Directory.Exists(@"Assets/Resources/Prefabs"))
        {
            return Resources.LoadAll<GameObject>("Prefabs").ToList<GameObject>();
        }
        return new List<GameObject>();
    }

    internal static List<MonoBehaviourSensorsManager> GetBehaviourSensorsManagers()
    {
        List<MonoBehaviourSensorsManager> existingManagers = new List<MonoBehaviourSensorsManager>();
        foreach(GameObject prefab in GetPrefabs())
        {
            MonoBehaviourSensorsManager manager = prefab.GetComponent<MonoBehaviourSensorsManager>();
            if (manager!=null)
            {
                existingManagers.Add(manager);
            }
        }
        return existingManagers;
    }
    internal static List<MonoBehaviourActuatorsManager> GetBehaviourActuatorsManagers()
    {
        List<MonoBehaviourActuatorsManager> existingManagers = new List<MonoBehaviourActuatorsManager>();
        foreach (GameObject prefab in GetPrefabs())
        {
            MonoBehaviourActuatorsManager manager = prefab.GetComponent<MonoBehaviourActuatorsManager>();
            if (manager != null)
            {
                existingManagers.Add(manager);
            }
        }
        return existingManagers;
    }
    internal static List<SensorConfiguration> GetSensorsConfigurations()
    {
        List<SensorConfiguration> existingConfigurations = new List<SensorConfiguration>();
        foreach (MonoBehaviourSensorsManager manager in GetBehaviourSensorsManagers())
        {
            if (manager.GetComponent<SensorConfiguration>() == null)
            {
                continue;
            }
            foreach(SensorConfiguration configuration in manager.GetComponents<SensorConfiguration>())
            {
                existingConfigurations.Add(configuration);
            }
        }
        return existingConfigurations;
    }
    internal static List<ActuatorConfiguration> GetActuatorsConfigurations()
    {
        List<ActuatorConfiguration> existingConfigurations = new List<ActuatorConfiguration>();
        foreach (MonoBehaviourActuatorsManager manager in GetBehaviourActuatorsManagers())
        {
            if (manager.GetComponent<ActuatorConfiguration>() == null)
            {
                continue;
            }
            foreach (ActuatorConfiguration configuration in manager.GetComponents<ActuatorConfiguration>())
            {
                existingConfigurations.Add(configuration);
            }
        }
        return existingConfigurations;
    }
}

