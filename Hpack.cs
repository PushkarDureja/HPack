using System.Text;

namespace HPack
{
    public class Hpack
    {
        #region variables

        private readonly DynamicTable _dynamicTable;

        private static readonly HashSet<string> _sensitiveHeaders = [];
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

        public List<byte> Pack(List<HeaderField> headerList)
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
                    EncodeHeader(BinaryFormat.IndexedHeaderField, headerField, packedHeaders, index);
                    continue;
                }

                // Literal Header Field Representation Never Indexed
                if (_sensitiveHeaders.Contains(headerField.Name))
                {
                    EncodeHeader(BinaryFormat.LiteralNeverIndexed, headerField, packedHeaders, index);
                    continue;
                }

                //if field is added successfully in dynamic table, use Literal Header Field Representation With Indexing else without Indexing
                if (_dynamicTable.Add(headerField))
                {
                    EncodeHeader(BinaryFormat.LiteralWithIndex, headerField, packedHeaders, index);
                }
                else
                {
                    EncodeHeader(BinaryFormat.LiteralWithoutIndex, headerField, packedHeaders, index);
                }
            }

            return packedHeaders;

        }

        public List<HeaderField> Unpack(List<byte> packedHeaderBytes)
        {
            List<HeaderField> headerList = [];

            int i = 0;
            while (i < packedHeaderBytes.Count)
            {
                BinaryFormat binaryFormat;

                //Indexed Header Field (1XXXXXXX)
                if ((packedHeaderBytes[i] & (1 << 7)) == 128)
                {
                    binaryFormat = BinaryFormat.IndexedHeaderField;
                }

                //Literal Header Field with Incremental Indexing (01XXXXXX)
                else if ((packedHeaderBytes[i] & (3 << 6)) == 64)
                {
                    binaryFormat = BinaryFormat.LiteralWithIndex;
                }

                //Literal Header Field without Indexing (0000XXXX)
                else if ((packedHeaderBytes[i] & (15 << 4)) == 0)
                {
                    binaryFormat = BinaryFormat.LiteralWithoutIndex;
                }

                //Literal Header Field Never Indexed (0001XXXX)
                else if ((packedHeaderBytes[i] & (15 << 4)) == 16)
                {
                    binaryFormat = BinaryFormat.LiteralNeverIndexed;
                }
                else
                {
                    throw new NotSupportedException("Header Prefix bytes does not match with any binary format");
                }

                (HeaderField headerField, int numberOfBytesProcessed) = DecodeHeader(binaryFormat, packedHeaderBytes, i);
                headerList.Add(headerField);
                i += numberOfBytesProcessed;

                if (binaryFormat == BinaryFormat.LiteralWithIndex)
                {
                    if (!_dynamicTable.Add(headerField))
                        throw new Exception("Unable to add to dynamic table");
                }
            }

            return headerList;
        }

        #endregion

        #region private

        private void EncodeHeader(BinaryFormat format, HeaderField headerField, List<byte> result, int headerIndex = int.MaxValue)
        {
            int N = 0;
            int prefixByte = 0;

            switch (format)
            {
                case BinaryFormat.None:
                    break;

                case BinaryFormat.IndexedHeaderField:
                    N = 7;
                    prefixByte = 1 << N;
                    break;

                case BinaryFormat.LiteralWithIndex:
                    N = 6;
                    prefixByte = 1 << N;
                    break;

                case BinaryFormat.LiteralWithoutIndex:
                    N = 4;
                    prefixByte = 0 << N;
                    break;

                case BinaryFormat.LiteralNeverIndexed:
                    N = 4;
                    prefixByte = 1 << N;
                    break;

                default:
                    throw new NotSupportedException($"Header Encoding Not Implemented for type {format}");
            }

            if (format == BinaryFormat.IndexedHeaderField)
            {
                if (headerIndex == 0 || headerIndex == int.MaxValue)
                    throw new NotSupportedException("Header Index out of range for BinaryFormat type 1");

                EncodeInteger((byte)prefixByte, headerIndex, N, result);
            }
            else
            {
                if (headerIndex != int.MaxValue)
                    EncodeInteger((byte)prefixByte, headerIndex, N, result);
                else
                    result.Add((byte)prefixByte);

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
            }
        }

        private (HeaderField, int) DecodeHeader(BinaryFormat format, List<byte> encodedBytes, int startIndex)
        {
            int numberOfBytesProcessed;
            int headerIndex;

            switch (format)
            {
                case BinaryFormat.IndexedHeaderField:
                    (headerIndex, numberOfBytesProcessed) = DecodeInteger(encodedBytes, 7, startIndex);
                    break;

                case BinaryFormat.LiteralWithIndex:
                    (headerIndex, numberOfBytesProcessed) = DecodeInteger(encodedBytes, 6, startIndex);
                    break;

                case BinaryFormat.LiteralNeverIndexed:
                case BinaryFormat.LiteralWithoutIndex:
                    (headerIndex, numberOfBytesProcessed) = DecodeInteger(encodedBytes, 4, startIndex);
                    break;

                default:
                    throw new NotSupportedException($"Header Encoding Not Implemented for type {format}");
            }

            if (headerIndex < 0 || headerIndex > _staticTable.Length + DynamicTable.Count)
                throw new NotSupportedException("Header index value out of bounds of static and dyanmic table");

            if (format == BinaryFormat.IndexedHeaderField)
            {
                if (headerIndex == 0)
                {
                    throw new NotSupportedException("Header Index cannot be 0 for BinaryFormat type 1");
                }

                HeaderField headerField = headerIndex <= _staticTable.Length ? _staticTable[headerIndex - 1] : _dynamicTable.GetElement(headerIndex - (_staticTable.Length + 1));
                return (headerField, numberOfBytesProcessed);
            }

            startIndex += numberOfBytesProcessed;

            string headerName;
            string headerValue;

            //Decoding Header Name

            //Get Header Name Index from Static/Dynamic Table
            if (headerIndex > 0)
            {
                headerName = headerIndex <= _staticTable.Length ? _staticTable[headerIndex - 1].Name : _dynamicTable.GetElement(headerIndex - (_staticTable.Length + 1)).Name;
            }
            else
            {
                //Check if Header Value is Huffman Encoded
                bool isHuffmanEncodedHeaderName = (encodedBytes[startIndex] & 128) == 128;

                //Get Header Name Length
                (int encodedHeaderNameLength, int bytesProcessedForHeaderNameLength) = DecodeInteger(encodedBytes, 7, startIndex);
                startIndex += bytesProcessedForHeaderNameLength;
                numberOfBytesProcessed += bytesProcessedForHeaderNameLength;

                //Get Header Name String
                if (isHuffmanEncodedHeaderName)
                {
                    headerName = Huffman.Decode([.. encodedBytes.GetRange(startIndex, encodedHeaderNameLength)]);
                }
                else
                {
                    headerName = Encoding.ASCII.GetString([.. encodedBytes.GetRange(startIndex, encodedHeaderNameLength)]);
                }

                startIndex += encodedHeaderNameLength;
                numberOfBytesProcessed += encodedHeaderNameLength;
            }

            //Decoding Header Value

            //Check if Header Value is Huffman Encoded
            bool isHuffmanEncodedHeaderValue = (encodedBytes[startIndex] & 128) == 128;

            //Get Header Value Length
            (int encodedHeaderValueLength, int bytesProcessedForHeaderValueLength) = DecodeInteger(encodedBytes, 7, startIndex);
            startIndex += bytesProcessedForHeaderValueLength;
            numberOfBytesProcessed += bytesProcessedForHeaderValueLength;

            //Get Header Value String
            if (isHuffmanEncodedHeaderValue)
            {
                headerValue = Huffman.Decode([.. encodedBytes.GetRange(startIndex, encodedHeaderValueLength)]);
            }
            else
            {
                headerValue = Encoding.ASCII.GetString([.. encodedBytes.GetRange(startIndex, encodedHeaderValueLength)]);
            }

            startIndex += encodedHeaderValueLength;
            numberOfBytesProcessed += encodedHeaderValueLength;

            HeaderField decodedHeaderField = new HeaderField(headerName, headerValue);
            return (decodedHeaderField, numberOfBytesProcessed);
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

        private (int, int) DecodeInteger(List<byte> bytes, int N, int startIndex)
        {
            if (startIndex < 0 || startIndex >= bytes.Count)
                throw new NotSupportedException("Decoding Failed, Invalid startIndex");

            int I = 0;
            int shift = 0;
            int idx = startIndex;
            bool isIntegerContinued = ((bytes[idx] << (8 - N)) | ((1 << (8 - N)) - 1)) == 0xFF;

            if (!isIntegerContinued)
            {
                I += (bytes[idx] & ((1 << N) - 1)) << shift;
                return (I, idx - startIndex + 1);
            }

            while (isIntegerContinued && idx < bytes.Count)
            {
                if (idx == startIndex)
                {
                    I += (int)Math.Pow(2, N) - 1;
                }
                else
                {
                    I += (bytes[idx] & ((1 << 7) - 1)) << shift;
                    isIntegerContinued = (byte)(bytes[idx] >> 7) == 1;
                    shift += 7;
                }

                idx++;
            }

            return (I, idx - startIndex);
        }

        #endregion

        #region properties

        public DynamicTable DynamicTable { get => _dynamicTable; }

        #endregion


        enum BinaryFormat
        {
            None = 0,
            IndexedHeaderField = 1,
            LiteralWithIndex = 2,
            LiteralWithoutIndex = 3,
            LiteralNeverIndexed = 4
        }

    }
}
