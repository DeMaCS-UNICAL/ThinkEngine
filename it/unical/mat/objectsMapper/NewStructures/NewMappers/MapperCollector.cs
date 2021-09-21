using newMappers;
using NewMappers.ASPMappers;
using System.Collections.Generic;

internal class MapperCollector
{
    internal static List<DataMapper> mappers = new List<DataMapper> { ASPSignedIntegerMapper.instance, ASPBoolMapper.instance, ASPCharMapper.instance,
                                                                     ASPEnumMapper.instance, ASPFloatingPointMapper.instance, ASPStringMapper.instance, 
                                                                        ASPUnsignedIntegerMapper.instance, ASPBasicArray2Mapper.instance, ASPAdvancedArray2Mapper.instance };
}