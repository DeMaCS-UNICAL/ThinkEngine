using EmbASP4Unity.it.unical.mat.objectsMapper.SensorsScripts.Mappers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EmbASP4Unity.it.unical.mat.objectsMapper.SensorsScripts
{
    
    public class MappingManager : ScriptableObject
    {
        public Dictionary<Type,IMapper> mappers;

        public MappingManager()
        {
            mappers = new Dictionary<Type, IMapper>();
        }

        public void OnEnable() { 
            mappers.Add(typeof(SimpleSensor), ScriptableObject.CreateInstance<ASPSensorMapper>());
            mappers.Add(typeof(bool), ScriptableObject.CreateInstance<ASPBoolMapper>());
            foreach (Type t in ReflectionExecutor.SignedIntegerTypes())
            {
                mappers.Add(t, ScriptableObject.CreateInstance<ASPSignedIntegerMapper>());
            }
            foreach (Type t in ReflectionExecutor.UnsignedIntegerTypes())
            {
                mappers.Add(t, ScriptableObject.CreateInstance<ASPUnsignedIntegerMapper>());
            }
            foreach (Type t in ReflectionExecutor.FloatingPointTypes())
            {
                mappers.Add(t, ScriptableObject.CreateInstance<ASPFloatingPointMapper>());
            }
            mappers.Add(typeof(string), ScriptableObject.CreateInstance<ASPStringMapper>());
            mappers.Add(typeof(Enum), ScriptableObject.CreateInstance<ASPEnumMapper>());
            mappers.Add(typeof(char), ScriptableObject.CreateInstance<ASPCharMapper>());
            
        }

        public IMapper getMapper(Type t)
        {
            
            return mappers[t];
        }
    }
}