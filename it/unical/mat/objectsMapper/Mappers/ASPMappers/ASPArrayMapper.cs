using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Mappers.BaseMappers
{
    class ASPArrayMapper : IDataMapper
    {
        #region SUPPORT CLASSES
        private class ArrayInfoAndValue : IInfoAndValue
        {
            internal int index;
            internal ArrayInfoAndValue(int i)
            {
                index = i;
            }

            public object GetValuesForPlaceholders()
            {
                return index;
            }
        }
        private class ArraySensors : ISensors
        {
            internal ISensors[] sensorsArray;
            internal ArraySensors(int i)
            {
                sensorsArray = new ISensors[i];
            }

            public List<Sensor> GetSensorsList()
            {
                List<Sensor> toReturn = new List<Sensor>();
                foreach (ISensors isensors in sensorsArray)
                {
                    if (isensors == null)
                    {
                        Debug.LogError("isensors is null");
                    }
                    toReturn.AddRange(isensors.GetSensorsList());
                }
                return toReturn;
            }

            public bool IsEmpty()
            {
                return sensorsArray.Length == 0;
            }
        }
        private class ArrayActuators : IActuators
        {
            internal IActuators[] actuatorsArray;
            internal ArrayActuators(int i)
            {
                actuatorsArray = new IActuators[i];
            }

            public List<MonoBehaviourActuator> GetActuatorsList()
            {
                List<MonoBehaviourActuator> toReturn = new List<MonoBehaviourActuator>();
                foreach (IActuators iactuators in actuatorsArray)
                {
                    toReturn.AddRange(iactuators.GetActuatorsList());
                }
                return toReturn;
            }

            public bool IsEmpty()
            {
                return actuatorsArray.Length == 0;
            }
        }
        #endregion
        private static ASPArrayMapper _instance;
        internal static ASPArrayMapper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ASPArrayMapper();
                }
                return _instance;
            }
        }
        public string ActuatorBasicMap(MonoBehaviourActuator actuator, object currentObject, int hierarchyLevel, MyListString residualPropertyHierarchy, List<object> valuesForPlaceHolders)
        {
            ArrayInfoAndValue info = (ArrayInfoAndValue)actuator.PropertyInfo[hierarchyLevel];
            valuesForPlaceHolders.Add(info.index);
            Array array = (Array)currentObject;
            UpdateResidualPropertyHierarchy(residualPropertyHierarchy, IsMappableElement(array.GetType()));
            return MapperManager.GetActuatorBasicMap(actuator, array.GetValue(info.index), residualPropertyHierarchy, valuesForPlaceHolders, hierarchyLevel + 1);
        }

        private bool IsMappableElement(Type type)
        {
            return MapperManager.ExistsMapper(type.GetElementType());
        }

        private void UpdateResidualPropertyHierarchy(MyListString residualPropertyHierarchy, bool mappable)
        {
            if (residualPropertyHierarchy.Count > 0)
            {
                residualPropertyHierarchy.RemoveAt(0);
                if (mappable && residualPropertyHierarchy.Count > 0)//if it's not a primitive type array
                {
                    residualPropertyHierarchy.RemoveAt(0);
                }
            }
        }

        public int GetAggregationSpecificIndex(Type type)
        {
            return MapperManager.GetAggregationSpecificIndex(type.GetElementType());
        }

        public Type GetAggregationTypes(Type type = null)
        {
            return MapperManager.GetAggregationTypes(type.GetElementType());
        }

        public string GetASPTemplate(ref InstantiationInformation information, List<string> variables)
        {
            variables.Add("Index" + (information.firstPlaceholder + 1));
            GenerateMapping(ref information);
            information.firstPlaceholder += 1;
            bool isMappableElement = IsMappableElement(information.currentType);
            information.currentType = information.currentType.GetElementType();
            if (information.currentObjectOfTheHierarchy != null)
            {
                Array array = (Array)information.currentObjectOfTheHierarchy;
                if (array.Length > 0)
                {
                    information.currentObjectOfTheHierarchy = array.GetValue(0);
                }
                else
                {
                    information.currentObjectOfTheHierarchy = null;
                }
            }
            UpdateResidualPropertyHierarchy(information.residualPropertyHierarchy, isMappableElement);
            return MapperManager.GetASPTemplate(ref information, variables);
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

        public IActuators InstantiateActuators(InstantiationInformation information)
        {
            if (information.currentObjectOfTheHierarchy == null)
            {
                return new ArrayActuators(0);
            }
            Array actualArray = (Array)information.currentObjectOfTheHierarchy;
            int x = actualArray.GetLength(0);
            ArrayActuators actuators = new ArrayActuators(x);
            GenerateMapping(ref information);
            information.firstPlaceholder += 1;
            UpdateResidualPropertyHierarchy(information.residualPropertyHierarchy, IsMappableElement(actualArray.GetType()));
            for (int i = 0; i < x; i++)
            {
                actuators.actuatorsArray[i] = InstantiateActuatorsForElement(information, actualArray, i);
            }
            return actuators;
        }

        private IActuators InstantiateActuatorsForElement(InstantiationInformation information, Array actualArray, int i)
        {
            InstantiationInformation localInformation = AddLocalInformation(information, actualArray, i);
            return MapperManager.InstantiateActuators(localInformation);
        }

        private InstantiationInformation AddLocalInformation(InstantiationInformation information, Array actualArray, int i)
        {
            InstantiationInformation localInformation = new InstantiationInformation(information);
            localInformation.hierarchyInfo.Add(new ArrayInfoAndValue(i));
            localInformation.currentObjectOfTheHierarchy = actualArray.GetValue(i);
            return localInformation;
        }

        public ISensors InstantiateSensors(InstantiationInformation information)
        {
            if (information.currentObjectOfTheHierarchy == null)
            {
                return null;
            }
            Array actualArray = (Array)information.currentObjectOfTheHierarchy;
            int x = actualArray.GetLength(0);
            ArraySensors sensors = new ArraySensors(x);
            GenerateMapping(ref information);
            information.firstPlaceholder += 1;
            UpdateResidualPropertyHierarchy(information.residualPropertyHierarchy, IsMappableElement(actualArray.GetType()));
            for (int i = 0; i < x; i++)
            {
                sensors.sensorsArray[i] = InstantiateSensorsForElement(information, actualArray, i);
            }
            return sensors;
        }

        private ISensors InstantiateSensorsForElement(InstantiationInformation information, Array actualArray, int i)
        {
            InstantiationInformation localInformation = AddLocalInformation(information, actualArray, i);
            return MapperManager.InstantiateSensors(localInformation);
        }

        public bool IsFinal(Type t)
        {
            return MapperManager.IsFinal(t.GetElementType());
        }

        public bool IsTypeExpandable(Type type)
        {
            return MapperManager.IsTypeExpandable(type.GetElementType());
        }

        public IActuators ManageActuators(InstantiationInformation information, IActuators instantiatedActuators)
        {
            if (instantiatedActuators == null)
            {
                return InstantiateActuators(information);
            }
            if (!(instantiatedActuators is ArrayActuators))
            {
                throw new Exception("Error in sensors generation.");
            }
            ArrayActuators actuators = (ArrayActuators)instantiatedActuators;
            if (actuators.actuatorsArray.Length == 0)
            {
                return InstantiateActuators(information);
            }
            if (information.currentObjectOfTheHierarchy == null && actuators.actuatorsArray.Length > 0)
            {
                return new ArrayActuators(0);
            }
            Array actualArray = (Array)information.currentObjectOfTheHierarchy;
            int x = actualArray.GetLength(0);
            if (x == actuators.actuatorsArray.GetLength(0))
            {
                return instantiatedActuators;
            }
            return GenerateUpdateActuators(information, actuators, actualArray, x);
        }

        private IActuators GenerateUpdateActuators(InstantiationInformation information, ArrayActuators actuators, Array actualArray, int x)
        {
            ArrayActuators toReturn = new ArrayActuators(x);
            information.firstPlaceholder += 1;
            UpdateResidualPropertyHierarchy(information.residualPropertyHierarchy, IsMappableElement(actualArray.GetType()));
            for (int i = 0; i < x; i++)
            {
                if (actuators.actuatorsArray.GetLength(0) > i)
                {
                    toReturn.actuatorsArray[i] = actuators.actuatorsArray[i];
                }
                else
                {
                    toReturn.actuatorsArray[i] = InstantiateActuatorsForElement(information, actualArray, i);
                }
            }
            return toReturn;
        }

        public ISensors ManageSensors(InstantiationInformation information, ISensors instantiatedSensors)
        {
            if (instantiatedSensors == null)
            {
                return InstantiateSensors(information);
            }
            if (!(instantiatedSensors is ArraySensors))
            {
                throw new Exception("Error in sensors generation.");
            }
            ArraySensors sensors = (ArraySensors)instantiatedSensors;
            if (sensors.sensorsArray.Length == 0)
            {
                return InstantiateSensors(information);
            }
            if (information.currentObjectOfTheHierarchy == null && sensors.sensorsArray.Length > 0)
            {
                return new ArraySensors(0);
            }
            Array actualArray = (Array)information.currentObjectOfTheHierarchy;
            int x = actualArray.GetLength(0);
            if (x == sensors.sensorsArray.GetLength(0))
            {
                return instantiatedSensors;
            }
            return GenerateUpdateSensors(information, sensors, actualArray, x);
        }

        private ISensors GenerateUpdateSensors(InstantiationInformation information, ArraySensors sensors, Array actualArray, int x)
        {
            ArraySensors toReturn = new ArraySensors(x);
            information.firstPlaceholder += 2;
            UpdateResidualPropertyHierarchy(information.residualPropertyHierarchy, IsMappableElement(actualArray.GetType()));
            for (int i = 0; i < x; i++)
            {
                if (sensors.sensorsArray.GetLength(0) > i)
                {
                    toReturn.sensorsArray[i] = sensors.sensorsArray[i];
                }
                else
                {
                    toReturn.sensorsArray[i] = InstantiateSensorsForElement(information, actualArray, i);
                }
            }
            return toReturn;
        }

        public bool NeedsAggregates(Type type)
        {
            return MapperManager.NeedsAggregates(type.GetElementType());
        }

        public bool NeedsSpecifications(Type type)
        {
            return true;
        }

        public Dictionary<MyListString, KeyValuePair<Type, object>> RetrieveProperties(Type objectType, MyListString currentObjectPropertyHierarchy, object currentObject)
        {
            if (!Supports(objectType))
            {
                throw new Exception(objectType + " is not supported.");
            }
            Type underlyingType = objectType.GetElementType();
            MyListString rootName = new MyListString(currentObjectPropertyHierarchy.myStrings);
            if (!MapperManager.ExistsMapper(underlyingType))
            {
                rootName.Add(underlyingType.Name);
            }
            return MapperManager.RetrieveProperties(underlyingType, rootName, null);
        }

        public string SensorBasicMap(Sensor sensor, object currentObject, int hierarchyLevel, MyListString residualPropertyHierarchy, List<object> valuesForPlaceholders)
        {
            ArrayInfoAndValue info = (ArrayInfoAndValue)sensor.PropertyInfo[hierarchyLevel];
            valuesForPlaceholders.Add(info.index);
            Array array = (Array)currentObject;
            UpdateResidualPropertyHierarchy(residualPropertyHierarchy, IsMappableElement(array.GetType()));
            return MapperManager.GetSensorBasicMap(sensor, array.GetValue(info.index), residualPropertyHierarchy, valuesForPlaceholders, hierarchyLevel + 1);
        }

        public void SetPropertyValue(MonoBehaviourActuator actuator, MyListString residualPropertyHierarchy, object currentObject, object valueToSet, int hierarchyLevel)
        {
            Array array = (Array)currentObject;
            ArrayInfoAndValue info = (ArrayInfoAndValue)actuator.PropertyInfo[hierarchyLevel];
            if (array.GetLength(0) > info.index)
            {
                UpdateResidualPropertyHierarchy(residualPropertyHierarchy, IsMappableElement(array.GetType()));
                if (MapperManager.IsBasic(array.GetType().GetElementType()))
                {
                    array.SetValue(Convert.ChangeType(valueToSet, array.GetType().GetElementType()), info.index);
                    return;
                }
                currentObject = array.GetValue(info.index);
                MapperManager.SetPropertyValue(actuator, residualPropertyHierarchy, currentObject, valueToSet, hierarchyLevel + 1);
            }
            else
            {
                UnityEngine.Object.Destroy(actuator);
            }
        }

        public bool Supports(Type type)
        {
            return type.IsArray && type.GetArrayRank() == 1;
        }

        public void UpdateSensor(Sensor sensor, object currentObject, MyListString residualPropertyHierarchy, int hierarchyLevel)
        {
            Array array = (Array)currentObject;
            ArrayInfoAndValue info = (ArrayInfoAndValue)sensor.PropertyInfo[hierarchyLevel];
            if (array.GetLength(0) > info.index)
            {
                object arrayElement = array.GetValue(info.index);
                UpdateResidualPropertyHierarchy(residualPropertyHierarchy, IsMappableElement(array.GetType()));
                MapperManager.UpdateSensor(sensor, arrayElement, residualPropertyHierarchy, hierarchyLevel + 1);
            }
            else
            {
                sensor.Destroy(); ;
            }
        }
    }
}
