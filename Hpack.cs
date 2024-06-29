using System.Text;

namespace HPack
{
    public class Hpack
    {
        #region variables

        private readonly DynamicTable _dynamicTable;

        private static readonly HashSet<string> _secureHeaders = [];
        private static readonly HeaderField[] _staticTable = [
            new HeaderField(":authority", string.Empty),
            new HeaderField(":method", "GET"),
            new HeaderField(":method", "POST"),
            new HeaderField(":path", "/"),
            new HeaderField(":path", "/index.html"),
            new HeaderField(":scheme", "http"),
            new HeaderField(":scheme", "https"),
            new HeaderField(":status", "200"),
            new HeaderField(":status", "204"),
            new HeaderField(":status", "206"),
            new HeaderField(":status", "304"),
            new HeaderField(":status", "400"),
            new HeaderField(":status", "404"),
            new HeaderField(":status", "500"),
            new HeaderField("accept-charset", string.Empty),
            new HeaderField("accept-encoding", "gzip, deflate"),
            new HeaderField("accept-language", string.Empty),
            new HeaderField("accept-ranges", string.Empty),
            new HeaderField("accept", string.Empty),
            new HeaderField("access-control-allow-origin", string.Empty),
            new HeaderField("age", string.Empty),
            new HeaderField("allow", string.Empty),
            new HeaderField("authorization", string.Empty),
            new HeaderField("cache-control", string.Empty),
            new HeaderField("content-disposition", string.Empty),
            new HeaderField("content-encoding", string.Empty),
            new HeaderField("content-language", string.Empty),
            new HeaderField("content-length", string.Empty),
            new HeaderField("content-location", string.Empty),
            new HeaderField("content-range", string.Empty),
            new HeaderField("content-type", string.Empty),
            new HeaderField("cookie", string.Empty),
            new HeaderField("date", string.Empty),
            new HeaderField("etag", string.Empty),
            new HeaderField("expect", string.Empty),
            new HeaderField("expires", string.Empty),
            new HeaderField("from", string.Empty),
            new HeaderField("host", string.Empty),
            new HeaderField("if-match", string.Empty),
            new HeaderField("if-modified-since", string.Empty),
            new HeaderField("if-none-match", string.Empty),
            new HeaderField("if-range", string.Empty),
            new HeaderField("if-unmodified-since", string.Empty),
            new HeaderField("last-modified", string.Empty),
            new HeaderField("link", string.Empty),
            new HeaderField("location", string.Empty),
            new HeaderField("max-forwards", string.Empty),
            new HeaderField("proxy-authenticate", string.Empty),
            new HeaderField("proxy-authorization", string.Empty),
            new HeaderField("range", string.Empty),
            new HeaderField("referer", string.Empty),
            new HeaderField("refresh", string.Empty),
            new HeaderField("retry-after", string.Empty),
            new HeaderField("server", string.Empty),
            new HeaderField("set-cookie", string.Empty),
            new HeaderField("strict-transport-security", string.Empty),
            new HeaderField("transfer-encoding", string.Empty),
            new HeaderField("user-agent", string.Empty),
            new HeaderField("vary", string.Empty),
            new HeaderField("via", string.Empty),
            new HeaderField("www-authenticate", string.Empty)
        ];

        #endregion

        #region constructor

        public Hpack(int tableSize = 256)
        {
            _dynamicTable = new DynamicTable(tableSize);
        }

        #endregion

        #region public

        public byte[] Pack(List<HeaderField> headerList)
        {
            List<byte> packedHeaders = [];

            foreach (HeaderField headerField in headerList)
            {
                int index = int.MaxValue;
                bool indexedHeaderFieldFound = false;

                //search in static table
                for (int i = 0; i < _staticTable.Length; i++)
                {
                    HeaderField staticHeaderField = _staticTable[i];
                    if (staticHeaderField.Name == headerField.Name)
                    {
                        index = Math.Min(index, i + 1);
                    }

                    if (staticHeaderField.Name == headerField.Name && staticHeaderField.Value == headerField.Value)
                    {
                        indexedHeaderFieldFound = true;
                        index = i + 1;
                        break;
                    }
                }

                //search in dynamic table if not found in static
                if (!indexedHeaderFieldFound)
                {
                    for (int i = 0; i < _dynamicTable.Count; i++)
                    {
                        HeaderField dynamicHeaderField = _dynamicTable.GetElement(i);
                        if (dynamicHeaderField.Name == headerField.Name)
                        {
                            index = Math.Min(_staticTable.Length + i + 1, index);
                        }

                        if (dynamicHeaderField.Name == headerField.Name && dynamicHeaderField.Value == headerField.Value)
                        {
                            index = _staticTable.Length + i + 1;
                            indexedHeaderFieldFound = true;
                            break;
                        }
                    }
                }

                // Indexed header and value
                if (indexedHeaderFieldFound)
                {
                    byte packedByte = 1 << 7;
                    EncodeHeader(BinaryFormat.IndexedHeaderField, packedByte, headerField, packedHeaders, index);
                    continue;
                }

                // Literal Header Field Representation Never Indexed
                if (_secureHeaders.Contains(headerField.Name))
                {
                    byte packedByte = 1 << 4;
                    EncodeHeader(BinaryFormat.LiteralNeverIndexed, packedByte, headerField, packedHeaders, index);
                    continue;
                }

                //TODO also handle the case of Literal Header Field Representation Never Indexed

                //in all other cases, Literal Header Field Representation With Indexing
                byte packedbyte = 1 << 6;
                EncodeHeader(BinaryFormat.LiteralWithIndex, packedbyte, headerField, packedHeaders, index);

                //Add to dynamic table
                _dynamicTable.Add(headerField);
            }

            return [.. packedHeaders];

        }

