using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using EmbASP4Unity.it.unical.mat.objectsMapper.Mappers;

namespace EmbASP4Unity.it.unical.mat.objectsMapper.SensorsScripts
{
    public class AdvancedSensor : SimpleSensor
    {
        public Dictionary<string,SimpleGameObjectsTracker> advancedConf;// for each advanced property (matrix, list...) there exists a SimpleGameObjectsTracker that tracks the collection's element
        public Dictionary<string, SimpleSensor[,]> matrixProperties;
        public Dictionary<string, List<SimpleSensor>> listProperties;
        public Dictionary<string, List<string>> mappings; //for each property, there is a template mapping: l[0]=function symbols; l[1]=actual value; l[2]=closing parentheses


        public AdvancedSensor(SensorConfiguration conf) :base(conf)
        {
            
            advancedConf = new Dictionary<string, SimpleGameObjectsTracker>();
            matrixProperties = new Dictionary<string, SimpleSensor[,]>();
            listProperties = new Dictionary<string, List<SimpleSensor>>();
            foreach (SimpleGameObjectsTracker st in conf.advancedConf)
            {
                ////Debug.Log(st.propertyName+" "+st.objType);
                advancedConf.Add(st.propertyName, st);
                if (st.propertyType.Equals("ARRAY2")) { 
                    ////Debug.Log("Adding to matrix");
                
                    matrixProperties.Add(st.propertyName, null);
                }
                else if(st.propertyType.Equals("LIST"))
                {
                    ////Debug.Log("Adding to list");
                    listProperties.Add(st.propertyName, new List<SimpleSensor>());
                }
            }
            mappings = ((ASPAdvancedSensorMapper)MappingManager.getInstance().getMapper(typeof(AdvancedSensor))).getTemplateASPRepresentation(this);
            
        }

        public string Map()
        {
            string facts = "";
            facts = basicPropertiesMapping(facts);
            facts = matrixPropertiesMapping(facts);
            facts = listPropertiesMapping(facts);

            return facts;
        }

        private string listPropertiesMapping(string facts)
        {
            foreach (string property in listProperties.Keys.Distinct())
            {
                for (int i = 0; i < listProperties[property].Count; i++)
                {
                    foreach (string inner in advancedConf[property].toSave)
                    {
                        object innerPropertyValue = listProperties[property][i].getPropertyValue(inner);
                        IMapper mapperForT = MappingManager.getInstance().getMapper(innerPropertyValue.GetType());
                        string temp = mapperForT.basicMap(innerPropertyValue);
                        facts += mappings[property + "$" + inner][0] + i + "," + mappings[property + "$" + inner][1] + temp + mappings[property + "$" + inner][2] + Environment.NewLine;
                    }
                }
            }
            return facts;
        }

        private string matrixPropertiesMapping(string facts)
        {
            foreach (string property in matrixProperties.Keys.Distinct())
            {
                for (int i = 0; i < matrixProperties[property].GetLength(0); i++)
                {
                    for (int j = 0; j < matrixProperties[property].GetLength(1); j++)
                    {
                        foreach (string inner in advancedConf[property].toSave)
                        {
                            object innerPropertyValue = matrixProperties[property][i, j].getPropertyValue(inner);
                            IMapper mapperForT = MappingManager.getInstance().getMapper(innerPropertyValue.GetType());
                            string temp = mapperForT.basicMap(innerPropertyValue);
                            facts += mappings[property + "$" + inner][0] + i + "," + j + "," + mappings[property + "$" + inner][1]+ temp + mappings[property + "$" + inner][2] + Environment.NewLine;
                        }
                    }
                }

            }

            return facts;
        }

        private string basicPropertiesMapping(string facts)
        {
            List<string> done = new List<string>();
            foreach (Type t in dictionaryPerType.Keys.Distinct())
            {
                IMapper mapperForT = MappingManager.getInstance().getMapper(t);
                
                foreach (string property in dictionaryPerType[t].Keys)
                {
                    if (done.Contains(property))
                    {
                        continue;
                    }
                    done.Add(property);
                    string temp = mapperForT.basicMap(Operation.compute(operationPerProperty[property], dictionaryPerType[t][property]));
                    facts += mappings[property][0] + temp + mappings[property][2] + Environment.NewLine;
                }
            }

            return facts;
        }

        protected override void advancedUpdate(FieldOrProperty property, string entire_name, object parent)
        {
            ////Debug.Log("updating " + entire_name);
            if(property.Type().IsArray && property.Type().GetArrayRank() == 2)
            {
                ////Debug.Log(entire_name + " is a matrix");
                Array matrix = property.GetValue(parent) as Array;
                int r = matrix.GetLength(0);
                int c = matrix.GetLength(1);
                if (matrixProperties[entire_name] is null)
                {
                    matrixProperties[entire_name] = new SimpleSensor[r,c];
                }
                
                SimpleSensor[,] current = matrixProperties[entire_name];
                if(current.GetLength(0)!=r || current.GetLength(1) != c)
                {
                    current = new SimpleSensor[r, c];
                    matrixProperties[entire_name] = current;
                }
                List<string> elementConf = advancedConf[entire_name].toSave;
                for(int i=0; i < r; i++)
                {
                    for(int j=0; j < c; j++)
                    {
                        ////Debug.Log(matrix.GetValue(i, j).GetType());
                        if (current[i, j] == null)
                        {
                            current[i, j] = new SimpleSensor(elementConf, advancedConf[entire_name].name, matrix.GetValue(i, j));
                        }
                        else
                        {
                            current[i, j].gO = matrix.GetValue(i, j);
                            current[i, j].init();
                        }
                        //current[i, j] = new SimpleSensor(elementConf, advancedConf[entire_name].name, matrix.GetValue(i, j));

                    }
                }  
                ////Debug.Log("rows " + matrixProperties[entire_name].GetLength(0) + " columns " + matrixProperties[entire_name].GetLength(1));
                ////Debug.Log(matrixProperties[entire_name].GetValue(0, 0));
            }
            if (property.Type().IsGenericType && property.Type().GetGenericTypeDefinition() == typeof(List<>))
            {
                IList list = property.GetValue(parent) as IList;
                
                List<SimpleSensor> current = listProperties[entire_name];
                List<string> elementConf = advancedConf[entire_name].toSave;
                ////Debug.Log("current count " + current.Count + " actual count " + list.Count);

                if (current.Count > list.Count)
                {
                   current.RemoveRange(list.Count, current.Count - list.Count);                    
                }
                for (int i = 0; i < list.Count; i++)
                {
                    if (current.Count < i + 1)
                    {
                        current.Add(new SimpleSensor(elementConf, advancedConf[entire_name].name, list[i]));
                    }
                    else
                    {
                        current[i].gO=list[i];
                        current[i].init();
                        //current[i] = new SimpleSensor(elementConf, advancedConf[entire_name].name, list[i]);
                    }
                }
                ////Debug.Log("elements in list " + listProperties[entire_name].Count);
            }
        }
    }
}
