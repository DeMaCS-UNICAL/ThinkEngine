using it.unical.mat.embasp.languages.asp;
using Mappers;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
internal class ActuatorsManager : MonoBehaviour
{
    private static Dictionary<Brain, List<string>> _instantiatedActuators;
    private static Queue<KeyValuePair<Brain, AnswerSet>> _actuatorsToApply;
    private static Queue<KeyValuePair<Brain, object>> _requestedObjectIndexes;

    internal static bool _configurationsChanged;
    internal static bool destroyed;
    private static Dictionary<Brain, List<string>> InstantiatedActuators
    {
        get
        {
            if (_instantiatedActuators == null)
            {
                _instantiatedActuators = new Dictionary<Brain, List<string>>();
            }
            return _instantiatedActuators;
        }
    }
    internal static Queue<KeyValuePair<Brain, AnswerSet>> ActuatorsToApply
    {
        get
        {
            if (_actuatorsToApply == null)
            {
                _actuatorsToApply = new Queue<KeyValuePair<Brain, AnswerSet>>();
            }
            return _actuatorsToApply;
        }
    }

    internal bool IsConfigurationNameValid(string temporaryName, ActuatorConfiguration newActuatorConfiguration)
    {
        if (name.Equals(""))
        {
            return false;
        }
#if UNITY_EDITOR
        foreach (MonoBehaviourActuatorsManager manager in Resources.FindObjectsOfTypeAll<MonoBehaviourActuatorsManager>())
        {
            if (UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetPrefabStage(newActuatorConfiguration.gameObject) != null)
            {
                GameObject managerPrefab = PrefabUtility.GetCorrespondingObjectFromSource(manager.gameObject);
                if (managerPrefab != null && newActuatorConfiguration.gameObject.Equals(managerPrefab))
                {
                    continue;
                }
            }
            if (manager != null && manager.ExistsConfigurationOtherThan(name, newActuatorConfiguration))
            {
                return false;
            }
        }
#endif
        return true;
    }

