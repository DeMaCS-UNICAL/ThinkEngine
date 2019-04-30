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
        public string actuatorName;
        public Dictionary<Type, IDictionary> dictionaryPerType;
        public Dictionary<string, string> unityASPVariationNames;
        public Dictionary<string, long> signedIntegerProperties;
        public Dictionary<string, ulong> unsignedIntegerProperties;
        public Dictionary<string, double> floatingPointProperties;
        public Dictionary<string, bool> boolProperties;
        public Dictionary<string, string> stringProperties;
        public Dictionary<string, char> charProperties;
        //public Dictionary<string, List<Enum>> enumProperties;
        public List<string> properties;
        public const BindingFlags BindingAttr = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;


        public SimpleActuator(List<string> propertiesToTrack, object obj)//ONLY FOR BASIC TYPE PROPERTIES IN OBJ
        {
            gO = obj;
            gOName = "";
            actuatorName = obj.GetType().ToString();
            cleanDataStructures();
            properties = new List<string>();
            properties.AddRange(propertiesToTrack);
            ReflectionExecutor re = ScriptableObject.CreateInstance<ReflectionExecutor>();
            cleanDataStructures();
            populateDataStructures();

        }

        public SimpleActuator(ActuatorConfiguration s)
        {
            
            actuatorName = s.configurationName;
            properties = new List<string>();
            ReflectionExecutor re = ScriptableObject.CreateInstance<ReflectionExecutor>();
            gO = re.GetGameObjectWithName(s.gOName);
           // Debug.Log(s.gOName);
           // Debug.Log(gO);
            gOName = s.gOName;
            cleanDataStructures();
            foreach(string st in s.properties)
            {
                if (!properties.Contains(st))
                {
                    properties.Add(st);
                }
            }
            
            populateDataStructures();

        }

        private void populateDataStructures()
        {
            Type gOType = gO.GetType();
            
            foreach (string st in properties)
            {

                if (!st.Contains("^"))
                {

                    setDefaultValue(st,st,gOType, gO);
                }
                else
                {
                    populateComposedProperty(st, st, gOType, gO);
                }

            }
        }

        private void populateComposedProperty(string entire_name, string st, Type objType, object obj)
        {
            string parentName = st.Substring(0, st.IndexOf("^"));
            string child = st.Substring(st.IndexOf("^") + 1, st.Length - st.IndexOf("^") - 1);
            MemberInfo[] members = objType.GetMember(parentName, BindingAttr);
            if (members.Length == 0)
            {
                populateComponent(entire_name, st, objType, obj);
                return;
            }
            FieldOrProperty parentProperty = new FieldOrProperty(objType.GetMember(parentName)[0]);
            object parent = parentProperty.GetValue(obj);
            Type parentType = parent.GetType();
            if (!child.Contains("^"))
            {
                setDefaultValue(entire_name, child, parentType, parent);
            }
            else
            {
                populateComposedProperty(entire_name, child, parentType, parent);
            }
        }

        internal void parse(AnswerSet set)
        {
            //Debug.Log("parsing " + actuatorName);
            MappingManager mapper = MappingManager.getInstance();
            IMapper actuatorMapper = mapper.getMapper(typeof(SimpleActuator));
            string[] mappedProperties = actuatorMapper.Map(this).Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach(string literal in set.GetAnswerSet())
            {
                //Debug.Log("literal "+literal);
                foreach(string mapped in mappedProperties)
                {
                    //Debug.Log("mapped " + mapped);
                    string withoutVariable = mapped.Substring(0,mapped.LastIndexOf('('));
                    if (literal.StartsWith(withoutVariable))
                    {
                        string clean = literal.Substring(("setOnActuator(" + actuatorName + "(" + gOName + "(").Length);
                        clean = clean.Replace('(', '^');
                        clean = clean.Remove(clean.IndexOf(')'));
                        string val = clean.Substring(clean.LastIndexOf('^')+1);
                        clean = clean.Remove(clean.LastIndexOf('^'));
                        //Debug.Log(clean+" has value "+val);
                        string property = unityASPVariationNames[clean];
                        foreach(Type t in dictionaryPerType.Keys)
                        {

                            //Debug.Log(t + " " + property);
                            IDictionary dic = dictionaryPerType[t];
                            if (dic.Contains(property))
                            {
                                dic[property] = Convert.ChangeType(val, dic.GetType().GetGenericArguments()[1]);
                                //Debug.Log(property + " " + dic[property]);
                            }
                            
                        }
                        break;
                    }

                }
            }
        }

        private void populateComponent(string entire_name, string st, Type objType, object obj)
        {
            string parentName = st.Substring(0, st.IndexOf("^"));
            string child = st.Substring(st.IndexOf("^") + 1, st.Length - st.IndexOf("^") - 1);
            if (objType == typeof(GameObject))
            {
                Component c = ((GameObject)gO).GetComponent(parentName);
                if (c != null)
                {
                    if (!child.Contains("^"))
                    {
                        setDefaultValue(entire_name, child, c.GetType(), c);

                    }
                    else
                    {
                        populateComposedProperty(entire_name, child, c.GetType(), c);
                    }
                }
            }
        }

        private void setDefaultValue(string entire_name, string st, Type objType, object obj)
        {
            MemberInfo[] members = objType.GetMember(st, BindingAttr);
            if (members.Length == 0)
            {
                return;
            }
            FieldOrProperty property = new FieldOrProperty(members[0]);
            //Debug.Log("property " + property.Name() + " " + property.Type());
            if (dictionaryPerType.ContainsKey(property.Type()))
            {
                IDictionary dic = dictionaryPerType[property.Type()];
                Type propertyInDictionaryType = dic.GetType().GetGenericArguments()[1];
                if (!dic.Contains(entire_name))
                {
                    //Debug.Log("Adding " + entire_name);
                    dic.Add(entire_name, Convert.ChangeType("0", propertyInDictionaryType));
                }
                else
                {
                    dic[entire_name] = Convert.ChangeType("0", propertyInDictionaryType);
                }

            }
        }

        public void cleanDataStructures()
        {
            unityASPVariationNames = new Dictionary<string, string>();
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
            foreach (string st in properties)
            {
                Debug.Log(st);
                if (!st.Contains("^"))
                {
                    
                        updateSimpleProperty(st,st,gOType,gO);
                }
                else
                {
                    updateComposedProperty(st,st, gOType,gO);
                }

            }
        }

        private void updateComponent(string entire_name, string st, Type gOType, object obj)
        {
            string parentName = st.Substring(0, st.IndexOf("^"));
            string child = st.Substring(st.IndexOf("^") + 1, st.Length - st.IndexOf("^") - 1);
            //Debug.Log("component " + entire_name + " parent " + parentName + " child " + child);
            if (gOType== typeof(GameObject))
            {
                Component c = ((GameObject)gO).GetComponent(parentName);
                if (c != null)
                {
                    //Debug.Log(c);
                    if (!child.Contains("^"))
                    {
                        //Debug.Log(c.GetType());
                        updateSimpleProperty(entire_name, child, c.GetType(), c);

                    }
                    else
                    {
                        updateComposedProperty(entire_name, child, c.GetType(), c);
                    }
                }
            }
        }

        private void updateSimpleProperty(string entire_name,string st, Type gOType, object obj)
        {
            MemberInfo[] members = gOType.GetMember(st,BindingAttr);
            Debug.Log("update "+entire_name+" members length"+ members.Length+" st "+st+" type "+gOType);
            if (members.Length == 0)
            {
                return;
            }
            FieldOrProperty property = new FieldOrProperty(members[0]);
            //Debug.Log(property.Type()+" contained: "+ dictionaryPerType.ContainsKey(property.Type()));
            if (dictionaryPerType.ContainsKey(property.Type()))
            {
                Type propertyInDictionaryType = dictionaryPerType[property.Type()].GetType().GetGenericArguments()[1];
                property.SetValue(obj, Convert.ChangeType(dictionaryPerType[property.Type()][entire_name],property.Type()));
                Debug.Log("sat value " + property.GetValue(obj));
            }

            //Debug.Log("added " + st + "with value " + ((IList)dictionaryPerType[property.Type()][st])[((IList)dictionaryPerType[property.Type()][st]).Count - 1]);
            
        }

        

        //public bool GetBoolProperty()
        private void updateComposedProperty(string entire_name,string st, Type objType, object obj)
         {
           
            string parentName = st.Substring(0, st.IndexOf("^"));
            string child = st.Substring(st.IndexOf("^") + 1, st.Length - st.IndexOf("^") - 1);
            MemberInfo[] members = objType.GetMember(parentName, BindingAttr);
          // Debug.Log("members with name " + parentName + " " + members.Length);
            if (members.Length == 0)
            {
                updateComponent(entire_name, st, objType,obj);
                return;
            }
            FieldOrProperty parentProperty = new FieldOrProperty(objType.GetMember(parentName)[0]);
            //Debug.Log(parentProperty.Name());
            object parent = parentProperty.GetValue(obj);
            Type parentType = parent.GetType();
            if (!child.Contains("^"))
            {

                updateSimpleProperty(entire_name, child, parentType, parent);

            }
            else
            {
                updateComposedProperty(entire_name, child, parentType, parent);
            }
            

        }

        //PARSE ANSWER SET
       
        public string getASPRepresentation()
        {
            return "";//mappingManager.mappers[this.GetType()].Map(this);
        }
     }
    }

