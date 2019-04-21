using EmbASP4Unity.it.unical.mat.objectsMapper.ActuatorsScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EmbASP4Unity.it.unical.mat.objectsMapper.Mappers
{
    public class ASPActuatorMapper : ScriptableObject, IMapper
    {
        public string Map(object o)
        {
            SimpleActuator actuator = (SimpleActuator)o;
            string actuatorMapping = "";
            foreach(IDictionary dic in actuator.dictionaryPerType.Values)
            {
                foreach(DictionaryEntry entry in dic)
                {
                    actuatorMapping += "setOnActuator("+actuator.actuatorName + "(";
                    string[] propertyPath = ((string)entry.Key).Split('^');
                    string suffix = "))";
                    foreach(string s in propertyPath)
                    {
                        actuatorMapping += s+"(";
                        suffix += ")";
                    }
                    actuatorMapping += "X" + suffix+":-"+"\n";
                }
            }
            return actuatorMapping;
        }
    }
}

