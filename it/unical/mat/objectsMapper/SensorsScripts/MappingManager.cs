using EmbASP4Unity.it.unical.mat.objectsMapper.ActuatorsScripts;
using EmbASP4Unity.it.unical.mat.objectsMapper.SensorsScripts;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EmbASP4Unity.it.unical.mat.objectsMapper.Mappers
{
    
    public class MappingManager : ScriptableObject
    {
        public Dictionary<Type,IMapper> mappers;

        public MappingManager()
        {
            mappers = new Dictionary<Type, IMapper>();
        }

        public void OnEnable() { 
            mappers.Add(typeof(SimpleSensor), ScriptableObject.CreateInstance<ASPSimpleSensorMapper>());
            mappers.Add(typeof(AdvancedSensor), ScriptableObject.CreateInstance<ASPAdvancedSensorMapper>());
            mappers.Add(typeof(SimpleActuator), ScriptableObject.CreateInstance<ASPActuatorMapper>());
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
            if (mappers.ContainsKey(t))
            {
                return mappers[t];
            }
            return null;
        }
    }
}