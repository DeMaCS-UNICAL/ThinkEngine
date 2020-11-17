using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public delegate object ReadSimpleProperty(string path, Type type, object obj);
public static class SensorsUtility
{
    public const BindingFlags BindingAttr = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

    public static IList getSpecificList(Type t)
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
    // Both GetArrayProperty  And  GetListProperty return an object[] that contain the object itself (the array or the list) and a list of FielOrProperty
    // that contains one FielOrProperty for each collectionElementProperty
    public static object[] GetArrayProperty(string path, List<string> collectionElementProperties,  Type type, object obj, int x, int y)
    {
        MemberInfo[] members = type.GetMember(path, BindingAttr);
        object[] toReturn = new object[2];
        if (members.Length == 0)
        {
            return toReturn;
        }

        FieldOrProperty property = new FieldOrProperty(members[0]);
        Array matrix = property.GetValue(obj) as Array;
        toReturn[0] = matrix;
        if (matrix!=null && matrix.GetLength(0) > x && matrix.GetLength(1) > y)
        {
            List<FieldOrProperty> listOfField = new List<FieldOrProperty>();
            for (int i = 0; i < collectionElementProperties.Count; i++)
            {
                MemberInfo[] m = matrix.GetValue(i, y).GetType().GetMember(collectionElementProperties[i], BindingAttr);
                addFieldOrProperty(m,listOfField);
            }
            if (listOfField.Count == 0)
            {
                return toReturn;
            }
            toReturn[1] = listOfField;
        }
        return toReturn;
    }

    
    public static object[] GetListProperty(string path, List<string> collectionElementProperties, Type type, object obj, int x)
    {
        object[] toReturn = new object[2];
        MemberInfo[] members = type.GetMember(path, BindingAttr);
        if (members.Length == 0)
        {
            return toReturn;
        }
        FieldOrProperty property = new FieldOrProperty(members[0]);
        IList list = property.GetValue(obj) as IList;
        toReturn[0] = list;
        if (list.Count > x)
        {
            List<FieldOrProperty> listOfField = new List<FieldOrProperty>();
            for (int i = 0; i < collectionElementProperties.Count; i++)
            {
                MemberInfo[] m = list[x].GetType().GetMember(collectionElementProperties[i], BindingAttr);
                addFieldOrProperty(m, listOfField);
            }
            if (listOfField.Count == 0)
            {
                return toReturn;
            }
            toReturn[1] = listOfField;
        }
        return toReturn;
    }

    private static void addFieldOrProperty(MemberInfo[] m, List<FieldOrProperty> listOfField)
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

    public static object ReadComposedProperty(GameObject gameObject, MyListString property, MyListString partialHierarchyProperty, Type objType, object obj, ReadSimpleProperty ReadSimpleProperty)
    {

        string parentName = partialHierarchyProperty[0];
        MyListString child = partialHierarchyProperty.GetRange(1,partialHierarchyProperty.Count-1);
        MemberInfo[] members = objType.GetMember(parentName, SensorsUtility.BindingAttr);
        //MyDebugger.MyDebug("members with name " + parentName + " " + members.Length);
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
    public static object ReadComponent(GameObject gameObject, MyListString property, MyListString partialHierarchyProperty, Type gOType, object obj, ReadSimpleProperty ReadSimpleProperty)
    {
        string parentName = partialHierarchyProperty[0];
        MyListString child = partialHierarchyProperty.GetRange(1,partialHierarchyProperty.Count - 1);
        //MyDebugger.MyDebug("component " + property + " parent " + parentName + " child " + child+" goType "+gOType);
        if (gOType == typeof(GameObject))
        {
            //MyDebugger.MyDebug(gameObject.name + " is the GO");
            foreach (Component c in gameObject.GetComponents(typeof(MonoBehaviour)))
            {
                if (c.GetType().Name.Equals(parentName))
                {
                    //MyDebugger.MyDebug("component " + c);
                    if (child.Count==1)
                    {
                        //MyDebugger.MyDebug(" of type " + c.GetType());
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

