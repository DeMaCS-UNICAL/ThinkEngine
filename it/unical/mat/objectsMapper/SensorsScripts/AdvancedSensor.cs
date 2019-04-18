using System;
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

        public AdvancedSensor(SensorConfiguration conf) :base(conf)
        {
            advancedConf = new Dictionary<string, SimpleGameObjectsTracker>();
            foreach(SimpleGameObjectsTracker st in conf.advancedConf)
            {
                advancedConf.Add(st.propertyName, st);
            }
            matrixProperties = new Dictionary<string, SimpleSensor[,]>();
        }

        protected override void advancedUpdate(FieldOrProperty property, string entire_name, object parent)
        {
            if(property.Type().IsArray && property.Type().GetArrayRank() == 2)
            {
                Array matrix = property.GetValue(parent) as Array;
                int r = matrix.GetLength(0);
                int c = matrix.GetLength(1);
                if (!matrixProperties.ContainsKey(entire_name))
                {
                    matrixProperties.Add(entire_name, new SimpleSensor[r,c]);
                }
                else
                {
                    SimpleSensor[,] current = matrixProperties[entire_name];
                    List<string> elementConf = advancedConf[entire_name].toSave;
                    for(int i=0; i < r; i++)
                    {
                        for(int j=0; j < c; j++)
                        {
                            Debug.Log(matrix.GetValue(i, j).GetType());
                            current[i,j] = new SimpleSensor(elementConf, matrix.GetValue(i,j));
                        }
                    }
                    
                }
                
            }
        }
    }
}
