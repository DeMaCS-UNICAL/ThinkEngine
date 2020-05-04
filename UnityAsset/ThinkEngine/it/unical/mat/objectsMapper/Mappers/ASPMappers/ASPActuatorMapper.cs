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
                    string actuatorNameNotCapital = char.ToLower(actuator.actuatorName[0]) + actuator.actuatorName.Substring(1);
                    string goNameNotCapital = char.ToLower(actuator.gOName[0]) + actuator.gOName.Substring(1);
                    actuatorMapping += "setOnActuator("+ actuatorNameNotCapital + "("+ goNameNotCapital + "(";
                    string rightFormat = ((string)entry.Key).Replace(".", "");
                    rightFormat = rightFormat.Replace(" ", "");
                    rightFormat = char.ToLower(rightFormat[0]) + rightFormat.Substring(1);
                    for(int i = 1; i < rightFormat.Length; i++)
                    {
                        if (rightFormat[i] == '^')
                        {
                            rightFormat = rightFormat.Substring(0, i + 1) + char.ToLower(rightFormat[i + 1]) +
                                rightFormat.Substring(i + 2);
                        }
                    }
                    if (!actuator.unityASPVariationNames.ContainsKey(rightFormat)) {
                        actuator.unityASPVariationNames.Add(rightFormat, (string)entry.Key);
                    }
                    string[] propertyPath = rightFormat.Split('^');
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

