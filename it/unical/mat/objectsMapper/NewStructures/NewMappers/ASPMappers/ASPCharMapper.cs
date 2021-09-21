﻿using NewMappers.ASPMappers;
using System;
using System.Collections;
using System.Collections.Generic;
using static NewMappers.NewOperationContainer;

namespace newMappers
{
    internal class ASPCharMapper : ASPBasicTypeMapper
    {
        #region SINGLETON FEATURES
        private static DataMapper _instance;
        public static DataMapper instance
        {
            get
            {
                if (_instance is null)
                {
                    _instance = new ASPCharMapper();

                }
                return _instance;
            }
        }
        private ASPCharMapper()
        {
            _operationList = new List<NewOperation>() { Newest, Oldest, Specific_Value };
            _supportedTypes = new List<Type> { typeof(char) };
            _convertingType = typeof(char);
        }
        #endregion

        public override string BasicMap(object currentObject)
        {
            if (!supportedTypes.Contains(currentObject.GetType()))
            {
                throw new Exception("Type " + currentObject.GetType() + " is not supported as Sensor");
            }
            return "\"" + currentObject + "\"";
        }

        public override Type GetAggregationTypes()
        {
            return typeof(StringOperations);
        }
    }
}