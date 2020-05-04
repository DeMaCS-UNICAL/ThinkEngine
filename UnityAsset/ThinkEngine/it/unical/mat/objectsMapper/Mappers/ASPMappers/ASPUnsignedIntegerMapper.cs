using System;
using System.Collections;
using UnityEngine;

namespace EmbASP4Unity.it.unical.mat.objectsMapper.Mappers
{ 
    internal class ASPUnsignedIntegerMapper : ScriptableObject, IMapper
    {
        public static ASPUnsignedIntegerMapper instance;
        public string Map(object o)
        {
            DictionaryEntry entry = (DictionaryEntry)o;
            string value = "(" + entry.Value + ")";
            return ASPMapperHelper.getInstance().buildMapping((string)entry.Key, '^', value);
        }

        internal static IMapper getInstance()
        {
            if (instance == null)
            {
                instance = new ASPUnsignedIntegerMapper();
            }
            return instance;
        }
    }
}