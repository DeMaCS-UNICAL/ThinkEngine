using Mappers;
using System;
using System.Collections.Generic;
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
    void Update()
    {
    }
    void LateUpdate()
    {
        if (!ready)
        {
            return;
        }
        if (SensorsManager.frameFromLastUpdate >= SensorsManager.updateFrequencyInFrames)
        {
            MapperManager.UpdateSensor(this, gameObject, new MyListString(property.myStrings), 0);
        }
    }

    internal string Map()
    {
        return MapperManager.GetSensorBasicMap(this, gameObject, new MyListString(property.myStrings), new List<object>(), 0);
    }
    void OnDisable()
    {
        Destroy(this);
    }
}

