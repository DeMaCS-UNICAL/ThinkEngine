using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEditor;
using EmbASP4Unity.it.unical.mat.objectsMapper;

public class AbstractConfiguration :MonoBehaviour
{
    protected GameObjectsTracker tracker;
    internal IManager manager;
    public List<List<string>> properties;
    public List<string> propertiesNames;
    public string configurationName;
    public List<SimpleGameObjectsTracker> advancedConf;

    protected void Awake()
    {
        gameObject.AddComponent<IndexTracker>();
        manager.addConfiguration(this);
    }
    public void SaveConfiguration(GameObjectsTracker tr)
    {
        properties = new List<List<string>>();
        propertiesNames = new List<string>();
        advancedConf = new List<SimpleGameObjectsTracker>();
        cleanSpecificDataStructure();
        configurationName = tr.configurationName;
        tracker = tr;
        Dictionary<string, FieldOrProperty> gOProperties = tr.ObjectsProperties[gameObject];
        List<Component> comp = tr.GOComponents[gameObject];
        foreach (string s in gOProperties.Keys)
        {
                
            if (tr.ObjectsToggled[gOProperties[s]])
            {
                //MyDebugger.MyDebug("property " + s + " toggled");
                List<string> currentProperty = new List<string>();
                properties.Add(currentProperty);
                if (tracker.IsMappable(gOProperties[s]))
                {
                    //MyDebugger.MyDebug("adding " + gOProperties[s].Name());
                    currentProperty.Add(s);
                    if (!tracker.IsBaseType(gOProperties[s]))
                    {
                        tracker.basicTypeCollectionsConfigurations[gOProperties[s]].propertyName = currentProperty;
                        advancedConf.Add(tracker.basicTypeCollectionsConfigurations[gOProperties[s]]);
                    }
                    else
                    {
                        specificConfiguration(gOProperties[s], currentProperty);
                    }
                }
                else if (tracker.ObjectDerivedFromFields.ContainsKey(gameObject))
                {
                    //MyDebugger.MyDebug("recursing on " + gOProperties[s].Name());
                    recursevelyAdd(gameObject, gOProperties[s], currentProperty);
                }
            }
        }
        foreach (Component c in comp)
        {
            if (tr.ObjectsToggled[c])
            {
                List<string> currentProperty = new List<string>();
                properties.Add(currentProperty);
                currentProperty.Add(c.GetType() + "");
                //MyDebugger.MyDebug("adding " + c.GetType().ToString());
                // properties.Add(c.GetType().ToString());
                Dictionary<string, FieldOrProperty> componentProperties = tr.ObjectsProperties[c];
                foreach (string s in componentProperties.Keys)
                {
                    if (tr.ObjectsToggled[componentProperties[s]])
                    {
                        currentProperty.Add(s);
                        if (tracker.IsMappable(componentProperties[s]))
                        {
                            // MyDebugger.MyDebug("adding " + c.GetType().ToString() + "^" + s);
                            if (tracker.IsBaseType(componentProperties[s]))
                            {
                                specificConfiguration(componentProperties[s], currentProperty);
                            }
                                
                            if (!tracker.IsBaseType(componentProperties[s]))
                            {
                                tracker.basicTypeCollectionsConfigurations[componentProperties[s]].propertyName = currentProperty;
                                advancedConf.Add(tracker.basicTypeCollectionsConfigurations[componentProperties[s]]);
                            }
                        }
                        else if (tracker.ObjectDerivedFromFields.ContainsKey(c))// && !tracker.IsBaseType(componentProperties[s]))
                        {
                            //MyDebugger.MyDebug("recursing on " + c.name);
                            recursevelyAdd(c, componentProperties[s], currentProperty);
                        }

                    }
                        
                }
            }
        }
        if (properties.Count == 0)
        {
            throw new Exception("No properties selected, invalid configuration to save.");
        }
        manager.addConfiguration(this);
        //MyDebugger.MyDebug("success");
    }

        

    protected void recursevelyAdd(object obj, FieldOrProperty fieldOrProperty, List<string> currentPropertyHierarchy)
    {
        object derivedObj = tracker.ObjectDerivedFromFields[obj][fieldOrProperty.Name()];
        if (derivedObj == null || (tracker.ObjectsOwners.ContainsKey(derivedObj) && !tracker.ObjectsOwners[derivedObj].Key.Equals(obj)) || !tracker.ObjectsOwners[derivedObj].Value.Equals(fieldOrProperty.Name()) || derivedObj.Equals(tracker.GO))
        {
            // MyDebugger.MyDebug(fieldOrProperty.Name()+" returning ");
            return;
        }
        if (tracker.ObjectsProperties.ContainsKey(derivedObj))
        {
            Dictionary<string, FieldOrProperty> derivedObjProperties = tracker.ObjectsProperties[derivedObj];
            foreach (string s in derivedObjProperties.Keys)
            {

                if (tracker.ObjectsToggled[derivedObjProperties[s]])
                {
                    //MyDebugger.MyDebug(s + " is toggled");
                    if (tracker.IsMappable(derivedObjProperties[s]))
                    {

                        //MyDebugger.MyDebug(derivedObjProperties[s].Name() + " toggled");
                        // MyDebugger.MyDebug("adding " + parent + fieldOrProperty.Name() + "^" + s);
                        currentPropertyHierarchy.Add(fieldOrProperty.Name());
                        if (tracker.IsBaseType(derivedObjProperties[s]))/////CHECK
                        {
                            specificConfiguration(derivedObjProperties[s], currentPropertyHierarchy);
                        }
                           
                        if (!tracker.IsBaseType(derivedObjProperties[s]))
                        {
                            tracker.basicTypeCollectionsConfigurations[derivedObjProperties[s]].propertyName = currentPropertyHierarchy;
                            advancedConf.Add(tracker.basicTypeCollectionsConfigurations[derivedObjProperties[s]]);
                        }
                    }

                    else if (tracker.ObjectDerivedFromFields.ContainsKey(derivedObj))
                    {
                        //MyDebugger.MyDebug("recursin on " + parent + fieldOrProperty.Name() + "^" + derivedObjProperties[s].Name());

                        recursevelyAdd(derivedObj, derivedObjProperties[s], currentPropertyHierarchy);

                    }
                }
            }
        }
    }
    internal virtual void specificConfiguration(FieldOrProperty fieldOrProperty, List<string> property) { }
    internal virtual void cleanSpecificDataStructure() { }
}
    

