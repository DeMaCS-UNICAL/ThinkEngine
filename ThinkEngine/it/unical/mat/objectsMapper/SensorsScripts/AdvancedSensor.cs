using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EmbASP4Unity.it.unical.mat.objectsMapper.SensorsScripts
{
    public class AdvancedSensor : SimpleSensor
    {
        public Dictionary<string,SimpleGameObjectsTracker> advancedConf;
        public Dictionary<string, SimpleSensor[,]> matrixProperties;
        public Dictionary<string, List<SimpleSensor>> listProperties;


        public AdvancedSensor(SensorConfiguration conf) :base(conf)
        {
            
            advancedConf = new Dictionary<string, SimpleGameObjectsTracker>();
            matrixProperties = new Dictionary<string, SimpleSensor[,]>();
            listProperties = new Dictionary<string, List<SimpleSensor>>();
            foreach (SimpleGameObjectsTracker st in conf.advancedConf)
            {
                //Debug.Log(st.propertyName+" "+st.objType);
                advancedConf.Add(st.propertyName, st);
                if (st.propertyType.Equals("ARRAY2")) { 
                    //Debug.Log("Adding to matrix");
                
                    matrixProperties.Add(st.propertyName, null);
                }
                else if(st.propertyType.Equals("LIST"))
                {
                    //Debug.Log("Adding to list");
                    listProperties.Add(st.propertyName, new List<SimpleSensor>());
                }
            }
            
            
            
        }

        protected override void advancedUpdate(FieldOrProperty property, string entire_name, object parent)
        {
            //Debug.Log("updating " + entire_name);
            if(property.Type().IsArray && property.Type().GetArrayRank() == 2)
            {
                //Debug.Log(entire_name + " is a matrix");
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
                        //Debug.Log(matrix.GetValue(i, j).GetType());
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
                //Debug.Log("rows " + matrixProperties[entire_name].GetLength(0) + " columns " + matrixProperties[entire_name].GetLength(1));
                //Debug.Log(matrixProperties[entire_name].GetValue(0, 0));
            }
            if (property.Type().IsGenericType && property.Type().GetGenericTypeDefinition() == typeof(List<>))
            {
                IList list = property.GetValue(parent) as IList;
                
                List<SimpleSensor> current = listProperties[entire_name];
                List<string> elementConf = advancedConf[entire_name].toSave;
                //Debug.Log("current count " + current.Count + " actual count " + list.Count);

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
                //Debug.Log("elements in list " + listProperties[entire_name].Count);
            }
        }
    }
}
