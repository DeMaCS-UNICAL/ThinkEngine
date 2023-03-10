using System;
using System.Collections.Generic;
using System.Reflection;
using ThinkEngine.Mappers;
using UnityEditor;
using UnityEngine;

namespace ThinkEngine
{
    [ExecuteInEditMode, Serializable, RequireComponent(typeof(IndexTracker)), RequireComponent(typeof(MonoBehaviourSensorsManager))]
    class SensorConfiguration : AbstractConfiguration, ISerializationCallbackReceiver
    {
        public bool isInvariant;
        public bool isFixedSize;
        [SerializeField, HideInInspector]
        internal List<int> operationPerPropertyIndexes;
        [SerializeField, HideInInspector]
        internal List<int> operationPerPropertyOperations;
        [SerializeField, HideInInspector]
        internal List<int> specificValuePerPropertyIndexes;
        [SerializeField, HideInInspector]
        internal List<string> specificValuePerPropertyValues;
        internal Dictionary<int, int> _operationPerProperty;
        internal Dictionary<int, int> OperationPerProperty
        {
            get
            {
                if (_operationPerProperty == null)
                {
                    _operationPerProperty = new Dictionary<int, int>();
                }
                return _operationPerProperty;
            }
            set
            {
                _operationPerProperty = value;
            }
        }
        [SerializeField, HideInInspector]
        internal Dictionary<int, string> _specificValuePerProperty;
        internal Dictionary<int, string> SpecificValuePerProperty
        {
            get
            {
                if (_specificValuePerProperty == null)
                {
                    _specificValuePerProperty = new Dictionary<int, string>();
                }
                return _specificValuePerProperty;
            }
            set
            {
                _specificValuePerProperty = value;
            }
        }

        //GMDG
        //This array contains the types of the sensor assiated with "this" SensorConfiguration

        [SerializeField]
        private List<SerializableSystemType> _serializableSensorsTypes = new List<SerializableSystemType>();

        internal List<string> _sensorsTypesNames = new List<string>();
        private List<Sensor> _sensorsInstances = new List<Sensor>();

        [SerializeField]
        List<MonoScript> _scripts = new List<MonoScript>();

        internal void AddSensorType(Type sensorType)
        {
            SerializableSystemType serializableSensorType = new SerializableSystemType(sensorType);

            if (_serializableSensorsTypes.Contains(serializableSensorType)) return;

            _serializableSensorsTypes.Add(serializableSensorType);
        }

        void Awake()
        {
            if(Application.isPlaying)
            {
                /*foreach (SerializableSystemType serializableSensorType in _serializableSensorsTypes)
                {
                    _sensorsInstances.Add((Sensor)serializableSensorType.SystemType.GetProperty("Instance", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null));
                    //_sensorsTypes.Add(serializableSensorType.SystemType);
                }*/
                /*foreach(Type sensorType in _sensorsTypes)
                {
                    sensorType.GetMethod("Istantiate").Invoke(null, null);
                }*/
                foreach(MonoScript script in _scripts)
                {
                    Type type = Type.GetType(script.GetClass()?.AssemblyQualifiedName);
                    _sensorsInstances.Add((Sensor)type.GetProperty("Instance", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null));
                }
                foreach (Sensor instance in _sensorsInstances)
                {
                    instance.Initialize(gameObject);
                }
            }
        }

        void OnEnable()
        {
            if (Application.isPlaying)
            {
                SensorsManager.SubscribeSensors(_sensorsInstances);
            }
        }

        void OnDisable()
        {
            if (Application.isPlaying)
            {
                SensorsManager.UnsubscribeSensors(_sensorsInstances);
            }
        }

        void OnDestroy()
        {
            if (Application.isPlaying)
            {
                /*foreach (Type sensorType in _sensorsTypes)
                {
                    sensorType.GetMethod("Destroy").Invoke(null, null);
                }*/
                foreach(Sensor instance in _sensorsInstances)
                {
                    instance.Destroy();
                }
            }
        }
        //GMDG

        internal override string ConfigurationName
        {
            set
            {
                if (!Utility.SensorsManager.IsConfigurationNameValid(value, this))
                {
                    throw new Exception("The chosen configuration name cannot be used.");
                }
                string old = _configurationName;
                _configurationName = value;
                if (!old.Equals(_configurationName))
                {
                    SensorsManager.ConfigurationsChanged = true;
                }
            }
        }

        internal override void Clear()
        {
            base.Clear();
            OperationPerProperty = new Dictionary<int, int>();
            SpecificValuePerProperty = new Dictionary<int, string>();
            _serializableSensorsTypes = new List<SerializableSystemType>(); // GMDG
            _sensorsTypesNames = new List<string>(); // GMDG
        }
        internal override string GetAutoConfigurationName()
        {
            string name;
            string toAppend = "";
            int count = 0;
            do
            {
                name = ASPMapperHelper.AspFormat(gameObject.name) + "Sensor" + toAppend;
                toAppend += count;
                count++;
            }
            while (!Utility.SensorsManager.IsConfigurationNameValid(name, this));
            return name;
        }
        internal void SetOperationPerProperty(MyListString property, int operation)
        {
            if (!SavedProperties.Contains(property))
            {
                throw new Exception("Property not selected");
            }
            OperationPerProperty[property.GetHashCode()] = operation;
        }
        internal void SetSpecificValuePerProperty(MyListString property, string value)
        {
            if (!SavedProperties.Contains(property))
            {
                throw new Exception("Property not selected");
            }
            SpecificValuePerProperty[property.GetHashCode()] = value;

        }
        internal override bool IsSensor()
        {
            return true;
        }
        protected override void PropertySelected(MyListString property)
        {
            OperationPerProperty[property.GetHashCode()] = 0;
            SpecificValuePerProperty[property.GetHashCode()] = "";
        }
        protected override void PropertyDeleted(MyListString property)
        {
            OperationPerProperty.Remove(property.GetHashCode());
            SpecificValuePerProperty.Remove(property.GetHashCode());
        }

        public void OnBeforeSerialize()
        {
            operationPerPropertyIndexes = new List<int>();
            operationPerPropertyOperations = new List<int>();
            specificValuePerPropertyIndexes = new List<int>();
            specificValuePerPropertyValues = new List<string>();
            foreach (int key in OperationPerProperty.Keys)
            {
                operationPerPropertyIndexes.Add(key);
                operationPerPropertyOperations.Add(OperationPerProperty[key]);
            }
            foreach (int key in SpecificValuePerProperty.Keys)
            {
                specificValuePerPropertyIndexes.Add(key);
                specificValuePerPropertyValues.Add(SpecificValuePerProperty[key]);
            }
        }

        public void OnAfterDeserialize()
        {
            OperationPerProperty = new Dictionary<int, int>();
            SpecificValuePerProperty = new Dictionary<int, string>();
            for (int i = 0; i < operationPerPropertyIndexes.Count; i++)
            {
                OperationPerProperty.Add(operationPerPropertyIndexes[i], operationPerPropertyOperations[i]);
            }
            for (int i = 0; i < specificValuePerPropertyIndexes.Count; i++)
            {
                SpecificValuePerProperty.Add(specificValuePerPropertyIndexes[i], specificValuePerPropertyValues[i]);
            }
        }

        internal override bool IsAValidName(string temporaryName)
        {
            return temporaryName.Equals(ConfigurationName) || Utility.SensorsManager.IsConfigurationNameValid(temporaryName, this);
        }
    }
}