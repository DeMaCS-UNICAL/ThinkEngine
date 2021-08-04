using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace newMappers
{
    internal class ASPSignedIntegerMapper : BasicTypeMapper
    {
        #region SINGLETON FEATURES
        public override List<Type> supportedTypes
        {
            get
            {
                return new List<Type> { typeof(int), typeof(long) };
            }
        }
        private static DataMapper _instance;
        public static DataMapper instance
        {
            get
            {
                if (_instance is null)
                {
                    _instance = new ASPSignedIntegerMapper();

                }
                return _instance;
            }
        }
        private ASPSignedIntegerMapper() { }
        #endregion
        internal override void Register()
        {
            MapperManager.RegisterSensorMapper(this);
            MapperManager.RegisterActuatorMapper(this);
        }
        internal override string SensorBasicMap(object currentObject)
        {
            if (!supportedTypes.Contains(currentObject.GetType()))
            {
                throw new Exception("Type " + currentObject.GetType() + " is not supported as Sensor");
            }
            return "" + currentObject + "";
        }
        internal override string ActuatorBasicMap(object currentObject)
        {
            return SensorBasicMap(currentObject);
        }

    }
}
