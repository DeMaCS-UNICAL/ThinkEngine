using System.Collections;

namespace ThinkEngine.it.unical.mat.objectsMapper.Mappers
{
    internal class ASPBoolMapper :  IMapper
    {
        private static ASPBoolMapper _instance;
        public static ASPBoolMapper instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ASPBoolMapper();
                }
                return _instance;
            }
        }
        public string BasicMap(object b)
        {
            return (((bool)b) + "").ToLower();
        }
    }
}