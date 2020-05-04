using System.Collections;
using UnityEngine;

namespace EmbASP4Unity.it.unical.mat.objectsMapper.Mappers
{
    internal class ASPFloatingPointMapper : IMapper
    {
        public static ASPFloatingPointMapper instance;
        public string Map(object o)
        {
            DictionaryEntry entry = (DictionaryEntry)o;
            string value =  "(\"" + entry.Value + "\")";
            return ASPMapperHelper.getInstance().buildMapping((string)entry.Key, '^', value);
        }
        public static IMapper getInstance()
        {
            if (instance == null)
            {
                instance = new ASPFloatingPointMapper();
            }
            return instance;
        }
    }
}