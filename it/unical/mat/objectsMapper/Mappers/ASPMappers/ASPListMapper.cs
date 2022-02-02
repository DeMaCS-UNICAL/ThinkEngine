using Mappers.BaseMappers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mappers.ASPMappers
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

            public object GetValuesForPlaceholders()
            {
                return index;
            }
        }
        private class ListSensors : ISensors
        {
            internal List<ISensors> sensorsList;
            internal ListSensors()
            {
                sensorsList = new List<ISensors>();
            }

            public List<MonoBehaviourSensor> GetSensorsList()
            {
                List<MonoBehaviourSensor> toReturn = new List<MonoBehaviourSensor>();
                foreach(ISensors isensors in sensorsList)
                {
                    toReturn.AddRange(isensors.GetSensorsList());
                }
                return toReturn;
            }

            public bool IsEmpty()
            {
                return sensorsList.Count == 0;
            }
        }
        private class ListActuators : IActuators
        {
            internal List<IActuators> actuatorsList;
            internal ListActuators()
            {
                actuatorsList = new List<IActuators>();
            }

            public List<MonoBehaviourActuator> GetActuatorsList()
            {
                List<MonoBehaviourActuator> toReturn = new List<MonoBehaviourActuator>();
                foreach (IActuators iactuators in actuatorsList)
                {
                    toReturn.AddRange(iactuators.GetActuatorsList());
                }
                return toReturn;
            }

            public bool IsEmpty()
            {
                return actuatorsList.Count == 0;
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

        public List<OperationContainer.Operation> OperationList()
        {
            throw new NotSupportedException();
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
            if (!MapperManager.ExistsMapper(underlyingType))
            {
                rootName.Add(underlyingType.Name);
            }
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
            InstantiationInformation localInformation = AddLocalInformation(information, actualList, i);
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

        public void UpdateSensor(MonoBehaviourSensor sensor, object currentObject, MyListString residualPropertyHierarchy, int hierarchyLevel)
        {
            IList list = (IList)currentObject;
            ListInfoAndValue info = (ListInfoAndValue)sensor.PropertyInfo[hierarchyLevel];
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
        public string SensorBasicMap(MonoBehaviourSensor sensor, object currentObject, int hierarchyLevel, MyListString residualPropertyHierarchy, List<object> valuesForPlaceholders)
        {
            ListInfoAndValue info = (ListInfoAndValue)sensor.PropertyInfo[hierarchyLevel];
            valuesForPlaceholders.Add(info.index);
            IList list = (IList)currentObject;
            UpdateResidualPropertyHierarchy(residualPropertyHierarchy);
            return MapperManager.GetSensorBasicMap(sensor, list[info.index], residualPropertyHierarchy, valuesForPlaceholders, hierarchyLevel + 1);
        }
        public IActuators InstantiateActuators(InstantiationInformation information)
        {
            if (information.currentObjectOfTheHierarchy == null)
            {
                return new ListActuators();
            }
            IList actualList = (IList)information.currentObjectOfTheHierarchy;
            int x = actualList.Count;
            ListActuators actuators = new ListActuators();
            GenerateMapping(ref information);
            information.firstPlaceholder += 1;
            UpdateResidualPropertyHierarchy(information.residualPropertyHierarchy);
            for (int i = 0; i < x; i++)
            {
                actuators.actuatorsList.Add(InstantiateActuatorsForElement(information, actualList, i));
            }
            return actuators;
        }
        private IActuators InstantiateActuatorsForElement(InstantiationInformation information, IList actualList, int i)
        {
            InstantiationInformation localInformation = AddLocalInformation(information, actualList, i);
            return MapperManager.InstantiateActuators(localInformation);
        }

        public IActuators ManageActuators(InstantiationInformation information, IActuators instantiatedActuators)
        {
            if (instantiatedActuators == null)
            {
                return InstantiateActuators(information);
            }
            if (!(instantiatedActuators is ListActuators))
            {
                throw new Exception("Error in sensors generation.");
            }
            ListActuators actuators = (ListActuators)instantiatedActuators;
            if (actuators.actuatorsList.Count == 0)
            {
                return InstantiateActuators(information);
            }
            if (information.currentObjectOfTheHierarchy == null && actuators.actuatorsList.Count > 0)
            {
                return new ListActuators();
            }
            IList actualList = (IList)information.currentObjectOfTheHierarchy;
            int x = actualList.Count;
            if (x == actuators.actuatorsList.Count)
            {
                return instantiatedActuators;
            }
            return GenerateUpdateActuators(information, actuators, actualList, x);
        }

        private IActuators GenerateUpdateActuators(InstantiationInformation information, ListActuators actuators, IList actualList, int x)
        {
            ListActuators toReturn = new ListActuators();
            information.firstPlaceholder += 1;
            UpdateResidualPropertyHierarchy(information.residualPropertyHierarchy);
            for (int i = 0; i < x; i++)
            {
                if (actuators.actuatorsList.Count > i)
                {
                    toReturn.actuatorsList.Add(actuators.actuatorsList[i]);
                }
                else
                {
                    toReturn.actuatorsList.Add(InstantiateActuatorsForElement(information, actualList, i));
                }
            }
            return toReturn;
        }

        public string ActuatorBasicMap(MonoBehaviourActuator actuator, object currentObject, int hierarchyLevel, MyListString residualPropertyHierarchy, List<object> valuesForPlaceholders)
        {
            ListInfoAndValue info = (ListInfoAndValue)actuator.PropertyInfo[hierarchyLevel];
            valuesForPlaceholders.Add(info.index);
            IList list = (IList)currentObject;
            UpdateResidualPropertyHierarchy(residualPropertyHierarchy);
            return MapperManager.GetActuatorBasicMap(actuator, list[info.index], residualPropertyHierarchy, valuesForPlaceholders, hierarchyLevel + 1);
        }
        public void SetPropertyValue(MonoBehaviourActuator actuator, MyListString residualPropertyHierarchy, object currentObject, object valueToSet, int hierarchyLevel)
        {
            IList list = (IList)currentObject;
            ListInfoAndValue info = (ListInfoAndValue)actuator.PropertyInfo[hierarchyLevel];
            if (list.Count > info.index)
            {
                UpdateResidualPropertyHierarchy(residualPropertyHierarchy);
                if (MapperManager.IsBasic(list.GetType().GenericTypeArguments[0]))
                {
                    list[info.index]= Convert.ChangeType(valueToSet, list.GetType().GenericTypeArguments[0]);
                    return;
                }
                currentObject = list[info.index];
                MapperManager.SetPropertyValue(actuator, residualPropertyHierarchy, currentObject, valueToSet, hierarchyLevel + 1);
            }
            else
            {
                UnityEngine.Object.Destroy(actuator);
            }
        }

        private static InstantiationInformation AddLocalInformation(InstantiationInformation information, IList actualList, int i)
        {
            InstantiationInformation localInformation = new InstantiationInformation(information);
            localInformation.hierarchyInfo.Add(new ListInfoAndValue(i));
            localInformation.currentObjectOfTheHierarchy = actualList[i];
            return localInformation;
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
            if (residualPropertyHierarchy.Count > 0)
            {
                residualPropertyHierarchy.RemoveAt(0);
                if (residualPropertyHierarchy.Count > 0)//if it's not a primitive type array
                {
                    residualPropertyHierarchy.RemoveAt(0);
                }
            }
        
        }

        public string GetASPTemplate(ref InstantiationInformation information, List<string> variables)
        {
            variables.Add("Index" + (information.firstPlaceholder+1));
            GenerateMapping(ref information);
            information.firstPlaceholder++;
            information.currentType = information.currentType.GetGenericArguments()[0];
            if (information.currentObjectOfTheHierarchy != null )
            {
                IList list = (IList)information.currentObjectOfTheHierarchy;
                if (list.Count > 0)
                {
                    information.currentObjectOfTheHierarchy = list[0];
                }
                else
                {
                    information.currentObjectOfTheHierarchy = null;
                }
            }
            UpdateResidualPropertyHierarchy(information.residualPropertyHierarchy);
            return MapperManager.GetASPTemplate(ref information, variables);

        }
    }
}
