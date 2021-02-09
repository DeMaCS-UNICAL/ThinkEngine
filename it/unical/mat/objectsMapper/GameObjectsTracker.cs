using System;
using System.Collections.Generic;
using UnityEngine;
//this class manage the logic behind the inspector graphic of a configuration
public class GameObjectsTracker
{
    public GameObject gameObject; //the root gameObject being tracked (the one to which a configuration is attached
    public Dictionary<object, bool> objectsToggled; //it saves the information of being toggled in the inspector for all the fields/properties/component (NOT A GENERIC OBJECT) of the gameObject hierarchy
    public Dictionary<object, KeyValuePair<object, string>> objectsOwners;//for each field/property istantiation stores the first parent property object
    public Dictionary<object, Dictionary<string, object>> objectDerivedFromFields;//for each object instantiation of the hierarchy stores the actual istantiation of its fileds/properties
    public Dictionary<object, Dictionary<string, FieldOrProperty>> objectsProperties;//for each object instantiation of the hierarchy stores the FieldOrProperty corresposnding to te respective field/property
    public Dictionary<GameObject, List<Component>> gameObjectsComponents;//for each gameObject in the hierarchy of the root stores a list of its components
    public Dictionary<object, string> propertiesName;//Feature not completed: this should store a chosen name for a specific property in the hierarchy
    public string configurationName = "";//name of the configuration associated to the tracker
    private Dictionary<FieldOrProperty, SimpleGameObjectsTracker> basicTypeCollectionsConfigurations; //configuration for T type for T[],T[,], List<T> etc.
    public Dictionary<FieldOrProperty, int> operationPerProperty;//for each property chosen, it stores which aggregate to use while generating the input facts for the ASP solver
    public Dictionary<FieldOrProperty, string> specificValuePerProperty;//a specialization of the above one (you want to check if a specific value is present in the data series collected)

