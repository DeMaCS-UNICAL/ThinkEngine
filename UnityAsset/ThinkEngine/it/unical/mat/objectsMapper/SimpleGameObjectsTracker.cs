using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SimpleGameObjectsTracker
{
    public Dictionary<string, bool> propertiesToggled { get; set; }
    public string objType;
    [SerializeField]
    public List<string> toSave;
    [SerializeField]
    public string propertyName;
    public string name;
    public string propertyType; //ARRAY2, LIST

    public SimpleGameObjectsTracker(Type type)
    {
        ReflectionExecutor re = ScriptableObject.CreateInstance<ReflectionExecutor>();
        Type listType = re.isListOfType(type);
        //Debug.Log("is of type " + listType);
        if (!(listType is null))
        {
            propertyType = "LIST";
            objType = listType.AssemblyQualifiedName;
            name = listType.ToString();
        }
        else
        {
            propertyType = "ARRAY2";
            objType = type.GetElementType().AssemblyQualifiedName;
            name = type.GetElementType().ToString();
        }
        //Debug.Log("to string " + objType);
        toSave = new List<string>();
        propertiesToggled = new Dictionary<string, bool>();
        
    }

  
    
    public void getBasicProperties()
    {
        Type local = Type.GetType(objType);
        Debug.Log("obtained type from " + objType + " is " + local);
        ReflectionExecutor re = ScriptableObject.CreateInstance<ReflectionExecutor>();
        List<FieldOrProperty> all = re.GetFieldsAndProperties(local);
        propertiesToggled = new Dictionary<string, bool>();
        
        foreach (FieldOrProperty f in all)
        {
            if (re.IsBaseType(f) && !propertiesToggled.ContainsKey(f.Name()))
            {
                bool toggled = false;
                if (toSave.Contains(f.Name()))
                {
                    toggled = true;
                }
                propertiesToggled.Add(f.Name(), toggled);
            }
        }
        
        
    }

    public void save()
    {
        foreach(string s in propertiesToggled.Keys)
        {
            if (propertiesToggled[s] && !toSave.Contains(s))
            {
                toSave.Add(s);
            }
        }
    }

}