using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public delegate MyPropertyInfo ReadSimpleProperty(string path, Type type, object obj);
public static class SensorsUtility
{
    public const BindingFlags BindingAttr = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

    public static IList GetSpecificList(Type t)
    {
        if (ReflectionExecutor.SignedIntegerTypes().Contains(t))
        {
            return new List<long>();
        }else if (ReflectionExecutor.UnsignedIntegerTypes().Contains(t))
        {
            return new List<ulong>();
        }
        else if (ReflectionExecutor.FloatingPointTypes().Contains(t))
        {
            return new List<double>();
        }
        else if (t==typeof(bool))
        {
            return new List<bool>();
        }
        else if (t == typeof(char))
        {
            return new List<char>();
        }
        else if (t == typeof(Enum))
        {
            return new List<Enum>();
        }
        else if (t == typeof(string))
        {
            return new List<string>();
        }
        return null;
    }
    // Both GetArrayProperty  And  GetListProperty return an object[] that contains the object itself (the array or the list) and a list of FieldOrProperty
    // that contains one FieldOrProperty for each collectionElementProperty
    public static ArrayInfo GetArrayProperty(string path,  Type type, object obj, int x, int y, List<string> collectionElementProperties = null)
    {
        ArrayInfo toReturn = new ArrayInfo();
        MemberInfo[] members = type.GetMember(path, BindingAttr);
        if (members.Length == 0)
        {
            return toReturn;
        }
        FieldOrProperty property = new FieldOrProperty(members[0]);
        Array matrix = property.GetValue(obj) as Array;
        toReturn.array = matrix;
        if (matrix != null && matrix.GetLength(0) > x && matrix.GetLength(1) > y)
        {
            if (collectionElementProperties == null)
            {
                toReturn.value = matrix.GetValue(x, y);
                toReturn.isBasic = true;
            }
            else
            {
                List<FieldOrProperty> listOfField = new List<FieldOrProperty>();
                for (int i = 0; i < collectionElementProperties.Count; i++)
                {
                    MemberInfo[] m = matrix.GetValue(x, y).GetType().GetMember(collectionElementProperties[i], BindingAttr);
                    AddFieldOrProperty(m, listOfField);
                }
                if (listOfField.Count == 0)
                {
                    return toReturn;
                }
                toReturn.properties = listOfField;
            }
        }
        return toReturn;
    }
    public static ListInfo GetListProperty(string path, Type type, object obj, int x, List<string> collectionElementProperties=null)
    {
        ListInfo toReturn = new ListInfo();
        MemberInfo[] members = type.GetMember(path, BindingAttr);
        if (members.Length == 0)
        {
            return toReturn;
        }
        FieldOrProperty property = new FieldOrProperty(members[0]);
        IList list = property.GetValue(obj) as IList;
        toReturn.list = list;
        if (list.Count > x)
        {
            if (collectionElementProperties == null)
            {
                toReturn.value = list[x];
                toReturn.isBasic = true;
            }
            else
            {
                List<FieldOrProperty> listOfField = new List<FieldOrProperty>();
                for (int i = 0; i < collectionElementProperties.Count; i++)
                {
                    MemberInfo[] m = list[x].GetType().GetMember(collectionElementProperties[i], BindingAttr);
                    AddFieldOrProperty(m, listOfField);
                }
                if (listOfField.Count == 0)
                {
                    return toReturn;
                }
                toReturn.properties = listOfField;
            }
        }
        return toReturn;
    }
    private static void AddFieldOrProperty(MemberInfo[] m, List<FieldOrProperty> listOfField)
    {
        if (m.Length == 0)
        {
            listOfField.Add(null);
        }
        else
        {
            listOfField.Add(new FieldOrProperty(m[0]));
        }
    }
    public static MyPropertyInfo ReadComposedProperty(GameObject gameObject, MyListString property, MyListString partialHierarchyProperty, Type objType, object obj, ReadSimpleProperty ReadSimpleProperty)
    {
        string parentName = partialHierarchyProperty[0];
        MyListString child = partialHierarchyProperty.GetRange(1,partialHierarchyProperty.Count-1);
        MemberInfo[] members = objType.GetMember(parentName, BindingAttr);
        if (members.Length == 0)
        {
            return ReadComponent(gameObject, property, partialHierarchyProperty, objType, obj, ReadSimpleProperty);
        }
        FieldOrProperty parentProperty = new FieldOrProperty(members[0]);
        object parent = parentProperty.GetValue(obj);
        Type parentType = parent.GetType();
        if (child.Count==1)
        {
            return ReadSimpleProperty(child[0], parentType, parent);
        }
        else
        {
            return ReadComposedProperty(gameObject, property, child, parentType, parent, ReadSimpleProperty);
        }
    }
    public static MyPropertyInfo ReadComponent(GameObject gameObject, MyListString property, MyListString partialHierarchyProperty, Type gOType, object obj, ReadSimpleProperty ReadSimpleProperty)
    {
        string parentName = partialHierarchyProperty[0];
        MyListString child = partialHierarchyProperty.GetRange(1,partialHierarchyProperty.Count - 1);
        if (gOType == typeof(GameObject))
        {
            foreach (Component c in gameObject.GetComponents(typeof(MonoBehaviour)))
            {
                if (c.GetType().ToString().Equals(parentName))
                {
                    if (child.Count==1)
                    {
                        return ReadSimpleProperty(child[0], c.GetType(), c);
                    }
                    else
                    {
                        return ReadComposedProperty(gameObject, property, child, c.GetType(), c, ReadSimpleProperty);
                    }
                }
            }
        }
        return null;
    }
}

