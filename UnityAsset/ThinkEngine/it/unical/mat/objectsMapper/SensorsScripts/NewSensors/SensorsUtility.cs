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
        //////MyDebugger.MyDebug("LOOKING FOR " + path + " MATRIX");
        object[] toReturn = new object[2];
        if (members.Length == 0)
        {
            return toReturn;
        }
        //////MyDebugger.MyDebug("MATRIX FOUND");

        FieldOrProperty property = new FieldOrProperty(members[0]);
        Array matrix = property.GetValue(obj) as Array;
        //////MyDebugger.MyDebug("THE MATRIX "+property.Name()+" IN " + obj + " IS " + matrix);
        toReturn[0] = matrix;
        if (matrix!=null && matrix.GetLength(0) > i && matrix.GetLength(1) > j)
        {
            ////MyDebugger.MyDebug("COLLECTION ELEMENT PROPERTY " + collectionElementProperty);
            MemberInfo[] m = matrix.GetValue(i, j).GetType().GetMember(collectionElementProperty, BindingAttr);
            ////MyDebugger.MyDebug("MATRIX ELEMENTS TYPE " + matrix.GetValue(i, j).GetType());
            if (m.Length == 0)
            {
                return toReturn;
            }
            else
            {
                ////MyDebugger.MyDebug("FOUND INNER PROPERTY ");
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
    public static object ReadComposedProperty(GameObject gameObject, List<string> property, List<string> partialHierarchyProperty, Type objType, object obj, ReadSimpleProperty ReadSimpleProperty)
    {

        ////Debug.unityLogger.logEnabled = false;
        string parentName = partialHierarchyProperty[0];
        List<string> child = partialHierarchyProperty.GetRange(1,partialHierarchyProperty.Count-1);
        MemberInfo[] members = objType.GetMember(parentName, SensorsUtility.BindingAttr);
        MyDebugger.MyDebug("members with name " + parentName + " " + members.Length);
        if (members.Length == 0)
        {
            return ReadComponent(gameObject, property, partialHierarchyProperty, objType, obj, ReadSimpleProperty);
        }
        FieldOrProperty parentProperty = new FieldOrProperty(members[0]);
        ///MyDebugger.MyDebug(parentProperty.Name());
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
    public static object ReadComponent(GameObject gameObject, List<string> property, List<string> partialHierarchyProperty, Type gOType, object obj, ReadSimpleProperty ReadSimpleProperty)
    {
        ////Debug.unityLogger.logEnabled = false;
        string parentName = partialHierarchyProperty[0];
        List<string> child = partialHierarchyProperty.GetRange(1,partialHierarchyProperty.Count - 1);
        MyDebugger.MyDebug("component " + property + " parent " + parentName + " child " + child+" goType "+gOType);
        if (gOType == typeof(GameObject))
        {
            MyDebugger.MyDebug(gameObject.name + " is the GO");
            foreach(Component c in gameObject.GetComponents(typeof(MonoBehaviour)))
            {
                if (c.GetType().Name.Equals(parentName))
                {
                    MyDebugger.MyDebug("component " + c);
                    if (child.Count==1)
                    {
                        MyDebugger.MyDebug(" of type " + c.GetType());
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

