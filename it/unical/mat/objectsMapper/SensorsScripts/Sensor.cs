﻿using Mappers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class Sensor
{
    GameObject gameObject;
    MonoBehaviourSensorsManager manager;
    internal SensorConfiguration configuration;
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
        gameObject = information.instantiateOn;
        manager = gameObject.GetComponent<MonoBehaviourSensorsManager>();
        configuration = (SensorConfiguration)information.configuration;
        property = new MyListString(information.propertyHierarchy.myStrings);
        PropertyInfo.AddRange(information.hierarchyInfo);
        if (information.instantiateOn.GetComponent<IndexTracker>() == null)
        {
            information.instantiateOn.AddComponent<IndexTracker>();
        }
        int index = information.instantiateOn.GetComponent<IndexTracker>().CurrentIndex;
        this._mapping = NewASPMapperHelper.AspFormat(configuration.ConfigurationName)+"("+NewASPMapperHelper.AspFormat(gameObject.name)+",objectIndex("+index+"),"+mapping+")."+Environment.NewLine;
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
        //VELOCIZZA EVITANDO COPIE!!
        List<object> values = new List<object>();
        for(int i=0; i<PropertyInfo.Count;i++)
        {
            object toAdd = PropertyInfo[i].GetValuesForPlaceholders();
            if (toAdd != null)
            {
                if (toAdd is IList listToAdd)
                {
                    for (int j = 0; j < listToAdd.Count; j++)
                    {
                        values.Add(listToAdd[j]);
                    }
                }
                else
                {
                    values.Add(toAdd);
                }
            }
        }
        //return MapperManager.GetSensorBasicMap(this, gameObject, new MyListString(property.myStrings), new List<object>(), 0);
        try
        {
            return string.Format(Mapping, values.ToArray());
        }catch(System.FormatException e)
        {
            return "";
        }
    }

    internal void Destroy()
    {
        manager.Destroy(this);
    }
}

