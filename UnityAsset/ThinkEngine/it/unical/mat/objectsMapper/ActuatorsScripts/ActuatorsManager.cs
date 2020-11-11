using EmbASP4Unity.it.unical.mat.objectsMapper.BrainsScripts;
using it.unical.mat.embasp.languages.asp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;

[ExecuteInEditMode]
public class ActuatorsManager : MonoBehaviour
{
    public static Dictionary<Brain,List<string>> instantiatedActuators;
    private static Queue<KeyValuePair<Brain, AnswerSet>> _actuatorsToApply;
    
    private static Queue<KeyValuePair<Brain, object>> _requestedObjectIndexes;
    internal bool _configurationsChanged;
    internal static bool destroyed;

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
                notifyBrains();
            }
        }
    }
    private void notifyBrains()
    {
        foreach (Brain brain in Resources.FindObjectsOfTypeAll<Brain>())
        {
            brain.actuatorsConfigurationsChanged = true;
        }
        configurationsChanged = false;
    }
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

    internal IEnumerable<string> configurationNames(Brain myScript)
    {
        if (!myScript.prefabBrain)
        {
            return configurationNames();
        }
        MonoBehaviourActuatorsManager monoBehaviourActuatorsManager = myScript.GetComponent<MonoBehaviourActuatorsManager>();
        if (monoBehaviourActuatorsManager != null)
        {
            return monoBehaviourActuatorsManager.GetAllConfigurationNames();
        }
        return new List<string>();
    }
    internal bool isSomeActiveInScene(List<string> configurationNames)
    {
        foreach (string configurationName in configurationNames)
        {
            foreach (MonoBehaviourActuatorsManager manager in FindObjectsOfType<MonoBehaviourActuatorsManager>())
            {
                if (manager.getConfiguration(configurationName) != null)
                {
                    return true;
                }
            }
        }
        return false;
    }
    private IEnumerable<string> configurationNames()
    {
        List<string> availableConfigurationNames = new List<string>();
        foreach (MonoBehaviourActuatorsManager manager in Resources.FindObjectsOfTypeAll<MonoBehaviourActuatorsManager>())
        {
            availableConfigurationNames.AddRange(manager.GetAllConfigurationNames());
        }
        return availableConfigurationNames;
    }
    void Update()
    {
        if (Application.isPlaying)
        {
            returnObjectIndexes();
            applyActuators();
        }
    }
    public void registerActuators(Brain b, List<string> instantiated)
    {
        //MyDebugger.MyDebug("adding "+instantiated.Count+" actuators to the manager");
        if (instantiatedActuators == null)
        {
            instantiatedActuators = new Dictionary<Brain, List<string>>();
        }
        if (!instantiatedActuators.ContainsKey(b))
        {
            instantiatedActuators.Add(b, new List<string>());
        }
        instantiatedActuators[b].AddRange(instantiated);
        MyDebugger.MyDebug("there are "+ instantiatedActuators[b].Count+" actuators for brain "+b.name);
    }
    internal static void requestObjectIndexes(Brain brain)
    {
        object toLock = new object();
        lock (toLock)
        {
            requestedObjectIndexes.Enqueue(new KeyValuePair<Brain, object>(brain, toLock));
            //MyDebugger.MyDebug("requesting map");
            Monitor.Wait(toLock);
        }
    }
    internal static void returnObjectIndexes()
    {
        int count = 0;
        while (requestedObjectIndexes.Count > 0 && count < 5)
        {
            count++;
            KeyValuePair<Brain, object> currentPair = requestedObjectIndexes.Dequeue();
            Brain brain = currentPair.Key;
            object toLock = currentPair.Value;
            lock (toLock)
            {
                brain.objectsIndexes = getObjectIndexes(brain);
                Monitor.Pulse(toLock);
            }
        }
    }
    internal static string getObjectIndexes(Brain brain)
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

    internal List<ActuatorConfiguration> getConfigurations(List<string> chosenActuatorConfigurations)
    {
        List<ActuatorConfiguration> toReturn = new List<ActuatorConfiguration>();
        foreach (string confName in chosenActuatorConfigurations)
        {
            foreach (MonoBehaviourActuatorsManager manager in Resources.FindObjectsOfTypeAll<MonoBehaviourActuatorsManager>())
            {
                if (PrefabStageUtility.GetPrefabStage(manager.gameObject) != null)
                {
                    continue;
                }
                ActuatorConfiguration currentConfiguration = manager.getConfiguration(confName);
                if (currentConfiguration != null)
                {
                    toReturn.Add(currentConfiguration);
                }
            }
        }
        return toReturn;
    }

    
    public void applyActuators()
    {
        while (actuatorsToApply.Count > 0)
        {
            KeyValuePair<Brain,AnswerSet> brainAnswerSet = actuatorsToApply.Dequeue();
            /*foreach(string s in brainAnswerSet.Value.GetAnswerSet())
            {
                Debug.Log(s);
            }*/
            if (brainAnswerSet.Key.prefabBrain)
            {
                applyActuatorForPrefabBrain(brainAnswerSet);
                continue;
            }
            foreach (MonoBehaviourActuatorsManager monobehaviourManager in FindObjectsOfType<MonoBehaviourActuatorsManager>())
            {
                foreach (string actuatorConf in instantiatedActuators[brainAnswerSet.Key])
                {
                    ActuatorConfiguration currentConfiguration = monobehaviourManager.getConfiguration(actuatorConf);
                    if (currentConfiguration!=null && currentConfiguration.checkIfApply())
                    {
                        List<MonoBehaviourActuator> actuators = monobehaviourManager.configurations[currentConfiguration];
                        Performance.updatingActuators = true;
                        foreach (MonoBehaviourActuator act in actuators)
                        {
                            Debug.Log("parsing");
                            act.toSet = act.parse(brainAnswerSet.Value);
                        }
                    }
                }
            }
        }
            
    }

    private void applyActuatorForPrefabBrain(KeyValuePair<Brain, AnswerSet> brainAnswerSet)
    {
        MonoBehaviourActuatorsManager monoBehaviourActuatorsManager = brainAnswerSet.Key.GetComponent<MonoBehaviourActuatorsManager>();
        if (monoBehaviourActuatorsManager == null)
        {
            return;
        }
        foreach (string actuatorConf in instantiatedActuators[brainAnswerSet.Key])
        {
            ActuatorConfiguration currentConfiguration = monoBehaviourActuatorsManager.getConfiguration(actuatorConf);
            if (currentConfiguration != null && currentConfiguration.checkIfApply())
            {
                List<MonoBehaviourActuator> actuators = monoBehaviourActuatorsManager.configurations[currentConfiguration];
                Performance.updatingActuators = true;
                foreach (MonoBehaviourActuator act in actuators)
                {
                    act.toSet = act.parse(brainAnswerSet.Value);
                }
            }
        }
    }

    public bool existsConfigurationWithName(string name, Brain brain=null)
    {
        if (brain!=null && brain.prefabBrain)
        {
            MonoBehaviourActuatorsManager monoBehaviourActuatorsManager = brain.GetComponent<MonoBehaviourActuatorsManager>();
            if (monoBehaviourActuatorsManager != null)
            {
                if (monoBehaviourActuatorsManager.getConfiguration(name) != null)
                {
                    return true;
                }
            }
            return false;
        }
        foreach (MonoBehaviourActuatorsManager manager in Resources.FindObjectsOfTypeAll<MonoBehaviourActuatorsManager>())
        {
            if (manager.getConfiguration(name)!=null)
            {
                return true;
            }
        }
        return false;
    }
    
    internal static void notifyActuators(Brain brain, AnswerSet answerSet)
    {
        actuatorsToApply.Enqueue(new KeyValuePair<Brain,AnswerSet>(brain,answerSet));
    }

    internal Brain assignedTo(string confName)
    {
        foreach(Brain brain in Resources.FindObjectsOfTypeAll<Brain>())
        {
            if (brain.chosenActuatorConfigurations.Contains(confName))
            {
                return brain;
            }
        }
        return null;
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
                ////MyDebugger.MyDebug("finalize");
                lock (toLock)
                {
                    Monitor.Pulse(toLock);
                }
            }
        }
    }
}

