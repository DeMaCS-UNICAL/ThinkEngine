using System;
using System.Collections.Generic;
using UnityEngine;
using EmbASP4Unity.it.unical.mat.objectsMapper;
using System.Linq;

public class GameObjectsTracker
{

     public GameObject GO { get; set; }
     public Dictionary<object, bool> ObjectsToggled { get; set; }
     public Dictionary<object, KeyValuePair<object,string>> ObjectsOwners { get; set; }
     public Dictionary<object, Dictionary<string, object>> ObjectDerivedFromFields { get; set; }
     public Dictionary<object, Dictionary<string, FieldOrProperty>> ObjectsProperties { get; set; }
     public Dictionary<GameObject, List<Component>> GOComponents { get; set; }
     public Dictionary<object, string> propertiesName { get; set; }
     public string configurationName = "";
     public Dictionary<object, SimpleGameObjectsTracker> basicTypeCollectionsConfigurations; //configuration for T type fot T[],T[,], List<T> etc.

    public Dictionary<FieldOrProperty, int> operationPerProperty { get; set; }
     public Dictionary<FieldOrProperty, string> specificValuePerProperty { get; set; }

   

    

    public List<Component> GetComponents(GameObject gO)
    {
        return ReflectionExecutor.GetComponents(gO);
    }

    public bool IsBaseType(FieldOrProperty obj)
    {
        return ReflectionExecutor.IsBaseType(obj);
    }

    public List<FieldOrProperty> GetFieldsAndProperties(object gO)
    {
        return ReflectionExecutor.GetFieldsAndProperties(gO);
    }

    public AbstractConfiguration saveConfiguration(AbstractConfiguration conf, string chosenGO)
    {        
        conf.SaveConfiguration(this);
        //MyDebugger.MyDebug(conf);
        return conf;
    }

