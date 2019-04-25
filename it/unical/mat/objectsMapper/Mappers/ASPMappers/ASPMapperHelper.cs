using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EmbASP4Unity.it.unical.mat.objectsMapper.Mappers
{
    public class ASPMapperHelper
    {
        public static ASPMapperHelper instance;

        public static ASPMapperHelper getInstance()
        {
            if (instance == null)
            {
                instance = new ASPMapperHelper();
            }
            return instance;
        }
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
