using System;
using System.Collections.Generic;
using UnityEngine;
using EmbASP4Unity.it.unical.mat.objectsMapper.SensorsScripts;
using EmbASP4Unity.it.unical.mat.objectsMapper;

public class GameObjectsTracker
{
     public ReflectionExecutor re;

     public GameObject GO { get; set; }
     public List<string> AvailableGameObjects { get; set; }
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

   

    public GameObjectsTracker()
    {
        re = ScriptableObject.CreateInstance<ReflectionExecutor>();
        updateGameObjects();
    }

    public void updateGameObjects()
    {
        AvailableGameObjects = re.GetGameObjects();
    }

    

    public GameObject GetGameObject(string chosenGO)
    {
        return re.GetGameObjectWithName(chosenGO);
    }

    public List<Component> GetComponents(GameObject gO)
    {
        return re.GetComponents(gO);
    }

    public bool IsBaseType(FieldOrProperty obj)
    {
        return re.IsBaseType(obj);
    }

    public List<FieldOrProperty> GetFieldsAndProperties(object gO)
    {
        return re.GetFieldsAndProperties(gO);
    }

    public AbstractConfiguration saveConfiguration(AbstractConfiguration conf, string chosenGO)
    {        
        conf.SaveConfiguration(this);
        return conf;
    }

