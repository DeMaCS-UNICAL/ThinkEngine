using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using it.unical.mat.embasp.languages.asp;
using UnityEngine;

internal class MonoBehaviourActuatorHider
{
    internal class MonoBehaviourActuator : MonoBehaviour
    {
        private const BindingFlags BindingAttr = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

        internal MyListString property;
        internal string actuatorName;
        private string _toSet;
        internal string toSet
        {
            get
            {
                return _toSet;
            }
            set
            {
                _toSet = value;
                ApplyToGameLogic();
            }
        }
        void Awake()
        {
            hideFlags = HideFlags.HideAndDontSave;
        }
        void OnDisable()
        {
            Destroy(this);
        }
        
        private void ApplyToGameLogic()
        {
            if (_toSet == null)
            {
                return;
            }
            if (property.Count == 1)//if the property is a direct one (i.e. a "field" of the gameobject)
            {
                UpdateSimpleProperty(property, property[0], typeof(GameObject), gameObject);
            }
            else
            {
                UpdateComposedProperty(property, property, typeof(GameObject), gameObject);
            }

        }
        private void UpdateSimpleProperty(MyListString currentProperty, string lastLevelHierarchy, Type gOType, object obj)//set the value of the tracked property with the one stored in _toSet 
        {
            MemberInfo[] members = gOType.GetMember(lastLevelHierarchy, BindingAttr);
            if (members.Length == 0)
            {
                return;
            }
            FieldOrProperty property = new FieldOrProperty(members[0]);
            property.SetValue(obj, Convert.ChangeType(_toSet, property.Type()));
        }
        private void UpdateComposedProperty(MyListString currentProperty, MyListString partialHierarchy, Type objType, object obj)
        {
            string parentName = partialHierarchy[0];
            MyListString child = partialHierarchy.GetRange(1, partialHierarchy.Count - 1);
            MemberInfo[] members = objType.GetMember(parentName, BindingAttr);
            if (members.Length == 0)
            {
                UpdateComponent(currentProperty, partialHierarchy, objType, obj);
                return;
            }
            FieldOrProperty parentProperty = new FieldOrProperty(objType.GetMember(parentName)[0]);
            object parent = parentProperty.GetValue(obj);
            if (parent == null)
            {
                return;
            }
            Type parentType = parent.GetType();
            if (child.Count == 1)
            {
                UpdateSimpleProperty(currentProperty, child[0], parentType, parent);
            }
            else
            {
                UpdateComposedProperty(currentProperty, child, parentType, parent);
            }
        }
        private void UpdateComponent(MyListString currentProperty, MyListString partialHierarchy, Type gOType, object obj)
        {
            string parentName = partialHierarchy[0];
            MyListString child = partialHierarchy.GetRange(1, partialHierarchy.Count - 1);
            if (gOType == typeof(GameObject))
            {
                Component c = ((GameObject)obj).GetComponent(parentName);
                if (c != null)
                {
                    if (child.Count == 1)
                    {
                        UpdateSimpleProperty(currentProperty, child[0], c.GetType(), c);
                    }
                    else
                    {
                        UpdateComposedProperty(currentProperty, child, c.GetType(), c);
                    }
                }
            }
        }
        internal string Parse(AnswerSet value)//parses an AnswerSet looking for a literal matching its property
        {
            List<string> myTemplate = GetMyConfiguration().GetTemplate(property);
            if (myTemplate.Count > 1)
            {
                throw new Exception("It is not expected to have more than 1 entry for actuators");
            }
            string myTemplatePrefix = myTemplate[0].Substring(0, myTemplate[0].LastIndexOf('('));
            string pattern = "objectIndex\\(([0-9]+)\\)";
            Regex regex = new Regex(@pattern);
            int myIndex = gameObject.GetComponent<IndexTracker>().currentIndex;
            foreach (string literal in value.GetAnswerSet())
            {
                string literalPrefix = literal.Substring(0, literal.LastIndexOf('('));
                Match matcher = regex.Match(literalPrefix);
                if (matcher.Success && int.Parse(matcher.Groups[1].Value) == myIndex)//if the index of the object associated to the actuator is different from the one of the literal
                {
                    if (literalPrefix.Equals(string.Format(myTemplatePrefix, myIndex)))
                    {
                        int startIndex = literal.LastIndexOf("(") + 1;//the value to assign to the property is wrapped in the inner pair of ()
                        string partialRes = literal.Substring(startIndex, literal.IndexOf(")", startIndex) - startIndex);
                        return partialRes.Trim('\"');//trim " to avoid conversion problems
                    }
                }
            }
            return null;
        }
        private ActuatorConfiguration GetMyConfiguration()//retrieves the configuration underlying the actuator
        {
            ActuatorConfiguration[] actuatorConfs = gameObject.GetComponents<ActuatorConfiguration>();
            if (actuatorConfs == null)
            {
                throw new Exception("ActuatorConfiguration missing for " + actuatorName);
            }
            foreach (ActuatorConfiguration actuatorConf in actuatorConfs)
            {
                if (actuatorConf.configurationName.Equals(actuatorName))
                {
                    return actuatorConf;
                }
            }
            throw new Exception("ActuatorConfiguration missing for " + actuatorName);
        }
    }
}
