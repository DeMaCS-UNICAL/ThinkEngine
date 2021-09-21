using NewMappers.ASPMappers;
using System;
using System.Collections;
using System.Collections.Generic;
using static NewMappers.NewOperationContainer;

namespace newMappers
{
    internal class ASPUnsignedIntegerMapper : ASPBasicTypeMapper
    {
        private static object Avg(IList values)
        {
            if (values.Count == 0)
            {
                throw new Exception("No values to compute avg on.");
            }
            ulong avg = 0;
            foreach (ulong d in values)
            {
                avg += d;
            }
            avg /= (ulong)values.Count;
            return avg;
        }
        private static object Min(IList values)
        {
            ulong min = ulong.MaxValue;
            foreach (ulong v in values)
            {
                if (v < min)
                {
                    min = v;
                }
            }
            return min;
        }
        private static object Max(IList values)
        {
            ulong max = ulong.MinValue;
            foreach (ulong v in values)
            {
                if (v > max)
                {
                    max = v;
                }
            }
            return max;
        }
        #region SINGLETON FEATURES
        private static DataMapper _instance;
        public static DataMapper instance
        {
            get
            {
                if (_instance is null)
                {
                    _instance = new ASPUnsignedIntegerMapper();

                }
                return _instance;
            }
        }
        private ASPUnsignedIntegerMapper() 
        {
            _operationList = new List<NewOperation>() { Newest, Oldest, Specific_Value, Max, Min, Avg };
            _supportedTypes = new List<Type> { typeof(byte), typeof(ushort), typeof(uint), typeof(ulong) };
            _convertingType = typeof(ulong);
        }
        #endregion
        public override string BasicMap(object currentObject)
        {
            if (!supportedTypes.Contains(currentObject.GetType()))
            {
                throw new Exception("Type " + currentObject.GetType() + " is not supported as Sensor");
            }
            return ""+currentObject+"";
        }
        public override Type GetAggregationTypes()
        {
            return typeof(NumericOperations);
        }
    }
}