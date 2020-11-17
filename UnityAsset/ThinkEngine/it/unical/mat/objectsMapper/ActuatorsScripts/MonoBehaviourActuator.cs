using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using it.unical.mat.embasp.languages.asp;
using UnityEngine;


public class MonoBehaviourActuator:MonoBehaviour
{
    public const BindingFlags BindingAttr = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

    internal MyListString property;
    internal string actuatorName;
    private string _toSet;
    internal string toSet
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
        if(_toSet == null)
        {
            return;
        }
        if (property.Count == 1)
        {
            updateSimpleProperty(property, property[0], typeof(GameObject), gameObject);
        }
        else
        {
            updateComposedProperty(property, property, typeof(GameObject), gameObject);
        }

    }

    private void updateSimpleProperty(MyListString currentProperty, string lastLevelHierarchy, Type gOType, object obj)
    {
        MemberInfo[] members = gOType.GetMember(lastLevelHierarchy, BindingAttr);
        if (members.Length == 0)
        {
            return;
        }
        FieldOrProperty property = new FieldOrProperty(members[0]);
        property.SetValue(obj, Convert.ChangeType(_toSet, property.Type()));
    }
    private void updateComposedProperty(MyListString currentProperty, MyListString partialHierarchy, Type objType, object obj)
    {

        string parentName = partialHierarchy[0];
        MyListString child = partialHierarchy.GetRange(1, partialHierarchy.Count - 1);
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

    internal string parse(AnswerSet value)
    {
        List<string> myTemplate = getMyConfiguration().GetTemplate(property);
        if (myTemplate.Count > 1)
        {
            throw new Exception("It is not expected to have more than 1 entry for actuators");
        }
        string myTemplatePrefix = myTemplate[0].Substring(0, myTemplate[0].LastIndexOf('('));
        string pattern = "objectIndex\\(([0-9]+)\\)";
        Regex regex = new Regex(@pattern);
        int myIndex = gameObject.GetComponent<IndexTracker>().currentIndex;
        Debug.Log("comparing " + myTemplatePrefix + ", index "+myIndex+" with ");
        foreach (string literal in value.GetAnswerSet())
        {
            Debug.Log(literal);
            string literalPrefix = literal.Substring(0, literal.LastIndexOf('('));
            Match matcher = regex.Match(literalPrefix);
            if (int.Parse(matcher.Groups[1].Value) != myIndex)
            {
                break;
            }
            //int gameObjectIndex = literalPrefix.
            if (literalPrefix.Equals(string.Format(myTemplatePrefix,myIndex)))
            {
                int startIndex = literal.LastIndexOf("(") + 1;
                string partialRes = literal.Substring(startIndex, literal.IndexOf(")", startIndex)-startIndex);
                return partialRes.Trim('\"');
            }
        }
        return null;
    }
    

    private ActuatorConfiguration getMyConfiguration()
    {
        foreach(ActuatorConfiguration actuatorConf in gameObject.GetComponents<ActuatorConfiguration>())
        {
            if (actuatorConf.configurationName.Equals(actuatorName))
            {
                return actuatorConf;
            }
        }
        return null;
    }

    private void updateComponent(MyListString currentProperty, MyListString partialHierarchy, Type gOType, object obj)
    {
        string parentName = partialHierarchy[0];
        MyListString child = partialHierarchy.GetRange(1, partialHierarchy.Count - 1);
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

