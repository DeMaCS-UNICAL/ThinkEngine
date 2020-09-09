using UnityEngine;
using System.Reflection;
using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using System.IO;


public static class ReflectionExecutor
{
    public const BindingFlags BindingAttr = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

    public static List<string> GetGameObjects()
    {
        object[] go = GameObject.FindObjectsOfType(typeof(GameObject));
        List<string> objectsNames = new List<string>();
        foreach(object o in go)
        {
          
            objectsNames.Add(((GameObject)o).name);
            
        }
        return objectsNames;
    }

   

    public static bool IsBaseType(FieldOrProperty obj)
    {
        List<Type> signedInteger = SignedIntegerTypes();
        List<Type> unsignedInteger = UnsignedIntegerTypes();
        List<Type> floatingPoint = FloatingPointTypes();
        //MyDebugger.MyDebug(" level " + level);
        Type objType = obj.Type();
        uint u = 1;
        byte b = 1;
        ulong l = 1;
        ushort s = 1;
        l = s;
        //MyDebugger.MyDebug(obj.GetProperties()[0].PropertyType+" with name "+ obj.GetProperties()[0].Name);
        bool isBase = signedInteger.Contains(objType) || unsignedInteger.Contains(objType) || floatingPoint.Contains(objType);
        isBase |= objType == typeof(char) || objType == typeof(bool) || objType == typeof(Enum) || objType == typeof(string);
        return  isBase ;
    }

    public static List<Type> GetAvailableBasicTypes()
    {
        List<Type> toReturn = SignedIntegerTypes();
        toReturn.AddRange(UnsignedIntegerTypes());
        toReturn.AddRange(FloatingPointTypes());
        toReturn.Add(typeof(char));
        toReturn.Add(typeof(string));
        toReturn.Add(typeof(Enum));
        toReturn.Add(typeof(bool));
        return toReturn;
    }

    public static Type GetCorrespondingBaseType(Type t)
    {
        List<Type> signedInteger = SignedIntegerTypes();
        List<Type> unsignedInteger = UnsignedIntegerTypes();
        List<Type> floatingPoint = FloatingPointTypes();
        //MyDebugger.MyDebug(" level " + level);
        return signedInteger.Contains(t) ? typeof(long) : unsignedInteger.Contains(t) ? typeof(ulong) : floatingPoint.Contains(t) ? typeof(float) :
            t == typeof(char) ? typeof(char): t == typeof(bool)? typeof(bool) : t== typeof(Enum)? typeof(Enum): t == typeof(string)? typeof(string):null;

    }

    public static int isArrayOfRank(FieldOrProperty obj)
    {
        Type objType = obj.Type();
        return objType.IsArray? objType.GetArrayRank():-1;
    }
    public static Type isList(FieldOrProperty obj)
    {
        Type objType = obj.Type();
        return isListOfType(objType);
    }
    public static Type isListOfType(Type type)
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

    public static List<Type> FloatingPointTypes()
    {
        return new List<Type> { typeof(double), typeof(float) };
    }

    public static List<Type> UnsignedIntegerTypes()
    {
        return new List<Type> { typeof(byte), typeof(ushort), typeof(uint), typeof(ulong) };
    }

    public static List<Type> SignedIntegerTypes()
    {
        return new List<Type> { typeof(sbyte), typeof(short), typeof(int), typeof(long) };
    }

    

    internal static bool isMappable(FieldOrProperty obj)
    {
        return IsBaseType(obj) || isMatrix(obj) || isList(obj)!=null;
    }

    private static bool isMatrix(FieldOrProperty obj)
    {
       
        return isArrayOfRank(obj)==2;
    }
 
    internal static Type TypeOf(FieldOrProperty f)
    {
        return f.Type();
    }

    public static List<FieldOrProperty> GetFieldsAndProperties(object go)
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

    public static GameObject GetGameObjectWithName(string name)
    {
        return GameObject.Find(name);
    }

    public static List<Component> GetComponents(GameObject go)
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
}