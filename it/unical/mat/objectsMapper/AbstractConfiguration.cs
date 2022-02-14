using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable,ExecuteInEditMode]
internal abstract class AbstractConfiguration : MonoBehaviour
{
    internal ObjectTracker _objectTracker;
    internal ObjectTracker ObjectTracker {
        get
        {
            if(_objectTracker == null)
            {
                _objectTracker = new ObjectTracker(gameObject);
            }
            return _objectTracker;
        }
    }
    [SerializeField, HideInInspector]
    internal List<MyListString> _savedProperties;
    internal  List<MyListString> SavedProperties
    {
        get
        {
            if (_savedProperties == null)
            {
                _savedProperties = new List<MyListString>();
            }
            return _savedProperties;
        }
        set
        {
            _savedProperties = value;
        }
    }
    [SerializeField, HideInInspector]
    internal List<MyListString> _toMapProperties;
    internal List<MyListString> ToMapProperties
    {
        get
        {
            if (_toMapProperties == null)
            {
                _toMapProperties = new List<MyListString>();
            }
            return _toMapProperties;
        }
        set
        {
            _toMapProperties = value;
        }
    }
    [SerializeField, HideInInspector]
    internal string _configurationName;
    internal virtual string ConfigurationName { 
        get
        {
            if (_configurationName == null)
            {
                _configurationName = GetAutoConfigurationName();
            }
            return _configurationName;
        }
        set { } 
    }

    internal void RefreshObjectTracker()
    {
        _objectTracker = new ObjectTracker(gameObject);
    }
    internal abstract bool IsSensor();
    internal abstract string GetAutoConfigurationName();

    internal virtual void Clear()
    {
        _objectTracker = new ObjectTracker(gameObject);
        SavedProperties = new List<MyListString>();
        ToMapProperties = new List<MyListString>();
    }
    internal bool IsPropertySelected(MyListString property)
    {
        return SavedProperties.Contains(property);
    }
    internal void ToggleProperty(MyListString property, bool isActive)
    {
        if (isActive)
        {
            SavedProperties.Add(property);
            PropertySelected(property);
            if (ObjectTracker.IsFinal(property))
            {
                ToMapProperties.Add(property);
            }
        }
        else
        {
            PropertyDeleted(property);
            SavedProperties.Remove(property);
            if (ToMapProperties.Contains(property))
            {
                ToMapProperties.Remove(property);
            }
        }
    }

    protected abstract void PropertyDeleted(MyListString property);
    protected abstract void PropertySelected(MyListString property);
    internal abstract bool IsAValidName(string temporaryName);
}

