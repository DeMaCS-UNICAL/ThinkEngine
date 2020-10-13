using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class MonoBehaviourActuator:MonoBehaviour
{
    public const BindingFlags BindingAttr = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

    internal List<string> property;
    internal string actuatorName;
    private object _toSet;
    internal object toSet
    {
        get {
            return _toSet;
        }
        set {
            _toSet = value;
            applyToGameLogic();
        }
    }

    private void applyToGameLogic()
    {
        if (property.Count == 1)
        {

            updateSimpleProperty(property, property[0], typeof(GameObject), gameObject);
        }
        else
        {
            updateComposedProperty(property, property, typeof(GameObject), gameObject);
        }

    }

    private void updateSimpleProperty(List<string> currentProperty, string lastLevelHierarchy, Type gOType, object obj)
    {
        MemberInfo[] members = gOType.GetMember(lastLevelHierarchy, BindingAttr);
        //MyDebugger.MyDebug("update "+entire_name+" members length"+ members.Length+" st "+st+" type "+gOType);
        if (members.Length == 0)
        {
            return;
        }
        FieldOrProperty property = new FieldOrProperty(members[0]);
        //MyDebugger.MyDebug(property.Type()+" contained: "+ dictionaryPerType.ContainsKey(property.Type()));
        property.SetValue(obj, Convert.ChangeType(_toSet, property.Type()));
        //MyDebugger.MyDebug("added " + st + "with value " + ((IList)dictionaryPerType[property.Type()][st])[((IList)dictionaryPerType[property.Type()][st]).Count - 1]);
    }
    private void updateComposedProperty(List<string> currentProperty, List<string> partialHierarchy, Type objType, object obj)
    {

        string parentName = partialHierarchy[0];
        List<string> child = partialHierarchy.GetRange(1, partialHierarchy.Count - 1);
        MemberInfo[] members = objType.GetMember(parentName, BindingAttr);
        // MyDebugger.MyDebug("members with name " + parentName + " " + members.Length);
        if (members.Length == 0)
        {
            updateComponent(currentProperty, partialHierarchy, objType, obj);
            return;
        }
        FieldOrProperty parentProperty = new FieldOrProperty(objType.GetMember(parentName)[0]);
        //MyDebugger.MyDebug(parentProperty.Name());
        object parent = parentProperty.GetValue(obj);
        Type parentType = parent.GetType();
        if (child.Count == 1)
        {

            updateSimpleProperty(currentProperty, child[0], parentType, parent);

        }
        else
        {
            updateComposedProperty(currentProperty, child, parentType, parent);
        }


    }
    private void updateComponent(List<string> currentProperty, List<string> partialHierarchy, Type gOType, object obj)
    {
        string parentName = partialHierarchy[0];
        List<string> child = partialHierarchy.GetRange(1, partialHierarchy.Count - 1);
        //MyDebugger.MyDebug("component " + entire_name + " parent " + parentName + " child " + child);
        if (gOType == typeof(GameObject))
        {
            Component c = ((GameObject)obj).GetComponent(parentName);
            if (c != null)
            {
                //MyDebugger.MyDebug(c);
                if (child.Count == 1)
                {
                    //MyDebugger.MyDebug(c.GetType());
                    updateSimpleProperty(currentProperty, child[0], c.GetType(), c);

                }
                else
                {
                    updateComposedProperty(currentProperty, child, c.GetType(), c);
                }
            }
        }
    }
}

