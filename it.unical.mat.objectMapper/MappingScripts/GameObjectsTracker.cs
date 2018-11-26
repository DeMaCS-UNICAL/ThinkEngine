using Assets.Scripts.MappingScripts;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectsTracker
{
     public ReflectionExecutor re;
     public Serializator Serializator;
     public List<string> AvailableGameObjects { get; set; }
    public Dictionary<object, bool> ObjectsToggled { get; set; }
     public Dictionary<object, object> ObjectsOwners { get; set; }
     public Dictionary<object, Dictionary<string, object>> ObjectDerivedFromFields { get; set; }
     public Dictionary<object, Dictionary<string, FieldOrProperty>> ObjectsProperties { get; set; }
     public Dictionary<GameObject, List<Component>> GOComponents { get; set; }


    public GameObjectsTracker()
    {
        re = (ReflectionExecutor)GameObject.FindObjectOfType(typeof(ReflectionExecutor));
        Serializator = new Serializator(this);
    }

    public void updateGameObjects()
    {
        AvailableGameObjects = re.GetGameObjects();
    }

    public void serialize(string chosenGO)
    {
        Serializator.serialize(re.GetGameObjectWithName(chosenGO));
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
}
