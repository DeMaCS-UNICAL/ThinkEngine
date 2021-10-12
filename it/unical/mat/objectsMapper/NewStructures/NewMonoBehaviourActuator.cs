﻿using it.unical.mat.embasp.languages.asp;
using newMappers;
using NewStructures;
using NewStructures.NewMappers;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

[RequireComponent(typeof(IndexTracker))]
internal class NewMonoBehaviourActuator : MonoBehaviour
{
    internal string configurationName;
    private object toPass;
    private MyListString property;
    private string mappingToCompare;
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
    private string _toSet;
    private string suffix;

    internal string toSet
    {
        get
        {
            return _toSet;
        }
        set
        {
            _toSet = value;
            ApplyToGameLogic();
        }
    }

    internal void Configure(InstantiationInformation information, string mapping)
    {
        toPass = gameObject;
        configurationName = information.configuration.ConfigurationName;
        property = new MyListString(information.propertyHierarchy.myStrings);
        PropertyInfo.AddRange(information.hierarchyInfo);
        int index = GetComponent<IndexTracker>().CurrentIndex;
        this._mapping = "setOnActuator("+NewASPMapperHelper.AspFormat(configurationName) + "(" + NewASPMapperHelper.AspFormat(gameObject.name) + ",objectIndex(" + index + ")," + mapping+"))";
        List<object> temp = new List<object>();
        for(int i=0; i < PropertyInfo.Count; i++)
        {
            object currentValue = PropertyInfo[i].GetValuesForPlaceholders();
            if (currentValue.GetType().IsArray)
            {
                Array array = (Array)currentValue;
                for (int j=0; j< array.Length; j++)
                {
                    temp.Add(array.GetValue(j));
                }
            }
            else
            {
                if (temp != null)
                {
                    temp.Add(currentValue);
                }
                else
                {
                    temp.Add("");
                }
            }
        }
        mappingToCompare = _mapping.Substring(0,_mapping.IndexOf("{" + (temp.Count) + "}"));
        int indexOfValue = _mapping.IndexOf("{" + temp.Count + "}");
        suffix = _mapping.Substring(indexOfValue + 3);
        mappingToCompare = string.Format(mappingToCompare, temp.ToArray());
    }
    void Update()
    {
        count++;

        if (count % 1000 == 0)
        {
            Debug.Log(MapperManager.GetActuatorBasicMap(this, gameObject, new MyListString(property.myStrings), new List<object>(), 0));
        }
    }
   
    void OnDisable()
    {
        Destroy(this);
    }

    private void ApplyToGameLogic()
    {
        if (_toSet == null)
        {
            return;
        }
        MapperManager.SetPropertyValue(this, new MyListString(property.myStrings), toPass, _toSet, 0);

    }
    internal string Parse(AnswerSet answerSet)
    {
        string pattern = "objectIndex\\(([0-9]+)\\)";
        Regex regex = new Regex(@pattern);
        int myIndex = gameObject.GetComponent<IndexTracker>().CurrentIndex;
        foreach (string literal in answerSet.GetAnswerSet())
        {
            string literalPrefix = literal.Substring(0, mappingToCompare.Length);
            Match matcher = regex.Match(literalPrefix);
            if (matcher.Success && int.Parse(matcher.Groups[1].Value) == myIndex)//if the index of the object associated to the actuator is different from the one of the literal
            {
                if (literalPrefix.Contains(mappingToCompare))
                {
                    string partialRes = literal.Substring(mappingToCompare.Length);
                    partialRes = partialRes.Remove(partialRes.IndexOf(suffix));
                    return partialRes.Trim('\"',' ');//trim " to avoid conversion problems
                }
            }
        }
        return null;
    }
}