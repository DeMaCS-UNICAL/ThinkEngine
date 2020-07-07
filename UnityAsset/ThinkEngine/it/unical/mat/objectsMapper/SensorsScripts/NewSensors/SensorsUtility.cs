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
    public static Dictionary<Type, Type> actualMonoBehaviourSensor;

    static SensorsUtility()
    {
        actualMonoBehaviourSensor = new Dictionary<Type, Type>();
        foreach(Type t in ReflectionExecutor.GetAvailableBasicTypes())
        {
            if (ReflectionExecutor.SignedIntegerTypes().Contains(t))
            {
                actualMonoBehaviourSensor.Add(t, typeof(MonoBehaviourSignedIntegerSensor));
            }else if (ReflectionExecutor.UnsignedIntegerTypes().Contains(t))
            {
                actualMonoBehaviourSensor.Add(t, typeof(MonoBehaviourUnsignedIntegerSensor));
            }
            else if (ReflectionExecutor.FloatingPointTypes().Contains(t))
            {
                actualMonoBehaviourSensor.Add(t, typeof(MonoBehaviourFloatingPointSensor));
            }
            else if (t==typeof(bool))
            {
                actualMonoBehaviourSensor.Add(t, typeof(MonoBehaviourBoolSensor));
            }
            else if (t == typeof(char))
            {
                actualMonoBehaviourSensor.Add(t, typeof(MonoBehaviourCharSensor));
            }
            else if (t == typeof(Enum))
            {
                actualMonoBehaviourSensor.Add(t, typeof(MonoBehaviourEnumSensor));
            }
            else if (t == typeof(string))
            {
                actualMonoBehaviourSensor.Add(t, typeof(MonoBehaviourStringSensor));
            }
        }
    }

    public static object[] GetArrayProperty(string path, string collectionElementProperty,  Type type, object obj, int i, int j)
    {
        //////Debug.unityLogger.logEnabled = false;

        MemberInfo[] members = type.GetMember(path, BindingAttr);
        //////Debug.Log("LOOKING FOR " + path + " MATRIX");
        object[] toReturn = new object[2];
        if (members.Length == 0)
        {
            return toReturn;
        }
        //////Debug.Log("MATRIX FOUND");

        FieldOrProperty property = new FieldOrProperty(members[0]);
        Array matrix = property.GetValue(obj) as Array;
        //////Debug.Log("THE MATRIX "+property.Name()+" IN " + obj + " IS " + matrix);
        toReturn[0] = matrix;
        if (matrix!=null && matrix.GetLength(0) > i && matrix.GetLength(1) > j)
        {
            ////Debug.Log("COLLECTION ELEMENT PROPERTY " + collectionElementProperty);
            MemberInfo[] m = matrix.GetValue(i, j).GetType().GetMember(collectionElementProperty, BindingAttr);
            ////Debug.Log("MATRIX ELEMENTS TYPE " + matrix.GetValue(i, j).GetType());
            if (m.Length == 0)
            {
                return toReturn;
            }
            else
            {
                ////Debug.Log("FOUND INNER PROPERTY ");
            }
            toReturn[1] = new FieldOrProperty(m[0]);

        }
        return toReturn;
    }

    public static object[] GetListProperty(string path, string collectionElementProperty, Type type, object obj, int i)
    {
        ////Debug.unityLogger.logEnabled = false;
        object[] toReturn = new object[2];
        MemberInfo[] members = type.GetMember(path, BindingAttr);
        if (members.Length == 0)
        {
            return toReturn;
        }
        FieldOrProperty property = new FieldOrProperty(members[0]);
        IList list = property.GetValue(obj) as IList;
        toReturn[0] = list;
        if (list.Count > i)
        {
            MemberInfo[] m = list[i].GetType().GetMember(collectionElementProperty, BindingAttr);
            if (m.Length == 0)
            {
                return toReturn;
            }
            toReturn[1] = new FieldOrProperty(m[0]);
        }
        return toReturn;
    }
    public static object ReadComposedProperty(GameObject gameObject, string entire_name, string st, Type objType, object obj, ReadSimpleProperty ReadSimpleProperty)
    {

        ////Debug.unityLogger.logEnabled = false;
        string parentName = st.Substring(0, st.IndexOf("^"));
        string child = st.Substring(st.IndexOf("^") + 1, st.Length - st.IndexOf("^") - 1);
        MemberInfo[] members = objType.GetMember(parentName, SensorsUtility.BindingAttr);
        //////Debug.Log("members with name " + parentName + " " + members.Length);
        if (members.Length == 0)
        {
            return ReadComponent(gameObject, entire_name, st, objType, obj, ReadSimpleProperty);
        }
        FieldOrProperty parentProperty = new FieldOrProperty(members[0]);
        //////Debug.Log(parentProperty.Name());
        object parent = parentProperty.GetValue(obj);
        Type parentType = parent.GetType();
        if (!child.Contains("^"))
        {
            return ReadSimpleProperty(child, parentType, parent);
        }
        else
        {
            return ReadComposedProperty(gameObject, entire_name, child, parentType, parent, ReadSimpleProperty);
        }
    }
    public static object ReadComponent(GameObject gameObject, string entire_name, string st, Type gOType, object obj, ReadSimpleProperty ReadSimpleProperty)
    {
        ////Debug.unityLogger.logEnabled = false;
        string parentName = st.Substring(0, st.IndexOf("^"));
        string child = st.Substring(st.IndexOf("^") + 1, st.Length - st.IndexOf("^") - 1);
        ////Debug.Log("component " + entire_name + " parent " + parentName + " child " + child);
        if (gOType == typeof(GameObject))
        {
            Component c = gameObject.GetComponent(parentName);
            if (c != null)
            {
                ////Debug.Log("component " +c);
                if (!child.Contains("^"))
                {
                    ////Debug.Log(" of type "+c.GetType());
                    return ReadSimpleProperty(child, c.GetType(), c);
                }
                else
                {
                    return ReadComposedProperty(gameObject, entire_name, child, c.GetType(), c, ReadSimpleProperty);
                }
            }
        }
        return null;
    }
}

