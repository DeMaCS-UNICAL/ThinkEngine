using Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(IndexTracker))]
class MonoBehaviourSensor : MonoBehaviour
{
    internal string configurationName;
    private MyListString property;
    private List<IInfoAndValue> _propertyInfo;
    private bool ready;
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
    public int count = 0;

    public void Configure(InstantiationInformation  information, string mapping)
    {
        configurationName = information.configuration.ConfigurationName;
        property = new MyListString(information.propertyHierarchy.myStrings);
        PropertyInfo.AddRange(information.hierarchyInfo);
        int index = GetComponent<IndexTracker>().CurrentIndex;
        this._mapping = NewASPMapperHelper.AspFormat(configurationName)+"("+NewASPMapperHelper.AspFormat(gameObject.name)+",objectIndex("+index+"),"+mapping+")."+Environment.NewLine;
        ready = true;
    }
    internal void UpdateValue()
    {
        if (!ready)
        {
            return;
        }
        MapperManager.UpdateSensor(this, gameObject, new MyListString(property.myStrings), 0);
    }

    internal string Map()
    {
        List<object> values = new List<object>();
        for(int i=0; i<PropertyInfo.Count;i++)
        {

            object toAdd = PropertyInfo[i].GetValuesForPlaceholders();
            values.Add(toAdd);
        }
        //return MapperManager.GetSensorBasicMap(this, gameObject, new MyListString(property.myStrings), new List<object>(), 0);
        return string.Format(Mapping, values.ToArray());
    }
    void OnDisable()
    {
        Destroy(this);
    }
}