    internal SimpleGameObjectsTracker GetSimpleTracker(FieldOrProperty key)
    {
        if (!basicTypeCollectionsConfigurations.ContainsKey(key))
        {
            basicTypeCollectionsConfigurations.Add(key, new SimpleGameObjectsTracker(key.Type()));
        }
        return basicTypeCollectionsConfigurations[key];
    }
    public List<Component> GetComponents(GameObject gO)
    {
        return ReflectionExecutor.GetComponents(gO);
    }
    public bool IsBaseType(FieldOrProperty obj)
    {
        return ReflectionExecutor.IsBaseType(obj);
    }
    public List<FieldOrProperty> GetFieldsAndProperties(object currentObject)
    {
        return ReflectionExecutor.GetFieldsAndProperties(currentObject);
    }
    internal void UpdateDataStructures(AbstractConfiguration configuration)
    {
        #region Exceptions
        if (configuration == null)
        {
            throw new Exception("Cannot update data structure for a null configuration.");
        }
        if (configuration.configurationName == null || configuration.configurationName.Equals(""))
        {
            throw new Exception("A configuration name cannot be null or empty.");
        }
        #endregion
        gameObject = configuration.gameObject;
        configurationName = configuration.configurationName;
        CleanDataStructures();
        objectsProperties.Add(gameObject, new Dictionary<string, FieldOrProperty>());
        objectDerivedFromFields.Add(gameObject, new Dictionary<string, object>());
        List<FieldOrProperty> fieldsAndProperties = GetFieldsAndProperties(gameObject);//retrieve all the field and properties of the parentObject
        foreach (FieldOrProperty currentFieldOrProperty in fieldsAndProperties)
        {
            MyListString currentPropertyHierarchy = new MyListString();
            UpdateDirectProperty(configuration, gameObject,currentFieldOrProperty, currentPropertyHierarchy);
        }
        gameObjectsComponents.Add(gameObject, GetComponents(gameObject));
        foreach (Component component in gameObjectsComponents[gameObject])
        {
            MyListString currentPropertyHierarchy = new MyListString();
            UpdateComponent(configuration, component, currentPropertyHierarchy);
        }
    }
    internal void UpdateDataStructures(object parentObject, AbstractConfiguration configuration, MyListString currentPropertyHierarchy)
    {
        objectsProperties.Add(parentObject, new Dictionary<string, FieldOrProperty>());
        objectDerivedFromFields.Add(parentObject, new Dictionary<string, object>());
        List<FieldOrProperty> fieldsAndProperties = GetFieldsAndProperties(parentObject);
        foreach (FieldOrProperty currentProperty in fieldsAndProperties)
        {
            MyListString newLayerPropertyHierarchy = currentPropertyHierarchy.GetClone();
            UpdateDirectProperty(configuration, parentObject,currentProperty, newLayerPropertyHierarchy);
        }
        if (parentObject.GetType() == typeof(GameObject))
        {
            gameObjectsComponents[(GameObject)parentObject] = GetComponents((GameObject)parentObject);
            foreach (Component component in gameObjectsComponents[(GameObject)parentObject])
            {
                MyListString newLayerPropertyHierarchy = currentPropertyHierarchy.GetClone();
                UpdateComponent(configuration, component, newLayerPropertyHierarchy);
            }
        }
    }
    internal void UpdateValueTypeDataStructures(FieldOrProperty parentProperty, AbstractConfiguration configuration, MyListString currentPropertyHierarchy)
    {
        objectsProperties.Add(parentProperty, new Dictionary<string, FieldOrProperty>());
        objectDerivedFromFields.Add(parentProperty, new Dictionary<string, object>());
        List<FieldOrProperty> fieldsAndProperties = GetFieldsAndProperties(parentProperty.Type());
        foreach (FieldOrProperty currentSubProperty in fieldsAndProperties)
        {
            MyListString newLayerPropertyHierarchy = currentPropertyHierarchy.GetClone();
            UpdateValueTypeDirectProperty(configuration, parentProperty, currentSubProperty, newLayerPropertyHierarchy);
        }
    }
    private void UpdateValueTypeDirectProperty(AbstractConfiguration configuration, FieldOrProperty parentProperty, FieldOrProperty currentProperty, MyListString currentPropertyHierarchy)
    {
        if (!objectsProperties[parentProperty].ContainsKey(currentProperty.Name()))
        {
            objectsProperties[parentProperty].Add(currentProperty.Name(), currentProperty);
        }
        if (!objectDerivedFromFields[parentProperty].ContainsKey(currentProperty.Name()))
        {
            objectDerivedFromFields[parentProperty].Add(currentProperty.Name(), currentProperty);
        }
        if (!objectsToggled.ContainsKey(currentProperty))
        {
            if (configuration != null && CheckIfPropertyIsToToggle(configuration.properties, currentPropertyHierarchy, currentProperty.Name()))
            {
                objectsToggled.Add(currentProperty, true);
                currentPropertyHierarchy.Add(currentProperty.Name());
                if (!IsMappable(currentProperty))
                {
                    UpdateValueTypeDataStructures(currentProperty, configuration, currentPropertyHierarchy);
                }
                else
                {
                    ConfigureMappableProperty(configuration, currentProperty, currentPropertyHierarchy);
                }
            }
            else
            {
                objectsToggled.Add(currentProperty, false);
            }
        }
    }

