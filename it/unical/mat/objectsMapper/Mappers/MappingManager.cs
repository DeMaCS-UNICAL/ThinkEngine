using EmbASP4Unity.it.unical.mat.objectsMapper.Mappers;
using System;
using System.Collections.Generic;

public class MappingManager
{
    private static Dictionary<Type, IMapper> _mappers;
    public static Dictionary<Type, IMapper> mappers
    {
        get
        {
            if (_mappers == null)
            {
                _mappers = new Dictionary<Type, IMapper>();
                Populate();
            }
            return _mappers;
        }
    }
    private static void Populate()
    {
        mappers.Add(typeof(bool), ASPBoolMapper.instance);
        foreach (Type t in ReflectionExecutor.SignedIntegerTypes())
        {
            mappers.Add(t, ASPSignedIntegerMapper.instance);
        }
        foreach (Type t in ReflectionExecutor.UnsignedIntegerTypes())
        {
            mappers.Add(t, ASPUnsignedIntegerMapper.instance);
        }
        foreach (Type t in ReflectionExecutor.FloatingPointTypes())
        {
            mappers.Add(t, ASPFloatingPointMapper.instance);
        }
        mappers.Add(typeof(string), ASPStringMapper.instance);
        mappers.Add(typeof(Enum), ASPEnumMapper.instance);
        mappers.Add(typeof(char), ASPCharMapper.instance);
    }
    public static IMapper GetMapper(Type t)
    {
        if (mappers.ContainsKey(t))
        {
            return mappers[t];
        }
        return null;
    }
}
