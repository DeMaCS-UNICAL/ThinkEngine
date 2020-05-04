using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EmbASP4Unity.it.unical.mat.objectsMapper.SensorsScripts
{
    static class Operation 
    {
        public static Dictionary<Type, Type> operationsPerType;
        private static Dictionary<Type, Dictionary<int, Action>> actionPerOperation;
        private static IList values;
        private static object returnValue;
        private static string valueToTrack;
        public enum boolOperations { Newest, Oldest,Specific_Value,Conjunction,Disjunction };
        public enum stringOperations { Newest, Oldest, Specific_Value };//, Mode};
        public enum numericOperations { Newest, Oldest,  Specific_Value, Max,Min,Avg};
        public enum matrixOperations {All}
        public static readonly int SPECIFIC = (int)boolOperations.Specific_Value;

        static Operation()
        {
            operationsPerType = new Dictionary<Type, Type>();
            actionPerOperation = new Dictionary<Type, Dictionary<int, Action>>();
            operationsPerType.Add(typeof(bool), typeof(boolOperations));
            operationsPerType.Add(typeof(string), typeof(stringOperations));
            foreach (Type t in ReflectionExecutor.SignedIntegerTypes())
            {
                operationsPerType.Add(t, typeof(numericOperations));
            }
            foreach (Type t in ReflectionExecutor.UnsignedIntegerTypes())
            {
                operationsPerType.Add(t, typeof(numericOperations));
            }
            foreach (Type t in ReflectionExecutor.FloatingPointTypes())
            {
                operationsPerType.Add(t, typeof(numericOperations));
            }
            operationsPerType.Add(typeof(Enum), typeof(stringOperations));
            operationsPerType.Add(typeof(char), typeof(stringOperations));
            actionPerOperation.Add(typeof(stringOperations), new Dictionary<int, Action>());
            actionPerOperation.Add(typeof(boolOperations), new Dictionary<int, Action>());
            actionPerOperation.Add(typeof(numericOperations), new Dictionary<int, Action>());
            actionPerOperation.Add(typeof(matrixOperations), new Dictionary<int, Action>());
            actionPerOperation[typeof(numericOperations)].Add(0, newest);
            actionPerOperation[typeof(numericOperations)].Add(1, oldest);
            actionPerOperation[typeof(numericOperations)].Add(2, specificValue);
            actionPerOperation[typeof(numericOperations)].Add(3, max);
            actionPerOperation[typeof(numericOperations)].Add(4, min);
            actionPerOperation[typeof(numericOperations)].Add(5, avg);
            actionPerOperation[typeof(matrixOperations)].Add(0, allMatrix);
            actionPerOperation[typeof(boolOperations)].Add(0, newest);
            actionPerOperation[typeof(boolOperations)].Add(1, oldest);
            actionPerOperation[typeof(boolOperations)].Add(2, specificValue);
            actionPerOperation[typeof(boolOperations)].Add(3, conjunction);
            actionPerOperation[typeof(boolOperations)].Add(4, disjunction);
            actionPerOperation[typeof(stringOperations)].Add(0, newest);
            actionPerOperation[typeof(stringOperations)].Add(1, oldest);
            actionPerOperation[typeof(stringOperations)].Add(2, specificValue);



        }

        internal static Type getOperationsPerType(Type type)
        {
            
            if (operationsPerType.ContainsKey(type))
            {
                return operationsPerType[type];
            }
            return typeof(matrixOperations);
        }

        internal static object compute(int op, object value)
        {
            values = (IList)value;
            Type typeForOperation = values.GetType().GetGenericArguments()[0];
            Type enumType = typeof(object);
            if (operationsPerType.ContainsKey(typeForOperation))
            {
                enumType = operationsPerType[typeForOperation];

            }
            else
            {
                if(typeForOperation.IsArray && typeForOperation.GetArrayRank() == 2)
                {
                    enumType = typeof(matrixOperations);
                }
            }
            actionPerOperation[enumType][op]();
            return returnValue;
        }

        internal static void avg()
        {
            Type valuesType = values.GetType().GetGenericArguments()[0];
            if (ReflectionExecutor.SignedIntegerTypes().Contains(valuesType))
            {
                longAvg();
            }else if (ReflectionExecutor.FloatingPointTypes().Contains(valuesType))
            {
                doubleAvg();
            }
            else if (ReflectionExecutor.UnsignedIntegerTypes().Contains(valuesType))
            {
                unsignedAvg();
            }

        }

        private static void unsignedAvg()
        {
            ulong avg = 0;
            foreach (ulong d in values)
            {
                avg += d;
            }
            avg /= (ulong)values.Count;
            returnValue = avg;
        }

        private static void doubleAvg()
        {
            double avg = 0;
            foreach(double d in values)
            {
                avg += d;
            }
            avg /= values.Count;
            returnValue = avg;
        }

        private static void longAvg()
        {
            long avg = 0;
            foreach (long d in values)
            {
                avg += d;
            }
            avg /= (long)values.Count;
            returnValue = avg;
        }

        internal static void min()
        {
            Type valuesType = values.GetType().GetGenericArguments()[0];
            if (ReflectionExecutor.SignedIntegerTypes().Contains(valuesType))
            {
                longMin();
            }
            else if (ReflectionExecutor.FloatingPointTypes().Contains(valuesType))
            {
                doubleMin();
            }
            else if (ReflectionExecutor.UnsignedIntegerTypes().Contains(valuesType))
            {
                unsignedMin();
            }
        }

        private static void unsignedMin()
        {
            ulong min = ulong.MaxValue;
            foreach(ulong v in values)
            {
                if (v < min)
                {
                    min = v;
                }
            }
            returnValue = min;
        }

        private static void doubleMin()
        {
            double min = double.MaxValue;
            foreach (double v in values)
            {
                if (v < min)
                {
                    min = v;
                }
            }
            returnValue = min;
        }

        private static void longMin()
        {
            long min = long.MaxValue;
            foreach (long v in values)
            {
                if (v < min)
                {
                    min = v;
                }
            }
            returnValue = min;
        }

        internal static void max()
        {
            Type valuesType = values.GetType().GetGenericArguments()[0];
            if (ReflectionExecutor.SignedIntegerTypes().Contains(valuesType))
            {
                longMax();
            }
            else if (ReflectionExecutor.FloatingPointTypes().Contains(valuesType))
            {
                doubleMax();
            }
            else if (ReflectionExecutor.UnsignedIntegerTypes().Contains(valuesType))
            {
                unsignedMax();
            }
        }

        private static void unsignedMax()
        {
            ulong max = ulong.MinValue;
            foreach (ulong v in values)
            {
                if (v > max)
                {
                    max = v;
                }
            }
            returnValue = max;
        }

        private static void doubleMax()
        {
            double max = double.MinValue;
            foreach (double v in values)
            {
                if (v > max)
                {
                    max = v;
                }
            }
            returnValue = max;
        }

        private static void longMax()
        {
            long max = long.MinValue;
            foreach (long v in values)
            {
                if (v > max)
                {
                    max = v;
                }
            }
            returnValue = max;
        }

        internal static void oldest()
        {
            returnValue = values[0];
        }
        internal static void newest()
        {
            returnValue = values[values.Count-1];
        }
        internal static void specificValue()
        {

        }
        internal static void mode()
        {
            
        }
        internal static void conjunction()
        {
            bool conj = true;
            foreach(bool v in values)
            {
                conj &= v;
            }
            returnValue = conj;
        }
        internal static void disjunction()
        {
            bool conj = true;
            foreach (bool v in values)
            {
                conj |= v;
            }
            returnValue = conj;
        }
        internal static void allMatrix()
        {
            returnValue = values[0];
        }
        
    }
    
}