    private void UpdateDirectProperty(AbstractConfiguration configuration, object parentObject, FieldOrProperty currentFieldOrProperty, MyListString currentPropertyHierarchy)
    {
        object parentObjectValueForCurrentProperty = PropertyValueAndOwner(currentFieldOrProperty,parentObject);
        if (!objectDerivedFromFields[parentObject].ContainsKey(currentFieldOrProperty.Name()))
        {
            objectDerivedFromFields[parentObject].Add(currentFieldOrProperty.Name(), parentObjectValueForCurrentProperty);
        }
        if (!objectsToggled.ContainsKey(currentFieldOrProperty))
        {
            if (configuration != null && CheckIfPropertyIsToToggle(configuration.properties, currentPropertyHierarchy, currentFieldOrProperty.Name()))
            {
                objectsToggled.Add(currentFieldOrProperty, true);
                currentPropertyHierarchy.Add(currentFieldOrProperty.Name());
                if (!IsMappable(currentFieldOrProperty) && parentObjectValueForCurrentProperty != null)
                {
                    UpdateDataStructures(parentObjectValueForCurrentProperty, configuration, currentPropertyHierarchy);
                }
                else
                {
                    ConfigureMappableProperty(configuration, currentFieldOrProperty, currentPropertyHierarchy);
                }
            }
            else
            {
                objectsToggled.Add(currentFieldOrProperty, false);
            }
        }
    }
    private void UpdateComponent(AbstractConfiguration configuration, Component component, MyListString currentPropertyHierarchy)
    {
        if (configuration != null && CheckIfPropertyIsToToggle(configuration.properties, currentPropertyHierarchy, component.GetType().ToString()))
        {
            objectsToggled.Add(component, true);
            currentPropertyHierarchy.Add(component.GetType().ToString());
            UpdateDataStructures(component, configuration, currentPropertyHierarchy);
        }
        else
        {
            objectsToggled.Add(component, false);
        }
    }
    private void ConfigureMappableProperty(AbstractConfiguration configuration, FieldOrProperty currentFieldOrProperty, MyListString currentPropertyHierarchy)
    {
        if (IsMappable(currentFieldOrProperty) && !IsBaseType(currentFieldOrProperty))
        {
            foreach (SimpleGameObjectsTracker currentSimpleTracker in configuration.advancedConf)
            {
                if (currentSimpleTracker.propertyName.Equals(currentPropertyHierarchy))
                {
                    basicTypeCollectionsConfigurations.Add(currentFieldOrProperty, currentSimpleTracker);
                    break;
                }
            }
        }
        if (configuration.GetType() == typeof(SensorConfiguration))
        {
            foreach (ListOfMyListStringIntPair currentOperationPerProperty in ((SensorConfiguration)configuration).operationPerProperty)
            {
                if (currentOperationPerProperty.Key[0].Equals(currentPropertyHierarchy))
                {
                    operationPerProperty.Add(currentFieldOrProperty, currentOperationPerProperty.Value);
                    if (currentOperationPerProperty.Value == Operation.SPECIFIC)
                    {
                        foreach (ListOfMyListStringStringPair currentSpecificValue in ((SensorConfiguration)configuration).specificValuePerProperty)
                        {
                            if (currentSpecificValue.Key[0].Equals(currentPropertyHierarchy))
                            {
                                specificValuePerProperty.Add(currentFieldOrProperty, currentSpecificValue.Value);
                                break;
                            }
                        }
                    }
                    break;
                }
            }
        }
    }

    internal void SetSimpleTrackerName(FieldOrProperty fieldOrProperty, MyListString currentPropertyHierarchy)
    {
        basicTypeCollectionsConfigurations[fieldOrProperty].propertyName = currentPropertyHierarchy;
    }

    internal bool ExistsSimpleTrackerFor(FieldOrProperty fieldOrProperty)
    {
        return basicTypeCollectionsConfigurations != null && basicTypeCollectionsConfigurations.ContainsKey(fieldOrProperty);
    }

    internal bool HasBasicGenericArgument(FieldOrProperty fieldOrProperty)
    {
        return ReflectionExecutor.HasBasicGenericArgument(fieldOrProperty);
    }

    private object PropertyValueAndOwner(FieldOrProperty currentFieldOrProperty, object parentObject)
    {
        object parentObjectValueForCurrentProperty;
        try
        {
            parentObjectValueForCurrentProperty = currentFieldOrProperty.GetValue(parentObject);//if the field/property is not istatiated, this throws an exception
        }
        catch
        {
            parentObjectValueForCurrentProperty = null;
        }
        if (!objectsProperties[parentObject].ContainsKey(currentFieldOrProperty.Name())){
            objectsProperties[parentObject].Add(currentFieldOrProperty.Name(), currentFieldOrProperty);
        }
        if (parentObjectValueForCurrentProperty != null) //if it is not a value type keep only the first parent that has been met.
        {
            if (!currentFieldOrProperty.Type().IsValueType)
            {
                if (!objectsOwners.ContainsKey(parentObjectValueForCurrentProperty))
                {
                    objectsOwners.Add(parentObjectValueForCurrentProperty, new KeyValuePair<object, string>(parentObject, currentFieldOrProperty.Name()));
                }
            }
            /*else
            {
                objectsOwners.Add(parentObjectValueForCurrentProperty, new KeyValuePair<object, string>(parentObject, currentFieldOrProperty.Name()));
            }*/

        }

