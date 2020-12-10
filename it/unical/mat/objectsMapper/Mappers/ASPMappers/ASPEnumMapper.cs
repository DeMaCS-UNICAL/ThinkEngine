namespace EmbASP4Unity.it.unical.mat.objectsMapper.Mappers
{
    internal class ASPEnumMapper : IMapper
    {
        private static ASPEnumMapper _instance;
        public static ASPEnumMapper instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ASPEnumMapper();
                }
                return _instance;
            }
        }
        public string BasicMap(object o)
        {
            return "" + o;
        }
    }
}