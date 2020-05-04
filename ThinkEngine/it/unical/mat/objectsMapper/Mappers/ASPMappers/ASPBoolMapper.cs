using System;
using System.Collections;
using UnityEngine;

namespace EmbASP4Unity.it.unical.mat.objectsMapper.Mappers
{
    public class ASPBoolMapper : ScriptableObject, IMapper
    {
        public static ASPBoolMapper instance;
        public string Map(object o)
        {
            DictionaryEntry entry = (DictionaryEntry)o;
            string value =  ("(" + entry.Value + ")").ToLower();
            return ASPMapperHelper.getInstance().buildMapping((string)entry.Key, '^', value);
        }

        internal static IMapper getInstance()
        {
            if (instance == null)
            {
                instance = new ASPBoolMapper();
            }
            return instance;
        }
    }
}