        return parentObjectValueForCurrentProperty;
    }

    internal void LoadBasicPropertiesSimpleTracker(FieldOrProperty currentGOProperty)
    {
        if (!basicTypeCollectionsConfigurations.ContainsKey(currentGOProperty))
        {
            basicTypeCollectionsConfigurations.Add(currentGOProperty, new SimpleGameObjectsTracker(currentGOProperty.Type()));
        }
        basicTypeCollectionsConfigurations[currentGOProperty].GetBasicProperties();
    }

    private bool CheckIfPropertyIsToToggle(List<MyListString> properties, MyListString currentPropertyHierarchy, string latterLevelProperty)
    {
        if (properties is null)
        {
            return false;
        }
        foreach (MyListString property in properties)
        {
            if (property.Count <= currentPropertyHierarchy.Count)
            {
                continue;
            }
            bool matching = true;
            for(int i=0; i < currentPropertyHierarchy.Count; i++)
            {
                if (!property[i].Equals(currentPropertyHierarchy[i]))
                {
                    matching = false ;
                    break;
                }
            }
            if (matching)
            {
                if (property[currentPropertyHierarchy.Count].Equals(latterLevelProperty))
                {
                    return true;
                }
            }
        }
        return false;
    }


    public bool IsMappable(FieldOrProperty obj)
    {
        return ReflectionExecutor.IsMappable(obj); 
    }
    public void CleanDataStructures()
    {
        objectsProperties = new Dictionary<object, Dictionary<string, FieldOrProperty>>();
        gameObjectsComponents = new Dictionary<GameObject, List<Component>>();
        objectsOwners = new Dictionary<object, KeyValuePair<object,string>>();
        objectsToggled = new Dictionary<object, bool>();
        objectDerivedFromFields = new Dictionary<object, Dictionary<string, object>>();
        propertiesName = new Dictionary<object, string>();
        specificValuePerProperty = new Dictionary<FieldOrProperty, string>();
        operationPerProperty = new Dictionary<FieldOrProperty, int>();
        basicTypeCollectionsConfigurations = new Dictionary<FieldOrProperty, SimpleGameObjectsTracker>();
    }

    internal void RemoveSimpleTracker(FieldOrProperty objectToConfigure)
    {
        if (basicTypeCollectionsConfigurations.ContainsKey(objectToConfigure))
        {
            basicTypeCollectionsConfigurations.Remove(objectToConfigure);
        }
    }

    internal void TogglePropertySimpleTracker(FieldOrProperty objectToConfigure, string currentPropertyName, bool v)
    {
        if (!basicTypeCollectionsConfigurations.ContainsKey(objectToConfigure))
        {
            basicTypeCollectionsConfigurations.Add(objectToConfigure, new SimpleGameObjectsTracker(objectToConfigure.Type()));
        }
        if (!basicTypeCollectionsConfigurations[objectToConfigure].propertiesToggled.ContainsKey(currentPropertyName))
        {
            basicTypeCollectionsConfigurations[objectToConfigure].propertiesToggled.Add(currentPropertyName, false);
        }
        basicTypeCollectionsConfigurations[objectToConfigure].propertiesToggled[currentPropertyName] = v;
    }

    internal bool IsPropertySimpleTrackerToggled(FieldOrProperty objectToConfigure, string currentPropertyName)
    {
        return basicTypeCollectionsConfigurations[objectToConfigure].propertiesToggled[currentPropertyName];
    }

    internal int SimpleTrackerToSaveCount(FieldOrProperty objectToConfigure)
    {
        return basicTypeCollectionsConfigurations[objectToConfigure].toSave.Count;
    }
    internal void SaveSimpleTracker(FieldOrProperty objectToConfigure)
    {
        basicTypeCollectionsConfigurations[objectToConfigure].save();
    }
}
