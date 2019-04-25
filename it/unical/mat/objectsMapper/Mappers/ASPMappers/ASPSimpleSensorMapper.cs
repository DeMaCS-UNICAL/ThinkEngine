using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using EmbASP4Unity.it.unical.mat.objectsMapper.SensorsScripts;

namespace EmbASP4Unity.it.unical.mat.objectsMapper.Mappers
{
    public class ASPSimpleSensorMapper :  IMapper
    {
        public static ASPSimpleSensorMapper instance;

        public static ASPSimpleSensorMapper getInstance()
        {
            if(instance == null)
            {
                instance = new ASPSimpleSensorMapper();
            }
            return instance;
        }
        public string Map(object o)//o is a Sensor
        {
            SimpleSensor s = (SimpleSensor)o;
            String sensorMapping = "";
            lock (s.toLock)
            {
                MappingManager manager = MappingManager.getInstance();
                foreach (IDictionary dictionary in s.dictionaryPerType.Values.Distinct())
                {
                    foreach (DictionaryEntry entry in dictionary)
                    {
                        //Debug.Log(entry.Key);
                        string keyWithoutDotsAndSpaces = ((string)entry.Key).Replace(".", "");
                        keyWithoutDotsAndSpaces = keyWithoutDotsAndSpaces.Replace(" ", "");
                        if (!s.unityASPVariationNames.ContainsKey(keyWithoutDotsAndSpaces))
                        {
                            s.unityASPVariationNames.Add(keyWithoutDotsAndSpaces, (string)entry.Key);
                        }
                        Type mapperType = entry.Value.GetType().GetGenericArguments()[0];//entry is a List<SOMETHING>
                        DictionaryEntry toMap = new DictionaryEntry();
                        toMap.Key = keyWithoutDotsAndSpaces;
                        if (s.operationPerProperty.ContainsKey((string)toMap.Key))
                        {
                            sensorMapping += s.sensorName + "(";
                            if (!s.gOName.Equals(""))
                            {
                                sensorMapping += s.gOName + "(";
                            }
                            //Debug.Log(sensorMapping + " " + entry.Value + " " + s.operationPerProperty[(string)toMap.Key]);
                            toMap.Value = Operation.compute(s.operationPerProperty[(string)toMap.Key], entry.Value);
                            //Debug.Log("toMap: " + toMap.Key + " " + toMap.Value);
                            sensorMapping += manager.getMapper(mapperType).Map(toMap) + ")";
                            if (!s.gOName.Equals(""))
                            {
                                sensorMapping += ").";
                            }
                            sensorMapping += Environment.NewLine;
                        }
                        //Debug.Log("sensorMapping done for " + sensorMapping);
                    }
                }
                foreach (IDictionary dic in s.dictionaryPerType.Values.Distinct())
                {
                    dic.Clear();
                }
            }
            return sensorMapping;
        }

        
    }
}
