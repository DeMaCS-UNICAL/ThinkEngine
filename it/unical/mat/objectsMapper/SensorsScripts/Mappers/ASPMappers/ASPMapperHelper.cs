using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EmbASP4Unity.it.unical.mat.objectsMapper.SensorsScripts.Mappers
{
    public class ASPMapperHelper : ScriptableObject
    {
        public string buildMapping(string name, char c, string value)
        {
            string mapping = name.Replace('^', '(');
            int size = mapping.Length;
            mapping += value;
            for (int i = 0; i < size; i++)
            {
                if (mapping[i] == '(')
                {
                    mapping += ')';
                }
            }
            return mapping;
        }
    }
}
