namespace ThinkEngine.it.unical.mat.objectsMapper.Mappers
{
    internal class ASPStringMapper : IMapper
    {
        private static ASPStringMapper _instance;
        public static ASPStringMapper instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ASPStringMapper();
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