using System;
using System.Collections;
using System.Collections.Generic;
using static Mappers.OperationContainer;

namespace Mappers
{
    internal class ASPFloatingPointMapper : BasicTypeMapper
    {
        private static object Avg(IList values)
        {
            if (values.Count == 0)
            {
                throw new Exception("No values to compute avg on.");
            }
            double avg = 0;
            foreach (double d in values)
            {
                avg += d;
            }
            avg /= values.Count;
            return avg;
        }
        private static object Min(IList values)
        {
            double min = double.MaxValue;
            foreach (double v in values)
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
            double max = double.MinValue;
            foreach (double v in values)
            {
                if (v > max)
                {
                    max = v;
                }
            }
            return max;
        }
        #region SINGLETON FEATURES
        private static IDataMapper _instance;
        public static IDataMapper Instance
        {
            get
            {
                if (_instance is null)
                {
                    _instance = new ASPFloatingPointMapper();

                }
                return _instance;
            }
        }
        private ASPFloatingPointMapper() 
        {
            _operationList = new List<Operation>() { Newest, Oldest, Specific_Value, Max, Min, Avg };
            _supportedTypes = new List<Type> { typeof(double), typeof(float) };
            _convertingType = typeof(double);
        }
        #endregion

        public override string BasicMap(object currentObject)
        {
            if (!SupportedTypes.Contains(currentObject.GetType()))
            {
                throw new Exception("Type " + currentObject.GetType() + " is not supported as Sensor");
            }
            return "\"" + currentObject + "\"";
        }

        public override Type GetAggregationTypes(Type type)
        {
            return typeof(NumericOperations);
        }
    }
}