using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
using System.Collections.Generic;

internal class MonoBehaviourSensorHider
{
    internal class MonoBehaviourSensor : MonoBehaviour
    {
        internal string sensorName { get; set; }
        internal MyListString property { get; set; }
        internal string propertyType { get; set; }
        internal int operationType { get; set; }
        internal List<string> collectionElementProperties { get; set; }
        internal List<int> indexes { get; set; }
        internal bool ready { get; set; }
        internal List<IList> propertyValues { get; set; }
        internal string collectionElementType { get; set; }
        #region Unity Messages
        void Awake()
        {
            hideFlags = HideFlags.HideAndDontSave;
            propertyValues = new List<IList>();
            indexes = new List<int>();
        }
        void LateUpdate()
        {
            if (!ready)
            {
                return;
            }
            if (SensorsManager.frameFromLastUpdate >= SensorsManager.updateFrequencyInFrames)
            {
                ReadProperty();
                if (propertyValues.Count == 101)
                {
                    propertyValues.RemoveAt(0);
                }
            }
        }
        void OnDisable()
        {
            Destroy(this);
        }
        #endregion
        internal void Done()
        {
            ready = true;
        }
        private void ReadProperty()
        {
            if (property.Count == 1)
            {
                ReadSimplePropertyMethod(property[0], typeof(GameObject), gameObject);
            }
            else
            {
                SensorsUtility.ReadComposedProperty(gameObject, property, property, typeof(GameObject), gameObject, ReadSimplePropertyMethod);
            }
        }
        private MyPropertyInfo ReadSimplePropertyMethod(string path, Type type, object obj)
        {
            if (propertyType.Equals("VALUE"))
            {
                ReadSimpleValueProperty(path, type, obj);
                return null;
            }
            if (propertyType.Equals("LIST"))
            {
                ReadListProperty(path, type, obj);
                return null;
            }
            if (propertyType.Equals("ARRAY2"))
            {
                ReadArrayProperty(path, type, obj);
                return null;
            }
            return null;
        }
        private void ReadArrayProperty(string propertyPath, Type type, object currentObject)
        {
            ArrayInfo matrixValue;
            if (collectionElementProperties.Count == 1 && collectionElementProperties[0].Equals("")) 
            {
                matrixValue = SensorsUtility.GetArrayProperty(propertyPath, type, currentObject, indexes[0], indexes[1]);
            }
            else
            {
                matrixValue = SensorsUtility.GetArrayProperty(propertyPath, type, currentObject, indexes[0], indexes[1], collectionElementProperties);
            }
            RetrieveCollectionValue(matrixValue);
        }
        private void ReadListProperty(string path, Type type, object obj)
        {
            ListInfo listValue;
            if (collectionElementProperties.Count == 1 && collectionElementProperties[0].Equals(""))
            {
                listValue = SensorsUtility.GetListProperty(path, type, obj, indexes[0]);
            }
            else
            {
                listValue = SensorsUtility.GetListProperty(path, type, obj, indexes[0], collectionElementProperties);
            }
            RetrieveCollectionValue(listValue);
        }
        public void ReadSimpleValueProperty(string path, Type gOType, object obj)
        {
            MemberInfo[] members = gOType.GetMember(path, SensorsUtility.BindingAttr);
            if (members.Length == 0)
            {
                return;
            }
            FieldOrProperty property = new FieldOrProperty(members[0]);
            RetrieveSinglePropertyValue(property, 0, obj);
        }
        private void RetrieveCollectionValue(MyPropertyInfo collectionValue)
        {
            if (!collectionValue.IsNull())
            {
                if (!collectionValue.isPrimitive && collectionValue.properties != null)
                {
                    List<FieldOrProperty> toRead = collectionValue.properties;
                    RetrievePropertiesValues(toRead, collectionValue.Collection());
                }
                else if (collectionValue.isPrimitive && collectionValue.value != null)
                {
                    IList currentValuesList = propertyValues[0];
                    Type currentValuesListType = currentValuesList.GetType().GetGenericArguments()[0];
                    currentValuesList.Add(Convert.ChangeType(collectionValue.value, currentValuesListType));
                }
                else
                {
                    CollectionShrank();
                    return;
                }
            }
            
        }
        private void RetrievePropertiesValues(List<FieldOrProperty> toRead, object collection)
        {
            if (toRead.Count != collectionElementProperties.Count)
            {
                throw new Exception("Count of the properties to track mismatch.");
            }
            for (int i = 0; i < collectionElementProperties.Count; i++)
            {
                object value = null;
                if (propertyType.Equals("ARRAY2"))
                {
                    value = ((Array)collection).GetValue(indexes[0], indexes[1]);
                }
                if (propertyType.Equals("LIST"))
                {
                    value = ((IList)collection)[indexes[0]];
                }
                if (value == null)
                {
                    return;
                }
                RetrieveSinglePropertyValue(toRead[i], i, value);
            }
        }
        private void RetrieveSinglePropertyValue(FieldOrProperty toRead, int i, object value)
        {
            IList currentValuesList = propertyValues[i];
            Type currentValuesListType = currentValuesList.GetType().GetGenericArguments()[0];
            currentValuesList.Add(Convert.ChangeType(toRead.GetValue(value), currentValuesListType));
        }
        private void CollectionShrank()
        {
            GetComponent<MonoBehaviourSensorsManager>().RemoveSensor(this);
            Destroy(this);
        }
        internal string Map()
        {
            List<string> myTemplate = GetCorrespondingConfiguration().GetTemplate(property);
            string toReturn = "";
            IMapper mapperForT;
            IList currentValuesList;
            Type currentValuesListType;
            string value;
            if (propertyType.Equals("VALUE"))
            {
                currentValuesList = propertyValues[0];
                currentValuesListType = currentValuesList.GetType().GetGenericArguments()[0];
                mapperForT = MappingManager.GetMapper(currentValuesListType);
                value = mapperForT.BasicMap(Operation.Compute(operationType, currentValuesList));
                return string.Format(myTemplate[0], gameObject.GetComponent<IndexTracker>().CurrentIndex, value) + Environment.NewLine;
            }
            if (propertyType.Equals("LIST") || propertyType.Equals("ARRAY2"))
            {
                object[] parameters;
                if (propertyType.Equals("LIST"))
                {
                    parameters = new object[3];
                    parameters[1] = indexes[0];
                }
                else
                {
                    parameters = new object[4];
                    parameters[1] = indexes[0];
                    parameters[2] = indexes[1];
                }
                parameters[0] = gameObject.GetComponent<IndexTracker>().CurrentIndex;
                for (int i = 0; i < collectionElementProperties.Count; i++)
                {
                    currentValuesList = propertyValues[i];
                    currentValuesListType = currentValuesList.GetType().GetGenericArguments()[0];
                    mapperForT = MappingManager.GetMapper(currentValuesListType);
                    value = mapperForT.BasicMap(currentValuesList[currentValuesList.Count - 1]);
                    parameters[parameters.Length - 1] = value;
                    toReturn += string.Format(myTemplate[i], parameters) + Environment.NewLine;
                }
            }
            return toReturn;
        }
        private SensorConfiguration GetCorrespondingConfiguration()
        {
            foreach (SensorConfiguration sensorConfiguration in gameObject.GetComponents<SensorConfiguration>())
            {
                if (sensorConfiguration.configurationName.Equals(sensorName))
                {
                    return sensorConfiguration;
                }
            }
            return null;
        }
    }
}