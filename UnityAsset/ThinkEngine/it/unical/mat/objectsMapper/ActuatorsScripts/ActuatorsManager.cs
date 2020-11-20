using it.unical.mat.embasp.languages.asp;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;

[ExecuteInEditMode]
public class ActuatorsManager : MonoBehaviour
{
    private static Dictionary<Brain,List<string>> _instantiatedActuators;
    private static Queue<KeyValuePair<Brain, AnswerSet>> _actuatorsToApply;
    private static Queue<KeyValuePair<Brain, object>> _requestedObjectIndexes;

    internal bool _configurationsChanged;
    internal static bool destroyed;
    private static Dictionary<Brain, List<string>> instantiatedActuators
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
    internal static Queue<KeyValuePair<Brain, AnswerSet>> actuatorsToApply
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
    internal static Queue<KeyValuePair<Brain, object>> requestedObjectIndexes
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
        while (requestedObjectIndexes.Count > 0)
        {
            KeyValuePair<Brain, object> pair = requestedObjectIndexes.Dequeue();
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
    private void NotifyBrains()
    {
        foreach (Brain brain in Resources.FindObjectsOfTypeAll<Brain>())
        {
            brain.actuatorsConfigurationsChanged = true;
        }
        configurationsChanged = false;
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
    private IEnumerable<string> ConfigurationNames()//return the names of ALL the actuator configurations available, both in scene or in prefabs
    {
        List<string> availableConfigurationNames = new List<string>();
        foreach (MonoBehaviourActuatorsManager manager in Resources.FindObjectsOfTypeAll<MonoBehaviourActuatorsManager>())
        {
            availableConfigurationNames.AddRange(manager.GetAllConfigurationNames());
        }
        return availableConfigurationNames;
    }
    public void RegisterActuators(Brain b, List<string> instantiated)//brains notify the manager with the configurations it is interested in
    {
        if (!instantiatedActuators.ContainsKey(b))
        {
            instantiatedActuators.Add(b, new List<string>());
        }
        instantiatedActuators[b].AddRange(instantiated);
    }
    internal static void RequestObjectIndexes(Brain brain)//a solver executor request the asp mapping of the index (IndexTracker) of the gameobjects to which its actuators are attached
    {
        object toLock = new object();
        lock (toLock)
        {
            requestedObjectIndexes.Enqueue(new KeyValuePair<Brain, object>(brain, toLock));
            Monitor.Wait(toLock);
        }
    }
    private static void ReturnObjectIndexes()//notifies the brains that requested the object indexes with the needed information 
    {
        int count = 0;
        while (requestedObjectIndexes.Count > 0 && count < 5)//count is used to avoid starvation (i.e. solver executors are faster than the main thread, thus the queue is never empty)
        {
            count++;
            KeyValuePair<Brain, object> currentPair = requestedObjectIndexes.Dequeue();
            Brain brain = currentPair.Key;
            object toLock = currentPair.Value;
            lock (toLock)
            {
                brain.objectsIndexes = GetObjectIndexes(brain);
                Monitor.Pulse(toLock);
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
                if (instantiatedActuators[brain].Contains(configuration.configurationName))
                {
                    toReturn += "objectIndex(" + configuration.configurationName + "," + configuration.GetComponent<IndexTracker>().currentIndex + ")." + Environment.NewLine;
                }
            }
        }
        return toReturn;
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
                if (PrefabStageUtility.GetPrefabStage(manager.gameObject) != null)
                {
                    continue;
                }
                ActuatorConfiguration currentConfiguration = manager.GetConfiguration(confName);
                if (currentConfiguration != null)
                {
                    toReturn.Add(currentConfiguration);
                }
            }
        }
        return toReturn;
    }
    public void ApplyActuators()
    {
        while (actuatorsToApply.Count > 0)
        {
            KeyValuePair<Brain,AnswerSet> brainAnswerSet = actuatorsToApply.Dequeue();
            Brain brain = brainAnswerSet.Key;
            AnswerSet answerSet = brainAnswerSet.Value;
            if (!instantiatedActuators.ContainsKey(brain))
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
                foreach (string actuatorConf in instantiatedActuators[brain])
                {
                    ActuatorConfiguration currentConfiguration = monobehaviourManager.GetConfiguration(actuatorConf);
                    if (currentConfiguration!=null && currentConfiguration.CheckIfApply())
                    {
                        List<MonoBehaviourActuator> actuators = monobehaviourManager.actuators[currentConfiguration];
                        Performance.updatingActuators = true;
                        foreach (MonoBehaviourActuator act in actuators)
                        {
                            act.toSet = act.Parse(answerSet);
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
        foreach (string actuatorConf in instantiatedActuators[brain])
        {
            ActuatorConfiguration currentConfiguration = monoBehaviourActuatorsManager.GetConfiguration(actuatorConf);
            if (currentConfiguration != null && currentConfiguration.CheckIfApply())
            {
                List<MonoBehaviourActuator> actuators = monoBehaviourActuatorsManager.actuators[currentConfiguration];
                Performance.updatingActuators = true;
                foreach (MonoBehaviourActuator act in actuators)
                {
                    act.toSet = act.Parse(answerSet);
                }
            }
        }
    }
    public bool ExistsConfigurationWithName(string name, Brain brain=null)
    {
        if (brain!=null && brain.prefabBrain)
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
            if (manager.GetConfiguration(name)!=null)
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
    internal static void NotifyActuators(Brain brain, AnswerSet answerSet)
    {
        actuatorsToApply.Enqueue(new KeyValuePair<Brain,AnswerSet>(brain,answerSet));
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
            if (brain.chosenActuatorConfigurations.Contains(confName))
            {
                return brain;
            }
        }
        return null;
    }
}

