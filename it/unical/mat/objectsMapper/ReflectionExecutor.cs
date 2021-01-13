using UnityEngine;
using System.Reflection;
using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using System.IO;


internal static class ReflectionExecutor
{
    private const BindingFlags BindingAttr = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

    internal static bool IsBaseType(FieldOrProperty obj)
    {
        Type objType = obj.Type();
        return IsBaseType(objType);
    }

    private static bool IsBaseType(Type objType)
    {
        List<Type> signedInteger = SignedIntegerTypes();
        List<Type> unsignedInteger = UnsignedIntegerTypes();
        List<Type> floatingPoint = FloatingPointTypes();
        bool isBase = signedInteger.Contains(objType) || unsignedInteger.Contains(objType) || floatingPoint.Contains(objType);
        isBase |= objType == typeof(char) || objType == typeof(bool) || objType == typeof(Enum) || objType == typeof(string);
        return isBase;
    }

    internal static int IsArrayOfRank(FieldOrProperty obj)
    {
        Type objType = obj.Type();
        return IsArrayOfRank(objType);
    }

    private static int IsArrayOfRank(Type objType)
    {
        return objType.IsArray ? objType.GetArrayRank() : -1;
    }

    internal static bool IsList(FieldOrProperty obj)
    {
        Type objType = obj.Type();
        return ListOfType(objType)!=null;
    }
    internal static Type ListOfType(Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition()==typeof(List<>))
        {
            Type[] listArguments = type.GetGenericArguments();
            if (listArguments.Length == 1 && !listArguments[0].IsGenericType)
            {
                return listArguments[0];
            }
        }
        return null;
    }
    internal static List<Type> FloatingPointTypes()
    {
        return new List<Type> { typeof(double), typeof(float) };
    }
    internal static List<Type> UnsignedIntegerTypes()
    {
        return new List<Type> { typeof(byte), typeof(ushort), typeof(uint), typeof(ulong) };
    }
    internal static List<Type> SignedIntegerTypes()
    {
        return new List<Type> { typeof(sbyte), typeof(short), typeof(int), typeof(long) };
    }
    internal static bool IsMappable(FieldOrProperty obj)
    {
        return IsBaseType(obj) || IsMatrix(obj) || IsList(obj);
    }
    private static bool IsMatrix(FieldOrProperty obj)
    {
        return IsArrayOfRank(obj) == 2;
    }
    private static bool IsMatrix(Type type)
    {
        return IsArrayOfRank(type) == 2;
    }
    internal static List<FieldOrProperty> GetFieldsAndProperties(object go)
    {
        List<FieldOrProperty> propertiesList = new List<FieldOrProperty>();
        Type t =go.GetType();
        if(go is Type)
        {
            t = (Type)go;
        }
        MemberInfo[] fields = t.GetFields(BindingAttr);
        MemberInfo[] properties = t.GetProperties();
        var members = fields.Union(properties);
        foreach (MemberInfo child in members)
        {
            propertiesList.Add(new FieldOrProperty(child));
        }
        
            return propertiesList;
    }
    internal static List<Component> GetComponents(GameObject go)
    {
        List<Component> components = new List<Component>();
        foreach (Component c in go.GetComponents(typeof(Component)))
        {
            if (!components.Contains(c) && c!=null)
            {
                components.Add(c);
            }
        }
        return components;
    }

    internal static bool HasBasicGenericArgument(FieldOrProperty fieldOrProperty)
    {
        Type objType = fieldOrProperty.Type();
        if (IsMatrix(objType))
        {
            return IsBaseType(objType.GetElementType());
        }
        if (IsList(fieldOrProperty))
        {
            return IsBaseType(ListOfType(objType));
        }
        return false;
    }
}