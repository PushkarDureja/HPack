namespace HPack
{
    public class HeaderField
    {
        #region variables

        readonly string _name;
        readonly string _value;

        #endregion

        #region constructor

        public HeaderField(string name, string value)
        {
            _name = name;
            _value = value;
        }

        #endregion

        #region properties

        public string Name { get => _name; }

        public string Value { get => _value; }

        public int Size { get => _name.Length + _value.Length + 32; }

        #endregion
    }
}
