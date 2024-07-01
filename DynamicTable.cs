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

        public bool Add(HeaderField header)
        {
            while (_currentSize > 0 && _currentSize + header.Size > _maxCapacity)
            {
                int lastTableItemSize = _table.Last().Size;
                _table.RemoveAt(_table.Count - 1);
                _currentSize -= lastTableItemSize;
            }

            if (_currentSize + header.Size <= _maxCapacity)
            {
                _table.Insert(0, header);
                _currentSize += header.Size;
                return true;
            }

            return false;
        }

        #endregion

        #region properties

        public int Count { get => _table.Count; }

        public int Capacity { get => _maxCapacity; }

        #endregion
    }
}
