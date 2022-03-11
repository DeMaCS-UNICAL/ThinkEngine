using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mappers.BaseMappers
{
    class ASPArray2Mapper : IDataMapper
    {
        #region SUPPORT CLASSES
        private class Array2InfoAndValue : IInfoAndValue
        {
            internal int[] indexes;
            internal Array2InfoAndValue(int i, int j)
            {
                indexes = new int[2] { i, j };
            }

            public object GetValuesForPlaceholders()
            {
                return indexes;
            }
        }
        private class Array2Sensors : ISensors
        {
            internal ISensors[,] sensorsMatrix;
            internal Array2Sensors(int i, int j)
            {
                sensorsMatrix = new ISensors[i, j];
            }

            public List<Sensor> GetSensorsList()
            {
                List<Sensor> toReturn = new List<Sensor>();
                foreach(ISensors isensors in sensorsMatrix)
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
                return sensorsMatrix.Length == 0;
            }
        }
        private class Array2Actuators : IActuators
        {
            internal IActuators[,] actuatorsMatrix;
            internal Array2Actuators(int i, int j)
            {
                actuatorsMatrix = new IActuators[i, j];
            }

            public List<MonoBehaviourActuator> GetActuatorsList()
            {
                List<MonoBehaviourActuator> toReturn = new List<MonoBehaviourActuator>();
                foreach (IActuators iactuators in actuatorsMatrix)
                {
                    toReturn.AddRange(iactuators.GetActuatorsList());
                }
                return toReturn;
            }

            public bool IsEmpty()
            {
                return actuatorsMatrix.Length == 0;
            }
        }
        #endregion
        private static ASPArray2Mapper _instance;
        internal static ASPArray2Mapper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ASPArray2Mapper();
                }
                return _instance;
            }
        }
        public bool NeedsSpecifications(Type type)
        {
            return true;
        }

        public bool IsFinal(Type type)
        {
            return MapperManager.IsFinal(type.GetElementType());
        }
        public bool Supports(Type type)
        {
            return type.IsArray && type.GetArrayRank() == 2;
        }
        public bool IsTypeExpandable(Type type)
        {
            return MapperManager.IsTypeExpandable(type.GetElementType());
        }

        public bool NeedsAggregates(Type type)
        {
            return MapperManager.NeedsAggregates(type.GetElementType());
        }

        public Type GetAggregationTypes(Type type)
        {
            return MapperManager.GetAggregationTypes(type.GetElementType());
        }

        public int GetAggregationSpecificIndex(Type type)
        {
            return MapperManager.GetAggregationSpecificIndex(type.GetElementType());
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

        public ISensors InstantiateSensors(InstantiationInformation information)
        {
            if (information.currentObjectOfTheHierarchy == null)
            {
                return null;
            }
            Array actualMatrix = (Array)information.currentObjectOfTheHierarchy;
            int x = actualMatrix.GetLength(0);
            int y = actualMatrix.GetLength(1);
            Array2Sensors sensors = new Array2Sensors(x, y);
            GenerateMapping(ref information);
            information.firstPlaceholder += 2;
            UpdateResidualPropertyHierarchy(information.residualPropertyHierarchy, IsMappableElement(actualMatrix.GetType()));
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    sensors.sensorsMatrix[i, j]=InstantiateSensorsForElement(information, actualMatrix, i, j);
                }
            }
            return sensors;
        }

        private bool IsMappableElement(Type matrixType)
        {
            return MapperManager.ExistsMapper(matrixType.GetElementType());
        }

        private ISensors InstantiateSensorsForElement(InstantiationInformation information, Array actualMatrix, int i, int j)
        {
            InstantiationInformation localInformation = AddLocalInformation(information, actualMatrix, i, j);
            ISensors toReturn = MapperManager.InstantiateSensors(localInformation);
            if (!information.mappingDone && localInformation.mappingDone)
            {
                information.mappingDone = true;
                information.prependMapping = localInformation.prependMapping;
                information.appendMapping = localInformation.appendMapping;
            }
            return toReturn;
        }


        private ISensors GenerateUpdateSensors(InstantiationInformation information, Array2Sensors sensors, Array actualMatrix, int x, int y)
        {
            Array2Sensors toReturn = new Array2Sensors(x, y);
            if (!information.mappingDone)
            {
                GenerateMapping(ref information);
            }
            information.firstPlaceholder += 2;
            UpdateResidualPropertyHierarchy(information.residualPropertyHierarchy, IsMappableElement(actualMatrix.GetType()));
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    if (sensors.sensorsMatrix.GetLength(0) > i && sensors.sensorsMatrix.GetLength(0) > j)
                    {
                        toReturn.sensorsMatrix[i, j] = sensors.sensorsMatrix[i, j];
                    }
                    else
                    {
                        toReturn.sensorsMatrix[i, j] = InstantiateSensorsForElement(information, actualMatrix, i, j);
                    }
                }
            }
            return toReturn;
        }

        private static void UpdateResidualPropertyHierarchy(MyListString residualPropertyHierarchy, bool mappable)
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

        public ISensors ManageSensors(InstantiationInformation information, ISensors instantiatedSensors)
        {
            if (instantiatedSensors == null)
            {
                return InstantiateSensors(information);
            }
            if (!(instantiatedSensors is Array2Sensors))
            {
                throw new Exception("Error in sensors generation.");
            }
            Array2Sensors sensors = (Array2Sensors)instantiatedSensors;
            if (sensors.sensorsMatrix.Length == 0)
            {
                return InstantiateSensors(information);
            }
            if (information.currentObjectOfTheHierarchy == null && sensors.sensorsMatrix.Length > 0)
            {
                return new Array2Sensors(0, 0);
            }
            Array actualMatrix = (Array)information.currentObjectOfTheHierarchy;
            int x = actualMatrix.GetLength(0);
            int y = actualMatrix.GetLength(1);
            if (x == sensors.sensorsMatrix.GetLength(0) && y == sensors.sensorsMatrix.GetLength(1))
            {
                return instantiatedSensors;
            }
            return GenerateUpdateSensors(information, sensors, actualMatrix, x, y);
        }
        

        public void UpdateSensor(Sensor sensor, object currentObject, MyListString residualPropertyHierarchy, int hierarchyLevel)
        {
            Array matrix = (Array)currentObject;
            Array2InfoAndValue info = (Array2InfoAndValue)sensor.PropertyInfo[hierarchyLevel];
            if (matrix.GetLength(0) > info.indexes[0] && matrix.GetLength(1) > info.indexes[1])
            {
                object matrixElement = matrix.GetValue(info.indexes[0], info.indexes[1]);
                UpdateResidualPropertyHierarchy(residualPropertyHierarchy, IsMappableElement(matrix.GetType()));
                MapperManager.UpdateSensor(sensor, matrixElement, residualPropertyHierarchy, hierarchyLevel + 1);
            }
            else
            {
               sensor.Destroy();
            }
        }
        public string SensorBasicMap(Sensor sensor, object currentObject, int hierarchyLevel, MyListString residualPropertyHierarchy, List<object> valuesForPlaceholders)
        {
            Array2InfoAndValue info = (Array2InfoAndValue)sensor.PropertyInfo[hierarchyLevel];
            valuesForPlaceholders.Add(info.indexes[0]);
            valuesForPlaceholders.Add(info.indexes[1]);
            Array matrix = (Array)currentObject;
            UpdateResidualPropertyHierarchy(residualPropertyHierarchy, IsMappableElement(matrix.GetType()));
            return MapperManager.GetSensorBasicMap(sensor, matrix.GetValue(info.indexes[0], info.indexes[1]), residualPropertyHierarchy, valuesForPlaceholders, hierarchyLevel + 1);
        }

        public IActuators InstantiateActuators(InstantiationInformation information)
        {
            if (information.currentObjectOfTheHierarchy == null)
            {
                return new Array2Actuators(0, 0);
            }
            Array actualMatrix = (Array)information.currentObjectOfTheHierarchy;
            int x = actualMatrix.GetLength(0);
            int y = actualMatrix.GetLength(1);
            Array2Actuators actuators = new Array2Actuators(x, y);
            GenerateMapping(ref information);
            information.firstPlaceholder += 2;
            UpdateResidualPropertyHierarchy(information.residualPropertyHierarchy, IsMappableElement(actualMatrix.GetType()));
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    actuators.actuatorsMatrix[i, j] = InstantiateActuatorsForElement(information, actualMatrix, i, j);
                }
            }
            return actuators;
        }

        private IActuators InstantiateActuatorsForElement(InstantiationInformation information, Array actualMatrix, int i, int j)
        {
            InstantiationInformation localInformation = AddLocalInformation(information, actualMatrix, i, j);
            return MapperManager.InstantiateActuators(localInformation);
        }

        public IActuators ManageActuators(InstantiationInformation information, IActuators instantiatedActuators)
        {
            if (instantiatedActuators == null)
            {
                return InstantiateActuators(information);
            }
            if (!(instantiatedActuators is Array2Actuators))
            {
                throw new Exception("Error in sensors generation.");
            }
            Array2Actuators actuators = (Array2Actuators)instantiatedActuators;
            if (actuators.actuatorsMatrix.Length == 0)
            {
                return InstantiateActuators(information);
            }
            if (information.currentObjectOfTheHierarchy == null && actuators.actuatorsMatrix.Length > 0)
            {
                return new Array2Actuators(0, 0);
            }
            Array actualMatrix = (Array)information.currentObjectOfTheHierarchy;
            int x = actualMatrix.GetLength(0);
            int y = actualMatrix.GetLength(1);
            if (x == actuators.actuatorsMatrix.GetLength(0) && y == actuators.actuatorsMatrix.GetLength(1))
            {
                return instantiatedActuators;
            }
            return GenerateUpdateActuators(information, actuators, actualMatrix, x, y);
        }

        private IActuators GenerateUpdateActuators(InstantiationInformation information, Array2Actuators actuators, Array actualMatrix, int x, int y)
        {
            Array2Actuators toReturn = new Array2Actuators(x, y);
            if (!information.mappingDone)
            {
                GenerateMapping(ref information);
            }
            information.firstPlaceholder += 2;
            UpdateResidualPropertyHierarchy(information.residualPropertyHierarchy,IsMappableElement(actualMatrix.GetType()));
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    if (actuators.actuatorsMatrix.GetLength(0) > i && actuators.actuatorsMatrix.GetLength(0) > j)
                    {
                        toReturn.actuatorsMatrix[i, j] = actuators.actuatorsMatrix[i, j];
                    }
                    else
                    {
                        toReturn.actuatorsMatrix[i, j] = InstantiateActuatorsForElement(information, actualMatrix, i, j);
                    }
                }
            }
            return toReturn;
        }

        public string ActuatorBasicMap(MonoBehaviourActuator actuator, object currentObject, int hierarchyLevel, MyListString residualPropertyHierarchy, List<object> valuesForPlaceholders)
        {
            Array2InfoAndValue info = (Array2InfoAndValue)actuator.PropertyInfo[hierarchyLevel];
            valuesForPlaceholders.Add(info.indexes[0]);
            valuesForPlaceholders.Add(info.indexes[1]);
            Array matrix = (Array)currentObject;
            UpdateResidualPropertyHierarchy(residualPropertyHierarchy, IsMappableElement(matrix.GetType()));
            return MapperManager.GetActuatorBasicMap(actuator, matrix.GetValue(info.indexes[0], info.indexes[1]), residualPropertyHierarchy, valuesForPlaceholders, hierarchyLevel + 1);

        }
        public void SetPropertyValue(MonoBehaviourActuator actuator, MyListString residualPropertyHierarchy,  object currentObject, object valueToSet, int hierarchyLevel)
        {
            Array matrix = (Array)currentObject;
            Array2InfoAndValue info = (Array2InfoAndValue)actuator.PropertyInfo[hierarchyLevel];
            if (matrix.GetLength(0) > info.indexes[0] && matrix.GetLength(1) > info.indexes[1])
            {
                UpdateResidualPropertyHierarchy(residualPropertyHierarchy,IsMappableElement(matrix.GetType()));
                if (MapperManager.IsBasic(matrix.GetType().GetElementType()))
                {
                    matrix.SetValue(Convert.ChangeType(valueToSet, matrix.GetType().GetElementType()), info.indexes[0], info.indexes[1]);
                    return;
                }
                currentObject = matrix.GetValue(info.indexes[0], info.indexes[1]);
                MapperManager.SetPropertyValue(actuator, residualPropertyHierarchy,  currentObject, valueToSet, hierarchyLevel + 1);
            }
            else
            {
                UnityEngine.Object.Destroy(actuator);
            }
        }

        private static InstantiationInformation AddLocalInformation(InstantiationInformation information, Array actualMatrix, int i, int j)
        {
            InstantiationInformation localInformation = new InstantiationInformation(information);
            localInformation.hierarchyInfo.Add(new Array2InfoAndValue(i, j));
            localInformation.currentObjectOfTheHierarchy = actualMatrix.GetValue(i, j);
            return localInformation;
        }
        protected void GenerateMapping(ref InstantiationInformation information)
        {
            if (information.mappingDone)
            {
                return;
            }
            string prepend = NewASPMapperHelper.AspFormat(information.residualPropertyHierarchy[0]) + "(";
            string append = ")";
            prepend += "{" + information.firstPlaceholder + "},{" + (information.firstPlaceholder + 1) + "},";
            information.prependMapping.Add(prepend);
            information.appendMapping.Insert(0, append);
        }

        public string GetASPTemplate(ref InstantiationInformation information, List<string> variables)
        {
            variables.Add("Index" + (information.firstPlaceholder+1));
            variables.Add("Index" + (information.firstPlaceholder+2));
            GenerateMapping(ref information);
            information.firstPlaceholder+=2;
            bool isMappableElement = IsMappableElement(information.currentType);
            information.currentType = information.currentType.GetElementType();
            if (information.currentObjectOfTheHierarchy != null)
            {
                Array matrix = (Array)information.currentObjectOfTheHierarchy;
                if (matrix.Length > 0)
                {
                    information.currentObjectOfTheHierarchy = matrix.GetValue(0, 0);
                }
                else
                {
                    information.currentObjectOfTheHierarchy = null;
                }
            }
            UpdateResidualPropertyHierarchy(information.residualPropertyHierarchy,isMappableElement);
            return MapperManager.GetASPTemplate(ref information, variables);
        }
    }
}
