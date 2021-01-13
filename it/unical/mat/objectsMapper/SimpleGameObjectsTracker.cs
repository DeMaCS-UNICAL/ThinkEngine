using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SimpleGameObjectsTracker
{
    public Dictionary<string, bool> _propertiesToggled;
    public Dictionary<string, bool> propertiesToggled
    {
        get
        {
            if (_propertiesToggled == null)
            {
                _propertiesToggled = new Dictionary<string, bool>();
            }
            return _propertiesToggled;
        }
    }
    public string objType;
    [SerializeField]
    public List<string> toSave;
    [SerializeField]
    public MyListString propertyName;
    public string name;
    public string propertyType; //ARRAY2, LIST
    public SimpleGameObjectsTracker(Type type)
    {
        Type listType = ReflectionExecutor.ListOfType(type);
        if (!(listType == null))
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
        toSave = new List<string>();
    }
    public void GetBasicProperties()
    {
        Type localType = Type.GetType(objType);
        List<FieldOrProperty> all = ReflectionExecutor.GetFieldsAndProperties(localType);
        foreach (FieldOrProperty f in all)
        {
            if (ReflectionExecutor.IsBaseType(f) && !propertiesToggled.ContainsKey(f.Name()))
            {
                propertiesToggled.Add(f.Name(), toSave.Contains(f.Name()));
            }
        }
    }
    public void save()
    {
        toSave = new List<string>();
        foreach(string s in propertiesToggled.Keys)
        {
            if (propertiesToggled[s])
            {
                toSave.Add(s);
            }
        }
    }

}