using newMappers;
using NewStructures;
using NewStructures.NewMappers;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IndexTracker))]
internal class NewMonoBehaviourActuator : MonoBehaviour
{
    internal string configurationName;
    private MyListString property;
    private List<IInfoAndValue> _propertyInfo;
    internal List<IInfoAndValue> PropertyInfo
    {
        get
        {
            if (_propertyInfo == null)
            {
                _propertyInfo = new List<IInfoAndValue>();
            }
            return _propertyInfo;
        }
    }
    private string _mapping;
    internal string Mapping
    {
        get
        {
            return _mapping;
        }
    }
    int count =0;
    internal void Configure(InstantiationInformation information, string mapping)
    {
        configurationName = information.configuration.name;
        property = new MyListString(information.propertyHierarchy.myStrings);
        PropertyInfo.AddRange(information.hierarchyInfo);
        int index = GetComponent<IndexTracker>().CurrentIndex;
        this._mapping = "setOnActuator("+NewASPMapperHelper.AspFormat(configurationName) + "(" + NewASPMapperHelper.AspFormat(gameObject.name) + ", objectIndex(" + index + ")," + mapping+"))";
    }
    void Update()
    {
        count++;

        if (count % 1000 == 0)
        {
            Debug.Log(MapperManager.GetActuatorBasicMap(this, gameObject, new MyListString(property.myStrings), new List<object>(), 0));
        }
    }
    void LateUpdate()
    {
        object go = gameObject;
        if (count % 100 == 0)
        {
            MapperManager.SetPropertyValue(this, new MyListString(property.myStrings), ref go, 5, 0);
        }
    }
}