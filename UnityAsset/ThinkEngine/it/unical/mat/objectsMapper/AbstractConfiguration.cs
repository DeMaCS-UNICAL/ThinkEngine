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
internal class AbstractConfiguration :MonoBehaviour
{
    private int hashCode;
    [SerializeField]
    private string previousGOName = "";

    [SerializeField]
    internal bool saved;
    internal List<MyListString> properties;
    internal List<string> propertiesNames;
    internal string configurationName;
    internal List<SimpleGameObjectsTracker> advancedConf;
    internal Dictionary<MyListString, List<string>> aspTemplate; //the int is the position of the property in "properties"

    protected bool playingMode;

    #region Unity Messages

    protected void OnEnable()
    {
        InitialConfiguration();
        previousGOName = gameObject.name;
    }
    void Start()
    {
        playingMode = Application.isPlaying;
    }
    private void OnDestroy()
    {
        if (!Utility.managersDestroyed)
        {
            DeleteConfiguration();
        }
    }
    void Update()
    {
        if (saved && !previousGOName.Equals(gameObject.name))
        {
            previousGOName = gameObject.name;
            //Debug.Log("previous " + previousGOName+" current "+gameObject.name);
            ASPRepresentation();
        }
    }

    #endregion

    #region Saving phase
    internal void SaveConfiguration(GameObjectsTracker tracker)
    {
        //Reallocating lists and dictionaries
        CleanDataStructures();
        //Recovering properties and components of the current GameObject (the one to which the configuration is attached and on which the tracker is configured)
        Dictionary<string, FieldOrProperty> gOProperties = tracker.ObjectsProperties[gameObject];
        List<Component> gameObjectComponents = tracker.GOComponents[gameObject];
        SaveDirectProperties(tracker, gOProperties);
        SaveComponents(tracker, gameObjectComponents);
        if (properties.Count == 0)//if no properties have been selected in the inspector
        {
            throw new Exception("No properties selected, invalid configuration to save.");
        }
        ASPRepresentation();//create the ASP representation
        saved = true;
        configurationName = tracker.configurationName;//update of the configurationName
        ConfigurationSaved(tracker);//specific operations of the Sensor/Actuator implementation of the class
        hashCode = configurationName.GetHashCode() * 7 + gameObject.GetHashCode() * 13;
    }
    private void CleanDataStructures()
    {
        properties = new List<MyListString>();
        propertiesNames = new List<string>();
        advancedConf = new List<SimpleGameObjectsTracker>();
        CleanSpecificDataStructure();
    }
    private void SaveDirectProperties(GameObjectsTracker tracker, Dictionary<string, FieldOrProperty> gOProperties)
    {
        foreach (string directProperty in gOProperties.Keys)//foreach name of the properties
        {
            if (tracker.ObjectsToggled[gOProperties[directProperty]])//if the property is toggled in the inspector
            {
                MyListString currentProperty = new MyListString();//Creating the root of some basic property hierarchy (possibly the property itself)
                currentProperty.Add(directProperty);
                if (tracker.IsMappable(gOProperties[directProperty]))//If the current property is a mappable one (basic type, list of object, matrix of object)
                {
                    AddMappableProperty(tracker, gOProperties, directProperty, currentProperty);
                }
                else if (tracker.ObjectDerivedFromFields.ContainsKey(gameObject))
                {
                    ExpandNonMappableProperty(gameObject, gOProperties[directProperty], currentProperty, tracker);
                }
            }
        }
    }
    private void SaveComponents(GameObjectsTracker tracker, List<Component> gameObjectComponents)
    {
        foreach (Component component in gameObjectComponents)//foreach component of the GameObject
        {
            if (tracker.ObjectsToggled[component])//if the component is toggled in the inspector
            {
                MyListString currentProperty = new MyListString();//Creating the root of some basic property hierarchy (possibly the property itself)
                currentProperty.Add(component.GetType().ToString());
                Dictionary<string, FieldOrProperty> componentProperties = tracker.ObjectsProperties[component];//Retrieving the properties of the component
                foreach (string componentProperty in componentProperties.Keys)
                {
                    if (tracker.ObjectsToggled[componentProperties[componentProperty]])
                    {
                        MyListString propertyHierarchy = currentProperty.GetClone();
                        propertyHierarchy.Add(componentProperty);
                        if (tracker.IsMappable(componentProperties[componentProperty]))
                        {
                            AddMappableProperty(tracker, componentProperties, componentProperty, propertyHierarchy);
                        }
                        else if (tracker.ObjectDerivedFromFields.ContainsKey(component))
                        {
                            ExpandNonMappableProperty(component, componentProperties[componentProperty], propertyHierarchy, tracker);
                        }
                    }
                }
            }
        }
    }
    private void AddMappableProperty(GameObjectsTracker tracker, Dictionary<string, FieldOrProperty> objectProperties, string propertyName, MyListString currentPropertyHierarchy)
    {
        properties.Add(currentPropertyHierarchy);//the hierarchy of the property is complete thus it is added to the monitored properties
        if (!tracker.IsBaseType(objectProperties[propertyName]))//if it is a list or matrix
        {
            //retrieve and configure the GameObjectSimpleTracker for the elements of the "collection"
            tracker.basicTypeCollectionsConfigurations[objectProperties[propertyName]].propertyName = currentPropertyHierarchy;
            advancedConf.Add(tracker.basicTypeCollectionsConfigurations[objectProperties[propertyName]]);
        }
        else
        {
            SpecificConfiguration(objectProperties[propertyName], currentPropertyHierarchy, tracker);//save the aggregate function chosen for the property
            advancedConf.Add(null);//note! It actually adds an empty GameObjectSimpleTracker
        }
    }
    protected void ExpandNonMappableProperty(object currentObject, FieldOrProperty property, MyListString currentPropertyHierarchy, GameObjectsTracker tracker)
    {
        object derivedObject = tracker.ObjectDerivedFromFields[currentObject][property.Name()];//actual value of the property
        bool derivedObjectAlreadyListed = tracker.ObjectsOwners.ContainsKey(derivedObject) && !tracker.ObjectsOwners[derivedObject].Key.Equals(currentObject);
        derivedObjectAlreadyListed = derivedObjectAlreadyListed && !tracker.ObjectsOwners[derivedObject].Value.Equals(property.Name());
        bool derivedObjectIsTheCurrentGO = derivedObject.Equals(tracker.GO);
        //if the actual value of the property is not null and it is not already listed beacause is a property of some othe object or beacause is the gameobject itself
        if (derivedObject != null && !derivedObjectAlreadyListed && !derivedObjectIsTheCurrentGO)
        {
            if (tracker.ObjectsProperties.ContainsKey(derivedObject))
            {
                Dictionary<string, FieldOrProperty> derivedObjProperties = tracker.ObjectsProperties[derivedObject];//retrieve properties of the derivedObject
                foreach (string subProperty in derivedObjProperties.Keys)
                {
                    if (tracker.ObjectsToggled[derivedObjProperties[subProperty]])
                    {
                        MyListString newLevelPropertyHierarchy = currentPropertyHierarchy.GetClone();
                        newLevelPropertyHierarchy.Add(subProperty);
                        if (tracker.IsMappable(derivedObjProperties[subProperty]))
                        {
                            AddMappableProperty(tracker, derivedObjProperties, subProperty, newLevelPropertyHierarchy);
                        }

                        else if (tracker.ObjectDerivedFromFields.ContainsKey(derivedObject))
                        {
                            ExpandNonMappableProperty(derivedObject, derivedObjProperties[subProperty], newLevelPropertyHierarchy, tracker);//recurse
                        }
                    }
                }
            }
        }
    }
    protected virtual void ConfigurationSaved(GameObjectsTracker tracker) { }
    internal virtual void SpecificConfiguration(FieldOrProperty fieldOrProperty, MyListString property, GameObjectsTracker tracker) { }
    #endregion
    protected void InitialConfiguration()
    {
        if (configurationName != null)
        {
            hashCode = configurationName.GetHashCode() * 7 + gameObject.GetHashCode() * 13;
        }
        else
        {
            configurationName = "";
        }
        if (gameObject.GetComponent<IndexTracker>() is null)
        {
            IndexTracker indexTracker = gameObject.AddComponent<IndexTracker>();
        }
        if (saved)
        {
            ASPRepresentation();
        }
    }
    internal virtual void ASPRepresentation()
    {
        aspTemplate = new Dictionary<MyListString, List<string>>();
        for (int i = 0; i < properties.Count; i++)
        {
            MyListString property = properties[i];
            aspTemplate.Add(property, new List<string>());
            string currentASPRep = "";
            string pathInASPFormat = "";
            string suffix = "";
            for (int j = 0; j < property.Count; j++)
            {
                pathInASPFormat += ASPMapperHelper.aspFormat(property[j]) + "(";
                suffix += ")";
            }

            string configurationNameAspFormat = ASPMapperHelper.aspFormat(configurationName);
            string goNameNotCapital = ASPMapperHelper.aspFormat(gameObject.name);
            currentASPRep = configurationNameAspFormat + "(" + goNameNotCapital + ",objectIndex({0}),";
            suffix += ")";
            currentASPRep += pathInASPFormat;
            if (advancedConf.Count > i && !(advancedConf[i] == null) && advancedConf[i].toSave.Count > 0)
            {
                string propertyType = advancedConf[i].propertyType;
                if (!(propertyType is null) && (propertyType.Equals("LIST") || propertyType.Equals("ARRAY2")))
                {
                    string indexesPlaceholder = "";
                    string valuePlaceholder = "";
                    if (advancedConf[i].propertyType.Equals("LIST"))
                    {
                        indexesPlaceholder = "{1}";
                        valuePlaceholder = "{2}";
                    }
                    else
                    {
                        indexesPlaceholder = "{1},{2}";
                        valuePlaceholder = "{3}";
                    }
                    for (int j = 0; j < advancedConf[i].toSave.Count; j++)
                    {
                        string localASPRep = currentASPRep;
                        localASPRep += indexesPlaceholder + "," + ASPMapperHelper.aspFormat(Type.GetType(advancedConf[i].objType).ToString()) + "("
                            + ASPMapperHelper.aspFormat(advancedConf[i].toSave[j]) + "(" + valuePlaceholder + ")";
                        localASPRep += ")" + suffix;
                        aspTemplate[property].Add(localASPRep);
                    }
                }
            }
            else
            {
                currentASPRep += "{1}" + suffix;
                aspTemplate[property].Add(currentASPRep);
            }
        }
    }
    internal virtual string GetAspTemplate()
    {
        string toReturn = "";
        if (aspTemplate == null)
        {
            ASPRepresentation();
        }
        for (int i = 0; i < properties.Count; i++)
        {
            if (advancedConf != null && advancedConf.Count > i && !(advancedConf[i] is null) && advancedConf[i].toSave.Count > 0)
            {
                if (advancedConf[i].propertyType.Equals("LIST"))
                {
                    for (int j = 0; j < advancedConf[i].toSave.Count; j++)
                    {
                        toReturn += String.Format(aspTemplate[properties[i]][j], "X", "P", "V") + Environment.NewLine;
                    }
                }
                else if (advancedConf[i].propertyType.Equals("ARRAY2"))
                {
                    for (int j = 0; j < advancedConf[i].toSave.Count; j++)
                    {
                        toReturn += String.Format(aspTemplate[properties[i]][j], "X", "R", "C", "V") + Environment.NewLine;
                    }
                }
            }
            else
            {
                toReturn += String.Format(aspTemplate[properties[i]][0], "X", "V") + Environment.NewLine;
            }
        }
        return toReturn;
    }
    internal virtual void CleanSpecificDataStructure() { }
    internal virtual void DeleteConfiguration() { }

    internal void Clean()
    {
        DeleteConfiguration();
        saved = false;
        properties = new List<MyListString>();
        propertiesNames = new List<string>();
        configurationName = "";
        hashCode = -1;
        advancedConf = new List<SimpleGameObjectsTracker>();
        aspTemplate = new Dictionary<MyListString, List<string>>();
    }
    internal List<string> GetTemplate(MyListString searchedProperty)
    {
        return aspTemplate[searchedProperty];
    }
    #region General overrides
    public override bool Equals(object other)
    {
        if (!(other is AbstractConfiguration))
        {
            return false;
        }
        AbstractConfiguration otherConf = (AbstractConfiguration)other;
        return GetType().Equals(other.GetType()) && hashCode == otherConf.hashCode;
    }
    public override int GetHashCode()
    {
        return hashCode;
    }
    #endregion

}