    internal void updateDataStructures(int objectIndex, AbstractConfiguration configuration)
    {
        
        if (objectIndex != -1)
        {
            configurationName = "";
            GO = GetGameObject(objectIndex);
        }
        else
        {
            GO = GetGameObject(configuration.GetComponent<IndexTracker>().currentIndex);
            configurationName = configuration.configurationName;

        }
        cleanDataStructures();
        List<FieldOrProperty> fieldsAndProperties = GetFieldsAndProperties(GO);
        ObjectsProperties.Add(GO, new Dictionary<string, FieldOrProperty>());        
        ObjectDerivedFromFields.Add(GO, new Dictionary<string, object>());
        foreach (FieldOrProperty currentProperty in fieldsAndProperties)
        {
            object gOValueForCurrentProperty;
            try
            {
                gOValueForCurrentProperty = currentProperty.GetValue(GO);
            }
            catch (Exception e)
            {
                gOValueForCurrentProperty = null;
            }
            ObjectsProperties[GO].Add(currentProperty.Name(), currentProperty);
            if (gOValueForCurrentProperty != null && !IsMappable(currentProperty)) 
            {
                if (!ObjectsOwners.ContainsKey(gOValueForCurrentProperty))
                {
                    ObjectsOwners.Add(gOValueForCurrentProperty, new KeyValuePair<object, string>(GO,currentProperty.Name()));
                }

            }

            ObjectDerivedFromFields[GO].Add(currentProperty.Name(), gOValueForCurrentProperty);
            if (!ObjectsToggled.ContainsKey(currentProperty))
            {
                if (configuration!= null && checkIfPropertyIsToToggle(configuration.properties, currentProperty.Name()))
                {
                    //MyDebugger.MyDebug(obj.Name() + " found.");
                    ObjectsToggled.Add(currentProperty, true);
                    if (!IsMappable(currentProperty) && gOValueForCurrentProperty != null)
                    {
                        // MyDebugger.MyDebug("calling update");
                        List<string> currentPropertyHierarchy = new List<string>();
                        currentPropertyHierarchy.Add(currentProperty.Name());
                        updateDataStructures(gOValueForCurrentProperty, configuration, currentPropertyHierarchy);
                    }
                    else
                    {
                        if(IsMappable(currentProperty) && !IsBaseType(currentProperty))
                        {
                            foreach (SimpleGameObjectsTracker st in configuration.advancedConf) {
                                if (st.propertyName.Equals(currentProperty.Name()))
                                {
                                    //st.objType = obj.Type().GetElementType().ToString();
                                    basicTypeCollectionsConfigurations.Add(currentProperty, st);
                                    //MyDebugger.MyDebug("Adding st for " + obj.Name() + " whit type " + st.objType);
                                    break;
                                }
                            }
                        }
                        if (configuration.GetType() == typeof(SensorConfiguration))
                        {
                            foreach (ListOfStringIntPair pair in ((SensorConfiguration)configuration).operationPerProperty)
                            {
                                if (pair.Key.Equals(currentProperty.Name()))
                                {
                                    operationPerProperty.Add(currentProperty, pair.Value);
                                    if (pair.Value == Operation.SPECIFIC)
                                    {
                                        foreach (ListOfStringStringPair pair2 in ((SensorConfiguration)configuration).specificValuePerProperty)
                                        {
                                            if (pair2.Key.Equals(currentProperty.Name()))
                                            {
                                                specificValuePerProperty.Add(currentProperty, pair2.Value);
                                                break;
                                            }
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    ObjectsToggled.Add(currentProperty, false);
                }
            }
        }
        GOComponents.Add(GO, GetComponents(GO));
        foreach (Component component in GOComponents[GO])
        {
            if (configuration != null && checkIfPropertyIsToToggle(configuration.properties, component.GetType().ToString()))
            {
                //MyDebugger.MyDebug("component "+c+" name "+c.name+" type "+c.GetType());
                ObjectsToggled.Add(component, true);
                List<string> currentPropertyHierarchy = new List<string>();
                currentPropertyHierarchy.Add(component.GetType().ToString());
                updateDataStructures(component, configuration, currentPropertyHierarchy);
                
            }
            else
            {
                ObjectsToggled.Add(component, false);
            }
        }
    }

    private bool checkIfPropertyIsToToggle(List<MyListString> properties, List<string> currentPropertyHierarchy, string latterLevelProperty)
    {
        if (properties is null || properties.Count == 0)
        {
            return false;
        }
        foreach (List<string> subList in properties)
        {
            if (subList.Count != currentPropertyHierarchy.Count+1)
            {
                continue;
            }
            bool matching = true;
            for(int i=0; i < currentPropertyHierarchy.Count && matching; i++)
            {
                if (!subList[i].Equals(currentPropertyHierarchy[i]))
                {
                    matching = false ;
                }
            }
            if (matching)
            {
                if (subList[subList.Count - 1].Equals(latterLevelProperty))
                {
                    return true;
                }
            }
        }
        return false;
    }
    private bool checkIfPropertyIsToToggle(List<MyListString> properties, string firstLevelProperty)
    {
        if(properties is null || properties.Count == 0)
        {
            return false;
        }
        foreach(List<string> subList in properties)
        {
            if (subList.Count != 1)
            {
                continue;
            }
            if (subList[0].Equals(firstLevelProperty))
            {
                return true;
            }
        }
        return false;
    }

    private GameObject GetGameObject(int objectIndex)
    {
        foreach(IndexTracker gameObj in GameObject.FindObjectsOfType<IndexTracker>())
        {
            if (gameObj.currentIndex == objectIndex)
            {
                return gameObj.gameObject;
            }
        }
        return null;
    }

    public bool IsMappable(FieldOrProperty obj)
    {
        return ReflectionExecutor.isMappable(obj); 
    }

    public int IsArrayOfRank(FieldOrProperty obj)
    {
        return ReflectionExecutor.isArrayOfRank(obj);
    }

    public void cleanDataStructures()
    {
        ObjectsProperties = new Dictionary<object, Dictionary<string, FieldOrProperty>>();
        GOComponents = new Dictionary<GameObject, List<Component>>();
        ObjectsOwners = new Dictionary<object, KeyValuePair<object,string>>();
        ObjectsToggled = new Dictionary<object, bool>();
        ObjectDerivedFromFields = new Dictionary<object, Dictionary<string, object>>();
        propertiesName = new Dictionary<object, string>();
        specificValuePerProperty = new Dictionary<FieldOrProperty, string>();
        operationPerProperty = new Dictionary<FieldOrProperty, int>();
        basicTypeCollectionsConfigurations = new Dictionary<object, SimpleGameObjectsTracker>();
    }

    internal void updateDataStructures(object parentObject, AbstractConfiguration configuration, List<string> currentPropertyHierarchy)
    {
        //MyDebugger.MyDebug("updating " + parent);
        
        List<FieldOrProperty> fieldsAndProperties = GetFieldsAndProperties(parentObject);
        ObjectsProperties.Add(parentObject, new Dictionary<string, FieldOrProperty>());
        //MyDebugger.MyDebug("Adding derived from fields entry");
        ObjectDerivedFromFields.Add(parentObject, new Dictionary<string, object>());
        
        foreach (FieldOrProperty currentProperty in fieldsAndProperties)
        {
            //MyDebugger.MyDebug("Property " + ob.Name());
            object parentObjectValueForCurrentProperty;
            try
            {
                parentObjectValueForCurrentProperty = currentProperty.GetValue(parentObject);
            }
            catch (Exception e)
            {
                //MyDebugger.MyDebug("cannot get value for property " + ob.Name());
                parentObjectValueForCurrentProperty = null;
            }
            ObjectsProperties[parentObject].Add(currentProperty.Name(), currentProperty);
            //MyDebugger.MyDebug("complete name " + parent + "^" + ob.Name());
            if (parentObjectValueForCurrentProperty != null) 
            {
                if (!ObjectsOwners.ContainsKey(parentObjectValueForCurrentProperty))
                {
                    ObjectsOwners.Add(parentObjectValueForCurrentProperty, new KeyValuePair<object, string>(parentObject, currentProperty.Name()));
                }
                //MyDebugger.MyDebug(ob.Name() + " owner is " + ObjectsOwners[objValueForOb]+"and its value is "+objValueForOb);

            }
            if (configuration!=null && checkIfPropertyIsToToggle(configuration.properties, currentPropertyHierarchy, currentProperty.Name()))
            {
                //MyDebugger.MyDebug("s contains " + parent + "^" + ob.Name());   
                ObjectsToggled.Add(currentProperty, true);
                currentPropertyHierarchy.Add(currentProperty.Name());
                if (!IsMappable(currentProperty) && parentObjectValueForCurrentProperty != null)
                {
                    updateDataStructures(parentObjectValueForCurrentProperty, configuration, currentPropertyHierarchy);
                }
                else
                {
                    if (IsMappable(currentProperty) && !IsBaseType(currentProperty))
                    {
                        foreach (SimpleGameObjectsTracker currentSimpleTracker in configuration.advancedConf)
                        {
                            if (currentSimpleTracker.propertyName.SequenceEqual(currentPropertyHierarchy))
                            {
                                //st.objType = ob.Type().GetElementType().ToString();
                                //MyDebugger.MyDebug("Adding st for " + ob.Name() + " whit type " + st.objType);
                                basicTypeCollectionsConfigurations.Add(currentProperty, currentSimpleTracker);
                                break;
                            }
                        }
                    }
                    if (configuration.GetType() == typeof(SensorConfiguration))
                    {
                        foreach (ListOfStringIntPair currentOperationPerProperty in ((SensorConfiguration)configuration).operationPerProperty)
                        {
                            if (currentOperationPerProperty.Key.SequenceEqual(currentPropertyHierarchy))
                            {
                                operationPerProperty.Add(currentProperty, currentOperationPerProperty.Value);
                                if (currentOperationPerProperty.Value == Operation.SPECIFIC)
                                {
                                    foreach (ListOfStringStringPair currentSpecificValue in ((SensorConfiguration)configuration).specificValuePerProperty)
                                    {
                                        if (currentSpecificValue.Key.SequenceEqual(currentPropertyHierarchy))
                                        {
                                            specificValuePerProperty.Add(currentProperty, currentSpecificValue.Value);
                                            break;
                                        }
                                    }
                                }
                                break;
                            }
                        }

                    }
                }
            }
            else
            {
                ObjectsToggled.Add(currentProperty, false);
            }
            
            ObjectDerivedFromFields[parentObject].Add(currentProperty.Name(), parentObjectValueForCurrentProperty);
            
            
        }
        if (parentObject.GetType() == typeof(GameObject))
        {
            GOComponents[(GameObject)parentObject] = GetComponents((GameObject)parentObject);
            foreach (Component component in GOComponents[(GameObject)parentObject])
            {
                if (configuration != null && checkIfPropertyIsToToggle(configuration.properties, currentPropertyHierarchy, component.GetType().ToString()))
                {
                    //MyDebugger.MyDebug(obj.Name() + " found.");
                    ObjectsToggled.Add(component, true);
                    currentPropertyHierarchy.Add(component.GetType().ToString());
                    updateDataStructures(component, configuration, currentPropertyHierarchy);

                }
                else
                {
                    ObjectsToggled.Add(component, false);
                }
            }
        }
    }

    public bool multipleEntriesFor(object obj)
    {
        object owner = ObjectsOwners[obj];
        bool found = false;
        Dictionary<string,object> ownerProperties = ObjectDerivedFromFields[owner];
        foreach(object o in ownerProperties.Values)
        {
            if (o == obj)
            {
                if (found)
                {
                    return true;
                }
                else
                {
                    found = true;
                }
            }
        }
        return false;
    }

    internal Type TypeOf(FieldOrProperty f)
    {
        return ReflectionExecutor.TypeOf(f);
    }
}
