using NewMappers.ASPMappers;
using NewMappers.IntermediateMappers;
using NewStructures;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NewMappers.BaseMappers
{
    abstract class Array2Mapper : SensorDataMapper
    {
        #region SUPPORT CLASSES
        internal class Array2InfoAndValue : InfoAndValue
        {
            internal int[] indexes;
            internal object elementCurrentValue;
            internal string mapping;
            internal Array2InfoAndValue(object value)
            {
                elementCurrentValue = value;
                indexes = new int[2];
                mapping = "";
            }
        }
        internal class Array2Sensors : Sensors
        {
            internal NewMonoBehaviourSensor[,] sensorsMatrix;
            internal Array2Sensors(int i, int j)
            {
                sensorsMatrix = new NewMonoBehaviourSensor[i, j];
            }
        }
        #endregion
        internal bool BaseSupport(Type type)
        {
            return type.IsArray && type.GetArrayRank() == 2;
        }
        public Sensors ManageSensors(GameObject gameobject,object actualObject, MyListString propertyHierarchy, MyListString residualPropertyHierarchy, NewSensorConfiguration configuration, Sensors instantiatedSensors)
        {
            if (!(instantiatedSensors is Array2Sensors))
            {
                throw new Exception("Error in sensors generation.");
            }
            Array2Sensors sensors = (Array2Sensors)instantiatedSensors;
            if (actualObject == null && sensors.sensorsMatrix.Length > 0)
            {
                return new Array2Sensors(0,0);
            }
            Array actualMatrix = (Array)actualObject;
            int x = actualMatrix.GetLength(0);
            int y = actualMatrix.GetLength(1);
            if (x == sensors.sensorsMatrix.GetLength(0) && y == sensors.sensorsMatrix.GetLength(1))
            {
                return instantiatedSensors;
            }
            return GenerateUpdateSensors(gameobject, propertyHierarchy, residualPropertyHierarchy, configuration, sensors, actualMatrix, x, y);
        }
        private Sensors GenerateUpdateSensors(GameObject gameobject, MyListString propertyHierarchy, MyListString residualPropertyHierarchy, NewSensorConfiguration configuration, Array2Sensors sensors, Array actualMatrix, int x, int y)
        {
            Array2Sensors toReturn = new Array2Sensors(x, y);
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
                        toReturn.sensorsMatrix[i, j] = gameobject.AddComponent<NewMonoBehaviourSensor>();
                        Array2InfoAndValue info = GetElementProperty(residualPropertyHierarchy, actualMatrix, i, j);
                        info.mapping = GenerateMapping(propertyHierarchy, residualPropertyHierarchy);
                        toReturn.sensorsMatrix[i, j].Configure(configuration.configurationName, propertyHierarchy, actualMatrix.GetType(), info);
                    }
                }
            }
            return toReturn;
        }
        public Sensors InstantiateSensors(GameObject gameobject, object actualObject, MyListString propertyHierarchy, MyListString property, NewSensorConfiguration configuration)
        {
            if (actualObject == null)
            {
                return new Array2Sensors(0, 0);
            }
            Array actualMatrix = (Array)actualObject;
            int x = actualMatrix.GetLength(0);
            int y = actualMatrix.GetLength(1);
            Array2Sensors sensors = new Array2Sensors(x, y);
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    Array2InfoAndValue info = GetElementProperty(property, actualMatrix, i, j);
                    sensors.sensorsMatrix[i, j] = gameobject.AddComponent<NewMonoBehaviourSensor>();
                    info.mapping = GenerateMapping(propertyHierarchy);
                    sensors.sensorsMatrix[i, j].Configure(configuration.configurationName, propertyHierarchy, actualMatrix.GetType(), info);
                }
            }
            return sensors;
        }
        public bool NeedsSpecifications()
        {
            return false;
        }
        /*private static void DeleteExceedingSensors(Array2Sensors sensors, int x, int y)
        {
            for (int i = x; i < sensors.sensorsMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < sensors.sensorsMatrix.GetLength(1); j++)
                {
                    DestroyMonoBehaviours(sensors, i, j);
                }
            }
            for (int i = 0; i < sensors.sensorsMatrix.GetLength(0); i++)
            {
                for (int j = y; j < sensors.sensorsMatrix.GetLength(1); j++)
                {
                    DestroyMonoBehaviours(sensors, i, j);
                }
            }
        }
        private static Sensors DeleteExistingSensors(Array2Sensors sensors)
        {
            for (int i = 0; i < sensors.sensorsMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < sensors.sensorsMatrix.GetLength(1); j++)
                {
                    DestroyMonoBehaviours(sensors, i, j);
                }
            }
            return new Array2Sensors(0, 0);
        }
        private static void DestroyMonoBehaviours(Array2Sensors sensors, int i, int j)
        {
            if (sensors.sensorsMatrix[i, j] != null)
            {
                UnityEngine.Object.Destroy(sensors.sensorsMatrix[i, j]);
            }
        }*/

        #region ABSTRACT METHODS
        protected abstract string GenerateMapping(MyListString propertyHierarchy, MyListString residualPropertyHierarchy=null);

        public abstract Dictionary<MyListString, KeyValuePair<Type, object>> RetrieveProperties(Type objectType, MyListString currentObjectPropertyHierarchy, object currentObject);
        public abstract bool Supports(Type type);
        public abstract bool IsTypeExpandable();
        
        protected abstract Array2InfoAndValue GetElementProperty(MyListString property, Array actualMatrix, int i, int j);
        public abstract void UpdateSensor(NewMonoBehaviourSensor sensor, object currentObject);
        #endregion
        #region UNIMPLEMENTED
        public object GetPropertyValue(object actualObject, MyListString propertyHierarchy)
        {
            throw new NotImplementedException();
        }
        public string SensorBasicMap(NewMonoBehaviourSensor sensor)
        {
            if (!Supports(sensor.currentPropertyType))
            {
                throw new Exception("Type " + sensor.currentPropertyType + " is not supported as Sensor");
            }
            Array2InfoAndValue infoAndValue = (Array2InfoAndValue)sensor.propertyInfo;
            string value = ASPBasicTypeMapper.BasicMapping(infoAndValue.elementCurrentValue);
            return string.Format(infoAndValue.mapping,infoAndValue.indexes[0],infoAndValue.indexes[1], value);

        }


        public string BasicMap(object value)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
