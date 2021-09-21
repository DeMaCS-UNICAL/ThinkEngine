using newMappers;
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
    class ASPBasicArray2Mapper : Array2Mapper
    {
        #region SINGLETON FEATURES
        
        private static DataMapper _instance;
        public static DataMapper instance
        {
            get
            {
                if (_instance is null)
                {
                    _instance = new ASPBasicArray2Mapper();

                }
                return _instance;
            }
        }

        private ASPBasicArray2Mapper() { }
        #endregion
       
        public override bool IsTypeExpandable()
        {
            return false;
        }
        public override Dictionary<MyListString, KeyValuePair<Type, object>> RetrieveProperties(Type objectType, MyListString currentObjectPropertyHierarchy, object currentObject)
        {
            return new Dictionary<MyListString, KeyValuePair<Type, object>>();
        }
        public override bool Supports(Type type)
        {
            if(!BaseSupport(type))
            {
                return false;
            }
            Type elementType = type.GetElementType();
            return elementType.IsPrimitive || elementType.Equals(typeof(string)) || elementType.Equals(typeof(Enum));
        }
        protected override Array2InfoAndValue GetElementProperty( MyListString property, Array actualMatrix, int i, int j)
        {
            Array2InfoAndValue info = new Array2InfoAndValue(actualMatrix.GetValue(i, j));
            info.indexes[0] = i;
            info.indexes[1] = j;
            return info;
        }
        public override void UpdateSensor(NewMonoBehaviourSensor sensor, object currentObject)
        {
            Array matrix = (Array)currentObject;
            Array2InfoAndValue info = (Array2InfoAndValue)sensor.propertyInfo;
            if(matrix.GetLength(0)>info.indexes[0] && matrix.GetLength(1) > info.indexes[1])
            {
                info.elementCurrentValue = matrix.GetValue(info.indexes[0], info.indexes[1]);
            }
            else
            {
                UnityEngine.Object.Destroy(sensor);
            }
        }
        protected override string GenerateMapping(MyListString propertyHierarchy, MyListString residualPropertyHierarchy=null)
        {
            string toReturn = "";
            string toAppend = "";
            for(int i=0; i < propertyHierarchy.Count; i++)
            {
                toReturn += NewASPMapperHelper.AspFormat(propertyHierarchy[i])+"(";
                toAppend += ")";
            }
            toReturn += "{0},{1},{2}" + toAppend;
            return toReturn;
        }
    }
}
