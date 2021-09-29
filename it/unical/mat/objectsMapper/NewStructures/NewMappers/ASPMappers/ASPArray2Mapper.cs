using newMappers;
using NewStructures;
using NewStructures.NewMappers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NewMappers.BaseMappers
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
        }
        private class Array2Sensors : ISensors
        {
            internal ISensors[,] sensorsMatrix;
            internal Array2Sensors(int i, int j)
            {
                sensorsMatrix = new ISensors[i, j];
            }


            public bool IsEmpty()
            {
                return sensorsMatrix.Length == 0;
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
        public List<NewOperationContainer.NewOperation> OperationList()
        {
            throw new NotImplementedException();
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
            rootName.Add(underlyingType.Name);
            Debug.Log("invoking manager with " + underlyingType);
            return MapperManager.RetrieveProperties(underlyingType, rootName, null);
        }

        public ISensors InstantiateSensors(InstantiationInformation information)
        {
            if (information.currentObjectOfTheHierarchy == null)
            {
                return new Array2Sensors(0, 0);
            }
            Array actualMatrix = (Array)information.currentObjectOfTheHierarchy;
            int x = actualMatrix.GetLength(0);
            int y = actualMatrix.GetLength(1);
            Array2Sensors sensors = new Array2Sensors(x, y);
            GenerateMapping(ref information);
            information.firstPlaceholder += 2;
            UpdateResidualPropertyHierarchy(information.residualPropertyHierarchy);
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    sensors.sensorsMatrix[i, j]=InstantiateSensorsForElement(information, actualMatrix, i, j);
                }
            }
            return sensors;
        }
        
        private ISensors InstantiateSensorsForElement(InstantiationInformation information, Array actualMatrix, int i, int j)
        {
            InstantiationInformation localInformation = new InstantiationInformation(information);
            localInformation.hierarchyInfo.Add(new Array2InfoAndValue(i, j));
            localInformation.currentObjectOfTheHierarchy = actualMatrix.GetValue(i, j);
            return MapperManager.InstantiateSensors(localInformation);
        }

        private ISensors GenerateUpdateSensors(InstantiationInformation information, Array2Sensors sensors, Array actualMatrix, int x, int y)
        {
            Array2Sensors toReturn = new Array2Sensors(x, y);
            information.firstPlaceholder += 2;
            UpdateResidualPropertyHierarchy(information.residualPropertyHierarchy);
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

        private static void UpdateResidualPropertyHierarchy(MyListString residualPropertyHierarchy)
        {
            residualPropertyHierarchy.RemoveAt(0);
            if (residualPropertyHierarchy.Count > 0)//if it's not a primitive type array
            {
                residualPropertyHierarchy.RemoveAt(0);
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
        

        public void UpdateSensor(NewMonoBehaviourSensor sensor, object currentObject, MyListString residualPropertyHierarchy, int hierarchyLevel)
        {
            Array matrix = (Array)currentObject;
            Array2InfoAndValue info = (Array2InfoAndValue)sensor.propertyInfo[hierarchyLevel];
            if (matrix.GetLength(0) > info.indexes[0] && matrix.GetLength(1) > info.indexes[1])
            {
                object matrixElement = matrix.GetValue(info.indexes[0], info.indexes[1]);
                UpdateResidualPropertyHierarchy(residualPropertyHierarchy);
                MapperManager.UpdateSensor(sensor, matrixElement, residualPropertyHierarchy, hierarchyLevel + 1);
            }
            else
            {
                UnityEngine.Object.Destroy(sensor);
            }
        }
        public string SensorBasicMap(NewMonoBehaviourSensor sensor, object currentObject, int hierarchyLevel, MyListString residualPropertyHierarchy, List<object> valuesForPlaceholders)
        {
            Array2InfoAndValue info = (Array2InfoAndValue)sensor.propertyInfo[hierarchyLevel];
            valuesForPlaceholders.Add(info.indexes[0]);
            valuesForPlaceholders.Add(info.indexes[1]);
            Array matrix = (Array)currentObject;
            UpdateResidualPropertyHierarchy(residualPropertyHierarchy);
            return MapperManager.GetSensorBasicMap(sensor, matrix.GetValue(info.indexes[0], info.indexes[1]), residualPropertyHierarchy, valuesForPlaceholders, hierarchyLevel + 1);
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






    }
}
