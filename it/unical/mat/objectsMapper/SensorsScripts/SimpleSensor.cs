using EmbASP4Unity.it.unical.mat.objectsMapper.Mappers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace EmbASP4Unity.it.unical.mat.objectsMapper.SensorsScripts
{
    public class SimpleSensor
    {
        public readonly object toLock = new object();
        public bool dataAvailable;/* {
            get {
                lock (toLock)
                {
                    return dataAvailable;
                }
            }
            set {
                lock (toLock)
                {
                    Debug.Log("locking");
                    dataAvailable = value;
                }
            }
        }*/
        public object gO;
        public string gOName;
        public string sensorName;
        public Dictionary<string, string> unityASPVariationNames;
        public Dictionary<Type, IDictionary> dictionaryPerType;
        public Dictionary<string, List<long>> signedIntegerProperties;
        public Dictionary<string, List<ulong>> unsignedIntegerProperties;
        public Dictionary<string, List<double>> floatingPointProperties;
        public Dictionary<string, List<bool>> boolProperties;
        public Dictionary<string, List<string>> stringProperties;
        public Dictionary<string, List<char>> charProperties;
        //public Dictionary<string, List<Enum>> enumProperties;
        public MappingManager mappingManager;
        public Dictionary<string, int> operationPerProperty { get; set; }
        public Dictionary<string, string> specificValuePerProperty { get; set; }
        public List<string> properties;
        public const BindingFlags BindingAttr = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;


        public SimpleSensor(List<string> propertiesToTrack, object obj)//ONLY FOR BASIC TYPE PROPERTIES IN OBJ
        {
            gO = obj;
            gOName = "";
            sensorName = obj.GetType().ToString();
            cleanDataStructures();
            properties = new List<string>();
            properties.AddRange(propertiesToTrack);
            mappingManager = MappingManager.getInstance();
            ReflectionExecutor re = ScriptableObject.CreateInstance<ReflectionExecutor>();
            cleanDataStructures();
            foreach(string st in properties)
            {
                //Debug.Log(st);
                operationPerProperty.Add(st, 0);
            }
            UpdateProperties();

        }

        public SimpleSensor(SensorConfiguration s)
        {
            
            sensorName = s.configurationName;
            properties = new List<string>();
            mappingManager = MappingManager.getInstance();
            ReflectionExecutor re = ScriptableObject.CreateInstance<ReflectionExecutor>();
            gO = re.GetGameObjectWithName(s.gOName);
            gOName = s.gOName;
            cleanDataStructures();
            properties.AddRange(s.properties);
            foreach (StringIntPair p in s.operationPerProperty)
            {
                operationPerProperty.Add(p.Key, p.Value);
                Debug.Log(p.Key + " " + p.Value);
                if (p.Value == Operation.SPECIFIC)
                {
                    foreach (StringStringPair pair2 in s.specificValuePerProperty)
                    {
                        //Debug.Log(pair2.Key + " " + pair2.Value);
                        if (pair2.Key.Equals(p.Key))
                        {
                            specificValuePerProperty.Add(p.Key, pair2.Value);
                            break;
                        }
                    }
                }
            }
            //UpdateProperties();
            
            
        }

        public void cleanDataStructures()
        {
            unityASPVariationNames = new Dictionary<string, string>();
            operationPerProperty = new Dictionary<string, int>();
            specificValuePerProperty = new Dictionary<string, string>();
            floatingPointProperties = new Dictionary<string, List<double>>();
            unsignedIntegerProperties = new Dictionary<string, List<ulong>>();
            signedIntegerProperties = new Dictionary<string, List<long>>();
            boolProperties = new Dictionary<string, List<bool>>();
            stringProperties = new Dictionary<string, List<string>>();
            charProperties = new Dictionary<string, List<char>>();
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
            lock (toLock)
            {
                Type gOType = gO.GetType();
                foreach (string st in properties)
                {
                    if (!st.Contains("^"))
                    {

                        updateSimpleProperty(st, st, gOType, gO);
                    }
                    else
                    {
                        updateComposedProperty(st, st, gOType, gO);
                    }
                }
                dataAvailable = true;
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
            //Debug.Log("update "+entire_name+" members length"+ members.Length+" st "+st+" type "+gOType);
            if (members.Length == 0)
            {
                return;
            }
            FieldOrProperty property = new FieldOrProperty(members[0]);
            //Debug.Log(property.Type()+" contained: "+ dictionaryPerType.ContainsKey(property.Type()));
            if (dictionaryPerType.ContainsKey(property.Type()))
            {
                Type listType = dictionaryPerType[property.Type()].GetType().GetGenericArguments()[1];
                Type argType = listType.GetGenericArguments()[0];
                if (!dictionaryPerType[property.Type()].Contains(entire_name))
                {
                    dictionaryPerType[property.Type()].Add(entire_name, Activator.CreateInstance(listType));
                }
                ((IList)dictionaryPerType[property.Type()][entire_name]).Add(Convert.ChangeType(property.GetValue(obj), argType));
                int listCount = ((IList)dictionaryPerType[property.Type()][entire_name]).Count;
                if (listCount > 500)
                {
                    ((IList)dictionaryPerType[property.Type()][entire_name]).RemoveAt(0);
                }
                //Debug.Log("added " + entire_name);  
                   
            }
            else
            {
                //Debug.Log("advanced update for " + entire_name);
                advancedUpdate(property,entire_name,obj);
            }

            //Debug.Log("added " + st + "with value " + ((IList)dictionaryPerType[property.Type()][st])[((IList)dictionaryPerType[property.Type()][st]).Count - 1]);
            
        }

        protected virtual void advancedUpdate(FieldOrProperty property, string original, object parent)
        {
        }

        //public bool GetBoolProperty()
        private void updateComposedProperty(string entire_name,string st, Type objType, object obj)
         {
           
            string parentName = st.Substring(0, st.IndexOf("^"));
            string child = st.Substring(st.IndexOf("^") + 1, st.Length - st.IndexOf("^") - 1);
            MemberInfo[] members = objType.GetMember(parentName, BindingAttr);
            //Debug.Log("members with name " + parentName + " " + members.Length);
            if (members.Length == 0)
            {
                updateComponent(entire_name, st, objType,obj);
                return;
            }
            FieldOrProperty parentProperty = new FieldOrProperty(members[0]);
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

       
        public string getASPRepresentation()
        {
            return mappingManager.mappers[this.GetType()].Map(this);
        }
     }
    }

