using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using EmbASP4Unity.it.unical.mat.objectsMapper.SensorsScripts;

namespace EmbASP4Unity.it.unical.mat.objectsMapper.Mappers
{
    class ASPAdvancedSensorMapper : ScriptableObject, IMapper
    {
        public string Map(object o)//o is a Sensor
        {
            AdvancedSensor s = (AdvancedSensor)o;
            MappingManager manager = CreateInstance<MappingManager>();
            Debug.Log("mapping " + s.sensorName);
            String sensorMapping = manager.getMapper(typeof(SimpleSensor)).Map(s);
            Debug.Log("mapped as simple "+sensorMapping);
            foreach(string matrixPath in s.matrixProperties.Keys)
            {
                SimpleSensor[,] matrix = s.matrixProperties[matrixPath];
                int r = matrix.GetLength(0), c = matrix.GetLength(1);
                string prefix = s.sensorName + "(";
                string suffix = ").";
                int start = 0;
                int indexOfCap = matrixPath.IndexOf('^', start);
                while ( indexOfCap!= -1)
                {
                    prefix += matrixPath.Substring(start, indexOfCap - start)+"(";
                    suffix = ")" + suffix;
                    start = indexOfCap + 1;
                    indexOfCap = matrixPath.IndexOf('^', start);
                }
                prefix += matrixPath.Substring(start, matrixPath.Length - start)+"(";
                suffix = ")" + suffix;
                for (int i = 0; i < r; i++)
                {
                    for (int j = 0; j < c; j++)
                    {
                        
                        Type mapperType = typeof(SimpleSensor);
                        string[] innerSensorMapping = manager.getMapper(mapperType).Map(matrix[i, j]).Split(
    new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                        foreach(string partialMap in innerSensorMapping)
                        {
                            Debug.Log(partialMap);
                            sensorMapping += prefix+ i + "," + j + ","+ partialMap + suffix+"\n";
                        }
                        
                    }
                }
                
            }
            return sensorMapping;
        }
    }
}
