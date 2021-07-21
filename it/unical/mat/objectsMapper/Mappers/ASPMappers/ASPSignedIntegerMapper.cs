namespace ThinkEngine.it.unical.mat.objectsMapper.Mappers
{
    internal class ASPSignedIntegerMapper: IMapper
    {
        private static ASPSignedIntegerMapper _instance;
        public static ASPSignedIntegerMapper instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ASPSignedIntegerMapper();
                }
                return _instance;
            }
        }
        public string BasicMap(object l)
        {
            return ""+l+"";
        }
    }
}