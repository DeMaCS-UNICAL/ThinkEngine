using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NewStructures
{
    [Serializable,ExecuteInEditMode]
    internal abstract class NewAbstractConfiguration : MonoBehaviour
    {
        internal ObjectTracker _objectTracker;
        internal ObjectTracker objectTracker {
            get
            {
                if(_objectTracker == null)
                {
                    _objectTracker = new ObjectTracker(this, IsSensor(), gameObject);
                }
                return _objectTracker;
            }
        }
        [SerializeField, HideInInspector]
        internal List<MyListString> _savedProperties;
        internal  List<MyListString> savedProperties
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
        internal string _configurationName;
        internal virtual string configurationName { 
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
            _objectTracker = new ObjectTracker(this, IsSensor(), gameObject);
        }
        internal abstract bool IsSensor();
        internal abstract string GetAutoConfigurationName();

        internal virtual void Clear()
        {
            _objectTracker = new ObjectTracker(this,IsSensor(), gameObject);
            configurationName = GetAutoConfigurationName();
            savedProperties = new List<MyListString>();
        }
        internal bool IsPropertySelected(MyListString property)
        {
            return savedProperties.Contains(property);
        }
        internal void ToggleProperty(MyListString property, bool isActive)
        {
            if (isActive)
            {
                savedProperties.Add(property);
                PropertySelected(property);
            }
            else
            {
                PropertyDeleted(property);
                savedProperties.Remove(property);
            }
        }

        protected abstract void PropertyDeleted(MyListString property);
        protected abstract void PropertySelected(MyListString property);
        internal abstract bool IsAValidName(string temporaryName);
    }
}
