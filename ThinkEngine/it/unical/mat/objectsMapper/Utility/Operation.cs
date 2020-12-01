using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;


static class Operation 
{
    public static Dictionary<Type, Type> operationsPerType;
    private static Dictionary<Type, Dictionary<int, Action>> actionPerOperation;
    private static IList values;
    private static object returnValue;
    public enum BoolOperations { Newest, Oldest,Specific_Value,Conjunction,Disjunction };
    public enum StringOperations { Newest, Oldest, Specific_Value };//, Mode};
    public enum NumericOperations { Newest, Oldest,  Specific_Value, Max,Min,Avg};
    public enum MatrixOperations {All}
    public static readonly int SPECIFIC = (int)BoolOperations.Specific_Value;

    static Operation()
    {
        operationsPerType = new Dictionary<Type, Type>();
        actionPerOperation = new Dictionary<Type, Dictionary<int, Action>>();
        operationsPerType.Add(typeof(bool), typeof(BoolOperations));
        operationsPerType.Add(typeof(string), typeof(StringOperations));
        foreach (Type t in ReflectionExecutor.SignedIntegerTypes())
        {
            operationsPerType.Add(t, typeof(NumericOperations));
        }
        foreach (Type t in ReflectionExecutor.UnsignedIntegerTypes())
        {
            operationsPerType.Add(t, typeof(NumericOperations));
        }
        foreach (Type t in ReflectionExecutor.FloatingPointTypes())
        {
            operationsPerType.Add(t, typeof(NumericOperations));
        }
        operationsPerType.Add(typeof(Enum), typeof(StringOperations));
        operationsPerType.Add(typeof(char), typeof(StringOperations));
        foreach (Type enumType in operationsPerType.Values.Distinct())
        {
            actionPerOperation.Add(enumType, new Dictionary<int, Action>());
            foreach (object b in Enum.GetValues(enumType))
            {
                actionPerOperation[enumType].Add((int)b, () =>
                {
                    MethodInfo currentMethod = typeof(Operation).GetMethod(Enum.GetName(enumType, b), BindingFlags.NonPublic | BindingFlags.Static);
                    currentMethod.Invoke(null, null);
                });
            }
        }
    }

    internal static Type GetOperationsPerType(Type type)
    {
        if (operationsPerType.ContainsKey(type))
        {
            return operationsPerType[type];
        }
        return typeof(MatrixOperations);
    }
    internal static object Compute(int op, object value)
    {
        values = (IList)value;
        Type typeForOperation = values.GetType().GetGenericArguments()[0];
        Type enumType = null;
        if (operationsPerType.ContainsKey(typeForOperation))
        {
            enumType = operationsPerType[typeForOperation];
        }
        else
        {
            if(typeForOperation.IsArray && typeForOperation.GetArrayRank() == 2)
            {
                enumType = typeof(MatrixOperations);
            }
        }
        actionPerOperation[enumType][op]();
        return returnValue;
    }
    internal static void Avg()
    {
        Type valuesType = values.GetType().GetGenericArguments()[0];
        if (ReflectionExecutor.SignedIntegerTypes().Contains(valuesType))
        {
            LongAvg();
        }else if (ReflectionExecutor.FloatingPointTypes().Contains(valuesType))
        {
            DoubleAvg();
        }
        else if (ReflectionExecutor.UnsignedIntegerTypes().Contains(valuesType))
        {
            UnsignedAvg();
        }
    }
    private static void UnsignedAvg()
    {
        ulong avg = 0;
        foreach (ulong d in values)
        {
            avg += d;
        }
        avg /= (ulong)values.Count;
        returnValue = avg;
    }
    private static void DoubleAvg()
    {
        double avg = 0;
        foreach(double d in values)
        {
            avg += d;
        }
        avg /= values.Count;
        returnValue = avg;
    }
    private static void LongAvg()
    {
        long avg = 0;
        foreach (long d in values)
        {
            avg += d;
        }
        avg /= (long)values.Count;
        returnValue = avg;
    }
    internal static void Min()
    {
        Type valuesType = values.GetType().GetGenericArguments()[0];
        if (ReflectionExecutor.SignedIntegerTypes().Contains(valuesType))
        {
            LongMin();
        }
        else if (ReflectionExecutor.FloatingPointTypes().Contains(valuesType))
        {
            DoubleMin();
        }
        else if (ReflectionExecutor.UnsignedIntegerTypes().Contains(valuesType))
        {
            UnsignedMin();
        }
    }
    private static void UnsignedMin()
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
    private static void DoubleMin()
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
    private static void LongMin()
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
    internal static void Max()
    {
        Type valuesType = values.GetType().GetGenericArguments()[0];
        if (ReflectionExecutor.SignedIntegerTypes().Contains(valuesType))
        {
            LongMax();
        }
        else if (ReflectionExecutor.FloatingPointTypes().Contains(valuesType))
        {
            DoubleMax();
        }
        else if (ReflectionExecutor.UnsignedIntegerTypes().Contains(valuesType))
        {
            UnsignedMax();
        }
    }
    private static void UnsignedMax()
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
    private static void DoubleMax()
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
    private static void LongMax()
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
    internal static void Oldest()
    {
        returnValue = values[0];
    }
    internal static void Newest()
    {
        returnValue = values[values.Count-1];
    }
    internal static void Specific_Value()
    {

    }
    internal static void Mode()
    {
            
    }
    internal static void Conjunction()
    {
        bool conj = true;
        foreach(bool v in values)
        {
            conj &= v;
        }
        returnValue = conj;
    }
    internal static void Disjunction()
    {
        bool conj = true;
        foreach (bool v in values)
        {
            conj |= v;
        }
        returnValue = conj;
    }
    internal static void All()
    {
        returnValue = values[0];
    }
        
}
    

