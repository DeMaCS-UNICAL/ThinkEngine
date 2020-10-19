using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEditor;
using EmbASP4Unity.it.unical.mat.objectsMapper;

[ExecuteInEditMode]
public class AbstractConfiguration :MonoBehaviour
{
    internal IManager manager;
    public List<MyListString> properties;
    public List<string> propertiesNames;
    public string configurationName;
    public List<SimpleGameObjectsTracker> advancedConf;
    internal Dictionary<MyListString, List<string>> aspTemplate; //the int is the position of the property in "properties"
    [SerializeField]
    internal bool saved;

    protected void OnEnable()
    {
        if(configurationName is null)
        {
            configurationName = "";
        }
        if (gameObject.GetComponent<IndexTracker>() is null)
        {
            IndexTracker indexTracker = gameObject.AddComponent<IndexTracker>();
            indexTracker.hideFlags = HideFlags.HideInInspector;
        }
        if (saved)
        {
            manager.addConfiguration(this);
            ASPRep();
        }
    }

    public void SaveConfiguration(GameObjectsTracker tr)
    {
        properties = new List<MyListString>();
        propertiesNames = new List<string>();
        advancedConf = new List<SimpleGameObjectsTracker>();
        cleanSpecificDataStructure();
        configurationName = tr.configurationName;
        GameObjectsTracker tracker = tr;
        Dictionary<string, FieldOrProperty> gOProperties = tr.ObjectsProperties[gameObject];
        List<Component> comp = tr.GOComponents[gameObject];
        foreach (string directProperty in gOProperties.Keys)
        {
                
            if (tr.ObjectsToggled[gOProperties[directProperty]])
            {
                //MyDebugger.MyDebug("property " + s + " toggled");
                MyListString currentProperty = new MyListString();
                properties.Add(currentProperty);
                if (tracker.IsMappable(gOProperties[directProperty]))
                {
                    //MyDebugger.MyDebug("adding " + gOProperties[s].Name());
                    currentProperty.Add(directProperty);
                    if (!tracker.IsBaseType(gOProperties[directProperty]))
                    {
                        tracker.basicTypeCollectionsConfigurations[gOProperties[directProperty]].propertyName = currentProperty;
                        advancedConf.Add(tracker.basicTypeCollectionsConfigurations[gOProperties[directProperty]]);
                    }
                    else
                    {
                        specificConfiguration(gOProperties[directProperty], currentProperty, tracker);
                        advancedConf.Add(null);
                    }
                }
                else if (tracker.ObjectDerivedFromFields.ContainsKey(gameObject))
                {
                    //MyDebugger.MyDebug("recursing on " + gOProperties[s].Name());
                    recursevelyAdd(gameObject, gOProperties[directProperty], currentProperty, tracker);
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
                                specificConfiguration(componentProperties[s], currentProperty, tracker);
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
                            recursevelyAdd(c, componentProperties[s], currentProperty, tracker);
                        }

                    }
                        
                }
            }
        }
        if (properties.Count == 0)
        {
            throw new Exception("No properties selected, invalid configuration to save.");
        }
        ASPRep();
        saved = true;
        manager.addConfiguration(this);
        //MyDebugger.MyDebug("success");
        MyDebugger.MyDebug("saved " + configurationName);
        foreach(MyListString property in properties)
        {
            MyDebugger.MyDebug("changing prop");
            for(int i=0; i < property.Count; i++)
            {
                MyDebugger.MyDebug(property[i]);
            }
        }
    }

    internal virtual void ASPRep()
    {
        aspTemplate = new Dictionary<MyListString, List<string>>();
        for(int i=0; i<properties.Count;i++)
        {
            aspTemplate.Add(properties[i], new List<string>());
            MyListString property = properties[i];
            string currentASPRep = "";
            string pathInASPFormat = "";
            string suffix = "";
            for(int j=0; j<property.Count;j++)
            {
                pathInASPFormat += ASPMapperHelper.aspFormat(property[j]) + "(";
                suffix += ")";
            }

            string configurationNameAspFormat = ASPMapperHelper.aspFormat(configurationName);
            string goNameNotCapital = ASPMapperHelper.aspFormat(gameObject.name);
            currentASPRep = configurationNameAspFormat + "(" + goNameNotCapital + ", objectIndex(+" + gameObject.GetComponent<IndexTracker>().currentIndex + "),";
            suffix += ")";
            currentASPRep += pathInASPFormat;
            if (advancedConf.Count > i && !(advancedConf[i] is null))
            {
                string propertyType = advancedConf[i].propertyType;
                if (!(propertyType is null) && (propertyType.Equals("LIST") || propertyType.Equals("ARRAY2")))
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
                        aspTemplate[properties[i]].Add(localASPRep);
                    }
                }
            }
            else
            {
                currentASPRep += "{0}"+suffix;
                aspTemplate[properties[i]].Add(currentASPRep);
            }
        }
    }

    internal virtual string getAspTemplate()
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
                        toReturn += String.Format(aspTemplate[properties[i]][j], "P", "V")+Environment.NewLine;
                    }
                }
                else if (advancedConf[i].propertyType.Equals("ARRAY2"))
                {
                    for (int j = 0; j < advancedConf[i].toSave.Count; j++)
                    {
                        toReturn += String.Format(aspTemplate[properties[i]][j], "R", "C", "V")+Environment.NewLine;
                    }
                }
            }
            else
            {
                toReturn += String.Format(aspTemplate[properties[i]][0], "V")+ Environment.NewLine;
            }
        }
        return toReturn;
    }

    protected void recursevelyAdd(object obj, FieldOrProperty fieldOrProperty, MyListString currentPropertyHierarchy, GameObjectsTracker tracker)
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
                            specificConfiguration(derivedObjProperties[s], currentPropertyHierarchy, tracker);
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

                        recursevelyAdd(derivedObj, derivedObjProperties[s], currentPropertyHierarchy, tracker);

                    }
                }
            }
        }
    }

    internal List<string> getTemplate(MyListString searchedProperty)
    {
        return aspTemplate[searchedProperty];
    }

    private void OnDestroy()
    {
        if (saved)
        {
            manager.deleteConfiguration(this);
        }
    }
    public override bool Equals(object other)
    {
        return GetType().Equals(other.GetType()) && configurationName.Equals(((AbstractConfiguration)other).configurationName);
    }
    public override int GetHashCode()
    {
        return configurationName.GetHashCode();
    }
    internal virtual void specificConfiguration(FieldOrProperty fieldOrProperty, MyListString property, GameObjectsTracker tracker) { }
    internal virtual void cleanSpecificDataStructure() { }
}
    

