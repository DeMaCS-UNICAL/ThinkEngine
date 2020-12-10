namespace EmbASP4Unity.it.unical.mat.objectsMapper.Mappers
{ 
    internal class ASPUnsignedIntegerMapper : IMapper
    {
        private static ASPUnsignedIntegerMapper _instance;
        public static ASPUnsignedIntegerMapper instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ASPUnsignedIntegerMapper();
                }
                return _instance;
            }
        }
        public string BasicMap(object o)
        {
            return "" + o + "";
        }
    }
}