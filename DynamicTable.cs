namespace HPack
{
    public class DynamicTable
    {
        #region variables

        int _currentSize = 0;

        readonly int _maxCapacity;
        readonly List<HeaderField> _table;

        #endregion

        #region constructor

        public DynamicTable(int maxCapacityInBytes = 256)
        {
            _table = [];
            _maxCapacity = maxCapacityInBytes;
        }

        #endregion

        #region public

        public HeaderField GetElement(int index)
        {
            return _table[index];
        }

        public void Add(HeaderField header)
        {
            while (_currentSize + header.Size > _maxCapacity)
            {
                int lastTableItemSize = _table.Last().Size;
                _table.RemoveAt(_table.Count - 1);
                _currentSize -= lastTableItemSize;
            }

            _table.Insert(0, header);
            _currentSize += header.Size;
        }

        #endregion

        #region properties

        public int Count { get => _table.Count; }

        #endregion
    }
}
