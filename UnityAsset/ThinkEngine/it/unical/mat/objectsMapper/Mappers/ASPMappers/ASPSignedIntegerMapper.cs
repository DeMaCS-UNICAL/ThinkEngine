using System;
using System.Collections;
using UnityEngine;

namespace EmbASP4Unity.it.unical.mat.objectsMapper.Mappers
{
    internal class ASPSignedIntegerMapper: IMapper
    {
        public static ASPSignedIntegerMapper instance;
        public string Map(object o)
        {
            DictionaryEntry entry = (DictionaryEntry)o;
            string value =  "(" + entry.Value + ")";
            return ASPMapperHelper.getInstance().buildMapping((string)entry.Key, '^', value);
        }

        public string basicMap(object l)
        {
            return "("+l+")";
        }

        internal static IMapper getInstance()
        {
            if (instance == null)
            {
                instance = new ASPSignedIntegerMapper();
            }
            return instance;
        }
    }
}