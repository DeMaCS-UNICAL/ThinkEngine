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
public class AbstractConfiguration : MonoBehaviour
{
    private int hashCode;
    [SerializeField]
    private string previousGOName = "";

    [SerializeField]
    internal bool saved;
    [SerializeField]
    internal List<MyListString> properties;
    [SerializeField]
    internal List<string> propertiesNames;
    [SerializeField]
    internal string configurationName;
    [SerializeField]
    internal List<SimpleGameObjectsTracker> advancedConf;
    internal Dictionary<MyListString, List<string>> aspTemplate; //the int is the position of the property in "properties"

    protected bool playingMode;

    #region Unity Messages
    protected void Reset()
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
        if (saved && previousGOName == null)
        {
            throw new Exception("Previous GameObject name undefined for " + configurationName + " in " + gameObject.name);
        }
        if (saved && !previousGOName.Equals(gameObject.name))
        {
            previousGOName = gameObject.name;
            ASPRepresentation();
        }
    }

    #endregion

    #region Saving phase
    internal void SaveConfiguration(GameObjectsTracker tracker)
    {
        if (tracker == null)
        {
            throw new Exception("GameObjectTracker is null while saving the configuration.");
        }
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
        if(gOProperties==null)
        {
            throw new Exception("No direct properties provided for the GameObject while saving the configuration.");
        }
        foreach (string directProperty in gOProperties.Keys)//foreach name of the properties
        {
            if(tracker.ObjectsToggled==null)
            {
                throw new Exception("No objects toggled provided while saving the configuration.");
            }
            if (!tracker.ObjectsToggled.ContainsKey(gOProperties[directProperty]))
            {
                throw new Exception(directProperty+" is not in the toggled object while saving the configuration.");
            }
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
        if (gameObjectComponents == null)
        {
            throw new Exception("There aren't components for the GameObject " + gameObject.name);
        }
        foreach (Component component in gameObjectComponents)//foreach component of the GameObject
        {
            if (!tracker.ObjectsToggled.ContainsKey(component))
            {
                throw new Exception(component.GetType() + " is not in the toggled object while saving the configuration.");
            }
            if (tracker.ObjectsToggled[component])//if the component is toggled in the inspector
            {
                #region Exceptions
                if (tracker.ObjectsProperties == null)
                {
                    throw new Exception("Objects' properties undefined.");
                }
                if (!tracker.ObjectsProperties.ContainsKey(component))
                {
                    throw new Exception("Objects' properties does not contain "+component.GetType());
                }
                #endregion
                MyListString currentProperty = new MyListString();//Creating the root of some basic property hierarchy (possibly the property itself)
                currentProperty.Add(component.GetType().ToString());
                Dictionary<string, FieldOrProperty> componentProperties = tracker.ObjectsProperties[component];//Retrieving the properties of the component
                if (componentProperties == null)
                {
                    throw new Exception("There aren't properties for the component " + component.GetType() + " while saving the configuration.");
                }
                foreach (string componentProperty in componentProperties.Keys)
                {
                    if (!tracker.ObjectsToggled.ContainsKey(componentProperties[componentProperty]))
                    {
                        throw new Exception(componentProperty + " is not present in the objects toggled while saving the configuration.");
                    }
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
            if (tracker.basicTypeCollectionsConfigurations == null || !tracker.basicTypeCollectionsConfigurations.ContainsKey(objectProperties[propertyName]))
            {
                throw new Exception("No advanced configuration provided for property " + currentPropertyHierarchy+"^"+propertyName);
            }
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
        #region Exceptions
        if (tracker.ObjectDerivedFromFields == null)
        {
            throw new Exception("No objects are present in the tracker.");
        }
        if (!tracker.ObjectDerivedFromFields.ContainsKey(currentObject))
        {
            throw new Exception("The object is not present in the tracker.");
        }
        if (!tracker.ObjectDerivedFromFields[currentObject].ContainsKey(property.Name()))
        {
            throw new Exception("No istantiation available for property "+property.Name());
        }
        if (tracker.ObjectsOwners == null)
        {
            throw new Exception("No objects owner defined.");
        }
        #endregion
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
        if (configurationName == null)
        {
            configurationName = "";
        }
        hashCode = configurationName.GetHashCode() * 7 + gameObject.GetHashCode() * 13;
        if (gameObject.GetComponent<IndexTracker>() is null)
        {
            gameObject.AddComponent<IndexTracker>();
        }
        if (saved)
        {
            ASPRepresentation();
        }
    }
    internal virtual void ASPRepresentation()
    {
        aspTemplate = new Dictionary<MyListString, List<string>>();
        if (properties == null)
        {
            throw new Exception("No properties available for "+configurationName);
        }
        for (int i = 0; i < properties.Count; i++)
        {
            MyListString property = properties[i];
            aspTemplate.Add(property, new List<string>());//add an entry for each property: it's a list of string because if the property is a "collection", then you could have multiple sub-properties
            string currentASPRep;
            string pathInASPFormat = "";
            string suffix = "";
            currentASPRep = StringManipulationToFitASP(property, ref pathInASPFormat, ref suffix);
            if(advancedConf==null || advancedConf.Count <= i)
            {
                throw new Exception("Advanced configurations have some problems.");
            }
            if (advancedConf.Count > i && !(advancedConf[i] == null) && advancedConf[i].toSave.Count > 0)//these conditions are true iff the property is a "collection"
            {
                string propertyType = advancedConf[i].propertyType;
                if (propertyType!=null && (propertyType.Equals("LIST") || propertyType.Equals("ARRAY2")))
                {
                    ASPRepresentationForComplexDataType(advancedConf[i], property, currentASPRep, suffix);
                }
            }
            else
            {
                currentASPRep += "{1}" + suffix;
                aspTemplate[property].Add(currentASPRep);
            }
        }
    }

    private void ASPRepresentationForComplexDataType(SimpleGameObjectsTracker advancedConf, MyListString property, string currentASPRep, string suffix)
    {
        //the template will contain some placeholder that will be substituted with the value of the property and the indexes of the "collection" for the factsFile
        //OR with variable for the templateFile generation
        string indexesPlaceholder;
        string valuePlaceholder;
        if (advancedConf.propertyType.Equals("LIST"))
        {
            indexesPlaceholder = "{1}";
            valuePlaceholder = "{2}";
        }
        else
        {
            indexesPlaceholder = "{1},{2}";
            valuePlaceholder = "{3}";
        }
        for (int j = 0; j < advancedConf.toSave.Count; j++)//add an entry for each sub-property in the template
        {
            string localASPRep = currentASPRep;
            localASPRep += indexesPlaceholder + "," + ASPMapperHelper.aspFormat(Type.GetType(advancedConf.objType).ToString()) + "("
                + ASPMapperHelper.aspFormat(advancedConf.toSave[j]) + "(" + valuePlaceholder + ")";
            localASPRep += ")" + suffix;
            aspTemplate[property].Add(localASPRep);
        }
    }

    private string StringManipulationToFitASP(MyListString property, ref string pathInASPFormat, ref string suffix)
    {
        string currentASPRep;
        for (int j = 0; j < property.Count; j++)
        {
            pathInASPFormat += ASPMapperHelper.aspFormat(property[j]) + "(";//leave only character and letters in the property hierarchy, starts with lowercase
            suffix += ")";
        }
        string configurationNameAspFormat = ASPMapperHelper.aspFormat(configurationName);//leave only character and letters in the configuration name, starts with lowercase
        string goNameNotCapital = ASPMapperHelper.aspFormat(gameObject.name);//leave only character and letters in the GO name, starts with lowercase
        currentASPRep = configurationNameAspFormat + "(" + goNameNotCapital + ",objectIndex({0}),"; //add IndexTracker placeholder 
        suffix += ")";
        currentASPRep += pathInASPFormat;
        return currentASPRep;
    }
    internal virtual string GetAspTemplate()
    {
        string toReturn = "";
        if (aspTemplate == null)//if the ASP representation is not available compute it
        {
            ASPRepresentation();
        }
        for (int i = 0; i < properties.Count; i++)//foreach property substitute the placeholder with variables
        {
            if(advancedConf == null || advancedConf.Count <= i)
            {
                throw new Exception("Advanced configurations have some problems.");
            }
            if ( advancedConf[i] != null && advancedConf[i].toSave.Count > 0)
            {
                if (advancedConf[i].propertyType.Equals("LIST"))
                {
                    for (int j = 0; j < advancedConf[i].toSave.Count; j++)
                    {
                        //X is the index of IndexTracker, P the position in the List, V the actual value of the property
                        toReturn += String.Format(aspTemplate[properties[i]][j], "X", "P", "V") + Environment.NewLine;
                    }
                }
                else if (advancedConf[i].propertyType.Equals("ARRAY2"))
                {
                    for (int j = 0; j < advancedConf[i].toSave.Count; j++)
                    {
                        //X is the index of IndexTracker, R the row index in the matrix, C the column one, V the actual value of the property
                        toReturn += String.Format(aspTemplate[properties[i]][j], "X", "R", "C", "V") + Environment.NewLine;
                    }
                }
            }
            else
            {
                //X is the index of IndexTracker, V the actual value of the property
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
        hashCode = configurationName.GetHashCode() * 7 + gameObject.GetHashCode() * 13;
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
        //Two configurations are equal iff they are of the same type (sensor/actuator), have the same name, are saved and have the same hashcode
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


