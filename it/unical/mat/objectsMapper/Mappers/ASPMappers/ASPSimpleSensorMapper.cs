using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using EmbASP4Unity.it.unical.mat.objectsMapper.SensorsScripts;

namespace EmbASP4Unity.it.unical.mat.objectsMapper.Mappers
{
    class ASPSimpleSensorMapper : ScriptableObject, IMapper
    {
        public string Map(object o)//o is a Sensor
        {
            SimpleSensor s = (SimpleSensor)o;
            String sensorMapping = "";
            MappingManager manager = CreateInstance<MappingManager>();
            foreach(IDictionary dictionary in s.dictionaryPerType.Values.Distinct())
            {
                foreach(DictionaryEntry entry in dictionary)
                {
                    //Debug.Log(entry.Key);
                    sensorMapping += s.sensorName + "(";
                    if (!s.gOName.Equals(""))
                    {
                        sensorMapping += s.gOName + "(";
                    }
                    Type mapperType = entry.Value.GetType().GetGenericArguments()[0];//entry is a List<SOMETHING>
                    DictionaryEntry toMap = new DictionaryEntry();
                    toMap.Key = entry.Key;
                    toMap.Value = Operation.compute(s.operationPerProperty[(string)toMap.Key],entry.Value);
                    Debug.Log("toMap: " + toMap.Key + " " + toMap.Value);
                    sensorMapping += manager.getMapper(mapperType).Map(toMap) +")";
                    if (!s.gOName.Equals(""))
                    {
                        sensorMapping +=").";
                    }
                    sensorMapping += Environment.NewLine;
                    Debug.Log("sensorMapping done for " + sensorMapping);
                }
            }
            return sensorMapping;
        }
    }
}
