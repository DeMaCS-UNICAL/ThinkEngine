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
        public object gO;
        public string gOName;
        public string sensorName;
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


        public SimpleSensor(List<string> propertiesToTrack, object obj)//ONLY FOR BASIC TYPE PROPERTIES IN OBJ
        {
            gO = obj;
            properties = new List<string>();
            properties.AddRange(propertiesToTrack);
            mappingManager = ScriptableObject.CreateInstance<MappingManager>();
            ReflectionExecutor re = ScriptableObject.CreateInstance<ReflectionExecutor>();
            cleanDataStructures();
            foreach(string st in properties)
            {
                operationPerProperty.Add(st, 0);
            }
        }

        public SimpleSensor(SensorConfiguration s)
        {
            
            sensorName = s.configurationName;
            properties = new List<string>();
            mappingManager = ScriptableObject.CreateInstance<MappingManager>();
            ReflectionExecutor re = ScriptableObject.CreateInstance<ReflectionExecutor>();
            gO = re.GetGameObjectWithName(s.gOName);
            cleanDataStructures();
            properties.AddRange(s.properties);
            foreach (StringIntPair p in s.operationPerProperty)
            {
                operationPerProperty.Add(p.Key, p.Value);
                //Debug.Log(p.Key + " " + p.Value);
                if (p.Value == Operation.SPECIFIC)
                {
                    foreach (StringStringPair pair2 in s.specificValuePerProperty)
                    {
                        Debug.Log(pair2.Key + " " + pair2.Value);
                        if (pair2.Key.Equals(p.Key))
                        {
                            specificValuePerProperty.Add(p.Key, pair2.Value);
                            break;
                        }
                    }
                }
                break;
            }
            //UpdateProperties();
            
            
        }

        public void cleanDataStructures()
        {
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
            Type gOType = gO.GetType();
            foreach (string st in properties)
            {
                
                if (!st.Contains("^"))
                {
                    MemberInfo[] members = gOType.GetMember(st);
                    if (members.Length > 0)
                    {
                        updateSimpleProperty(st,gOType);
                    }
                    else
                    {
                        if (gO.GetType() == typeof(GameObject))
                        {
                            updateComponent(st,gOType);
                        }
                    }
                    
                    
                    
                }
                else
                {
                    addComposedProperty(st,st, gOType,gO);
                }

            }
        }

        private void updateComponent(string st, Type gOType)
        {
            if (gOType.GetType() == typeof(GameObject))
            {
                Component c = ((GameObject)gO).GetComponent(st);
                if (c != null)
                {
                    //WORK ON COMPONENTS. CHECK WHAT HAPPEN IN TRACKER WHEN IS TOGGLED A COMPONENT
                }
            }
        }

        private void updateSimpleProperty(string st, Type gOType)
        {
            FieldOrProperty property = new FieldOrProperty(gOType.GetMember(st)[0]);
            if (dictionaryPerType.ContainsKey(property.Type()))
            {
                Type listType = dictionaryPerType[property.Type()].GetType().GetGenericArguments()[1];
                Type argType = listType.GetGenericArguments()[0];
                if (!dictionaryPerType[property.Type()].Contains(st))
                {
                    dictionaryPerType[property.Type()].Add(st, Activator.CreateInstance(listType));
                }
                ((IList)dictionaryPerType[property.Type()][st]).Add(Convert.ChangeType(property.GetValue(gO), argType));

                Debug.Log("added " + st + "with value " + ((IList)dictionaryPerType[property.Type()][st])[((IList)dictionaryPerType[property.Type()][st]).Count - 1]);
            }
            else
            {
                advancedUpdate(property, st, gO);
            }
        }

        protected virtual void advancedUpdate(FieldOrProperty property, string original, object parent)
        {
        }

        //public bool GetBoolProperty()
        private void addComposedProperty(string entire_name,string st, Type objType, object obj)
         {
           
            string parentName = st.Substring(0, st.IndexOf("^"));
            string child = st.Substring(st.IndexOf("^") + 1, st.Length - st.IndexOf("^") - 1);
            FieldOrProperty parentProperty = new FieldOrProperty(objType.GetMember(parentName)[0]);
            //Debug.Log(parentProperty.Name());
            object parent = parentProperty.GetValue(obj);
            Type parentType = parent.GetType();
            if (!child.Contains("^"))
            {
                
                FieldOrProperty property = new FieldOrProperty(parentType.GetMember(child)[0]);
                
                if (dictionaryPerType.ContainsKey(property.Type()))
                {
                    Type listType = dictionaryPerType[property.Type()].GetType().GetGenericArguments()[1];
                    Type argType = listType.GetGenericArguments()[0];
                    if (!dictionaryPerType[property.Type()].Contains(entire_name))
                    {
                        dictionaryPerType[property.Type()].Add(entire_name, Activator.CreateInstance(listType));
                    }
                    ((IList)dictionaryPerType[property.Type()][entire_name]).Add(Convert.ChangeType(property.GetValue(parent), argType));
                    int listCount = ((IList)dictionaryPerType[property.Type()][entire_name]).Count;
                    //Debug.Log("original filtered value "+Operation.compute(operationPerProperty[original], (IList)dictionaryPerType[property.Type()][original]));
                    //Debug.Log("added "+listCount+"th to " + original + "with value " + ((IList)dictionaryPerType[property.Type()][original])[listCount - 1]);
                   
                }
                else
                {
                    advancedUpdate(property,entire_name,parent);
                }

            }
            else
            {
                addComposedProperty(entire_name, child, parentType, parent);
            }
            

        }

       
        public string getASPRepresentation()
        {
            return mappingManager.mappers[this.GetType()].Map(this);
        }
     }
    }

