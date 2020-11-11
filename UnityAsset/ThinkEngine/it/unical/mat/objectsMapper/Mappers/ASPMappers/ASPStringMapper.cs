using System;
using System.Collections;
using UnityEngine;

namespace EmbASP4Unity.it.unical.mat.objectsMapper.Mappers
{
    internal class ASPStringMapper : ScriptableObject, IMapper
    {
        public static ASPStringMapper instance;
        public string Map(object o)
        {
            DictionaryEntry entry = (DictionaryEntry)o;
            string value = "(" + entry.Value + ")";
            return ASPMapperHelper.buildMapping((string)entry.Key, '^', value);
        }

        public string basicMap(object o)
        {
            return "" + o + "";
        }

        internal static IMapper getInstance()
        {
            if (instance == null)
            {
                instance = new ASPStringMapper();
            }
            return instance;
        }
    }
}