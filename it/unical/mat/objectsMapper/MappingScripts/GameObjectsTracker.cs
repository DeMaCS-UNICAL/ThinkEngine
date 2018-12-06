using System;
using System.Collections.Generic;
using UnityEngine;
using EmbASP4Unity.it.unical.mat.objectsMapper.MappingScripts;

public class GameObjectsTracker
{
     public ReflectionExecutor re;

     public GameObject GO { get; set; }
     public List<string> AvailableGameObjects { get; set; }
     public Dictionary<object, bool> ObjectsToggled { get; set; }
     public Dictionary<object, object> ObjectsOwners { get; set; }
     public Dictionary<object, Dictionary<string, object>> ObjectDerivedFromFields { get; set; }
     public Dictionary<object, Dictionary<string, FieldOrProperty>> ObjectsProperties { get; set; }
     public Dictionary<GameObject, List<Component>> GOComponents { get; set; }


    public GameObjectsTracker()
    {
        re = (ReflectionExecutor)GameObject.FindObjectOfType(typeof(ReflectionExecutor));
        Debug.Log(re);
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

    public SensorConfiguration saveConfiguration(string chosenGO)
    {
        SensorConfiguration sens = new SensorConfiguration();
        sens.SaveSensorConfiguration(this);
        return sens;
    }

    internal void updateDataStructures(string chosenGO, SensorConfiguration s)
    {
        
        if (chosenGO != null)
        {
            GO = GetGameObject(chosenGO);
        }
        else
        {
            GO = GetGameObject(s.gOName);
            
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
            if (gOValueForObj != null && !IsBaseType(obj) && !ObjectsOwners.ContainsKey(gOValueForObj))
            {
                ObjectsOwners.Add(gOValueForObj, GO);
            }

            ObjectDerivedFromFields[GO].Add(obj.Name(), gOValueForObj);
            if (!ObjectsToggled.ContainsKey(obj))
            {
                if (s!= null && s.properties.Contains(obj.Name()))
                {
                    //Debug.Log(obj.Name() + " found.");
                    ObjectsToggled.Add(obj, true);
                    if (!IsBaseType(obj) && gOValueForObj!=null)
                    {
                       // Debug.Log("calling update");
                        updateDataStructures(gOValueForObj, s,obj.Name());
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

    public void cleanDataStructures()
    {
        ObjectsProperties = new Dictionary<object, Dictionary<string, FieldOrProperty>>();
        GOComponents = new Dictionary<GameObject, List<Component>>();
        ObjectsOwners = new Dictionary<object, object>();
        ObjectsToggled = new Dictionary<object, bool>();
        ObjectDerivedFromFields = new Dictionary<object, Dictionary<string, object>>();
    }

    internal void updateDataStructures(object obj, SensorConfiguration s, string parent)
    {
        //Debug.Log("updating " + parent);
        List<FieldOrProperty> p = GetFieldsAndProperties(obj);
        ObjectsProperties.Add(obj, new Dictionary<string, FieldOrProperty>());
       
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
                //Debug.Log("cannot get value for property " + ob.Name());
                objValueForOb = null;
            }
            ObjectsProperties[obj].Add(ob.Name(), ob);
           // Debug.Log("complete name " + parent + "^" + ob.Name());
            if(s!=null && s.properties.Contains(parent+"^"+ob.Name()))
            {
                
                ObjectsToggled.Add(ob, true);
                if (!IsBaseType(ob) && objValueForOb!=null)
                {
                    updateDataStructures(objValueForOb, s, parent + "^" + ob.Name());
                }
            }
            else
            {
                ObjectsToggled.Add(ob, false);
            }
            
            ObjectDerivedFromFields[obj].Add(ob.Name(), objValueForOb);
            if (objValueForOb != null && !ObjectsOwners.ContainsKey(objValueForOb))
            {
                ObjectsOwners.Add(objValueForOb, obj);
            }
            
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

    
}
