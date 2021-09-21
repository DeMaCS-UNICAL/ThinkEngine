using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using newMappers;
using NewStructures;

namespace NewMappers.ASPMappers
{
    internal abstract class ASPBasicTypeMapper : BasicTypeMapper
    {
        protected override string GenerateMapping(MyListString propertyHierarchy)
        {
            string toReturn = "";
            string toAppend = "";
            for(int i=0; i < propertyHierarchy.Count; i++)
            {
                toReturn += NewASPMapperHelper.AspFormat(propertyHierarchy[i])+"(";
                toAppend += ")";

            }
            toReturn += "{0}"+toAppend;
            return toReturn;
        }
        public override string SensorBasicMap(NewMonoBehaviourSensor sensor)
        {
            if (!supportedTypes.Contains(sensor.currentPropertyType))
            {
                throw new Exception("Type " + sensor.currentPropertyType + " is not supported as Sensor");
            }
            BasicTypeInfoAndValue infoAndValue = (BasicTypeInfoAndValue)sensor.propertyInfo;
            string value = BasicMap(infoAndValue.operation(infoAndValue.values));
            return string.Format(infoAndValue.mapping, value);
        }
        internal static string BasicMapping(object elementCurrentValue)
        {
            return MapperManager.GetMapper(elementCurrentValue.GetType()).BasicMap(elementCurrentValue);
        }
        public override string ActuatorBasicMap(object currentObject)
        {
            throw new NotImplementedException();
        }
    }
    
}
