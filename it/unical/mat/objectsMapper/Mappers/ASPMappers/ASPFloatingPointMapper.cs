namespace EmbASP4Unity.it.unical.mat.objectsMapper.Mappers
{
    internal class ASPFloatingPointMapper : IMapper
    {
        private static ASPFloatingPointMapper _instance;
        public static ASPFloatingPointMapper instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ASPFloatingPointMapper();
                }
                return _instance;
            }
        }
        public string BasicMap(object o)
        {
            return "\"" + o+ "\"";
        }
    }
}