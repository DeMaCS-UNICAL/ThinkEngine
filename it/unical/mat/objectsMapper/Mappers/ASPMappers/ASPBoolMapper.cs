using System.Collections;
using UnityEngine;

namespace EmbASP4Unity.it.unical.mat.objectsMapper.Mappers
{
    public class ASPBoolMapper : ScriptableObject, IMapper
    {
        public string Map(object o)
        {
            DictionaryEntry entry = (DictionaryEntry)o;
            string value =  "(" + entry.Value + ")";
            return ScriptableObject.CreateInstance<ASPMapperHelper>().buildMapping((string)entry.Key, '^', value);
        }
    }
}