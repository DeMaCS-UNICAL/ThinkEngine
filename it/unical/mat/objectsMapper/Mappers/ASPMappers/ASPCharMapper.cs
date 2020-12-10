namespace EmbASP4Unity.it.unical.mat.objectsMapper.Mappers
{
    internal class ASPCharMapper : IMapper
    {
        private static ASPCharMapper _instance;
        public static ASPCharMapper instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ASPCharMapper();
                }
                return _instance;
            }
        }
        public string BasicMap(object o)
        {
            return "\"" + o + "\"";
        }
    }
}