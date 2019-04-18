using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EmbASP4Unity.it.unical.mat.objectsMapper.SensorsScripts.Mappers
{
    class ASPSensorMapper : ScriptableObject, IMapper
    {
        public string Map(object o)//o is a Sensor
        {
            SimpleSensor s = (SimpleSensor)o;
            String sensorMapping = s.sensorName+"(";
            MappingManager manager = CreateInstance<MappingManager>();
            foreach(IDictionary dictionary in s.dictionaryPerType.Values.Distinct())
            {
                foreach(DictionaryEntry entry in dictionary)
                {
                    //Debug.Log(entry.Key);
                    Type mapperType = entry.Value.GetType().GetGenericArguments()[0];//entry is a List<SOMETHING>
                    DictionaryEntry toMap = new DictionaryEntry();
                    toMap.Key = entry.Key;
                    toMap.Value = Operation.compute(s.operationPerProperty[(string)toMap.Key],entry.Value);
                    sensorMapping += manager.getMapper(mapperType).Map(toMap) +",";
                }
            }
            return sensorMapping.Substring(0, sensorMapping.Length - 1) + ")";
        }
    }
}
