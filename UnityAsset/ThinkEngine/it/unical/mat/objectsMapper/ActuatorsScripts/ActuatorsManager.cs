using EmbASP4Unity.it.unical.mat.embasp.languages.asp;
using EmbASP4Unity.it.unical.mat.objectsMapper.BrainsScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class ActuatorsManager : MonoBehaviour, IManager
{
    private static List<string> configuredGameObject;
    private static List<string> configurationsNames;
    public static Dictionary<Brain,List<ActuatorConfiguration>> instantiatedActuators;
    internal static Queue<KeyValuePair<Brain, AnswerSet>> actuatorsToApply;
        
        
    public List<string> getUsedNames()
    {
        return configurationsNames;
    }
    void OnEnable()
    {
        if (FindObjectsOfType<ActuatorsManager>().Length > 1)
        {
            try
            {
                throw new Exception("Only one ActuatorsManager can be instantiated");
            }
            finally
            {
                Destroy(this);
            }
        }
        configuredGameObject = new List<string>();
        configurationsNames = new List<string>();
        actuatorsToApply = new Queue<KeyValuePair<Brain, AnswerSet>>();
    }

    void Update()
    {
        applyActuators();
    }
    public void registerActuators(Brain b, List<ActuatorConfiguration> instantiated)
    {
        if (instantiatedActuators == null)
        {
            instantiatedActuators = new Dictionary<Brain, List<ActuatorConfiguration>>();
        }
        if (!instantiatedActuators.ContainsKey(b))
        {
            instantiatedActuators.Add(b, instantiated);
        }
        else
        {
            instantiatedActuators[b]= instantiated;
        }
    }

    public void applyActuators()
    {
        while (actuatorsToApply.Count > 0)
        {
            KeyValuePair<Brain,AnswerSet> brainAnswerSet = actuatorsToApply.Dequeue();
            foreach(ActuatorConfiguration actuatorConf in instantiatedActuators[brainAnswerSet.Key])
            {
                if (actuatorConf.checkIfApply())
                {
                    List<MonoBehaviourActuator> actuators = actuatorConf.gameObject.GetComponent<MonoBehaviourActuatorsManager>().configurations[actuatorConf];
                    Performance.updatingActuators = true;
                    foreach (MonoBehaviourActuator act in actuators) {
                        act.toSet = act.parse(brainAnswerSet.Value);
                    }
                }
            }
        }
            
    }

    public List<string> getConfiguredGameObject()
    {
        return configuredGameObject;
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

    public void deleteConfiguration(AbstractConfiguration abstractConfiguration)
    {
        if (configurationsNames.Contains(abstractConfiguration.configurationName))
        {
            int toDelete = configurationsNames.IndexOf(abstractConfiguration.configurationName);
            configurationsNames.RemoveAt(toDelete);
            configuredGameObject.RemoveAt(toDelete);
        }
    }

    internal static void notifyActuators(Brain brain, AnswerSet answerSet)
    {
        actuatorsToApply.Enqueue(new KeyValuePair<Brain,AnswerSet>(brain,answerSet));
    }
}

