using EmbASP4Unity.it.unical.mat.objectsMapper.ActuatorsScripts;
using EmbASP4Unity.it.unical.mat.objectsMapper.SensorsScripts;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EmbASP4Unity.it.unical.mat.objectsMapper.Mappers
{
    
    public class MappingManager
    {
        public Dictionary<Type,IMapper> mappers;
        public static MappingManager instance;

        private MappingManager()
        {
            mappers = new Dictionary<Type, IMapper>();
            populate();
        }

        public static MappingManager getInstance()
        {
            if (instance == null)
            {
                instance = new MappingManager();
            }
            return instance;
        }

        public void populate() { 
            mappers.Add(typeof(SimpleSensor), ASPSimpleSensorMapper.getInstance());
            mappers.Add(typeof(AdvancedSensor), ASPAdvancedSensorMapper.getInstance());
            mappers.Add(typeof(SimpleActuator), ASPActuatorMapper.getInstance());
            mappers.Add(typeof(bool), ASPBoolMapper.getInstance());
            foreach (Type t in ReflectionExecutor.SignedIntegerTypes())
            {
                mappers.Add(t, ASPSignedIntegerMapper.getInstance());
            }
            foreach (Type t in ReflectionExecutor.UnsignedIntegerTypes())
            {
                mappers.Add(t, ASPUnsignedIntegerMapper.getInstance());
            }
            foreach (Type t in ReflectionExecutor.FloatingPointTypes())
            {
                mappers.Add(t, ASPFloatingPointMapper.getInstance());
            }
            mappers.Add(typeof(string), ASPStringMapper.getInstance());
            mappers.Add(typeof(Enum), ASPEnumMapper.getInstance());
            mappers.Add(typeof(char), ASPCharMapper.getInstance());
            
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