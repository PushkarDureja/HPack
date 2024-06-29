namespace HPack
{
    public class Program
    {
        static void Main(string[] args)
        {
            HuffmanTest();
            HpackTest1();
            HpackTest2();
        }

        public static void HuffmanTest()
        {
            //string input = "\0\u0001\u0002\u0003\u0004\u0005\u0006\u0007\u0008\u0009\n\u000B\u000C\r\u000E\u000F\u0010\u0011\u0012\u0013\u0014\u0015\u0016\u0017\u0018\u0019\u001A\u001B\u001C\u001D\u001E\u001F !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~\u007F";
            string input = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            byte[] encodedData = Huffman.Encode(input);
            string decodedData = Huffman.Decode(encodedData);

            Console.WriteLine(decodedData.Equals(input));
        }

        public static void HpackTest1()
        {
            Hpack hpack = new Hpack();

            List<HeaderField> headers = [
                new(":method", "GET"),
                new(":scheme", "http"),
                new(":path", "/"),
                new(":authority", "www.example.com"),
            ];

            byte[] packedHeaders = hpack.Pack(headers);
            File.WriteAllBytes("encodedHeaders1", packedHeaders);

            List<HeaderField> headers2 = [
                new(":method", "GET"),
                new(":scheme", "http"),
                new(":path", "/"),
                new(":authority", "www.example.com"),
                new("cache-control", "no-cache"),
            ];
            byte[] packedHeaders2 = hpack.Pack(headers2);
            File.WriteAllBytes("encodedHeaders2", packedHeaders2);

            List<HeaderField> headers3 = [
               new(":method", "GET"),
                new(":scheme", "https"),
                new(":path", "/index.html"),
                new(":authority", "www.example.com"),
                new("custom-key", "custom-value"),
            ];
            byte[] packedHeaders3 = hpack.Pack(headers3);
            File.WriteAllBytes("encodedHeaders3", packedHeaders3);
        }

        public static void HpackTest2()
        {
            Hpack hpack = new Hpack();
            List<HeaderField> responseHeaders = [
                new(":status", "302"),
                new("cache-control", "private"),
                new("date", "Mon, 21 Oct 2013 20:13:21 GMT"),
                new("location", "https://www.example.com"),
            ];

            byte[] packedHeaders = hpack.Pack(responseHeaders);
            File.WriteAllBytes("encodedResponseHeaders1", packedHeaders);

            List<HeaderField> responseHeaders2 = [
                new(":status", "307"),
                new("cache-control", "private"),
                new("date", "Mon, 21 Oct 2013 20:13:21 GMT"),
                new("location", "https://www.example.com"),
            ];

            byte[] packedHeaders2 = hpack.Pack(responseHeaders2);
            File.WriteAllBytes("encodedResponseHeaders2", packedHeaders2);

            List<HeaderField> responseHeaders3 = [
                new(":status", "200"),
                new("cache-control", "private"),
                new("date", "Mon, 21 Oct 2013 20:13:22 GMT"),
                new("location", "https://www.example.com"),
                new("content-encoding", "gzip"),
                new("set-cookie", "foo=ASDJKHQKBZXOQWEOPIUAXQWEOIU; max-age=3600; version=1"),
            ];

            byte[] packedHeaders3 = hpack.Pack(responseHeaders3);
            File.WriteAllBytes("encodedResponseHeaders3", packedHeaders3);
        }
    }
}
