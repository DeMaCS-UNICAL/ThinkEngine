using Mappers.BaseMappers;
using System;
using System.Collections;
using System.Collections.Generic;
using static Mappers.OperationContainer;

namespace Mappers
{
    internal class ASPEnumMapper : BasicTypeMapper
    {
        #region SINGLETON FEATURES
        private static IDataMapper _instance;
        public static IDataMapper Instance
        {
            get
            {
                if (_instance is null)
                {
                    _instance = new ASPEnumMapper();

                }
                return _instance;
            }
        }
        private ASPEnumMapper()
        {
            _operationList = new List<Operation>() { Newest, Oldest, Specific_Value };
            _supportedTypes = new List<Type> { typeof(Enum) };
            _convertingType = typeof(Enum);

        }
        #endregion

        public override string BasicMap(object currentObject)
        {
            if (!SupportedTypes.Contains(currentObject.GetType()))
            {
                throw new Exception("Type " + currentObject.GetType() + " is not supported as Sensor");
            }
            return "\"" + currentObject+"\"";
        }

        public override Type GetAggregationTypes(Type type)
        {
            return typeof(StringOperations);
        }
    }
}