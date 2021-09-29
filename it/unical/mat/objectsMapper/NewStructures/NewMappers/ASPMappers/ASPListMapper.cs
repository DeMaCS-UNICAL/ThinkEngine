using newMappers;
using NewStructures;
using NewStructures.NewMappers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NewMappers.ASPMappers
{
    class ASPListMapper : IDataMapper
    {
        #region SUPPORT CLASSES
        private class ListInfoAndValue : IInfoAndValue
        {
            internal int index;
            internal ListInfoAndValue(int i)
            {
                index = i;
            }
        }
        private class ListSensors : ISensors
        {
            internal List<ISensors> sensorsList;
            internal ListSensors()
            {
                sensorsList = new List<ISensors>();
            }

            public bool IsEmpty()
            {
                return sensorsList.Count == 0;
            }
        }
        #endregion
        private static ASPListMapper _instance;
        internal static ASPListMapper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ASPListMapper();
                }
                return _instance;
            }
        }
        public bool Supports(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        }

        public int GetAggregationSpecificIndex(Type type)
        {
            return MapperManager.GetAggregationSpecificIndex(type.GenericTypeArguments[0]);
        }

        public Type GetAggregationTypes(Type type = null)
        {
            return MapperManager.GetAggregationTypes(type.GenericTypeArguments[0]);
        }

        public bool NeedsAggregates(Type type)
        {
            return MapperManager.NeedsAggregates(type.GenericTypeArguments[0]);
        }

        public bool NeedsSpecifications(Type type)
        {
            return true;
        }

        public List<NewOperationContainer.NewOperation> OperationList()
        {
            throw new NotImplementedException();
        }

        public bool IsFinal(Type type)
        {
            return MapperManager.IsFinal(type.GenericTypeArguments[0]);
        }

        public bool IsTypeExpandable(Type type)
        {
            return MapperManager.IsTypeExpandable(type.GenericTypeArguments[0]);
        }

        public Dictionary<MyListString, KeyValuePair<Type, object>> RetrieveProperties(Type objectType, MyListString currentObjectPropertyHierarchy, object currentObject)
        {
            if (!Supports(objectType))
            {
                throw new Exception(objectType + " is not supported.");
            }
            Type underlyingType = objectType.GenericTypeArguments[0];
            MyListString rootName = new MyListString(currentObjectPropertyHierarchy.myStrings);
            rootName.Add(underlyingType.Name);
            return MapperManager.RetrieveProperties(underlyingType, rootName, null);
        }

        

        public ISensors InstantiateSensors(InstantiationInformation information)
        {
            if (information.currentObjectOfTheHierarchy == null)
            {
                return new ListSensors();
            }
            IList actualList = (IList)information.currentObjectOfTheHierarchy;
            int x = actualList.Count;
            ListSensors sensors = new ListSensors();
            GenerateMapping(ref information);
            information.firstPlaceholder += 1;
            UpdateResidualPropertyHierarchy(information.residualPropertyHierarchy);
            for (int i = 0; i < x; i++)
            {
                sensors.sensorsList.Add(InstantiateSensorsForElement(information, actualList, i));
            }
            return sensors;
        }

        private ISensors InstantiateSensorsForElement(InstantiationInformation information, IList actualList, int i)
        {
            InstantiationInformation localInformation = new InstantiationInformation(information);
            localInformation.hierarchyInfo.Add(new ListInfoAndValue(i));
            localInformation.currentObjectOfTheHierarchy = actualList[i];
            return MapperManager.InstantiateSensors(localInformation);
        }

        public ISensors ManageSensors(InstantiationInformation information, ISensors instantiatedSensors)
        {
            if (instantiatedSensors == null)
            {
                return InstantiateSensors(information);
            }
            if (!(instantiatedSensors is ListSensors))
            {
                throw new Exception("Error in sensors generation.");
            }
            ListSensors sensors = (ListSensors)instantiatedSensors;
            if (sensors.sensorsList.Count == 0)
            {
                return InstantiateSensors(information);
            }
            if (information.currentObjectOfTheHierarchy == null && sensors.sensorsList.Count > 0)
            {
                return new ListSensors();
            }
            IList actualList = (IList)information.currentObjectOfTheHierarchy;
            int x = actualList.Count;
            if (x == sensors.sensorsList.Count)
            {
                return instantiatedSensors;
            }
            return GenerateUpdateSensors(information, sensors, actualList, x);
        }

        private ISensors GenerateUpdateSensors(InstantiationInformation information, ListSensors sensors, IList actualList, int x)
        {
            ListSensors toReturn = new ListSensors();
            information.firstPlaceholder += 1;
            UpdateResidualPropertyHierarchy(information.residualPropertyHierarchy);
            for (int i = 0; i < x; i++)
            {
                if (sensors.sensorsList.Count > i)
                {
                    toReturn.sensorsList.Add(sensors.sensorsList[i]);
                }
                else
                {
                    toReturn.sensorsList.Add(InstantiateSensorsForElement(information, actualList, i));
                }
            }
            return toReturn;
        }

        public void UpdateSensor(NewMonoBehaviourSensor sensor, object currentObject, MyListString residualPropertyHierarchy, int hierarchyLevel)
        {
            IList list = (IList)currentObject;
            ListInfoAndValue info = (ListInfoAndValue)sensor.propertyInfo[hierarchyLevel];
            if (list.Count > info.index)
            {
                UpdateResidualPropertyHierarchy(residualPropertyHierarchy);
                MapperManager.UpdateSensor(sensor, list[info.index], residualPropertyHierarchy, hierarchyLevel + 1);
            }
            else
            {
                UnityEngine.Object.Destroy(sensor);
            }
        }
        public string SensorBasicMap(NewMonoBehaviourSensor sensor, object currentObject, int hierarchyLevel, MyListString residualPropertyHierarchy, List<object> valuesForPlaceholders)
        {
            ListInfoAndValue info = (ListInfoAndValue)sensor.propertyInfo[hierarchyLevel];
            valuesForPlaceholders.Add(info.index);
            IList list = (IList)currentObject;
            UpdateResidualPropertyHierarchy(residualPropertyHierarchy);
            return MapperManager.GetSensorBasicMap(sensor, list[info.index], residualPropertyHierarchy, valuesForPlaceholders, hierarchyLevel + 1);
        }
        public void InstantiateActuators(object actualObject, MyListString propertyHierarchy)
        {
            throw new NotImplementedException();
        }
        public void ManageActuators(object actualObject, MyListString propertyHierarchy, List<MonoBehaviourActuatorHider.MonoBehaviourActuator> instantiatedActuators)
        {
            throw new NotImplementedException();
        }

        public string ActuatorBasicMap(object currentObject)
        {
            throw new NotImplementedException();
        }
        public void SetPropertyValue(object actualObject, MyListString propertyHierarchy, object value)
        {
            throw new NotImplementedException();
        }
        
        private void GenerateMapping(ref InstantiationInformation information)
        {
            if (information.mappingDone)
            {
                return;
            }
            string prepend = NewASPMapperHelper.AspFormat(information.residualPropertyHierarchy[0]) + "(";
            string append = ")";
            prepend += "{" + information.firstPlaceholder + "},";
            information.prependMapping.Add(prepend);
            information.appendMapping.Insert(0, append);
        }
        private static void UpdateResidualPropertyHierarchy(MyListString residualPropertyHierarchy)
        {
            residualPropertyHierarchy.RemoveAt(0);
            if (residualPropertyHierarchy.Count > 0)//if it's not a primitive type array
            {
                residualPropertyHierarchy.RemoveAt(0);
            }
        }
    }
}
