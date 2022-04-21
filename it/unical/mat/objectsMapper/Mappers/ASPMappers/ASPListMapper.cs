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
            internal bool needsUpdate;

            internal ListSensors()
            {
                sensorsList = new List<ISensors>();
            }

            public List<Sensor> GetSensorsList()
            {
                List<Sensor> toReturn = new List<Sensor>();
                foreach(ISensors isensors in sensorsList)
                {
                    if (isensors != null)
                    {
                        toReturn.AddRange(isensors.GetSensorsList());
                    }
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
                return null;
            }
            IList actualList = (IList)information.currentObjectOfTheHierarchy;
            int x = actualList.Count;
            ListSensors sensors = new ListSensors();
            if (x == 0)
            {
                return null;
            }
            GenerateMapping(ref information, actualList[0].GetType());
            information.firstPlaceholder += 1;
            UpdateResidualPropertyHierarchy(information.residualPropertyHierarchy, actualList[0].GetType());
            for (int i = 0; i < x; i++)
            {
                sensors.sensorsList.Add(InstantiateSensorsForElement(information, actualList, i));
                if (sensors.sensorsList[i] == null)
                {
                    sensors.needsUpdate = true;
                }
            }
            return sensors;

        }

        private ISensors InstantiateSensorsForElement(InstantiationInformation information, IList actualList, int i)
        {
            InstantiationInformation localInformation = AddLocalInformation(information, actualList, i);
            ISensors toReturn = MapperManager.InstantiateSensors(localInformation);
            if(!information.mappingDone && localInformation.mappingDone)
            {
                information.mappingDone=true;
                information.prependMapping = localInformation.prependMapping;
                information.appendMapping = localInformation.appendMapping;
            }
            return toReturn;
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
            if (!sensors.needsUpdate &&  x == sensors.sensorsList.Count)
            {
                return instantiatedSensors;
            }
            return GenerateUpdateSensors(information, sensors, actualList, x);
        }

        private ISensors GenerateUpdateSensors(InstantiationInformation information, ListSensors sensors, IList actualList, int x)
        {
            ListSensors toReturn = new ListSensors();
            if (x == 0)
            {
                return toReturn;
            }
            if (!information.mappingDone)
            {
                GenerateMapping(ref information, actualList[0].GetType());
            }
            information.firstPlaceholder += 1;
            UpdateResidualPropertyHierarchy(information.residualPropertyHierarchy, actualList[0].GetType());
            for (int i = 0; i < x; i++)
            {
                if (sensors.sensorsList.Count > i && sensors.sensorsList[i]!=null)
                {
                    toReturn.sensorsList.Add(sensors.sensorsList[i]);
                }
                else
                {
                    toReturn.sensorsList.Add(InstantiateSensorsForElement(information, actualList, i));
                    if (toReturn.sensorsList[i] == null)
                    {
                        sensors.needsUpdate = true;
                    }
                }
            }
            return toReturn;
        }

        public void UpdateSensor(Sensor sensor, object currentObject, MyListString residualPropertyHierarchy, int hierarchyLevel)
        {
            IList list = (IList)currentObject;
            ListInfoAndValue info = (ListInfoAndValue)sensor.PropertyInfo[hierarchyLevel];
            if (list.Count > info.index)
            {
                UpdateResidualPropertyHierarchy(residualPropertyHierarchy, list[info.index].GetType());
                MapperManager.UpdateSensor(sensor, list[info.index], residualPropertyHierarchy, hierarchyLevel + 1);
            }
            else
            {
                sensor.Destroy();
            }
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
            if (x == 0)
            {
                return actuators;
            }
            GenerateMapping(ref information, actualList[0].GetType());
            information.firstPlaceholder += 1;
            UpdateResidualPropertyHierarchy(information.residualPropertyHierarchy, actualList[0].GetType());
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
            if (x == 0)
            {
                return new ListActuators();
            }
            if (x == actuators.actuatorsList.Count)
            {
                return instantiatedActuators;
            }
            return GenerateUpdateActuators(information, actuators, actualList, x);
        }

        private IActuators GenerateUpdateActuators(InstantiationInformation information, ListActuators actuators, IList actualList, int x)
        {
            ListActuators toReturn = new ListActuators();
            if (!information.mappingDone)
            {
                GenerateMapping(ref information, actualList[0].GetType());
            }
            information.firstPlaceholder += 1;
            UpdateResidualPropertyHierarchy(information.residualPropertyHierarchy, actualList[0].GetType());
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
            UpdateResidualPropertyHierarchy(residualPropertyHierarchy, list[info.index].GetType());
            return MapperManager.GetActuatorBasicMap(actuator, list[info.index], residualPropertyHierarchy, valuesForPlaceholders, hierarchyLevel + 1);
        }
        public void SetPropertyValue(MonoBehaviourActuator actuator, MyListString residualPropertyHierarchy, object currentObject, object valueToSet, int hierarchyLevel)
        {
            IList list = (IList)currentObject;
            ListInfoAndValue info = (ListInfoAndValue)actuator.PropertyInfo[hierarchyLevel];
            if (list.Count > info.index)
            {
                UpdateResidualPropertyHierarchy(residualPropertyHierarchy, list[info.index].GetType());
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


        private void GenerateMapping(ref InstantiationInformation information, Type elementType)
        {
            if (information.mappingDone)
            {
                return;
            }
            string prepend= "{" + information.firstPlaceholder + "},";
                Debug.Log("TEMPORARY:    " + information.temporaryMapping);
            if (!MapperManager.ExistsMapper(elementType))
            {
                prepend = NewASPMapperHelper.AspFormat(information.residualPropertyHierarchy[0])+"("+information.temporaryMapping+prepend;
                Debug.Log("PREPEND:    " + prepend);
                string append=")";
                information.prependMapping.Add(prepend);
                information.appendMapping.Insert(0, append);
                information.temporaryMapping = "";
            }
            else
            {
                information.temporaryMapping += prepend;
                Debug.Log("TEMPORARY:    " + information.temporaryMapping);
            }
        }
        private static void UpdateResidualPropertyHierarchy(MyListString residualPropertyHierarchy, Type elementType)
        {
            if (MapperManager.ExistsMapper(elementType) && !MapperManager.IsBasic(elementType))
            {
                return;
            }
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
            information.currentType = information.currentType.GetGenericArguments()[0];
            GenerateMapping(ref information, information.currentType);
            information.firstPlaceholder++;
            Debug.Log(information.currentType);
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
            UpdateResidualPropertyHierarchy(information.residualPropertyHierarchy, information.currentType);
            return MapperManager.GetASPTemplate(ref information, variables);

        }
    }
}
