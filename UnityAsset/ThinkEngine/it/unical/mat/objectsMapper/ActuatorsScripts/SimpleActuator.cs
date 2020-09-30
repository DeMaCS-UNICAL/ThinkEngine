using EmbASP4Unity.it.unical.mat.objectsMapper.ActuatorsScripts;
using EmbASP4Unity.it.unical.mat.objectsMapper.Mappers;
using EmbASP4Unity.it.unical.mat.embasp.languages.asp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace EmbASP4Unity.it.unical.mat.objectsMapper.ActuatorsScripts
{
    public class SimpleActuator
    {
        public object gO;
        public string gOName;
        public int gOIndex;
        public string actuatorName;
        public Dictionary<Type, IDictionary> dictionaryPerType;
        public Dictionary<string, List<string>> unityASPVariationNames;
        public Dictionary<string, long> signedIntegerProperties;
        public Dictionary<string, ulong> unsignedIntegerProperties;
        public Dictionary<string, double> floatingPointProperties;
        public Dictionary<string, bool> boolProperties;
        public Dictionary<string, string> stringProperties;
        public Dictionary<string, char> charProperties;
        //public Dictionary<string, List<Enum>> enumProperties;
        public List<List<string>> properties;
        public const BindingFlags BindingAttr = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;


        public SimpleActuator(List<List<string>> propertiesToTrack, object obj)//ONLY FOR BASIC TYPE PROPERTIES IN OBJ
        {
            gO = obj;
            gOName = "";
            actuatorName = obj.GetType().ToString();
            cleanDataStructures();
            properties = new List<List<string>>();
            properties.AddRange(propertiesToTrack);
            cleanDataStructures();
            populateDataStructures();

        }

        public SimpleActuator(ActuatorConfiguration configuration)
        {
            
            actuatorName = configuration.configurationName;
            properties = new List<List<string>>();
            gO = configuration.gameObject;
           // MyDebugger.MyDebug(s.gOName);
           // MyDebugger.MyDebug(gO);
            gOName = configuration.gameObject.name;
            gOIndex = configuration.gameObject.GetComponent<IndexTracker>().currentIndex;
            cleanDataStructures();
            foreach(List<string> property in configuration.properties)
            {
                if (!properties.Contains(property))
                {
                    properties.Add(property);
                }
            }
            populateDataStructures();

        }

        private void populateDataStructures()
        {
            Type gOType = gO.GetType();
            
            foreach (List<string> property in properties)
            {

                if (property.Count==1)
                {
                    setDefaultValue(property,property[0],gOType, gO);
                }
                else
                {
                    populateComposedProperty(property, property, gOType, gO);
                }

            }
        }

        private void populateComposedProperty(List<string> currentProperty, List<string> partialHierarchy, Type objType, object obj)
        {
            string parentName = partialHierarchy[0];
            List<string> child = partialHierarchy.GetRange(1,partialHierarchy.Count-1);
            MemberInfo[] members = objType.GetMember(parentName, BindingAttr);
            if (members.Length == 0)
            {
                populateComponent(currentProperty, partialHierarchy, objType, obj);
                return;
            }
            FieldOrProperty parentProperty = new FieldOrProperty(objType.GetMember(parentName)[0]);
            object parent = parentProperty.GetValue(obj);
            Type parentType = parent.GetType();
            if (child.Count==1)
            {
                setDefaultValue(currentProperty, child[0], parentType, parent);
            }
            else
            {
                populateComposedProperty(currentProperty, child, parentType, parent);
            }
        }

        internal void parse(AnswerSet set)
        {
            MappingManager mapper = MappingManager.getInstance();
            IMapper actuatorMapper = mapper.getMapper(typeof(SimpleActuator));
            string[] mappedProperties = actuatorMapper.Map(this).Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach(string literal in set.GetAnswerSet())
            {
                //MyDebugger.MyDebug("literal "+literal);
                foreach(string mapped in mappedProperties)
                {
                    //MyDebugger.MyDebug("mapped " + mapped);
                    string withoutVariable = mapped.Substring(0,mapped.LastIndexOf('('));
                    if (literal.StartsWith(withoutVariable))
                    {
                        string clean = literal.Substring(("setOnActuator(" + actuatorName + "(" + gOName + "(").Length);
                        List<string> currentProperty = clean.Split('(').ToList();
                        string value = currentProperty.Last() ;
                        currentProperty.RemoveAt(currentProperty.Count - 1);
                        //MyDebugger.MyDebug(clean+" has value "+val);
                        List<string> property = unityASPVariationNames[currentProperty];
                        foreach(Type type in dictionaryPerType.Keys)
                        {

                            //MyDebugger.MyDebug(t + " " + property);
                            IDictionary dictionary = dictionaryPerType[type];
                            if (dictionary.Contains(property))
                            {
                                dictionary[property] = Convert.ChangeType(value, dictionary.GetType().GetGenericArguments()[1]);
                                //MyDebugger.MyDebug(property + " " + dic[property]);
                            }
                            
                        }
                        break;
                    }

                }
            }
        }

        private void populateComponent(List<string> currentProperty, List<string> partialHierarchy, Type objType, object obj)
        {
            string parentName = partialHierarchy[0];
            List<string> child = partialHierarchy.GetRange(1,partialHierarchy.Count-1);
            if (objType == typeof(GameObject))
            {
                Component c = ((GameObject)gO).GetComponent(parentName);
                if (c != null)
                {
                    if (child.Count==1)
                    {
                        setDefaultValue(currentProperty, child[0], c.GetType(), c);

                    }
                    else
                    {
                        populateComposedProperty(currentProperty, child, c.GetType(), c);
                    }
                }
            }
        }

        private void setDefaultValue(List<string> currentProperty, string lasteLevelHierarchy, Type objType, object obj)
        {
            MemberInfo[] members = objType.GetMember(lasteLevelHierarchy, BindingAttr);
            if (members.Length == 0)
            {
                return;
            }
            FieldOrProperty property = new FieldOrProperty(members[0]);
            //MyDebugger.MyDebug("property " + property.Name() + " " + property.Type());
            if (dictionaryPerType.ContainsKey(property.Type()))
            {
                IDictionary dic = dictionaryPerType[property.Type()];
                Type propertyInDictionaryType = dic.GetType().GetGenericArguments()[1];
                if (!dic.Contains(currentProperty))
                {
                    //MyDebugger.MyDebug("Adding " + entire_name);
                    dic.Add(currentProperty, Convert.ChangeType("0", propertyInDictionaryType));
                }
                else
                {
                    dic[currentProperty] = Convert.ChangeType("0", propertyInDictionaryType);
                }

            }
        }

        public void cleanDataStructures()
        {
            unityASPVariationNames = new Dictionary<List<string>, List<string>>();
            floatingPointProperties = new Dictionary<string, double>();
            unsignedIntegerProperties = new Dictionary<string, ulong>();
            signedIntegerProperties = new Dictionary<string, long>();
            boolProperties = new Dictionary<string, bool>();
            stringProperties = new Dictionary<string, string>();
            charProperties = new Dictionary<string, char>();
            //enumProperties = new Dictionary<string, Enum>();
            dictionaryPerType = new Dictionary<Type, IDictionary>();
            foreach (Type t in ReflectionExecutor.SignedIntegerTypes())
            {
                dictionaryPerType.Add(t, signedIntegerProperties);
            }
            foreach (Type t in ReflectionExecutor.UnsignedIntegerTypes())
            {
                dictionaryPerType.Add(t, unsignedIntegerProperties);
            }
            foreach (Type t in ReflectionExecutor.FloatingPointTypes())
            {
                dictionaryPerType.Add(t, floatingPointProperties);
            }
            dictionaryPerType.Add(typeof(bool), boolProperties);
            dictionaryPerType.Add(typeof(string), stringProperties);
           // dictionaryPerType.Add(typeof(Enum), enumProperties);
            dictionaryPerType.Add(typeof(char), charProperties);
        }

        public void UpdateProperties()
        {
            Type gOType = gO.GetType();
            foreach (List<string> property in properties)
            {
                //MyDebugger.MyDebug(st);
                if (property.Count==1)
                {
                    
                        updateSimpleProperty(property,property[0],gOType,gO);
                }
                else
                {
                    updateComposedProperty(property,property, gOType,gO);
                }

            }
        }

        private void updateComponent(List<string> currentProperty, List<string> partialHierarchy, Type gOType, object obj)
        {
            string parentName = partialHierarchy[0];
            List<string> child = partialHierarchy.GetRange(1, partialHierarchy.Count - 1);
            //MyDebugger.MyDebug("component " + entire_name + " parent " + parentName + " child " + child);
            if (gOType== typeof(GameObject))
            {
                Component c = ((GameObject)gO).GetComponent(parentName);
                if (c != null)
                {
                    //MyDebugger.MyDebug(c);
                    if (child.Count==1)
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

        private void updateSimpleProperty(List<string> currentProperty,string lastLevelHierarchy, Type gOType, object obj)
        {
            MemberInfo[] members = gOType.GetMember(lastLevelHierarchy,BindingAttr);
            //MyDebugger.MyDebug("update "+entire_name+" members length"+ members.Length+" st "+st+" type "+gOType);
            if (members.Length == 0)
            {
                return;
            }
            FieldOrProperty property = new FieldOrProperty(members[0]);
            //MyDebugger.MyDebug(property.Type()+" contained: "+ dictionaryPerType.ContainsKey(property.Type()));
            if (dictionaryPerType.ContainsKey(property.Type()))
            {
                Type propertyInDictionaryType = dictionaryPerType[property.Type()].GetType().GetGenericArguments()[1];
                property.SetValue(obj, Convert.ChangeType(dictionaryPerType[property.Type()][currentProperty],property.Type()));
                //MyDebugger.MyDebug("sat value " + property.GetValue(obj));
            }

            //MyDebugger.MyDebug("added " + st + "with value " + ((IList)dictionaryPerType[property.Type()][st])[((IList)dictionaryPerType[property.Type()][st]).Count - 1]);
            
        }

        

        //public bool GetBoolProperty()
        private void updateComposedProperty(List<string> currentProperty, List<string> partialHierarchy, Type objType, object obj)
         {
           
            string parentName = partialHierarchy[0];
            List<string> child = partialHierarchy.GetRange(1,partialHierarchy.Count-1);
            MemberInfo[] members = objType.GetMember(parentName, BindingAttr);
          // MyDebugger.MyDebug("members with name " + parentName + " " + members.Length);
            if (members.Length == 0)
            {
                updateComponent(currentProperty, partialHierarchy, objType,obj);
                return;
            }
            FieldOrProperty parentProperty = new FieldOrProperty(objType.GetMember(parentName)[0]);
            //MyDebugger.MyDebug(parentProperty.Name());
            object parent = parentProperty.GetValue(obj);
            Type parentType = parent.GetType();
            if (child.Count==1)
            {

                updateSimpleProperty(currentProperty, child[0], parentType, parent);

            }
            else
            {
                updateComposedProperty(currentProperty, child, parentType, parent);
            }
            

        }

        //PARSE ANSWER SET
       
        public string getASPRepresentation()
        {
            return "";//mappingManager.mappers[this.GetType()].Map(this);
        }
     }
    }

