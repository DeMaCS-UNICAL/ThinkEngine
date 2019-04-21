using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SimpleGameObjectsTracker
{
    public Dictionary<string, bool> propertiesToggled { get; set; }
    public Type objType;
    [SerializeField]
    public List<string> toSave;
    [SerializeField]
    public string propertyName;

    public SimpleGameObjectsTracker(Type type)
    {
        objType = type.GetElementType();
        toSave = new List<string>();
        propertiesToggled = new Dictionary<string, bool>();
        
    }

  
    
    public void getBasicProperties()
    {
        ReflectionExecutor re = ScriptableObject.CreateInstance<ReflectionExecutor>();
        List<FieldOrProperty> all = re.GetFieldsAndProperties(objType);
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
                propertiesToggled.Add(f.Name(),toggled);
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