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
        public static ASPActuatorMapper instance;
        public string Map(object o)
        {
            SimpleActuator actuator = (SimpleActuator)o;
            string actuatorMapping = "";
            foreach(IDictionary dic in actuator.dictionaryPerType.Values.Distinct())
            {
                foreach(DictionaryEntry entry in dic)
                {
                    actuatorMapping += "setOnActuator("+actuator.actuatorName + "("+actuator.gOName+"(";
                    string keyWithoutDotsAndSpaces = ((string)entry.Key).Replace(".", "");
                    keyWithoutDotsAndSpaces = keyWithoutDotsAndSpaces.Replace(" ", "");
                    if (!actuator.unityASPVariationNames.ContainsKey(keyWithoutDotsAndSpaces)) {
                        actuator.unityASPVariationNames.Add(keyWithoutDotsAndSpaces, (string)entry.Key);
                    }
                    string[] propertyPath = keyWithoutDotsAndSpaces.Split('^');
                    string suffix = ")))";
                    foreach(string s in propertyPath)
                    {
                        actuatorMapping += s+"(";
                        suffix += ")";
                    }
                    actuatorMapping += "X" + suffix+":-"+ Environment.NewLine;
                }
            }
            return actuatorMapping;
        }

        internal static IMapper getInstance()
        {
            if (instance == null)
            {
                instance = new ASPActuatorMapper();
            }
            return instance;
        }
    }
}