    internal void updateDataStructures(string chosenGO, AbstractConfiguration s)
    {
        
        if (chosenGO != null)
        {
            configurationName = "";
            GO = GetGameObject(chosenGO);
        }
        else
        {
            GO = GetGameObject(s.gOName);
            configurationName = s.configurationName;

        }
        cleanDataStructures();
        List<FieldOrProperty> p = GetFieldsAndProperties(GO); ;
        ObjectsProperties.Add(GO, new Dictionary<string, FieldOrProperty>());        
        ObjectDerivedFromFields.Add(GO, new Dictionary<string, object>());
        foreach (FieldOrProperty obj in p)
        {
            object gOValueForObj;
            try
            {
                gOValueForObj = obj.GetValue(GO);
            }
            catch (Exception e)
            {
                gOValueForObj = null;
            }
            ObjectsProperties[GO].Add(obj.Name(), obj);
            if (gOValueForObj != null && !IsMappable(obj)) 
            {
                if (!ObjectsOwners.ContainsKey(gOValueForObj))
                {
                    ObjectsOwners.Add(gOValueForObj, new KeyValuePair<object, string>(GO,obj.Name()));
                }

            }

            ObjectDerivedFromFields[GO].Add(obj.Name(), gOValueForObj);
            if (!ObjectsToggled.ContainsKey(obj))
            {
                if (s!= null && s.properties.Contains(obj.Name()))
                {
                    //Debug.Log(obj.Name() + " found.");
                    ObjectsToggled.Add(obj, true);
                    if (!IsMappable(obj) && gOValueForObj != null)
                    {
                        // Debug.Log("calling update");
                        updateDataStructures(gOValueForObj, s, obj.Name());
                    }
                    else
                    {
                        if(IsMappable(obj) && !IsBaseType(obj))
                        {
                            foreach (SimpleGameObjectsTracker st in s.advancedConf) {
                                if (st.propertyName.Equals(obj.Name()))
                                {
                                    st.objType = obj.Type().GetElementType();
                                    basicTypeCollectionsConfigurations.Add(obj, st);
                                    Debug.Log("Adding st for " + obj.Name() + " whit type " + st.objType);
                                    break;
                                }
                            }
                        }
                        if (s.GetType() == typeof(SensorConfiguration))
                        {
                            foreach (StringIntPair pair in ((SensorConfiguration)s).operationPerProperty)
                            {
                                if (pair.Key.Equals(obj.Name()))
                                {
                                    operationPerProperty.Add(obj, pair.Value);
                                    if (pair.Value == Operation.SPECIFIC)
                                    {
                                        foreach (StringStringPair pair2 in ((SensorConfiguration)s).specificValuePerProperty)
                                        {
                                            if (pair2.Key.Equals(obj.Name()))
                                            {
                                                specificValuePerProperty.Add(obj, pair2.Value);
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
                    ObjectsToggled.Add(obj, false);
                }
            }
        }
        GOComponents.Add(GO, GetComponents(GO));
        foreach (Component c in GOComponents[GO])
        {
            if (s != null && s.properties.Contains(c.GetType().ToString()))
            {
                //Debug.Log(obj.Name() + " found.");
                ObjectsToggled.Add(c, true);                
                updateDataStructures(c, s, c.GetType().ToString());
                
            }
            else
            {
                ObjectsToggled.Add(c, false);
            }
        }
    }

    public bool IsMappable(FieldOrProperty obj)
    {
        return re.isMappable(obj); 
    }

    public int IsArrayOfRank(FieldOrProperty obj)
    {
        return re.isArrayOfRank(obj);
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

    internal void updateDataStructures(object obj, AbstractConfiguration s, string parent)
    {
        Debug.Log("updating " + parent);
        
        List<FieldOrProperty> p = GetFieldsAndProperties(obj);
        ObjectsProperties.Add(obj, new Dictionary<string, FieldOrProperty>());
        //Debug.Log("Adding derived from fields entry");
        ObjectDerivedFromFields.Add(obj, new Dictionary<string, object>());
        
        foreach (FieldOrProperty ob in p)
        {
            //Debug.Log("Property " + ob.Name());
            object objValueForOb;
            try
            {
                objValueForOb = ob.GetValue(obj);
            }
            catch (Exception e)
            {
                Debug.Log("cannot get value for property " + ob.Name());
                objValueForOb = null;
            }
            ObjectsProperties[obj].Add(ob.Name(), ob);
            // Debug.Log("complete name " + parent + "^" + ob.Name());
            if (objValueForOb != null) 
            {
                if (!ObjectsOwners.ContainsKey(objValueForOb))
                {
                    ObjectsOwners.Add(objValueForOb, new KeyValuePair<object, string>(obj, ob.Name()));
                }
            }

            if (s!=null && s.properties.Contains(parent+"^"+ob.Name()))
            {
                Debug.Log("s contains " + parent + "^" + ob.Name());   
                ObjectsToggled.Add(ob, true);
                if (!IsMappable(ob) && objValueForOb != null)
                {
                    updateDataStructures(objValueForOb, s, parent + "^" + ob.Name());
                }
                else
                {
                    if (IsMappable(ob) && !IsBaseType(ob))
                    {
                        foreach (SimpleGameObjectsTracker st in s.advancedConf)
                        {
                            if (st.propertyName.Equals(parent + "^" + ob.Name()))
                            {
                                st.objType = ob.Type().GetElementType();
                                Debug.Log("Adding st for " + ob.Name() + " whit type " + st.objType);
                                basicTypeCollectionsConfigurations.Add(ob, st);
                                break;
                            }
                        }
                    }
                    if (s.GetType() == typeof(SensorConfiguration))
                    {
                        foreach (StringIntPair pair in ((SensorConfiguration)s).operationPerProperty)
                        {
                            if (pair.Key.Equals(parent + "^" + ob.Name()))
                            {
                                operationPerProperty.Add(ob, pair.Value);
                                if (pair.Value == Operation.SPECIFIC)
                                {
                                    foreach (StringStringPair pair2 in ((SensorConfiguration)s).specificValuePerProperty)
                                    {
                                        if (pair2.Key.Equals(parent + "^" + ob.Name()))
                                        {
                                            specificValuePerProperty.Add(ob, pair2.Value);
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
                ObjectsToggled.Add(ob, false);
            }
            
            ObjectDerivedFromFields[obj].Add(ob.Name(), objValueForOb);
            
            
        }
        if (obj.GetType() == typeof(GameObject))
        {
            GOComponents[(GameObject)obj] = GetComponents((GameObject)obj);
            foreach (Component c in GOComponents[(GameObject)obj])
            {
                if (s != null && s.properties.Contains(c.GetType().ToString()))
                {
                    //Debug.Log(obj.Name() + " found.");
                    ObjectsToggled.Add(c, true);
                    updateDataStructures(c, s, c.GetType().ToString());

                }
                else
                {
                    ObjectsToggled.Add(c, false);
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
        return re.TypeOf(f);
    }
}