        #endregion

        #region private

        private void EncodeHeader(BinaryFormat type, byte destination, HeaderField headerField, List<byte> result, int headerIndex = int.MaxValue)
        {
            switch (type)
            {
                case BinaryFormat.IndexedHeaderField:
                    if (headerIndex == int.MaxValue)
                        throw new Exception("Header Index cannot be -1 for BinaryFormat type 1");

                    EncodeInteger(destination, headerIndex, 7, result);
                    break;

                case BinaryFormat.LiteralNeverIndexed:
                case BinaryFormat.LiteralWithoutIndex:
                case BinaryFormat.LiteralWithIndex:
                    if (headerIndex != int.MaxValue)
                        EncodeInteger(destination, headerIndex, 6, result);
                    else
                        result.Add(destination);

                    //Encode header name
                    if (headerIndex == int.MaxValue)
                    {
                        byte[] huffmanEncodedHeaderName = Huffman.Encode(headerField.Name);
                        byte[] asciiEncodedHeaderName = Encoding.ASCII.GetBytes(headerField.Name);

                        bool useHuffmanEncodedHeaderName = huffmanEncodedHeaderName.Length < asciiEncodedHeaderName.Length;

                        //use ASCII Encoding
                        if (!useHuffmanEncodedHeaderName)
                        {
                            int headerNameLength = asciiEncodedHeaderName.Length;
                            byte headerNameLengthByte = 0;

                            EncodeInteger(headerNameLengthByte, headerNameLength, 7, result);
                            result.AddRange(asciiEncodedHeaderName);
                        }

                        //use Huffman Encoding
                        else
                        {
                            int headerNameLength = huffmanEncodedHeaderName.Length;
                            byte headerNameLengthByte = 1 << 7;

                            EncodeInteger(headerNameLengthByte, headerNameLength, 7, result);
                            result.AddRange(huffmanEncodedHeaderName);
                        }
                    }

                    //Encode header value
                    byte[] huffmanEncodedHeaderValue = Huffman.Encode(headerField.Value);
                    byte[] asciiEncodedHeaderValue = Encoding.ASCII.GetBytes(headerField.Value);

                    bool useHuffmanEncodedHeaderValue = huffmanEncodedHeaderValue.Length < headerField.Value.Length;

                    //use ASCII Encoding
                    if (!useHuffmanEncodedHeaderValue)
                    {
                        int headerValueLength = asciiEncodedHeaderValue.Length;
                        byte headerValueLengthByte = 0;

                        EncodeInteger(headerValueLengthByte, headerValueLength, 7, result);
                        result.AddRange(asciiEncodedHeaderValue);
                    }

                    //use Huffman Encoding
                    else
                    {
                        int headerValueLength = huffmanEncodedHeaderValue.Length;
                        byte headerValueLengthByte = 1 << 7;

                        EncodeInteger(headerValueLengthByte, headerValueLength, 7, result);
                        result.AddRange(huffmanEncodedHeaderValue);
                    }

                    break;

                default:
                    throw new NotImplementedException($"Header Encoding Not Implemented for type {type}");
            }
        }

        private void EncodeInteger(byte destination, int I, int N, List<byte> result)
        {
            if (I < Math.Pow(2, N) - 1)
            {
                destination |= (byte)I;
                result.Add(destination);
                return;
            }

            destination |= (byte)((1 << N) - 1);
            result.Add(destination);

            I -= (int)(Math.Pow(2, N) - 1);
            while (I > 127)
            {
                int newI = (I % 128);
                EncodeInteger(128, newI, 7, result);
                I /= 128;
            }

            EncodeInteger(0, I, 8, result);
        }

        private int DecodeInt(List<byte> bytes, int N)
        {
            int I = 0;
            for (int i = 0; i < bytes.Count; i++)
            {
                if (i == 0)
                {
                    byte res = (byte)((bytes[i] << (8 - N)) | ((1 << (8 - N)) - 1));

                    //check if all the bits are prefix bits are 1, meaning if number is continued in next bytes or not
                    if (res == 0xFF)
                    {
                        I += (int)Math.Pow(2, N) - 1;
                    }
                    else
                    {
                        I += bytes[i] & ((1 << N) - 1);
                        return I;
                    }
                }
                else
                {
                    byte continuationByteVal = (byte)(bytes[i] >> 7);
                    if (continuationByteVal == 1)
                    {
                        I += bytes[i] & ((1 << 7) - 1);
                    }
                    else
                    {
                        I += (bytes[i] & ((1 << 7) - 1)) * 128;
                        return I;
                    }
                }
            }
            return 0;
        }

        enum BinaryFormat
        {
            IndexedHeaderField = 1,
            LiteralWithIndex = 2,
            LiteralWithoutIndex = 3,
            LiteralNeverIndexed = 4
        }

        #endregion
    }

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
