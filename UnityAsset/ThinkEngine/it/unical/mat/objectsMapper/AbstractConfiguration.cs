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
    public List<MyListString> properties;
    public List<string> propertiesNames;
    public string configurationName;
    public List<SimpleGameObjectsTracker> advancedConf;
    internal Dictionary<int,List<string>> aspTemplate; //the int is the position of the property in "properties"

    protected void Awake()
    {
        gameObject.AddComponent<IndexTracker>();
        manager.addConfiguration(this);
        ASPRep();
    }
    public void SaveConfiguration(GameObjectsTracker tr)
    {
        properties = new List<MyListString>();
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
                MyListString currentProperty = new MyListString();
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
                        advancedConf.Add(null);
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
                MyListString currentProperty = new MyListString();
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
                                advancedConf.Add(null);
                            }
                            else
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
        ASPRep();
        //MyDebugger.MyDebug("success");
    }

    internal virtual void ASPRep()
    {
        aspTemplate = new Dictionary<int,List<string>>();
        for(int i=0; i<properties.Count;i++)
        {
            aspTemplate.Add(i, new List<string>());
            MyListString property = properties[i];
            string currentASPRep = "";
            string pathInASPFormat = "";
            string suffix = "";
            for(int j=0; i<property.Count;j++)
            {
                pathInASPFormat += ASPMapperHelper.aspFormat(property[j]) + "(";
                suffix += ")";
            }

            string configurationNameAspFormat = ASPMapperHelper.aspFormat(configurationName);
            ////MyDebugger.MyDebug("goname " + s.gOName);
            string goNameNotCapital = ASPMapperHelper.aspFormat(gameObject.name);
            currentASPRep = configurationNameAspFormat + "(" + goNameNotCapital + ", objectIndex(+" + gameObject.GetComponent<IndexTracker>().currentIndex + "),";
            suffix += ")";
            currentASPRep += pathInASPFormat;
            if (advancedConf.Count > i)
            {
                if (advancedConf[i].propertyType.Equals("LIST") || advancedConf[i].propertyType.Equals("ARRAY2"))
                {
                    string indexesPlaceholder = "";
                    string valuePlaceholder = "";
                    if (advancedConf[i].propertyType.Equals("LIST"))
                    {
                        indexesPlaceholder = "{0}";
                        valuePlaceholder = "{1}";
                    }
                    else
                    {
                        indexesPlaceholder = "{0}{1}";
                        valuePlaceholder = "{2}";
                    }
                    for(int j=0; j< advancedConf[i].toSave.Count; j++)
                    {
                        string localASPRep = currentASPRep;
                        localASPRep += indexesPlaceholder+"," + ASPMapperHelper.aspFormat(Type.GetType(advancedConf[i].objType).ToString()) + "("
                            + ASPMapperHelper.aspFormat(advancedConf[i].toSave[j]) + "("+ valuePlaceholder+")";
                        localASPRep = ")" + suffix;
                        aspTemplate[i].Add(localASPRep);
                    }
                }
            }
            else
            {
                currentASPRep += "{0}"+suffix;
                aspTemplate[i].Add(currentASPRep);
            }
        }
    }

    internal string getAspTemplate()
    {
        string toReturn = "";
        for (int i = 0; i < properties.Count; i++)
        {
            if (advancedConf.Count > i && !(advancedConf[i] is null))
            {
                if (advancedConf[i].propertyType.Equals("LIST"))
                {
                    for (int j = 0; j < advancedConf[i].toSave.Count; j++)
                    {
                        toReturn += String.Format(aspTemplate[i][j], "P", "V")+Environment.NewLine;
                    }
                }
                else if (advancedConf[i].propertyType.Equals("ARRAY2"))
                {
                    for (int j = 0; j < advancedConf[i].toSave.Count; j++)
                    {
                        toReturn += String.Format(aspTemplate[i][j], "R", "C", "V")+Environment.NewLine;
                    }
                }
            }
            else
            {
                toReturn += String.Format(aspTemplate[i][0], "V")+ Environment.NewLine;
            }
        }
        return toReturn;
    }

    protected void recursevelyAdd(object obj, FieldOrProperty fieldOrProperty, MyListString currentPropertyHierarchy)
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
                            advancedConf.Add(null);
                        }
                        else
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
    private void OnDestroy()
    {
        manager.deleteConfiguration(this);
    }
    public override bool Equals(object other)
    {
        return GetType().Equals(other.GetType()) && configurationName.Equals(((AbstractConfiguration)other).configurationName);
    }

    internal virtual void specificConfiguration(FieldOrProperty fieldOrProperty, MyListString property) { }
    internal virtual void cleanSpecificDataStructure() { }
}
    