    internal static Queue<KeyValuePair<Brain, object>> RequestedObjectIndexes
    {
        get
        {
            if (_requestedObjectIndexes == null)
            {
                _requestedObjectIndexes = new Queue<KeyValuePair<Brain, object>>();
            }
            return _requestedObjectIndexes;
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
    void Start()
    {
        if (Application.isPlaying)
        {
            Reset();
        }
    }
    void Reset()
    {
        if (FindObjectsOfType<ActuatorsManager>().Length > 1)
        {
            try
            {
                throw new Exception("Only one ActuatorsManager can be instantiated");
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
            ReturnObjectIndexes();
            ApplyActuators();
        }
    }
    void OnApplicationQuit()
    {
        while (RequestedObjectIndexes.Count > 0)
        {
            KeyValuePair<Brain, object> pair = RequestedObjectIndexes.Dequeue();
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
    #endregion
    #region Design-time methods
    private static void NotifyBrains()
    {
        foreach (Brain brain in Resources.FindObjectsOfTypeAll<Brain>())
        {
            brain.actuatorsConfigurationsChanged = true;
        }
        ConfigurationsChanged = false;
    }
    internal IEnumerable<string> AvailableConfigurationNames(Brain myScript) //Names of the actuator configurations that could be associated to the brain
    {
        if (!myScript.prefabBrain)//if it is not a prefab, all the acutator configurations can be associated to the brain
        {
            return ConfigurationNames();
        }
        //otherwise only the configuration attached to the gameobject of the brain can be associated
        MonoBehaviourActuatorsManager monoBehaviourActuatorsManager = myScript.GetComponent<MonoBehaviourActuatorsManager>();
        if (monoBehaviourActuatorsManager != null)
        {
            return monoBehaviourActuatorsManager.GetAllConfigurationNames();
        }
        return new List<string>();
    }
    private IEnumerable<string> ConfigurationNames()//return the names of ALL the actuator configurations available, both in scene or in prefabs
    {
        List<string> availableConfigurationNames = new List<string>();
        foreach (MonoBehaviourActuatorsManager manager in Resources.FindObjectsOfTypeAll<MonoBehaviourActuatorsManager>())
        {
            availableConfigurationNames.AddRange(manager.GetAllConfigurationNames());
        }
        return availableConfigurationNames;
    }
    internal List<ActuatorConfiguration> GetCorrespondingConfigurations(List<string> chosenActuatorConfigurations)
    {
        List<ActuatorConfiguration> toReturn = new List<ActuatorConfiguration>();
        foreach (string confName in chosenActuatorConfigurations)
        {
            MonoBehaviourActuatorsManager[] managers = Resources.FindObjectsOfTypeAll<MonoBehaviourActuatorsManager>();
            if (managers == null)
            {
                return toReturn;
            }
            foreach (MonoBehaviourActuatorsManager manager in managers)
            {
#if UNITY_EDITOR
                if (UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetPrefabStage(manager.gameObject) != null)
                {
                    continue;
                }
#endif
                ActuatorConfiguration currentConfiguration = manager.GetConfiguration(confName);
                if (currentConfiguration != null)
                {
                    toReturn.Add(currentConfiguration);
                }
            }
        }
        return toReturn;
    }
    public bool ExistsConfigurationWithName(string name, Brain brain = null)
    {
        if (brain != null && brain.prefabBrain)
        {
            return ExistsLocalConfigurationWithName(name, brain);
        }

        MonoBehaviourActuatorsManager[] managers = Resources.FindObjectsOfTypeAll<MonoBehaviourActuatorsManager>();
        if (managers == null)
        {
            return false;
        }
        foreach (MonoBehaviourActuatorsManager manager in managers)
        {
            if (manager.GetConfiguration(name) != null)
            {
                return true;
            }
        }
        return false;
    }
    private static bool ExistsLocalConfigurationWithName(string name, Brain brain)
    {
        MonoBehaviourActuatorsManager monoBehaviourActuatorsManager = brain.GetComponent<MonoBehaviourActuatorsManager>();
        if (monoBehaviourActuatorsManager != null)
        {
            if (monoBehaviourActuatorsManager.GetConfiguration(name) != null)
            {
                return true;
            }
        }
        return false;
    }
    internal Brain AssignedTo(string confName)//return the brain to which the actuator configuration is assigned
    {
        Brain[] brains = Resources.FindObjectsOfTypeAll<Brain>();
        if (brains == null)
        {
            return null;
        }
        foreach (Brain brain in brains)
        {
            if (brain.ChosenActuatorConfigurations.Contains(confName))
            {
                return brain;
            }
        }
        return null;
    }
#endregion
#region Run-time methods
    internal bool IsSomeActiveInScene(List<string> configurationNames)//returns true iff there is at least a gameobject in the scene for any of requested configuration
    {
        foreach (string configurationName in configurationNames)
        {
            foreach (MonoBehaviourActuatorsManager manager in FindObjectsOfType<MonoBehaviourActuatorsManager>())
            {
                if (manager.GetConfiguration(configurationName) != null)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public void RegisterBrainActuatorConfigurations(Brain b, List<string> instantiated)//brains notify the manager with the configurations it is interested in
    {
        if (!InstantiatedActuators.ContainsKey(b))
        {
            InstantiatedActuators.Add(b, new List<string>());
        }
        InstantiatedActuators[b].AddRange(instantiated);
    }
    internal static void RequestObjectIndexes(Brain brain)//a solver executor request the asp mapping of the index (IndexTracker) of the gameobjects to which its actuators are attached
    {
        object toLock = new object();
        lock (toLock)
        {
            RequestedObjectIndexes.Enqueue(new KeyValuePair<Brain, object>(brain, toLock));
            Monitor.Wait(toLock);
        }
    }
    private static void ReturnObjectIndexes()//notifies the brains that requested the object indexes with the needed information 
    {
        int count = 0;
        while (RequestedObjectIndexes.Count > 0 && count < 5)//count is used to avoid starvation (i.e. solver executors are faster than the main thread, thus the queue is never empty)
        {
            count++;
            KeyValuePair<Brain, object> currentPair = RequestedObjectIndexes.Dequeue();
            Brain brain = currentPair.Key;
            object toLock = currentPair.Value;
            lock (toLock)
            {
                string objectIndexes = GetObjectIndexes(brain);
                if (objectIndexes.Equals(""))
                {
                    RequestedObjectIndexes.Enqueue(currentPair);
                }
                else
                {
                    brain.objectsIndexes = objectIndexes;
                    Monitor.Pulse(toLock);
                }
            }
        }
    }
    private static string GetObjectIndexes(Brain brain)//retrieves the asp representation of the indexes (IndexTracker) of the gameobject to which it is associated an actuator assigned to the relative brain 
    {
        string toReturn = "";
        MonoBehaviourActuatorsManager[] monoBehaviourActuatorsManager;
        if (brain.prefabBrain)
        {
            monoBehaviourActuatorsManager = brain.GetComponents<MonoBehaviourActuatorsManager>();
        }
        else
        {
            monoBehaviourActuatorsManager = FindObjectsOfType<MonoBehaviourActuatorsManager>();
        }
        if (monoBehaviourActuatorsManager == null)
        {
            return toReturn;
        }
        foreach (MonoBehaviourActuatorsManager monobehaviourManager in monoBehaviourActuatorsManager)
        {
            if (!monobehaviourManager.ready)
            {
                continue;
            }
            foreach (ActuatorConfiguration configuration in monobehaviourManager.GetComponents<ActuatorConfiguration>())
            {
                if (InstantiatedActuators[brain].Contains(configuration.ConfigurationName))
                {
                    toReturn += "objectIndex(" + NewASPMapperHelper.AspFormat(configuration.ConfigurationName) + "," + configuration.GetComponent<IndexTracker>().CurrentIndex + ")." + Environment.NewLine;
                }
            }
        }
        return toReturn;
    }
    public void ApplyActuators()
    {
        while (ActuatorsToApply.Count > 0)
        {
            KeyValuePair<Brain, AnswerSet> brainAnswerSet = ActuatorsToApply.Dequeue();
            Brain brain = brainAnswerSet.Key;
            AnswerSet answerSet = brainAnswerSet.Value;
            if (!InstantiatedActuators.ContainsKey(brain))
            {
                continue;
            }
            if (brain.prefabBrain)
            {
                ApplyActuatorForPrefabBrain(brain, answerSet);
                continue;
            }
            MonoBehaviourActuatorsManager[] monobehaviourManagers = FindObjectsOfType<MonoBehaviourActuatorsManager>();
            if (monobehaviourManagers == null)
            {
                continue;
            }
            foreach (MonoBehaviourActuatorsManager monobehaviourManager in monobehaviourManagers)
            {
                if (!monobehaviourManager.ready)
                {
                    continue;
                }
                foreach (string actuatorConf in InstantiatedActuators[brain])
                {
                    ActuatorConfiguration currentConfiguration = monobehaviourManager.GetConfiguration(actuatorConf);
                    if (currentConfiguration != null && currentConfiguration.CheckIfApply())
                    {
                        List<MonoBehaviourActuator> actuators = monobehaviourManager.Actuators[currentConfiguration];
                        Performance.updatingActuators = true;
                        foreach (MonoBehaviourActuator act in actuators)
                        {
                            act.ToSet = act.Parse(answerSet);
                        }
                    }
                }
            }
        }
    }
    private void ApplyActuatorForPrefabBrain(Brain brain, AnswerSet answerSet)
    {
        MonoBehaviourActuatorsManager monoBehaviourActuatorsManager = brain.GetComponent<MonoBehaviourActuatorsManager>();
        if (monoBehaviourActuatorsManager == null)
        {
            return;
        }
        foreach (string actuatorConf in InstantiatedActuators[brain])
        {
            ActuatorConfiguration currentConfiguration = monoBehaviourActuatorsManager.GetConfiguration(actuatorConf);
            if (currentConfiguration != null && currentConfiguration.CheckIfApply())
            {
                List<MonoBehaviourActuator> actuators = monoBehaviourActuatorsManager.Actuators[currentConfiguration];
                Performance.updatingActuators = true;
                foreach (MonoBehaviourActuator act in actuators)
                {
                    act.ToSet = act.Parse(answerSet);
                }
            }
        }
    }
    internal static void NotifyActuators(Brain brain, AnswerSet answerSet)
    {
        ActuatorsToApply.Enqueue(new KeyValuePair<Brain, AnswerSet>(brain, answerSet));
    }
#endregion
}

