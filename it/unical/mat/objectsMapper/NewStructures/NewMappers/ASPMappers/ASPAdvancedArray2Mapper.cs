using NewMappers.BaseMappers;
using NewStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NewMappers.ASPMappers
{
    class ASPAdvancedArray2Mapper : Array2Mapper
    {
        private static ASPAdvancedArray2Mapper _instance;
        internal static ASPAdvancedArray2Mapper instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ASPAdvancedArray2Mapper();
                }
                return _instance;
            }
        }
        private ASPAdvancedArray2Mapper() { }
        public override bool IsTypeExpandable()
        {
            return true;
        }

        public override Dictionary<MyListString, KeyValuePair<Type, object>> RetrieveProperties(Type objectType, MyListString currentObjectPropertyHierarchy, object currentObject)
        {
            if (!Supports(objectType))
            {
                throw new Exception(objectType + " is not supported.");
            }
            Type underlyingType = objectType.GetElementType();
            MyListString rootName = new MyListString( currentObjectPropertyHierarchy.myStrings);
            rootName.Add(underlyingType.Name);
            return MapperManager.RetrieveSensorProperties(underlyingType, rootName, null);
        }
        public override bool Supports(Type type)
        {
            if (!BaseSupport(type))
            {
                return false;
            }
            return !ASPBasicArray2Mapper.instance.Supports(type) && !type.GetElementType().IsGenericType;
        }
        protected override Array2InfoAndValue GetElementProperty(MyListString property, Array actualMatrix, int i, int j)
        {
            throw new NotImplementedException();
        }

        public override void UpdateSensor(NewMonoBehaviourSensor sensor, object currentObject)
        {
            throw new NotImplementedException();
        }

        protected override string GenerateMapping(MyListString propertyHierarchy, MyListString residualPropertyHierarchy)
        {
            throw new NotImplementedException();
        }

    }
}
