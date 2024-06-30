namespace HPack
{
    public class Program
    {
        static void Main(string[] args)
        {
            EndToEndHpackTest();
        }

        public static void EndToEndHpackTest()
        {
            List<List<HeaderField>> headerList = [
                [
                    new(":status", "302"),
                    new("cache-control", "private"),
                    new("date", "Mon, 21 Oct 2013 20:13:21 GMT"),
                    new("location", "https://www.example.com")
                ],
                [
                    new(":status", "307"),
                    new("cache-control", "private"),
                    new("date", "Mon, 21 Oct 2013 20:13:21 GMT"),
                    new("location", "https://www.example.com"),
                ],
                [
                    new(":status", "200"),
                    new("cache-control", "private"),
                    new("date", "Mon, 21 Oct 2013 20:13:22 GMT"),
                    new("location", "https://www.example.com"),
                    new("content-encoding", "gzip"),
                    new("set-cookie", "foo=ASDJKHQKBZXOQWEOPIUAXQWEOIU; max-age=3600; version=1"),
                ],
                [
                    new(":method", "GET"),
                    new(":scheme", "http"),
                    new(":path", "/"),
                    new(":authority", "www.example.com"),
                ],
                [
                    new(":method", "GET"),
                    new(":scheme", "http"),
                    new(":path", "/"),
                    new(":authority", "www.example.com"),
                    new("cache-control", "no-cache"),
                ],
                [
                    new(":method", "GET"),
                    new(":scheme", "https"),
                    new(":path", "/index.html"),
                    new(":authority", "www.example.com"),
                    new("custom-key", "custom-value"),
                ]
            ];

            foreach (List<HeaderField> headers in headerList)
            {
                Console.WriteLine("### Original Headers ###");
                PrintHeaders(headers);

                Hpack clientHpack = new Hpack();
                List<byte> packedHeaders = clientHpack.Pack(headers);

                Hpack serverHpack = new Hpack();
                List<HeaderField> decodedHeaders = serverHpack.Unpack(packedHeaders);

                Console.WriteLine("### Decoded Headers ###");
                PrintHeaders(decodedHeaders);

                Console.WriteLine("\r\n");

                bool headersMatch = AreHeadersEqual(headers, decodedHeaders);
                Console.WriteLine("Headers match: " + headersMatch);

                bool dynamicTableMatch = AreDynamicTableEqual(clientHpack.DynamicTable, serverHpack.DynamicTable);
                Console.WriteLine("Dynamic Table match: " + dynamicTableMatch);

                Console.WriteLine("\r\n");
            }
        }

        public static void PrintHeaders(List<HeaderField> headers)
        {
            foreach (HeaderField headerField in headers)
            {
                Console.WriteLine($"{headerField.Name} : {headerField.Value}");
            }
        }

        public static bool AreHeadersEqual(List<HeaderField> originalHeaders, List<HeaderField> decodedHeaders)
        {
            if (originalHeaders.Count != decodedHeaders.Count)
            {
                return false;
            }

            for (int i = 0; i < originalHeaders.Count; i++)
            {
                if (originalHeaders[i].Name != decodedHeaders[i].Name ||
                    originalHeaders[i].Value != decodedHeaders[i].Value)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool AreDynamicTableEqual(DynamicTable table1, DynamicTable table2)
        {
            if (table1.Count != table2.Count)
            {
                return false;
            }

            for (int i = 0; i < table1.Count; i++)
            {
                if (table1.GetElement(i).Name != table2.GetElement(i).Name ||
                    table1.GetElement(i).Value != table2.GetElement(i).Value)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
