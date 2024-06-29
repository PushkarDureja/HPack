namespace HPack
{
    public static class Huffman
    {
        #region variables

        static readonly HuffmanField[] huffmanTable = {
        new HuffmanField(0x1ff8, 13),
        new HuffmanField(0x7fffd8, 23),
        new HuffmanField(0xfffffe2, 28),
        new HuffmanField(0xfffffe3, 28),
        new HuffmanField(0xfffffe4, 28),
        new HuffmanField(0xfffffe5, 28),
        new HuffmanField(0xfffffe6, 28),
        new HuffmanField(0xfffffe7, 28),
        new HuffmanField(0xfffffe8, 28),
        new HuffmanField(0xffffea, 24),
        new HuffmanField(0x3ffffffc, 30),
        new HuffmanField(0xfffffe9, 28),
        new HuffmanField(0xfffffea, 28),
        new HuffmanField(0x3ffffffd, 30),
        new HuffmanField(0xfffffeb, 28),
        new HuffmanField(0xfffffec, 28),
        new HuffmanField(0xfffffed, 28),
        new HuffmanField(0xfffffee, 28),
        new HuffmanField(0xfffffef, 28),
        new HuffmanField(0xffffff0, 28),
        new HuffmanField(0xffffff1, 28),
        new HuffmanField(0xffffff2, 28),
        new HuffmanField(0x3ffffffe, 30),
        new HuffmanField(0xffffff3, 28),
        new HuffmanField(0xffffff4, 28),
        new HuffmanField(0xffffff5, 28),
        new HuffmanField(0xffffff6, 28),
        new HuffmanField(0xffffff7, 28),
        new HuffmanField(0xffffff8, 28),
        new HuffmanField(0xffffff9, 28),
        new HuffmanField(0xffffffa, 28),
        new HuffmanField(0xffffffb, 28),
        new HuffmanField(0x14, 6),
        new HuffmanField(0x3f8, 10),
        new HuffmanField(0x3f9, 10),
        new HuffmanField(0xffa, 12),
        new HuffmanField(0x1ff9, 13),
        new HuffmanField(0x15, 6),
        new HuffmanField(0xf8, 8),
        new HuffmanField(0x7fa, 11),
        new HuffmanField(0x3fa, 10),
        new HuffmanField(0x3fb, 10),
        new HuffmanField(0xf9, 8),
        new HuffmanField(0x7fb, 11),
        new HuffmanField(0xfa, 8),
        new HuffmanField(0x16, 6),
        new HuffmanField(0x17, 6),
        new HuffmanField(0x18, 6),
        new HuffmanField(0x0, 5),
        new HuffmanField(0x1, 5),
        new HuffmanField(0x2, 5),
        new HuffmanField(0x19, 6),
        new HuffmanField(0x1a, 6),
        new HuffmanField(0x1b, 6),
        new HuffmanField(0x1c, 6),
        new HuffmanField(0x1d, 6),
        new HuffmanField(0x1e, 6),
        new HuffmanField(0x1f, 6),
        new HuffmanField(0x5c, 7),
        new HuffmanField(0xfb, 8),
        new HuffmanField(0x7ffc, 15),
        new HuffmanField(0x20, 6),
        new HuffmanField(0xffb, 12),
        new HuffmanField(0x3fc, 10),
        new HuffmanField(0x1ffa, 13),
        new HuffmanField(0x21, 6),
        new HuffmanField(0x5d, 7),
        new HuffmanField(0x5e, 7),
        new HuffmanField(0x5f, 7),
        new HuffmanField(0x60, 7),
        new HuffmanField(0x61, 7),
        new HuffmanField(0x62, 7),
        new HuffmanField(0x63, 7),
        new HuffmanField(0x64, 7),
        new HuffmanField(0x65, 7),
        new HuffmanField(0x66, 7),
        new HuffmanField(0x67, 7),
        new HuffmanField(0x68, 7),
        new HuffmanField(0x69, 7),
        new HuffmanField(0x6a, 7),
        new HuffmanField(0x6b, 7),
        new HuffmanField(0x6c, 7),
        new HuffmanField(0x6d, 7),
        new HuffmanField(0x6e, 7),
        new HuffmanField(0x6f, 7),
        new HuffmanField(0x70, 7),
        new HuffmanField(0x71, 7),
        new HuffmanField(0x72, 7),
        new HuffmanField(0xfc, 8),
        new HuffmanField(0x73, 7),
        new HuffmanField(0xfd, 8),
        new HuffmanField(0x1ffb, 13),
        new HuffmanField(0x7fff0, 19),
        new HuffmanField(0x1ffc, 13),
        new HuffmanField(0x3ffc, 14),
        new HuffmanField(0x22, 6),
        new HuffmanField(0x7ffd, 15),
        new HuffmanField(0x3, 5),
        new HuffmanField(0x23, 6),
        new HuffmanField(0x4, 5),
        new HuffmanField(0x24, 6),
        new HuffmanField(0x5, 5),
        new HuffmanField(0x25, 6),
        new HuffmanField(0x26, 6),
        new HuffmanField(0x27, 6),
        new HuffmanField(0x6, 5),
        new HuffmanField(0x74, 7),
        new HuffmanField(0x75, 7),
        new HuffmanField(0x28, 6),
        new HuffmanField(0x29, 6),
        new HuffmanField(0x2a, 6),
        new HuffmanField(0x7, 5),
        new HuffmanField(0x2b, 6),
        new HuffmanField(0x76, 7),
        new HuffmanField(0x2c, 6),
        new HuffmanField(0x8, 5),
        new HuffmanField(0x9, 5),
        new HuffmanField(0x2d, 6),
        new HuffmanField(0x77, 7),
        new HuffmanField(0x78, 7),
        new HuffmanField(0x79, 7),
        new HuffmanField(0x7a, 7),
        new HuffmanField(0x7b, 7),
        new HuffmanField(0x7ffe, 15),
        new HuffmanField(0x7fc, 11),
        new HuffmanField(0x3ffd, 14),
        new HuffmanField(0x1ffd, 13),
        new HuffmanField(0xffffffc, 28),
        new HuffmanField(0xfffe6, 20),
        new HuffmanField(0x3fffd2, 22),
        new HuffmanField(0xfffe7, 20),
        new HuffmanField(0xfffe8, 20),
        new HuffmanField(0x3fffd3, 22),
        new HuffmanField(0x3fffd4, 22),
        new HuffmanField(0x3fffd5, 22),
        new HuffmanField(0x7fffd9, 23),
        new HuffmanField(0x3fffd6, 22),
        new HuffmanField(0x7fffda, 23),
        new HuffmanField(0x7fffdb, 23),
        new HuffmanField(0x7fffdc, 23),
        new HuffmanField(0x7fffdd, 23),
        new HuffmanField(0x7fffde, 23),
        new HuffmanField(0xffffeb, 24),
        new HuffmanField(0x7fffdf, 23),
        new HuffmanField(0xffffec, 24),
        new HuffmanField(0xffffed, 24),
        new HuffmanField(0x3fffd7, 22),
        new HuffmanField(0x7fffe0, 23),
        new HuffmanField(0xffffee, 24),
        new HuffmanField(0x7fffe1, 23),
        new HuffmanField(0x7fffe2, 23),
        new HuffmanField(0x7fffe3, 23),
        new HuffmanField(0x7fffe4, 23),
        new HuffmanField(0x1fffdc, 21),
        new HuffmanField(0x3fffd8, 22),
        new HuffmanField(0x7fffe5, 23),
        new HuffmanField(0x3fffd9, 22),
        new HuffmanField(0x7fffe6, 23),
        new HuffmanField(0x7fffe7, 23),
        new HuffmanField(0xffffef, 24),
        new HuffmanField(0x3fffda, 22),
        new HuffmanField(0x1fffdd, 21),
        new HuffmanField(0xfffe9, 20),
        new HuffmanField(0x3fffdb, 22),
        new HuffmanField(0x3fffdc, 22),
        new HuffmanField(0x7fffe8, 23),
        new HuffmanField(0x7fffe9, 23),
        new HuffmanField(0x1fffde, 21),
        new HuffmanField(0x7fffea, 23),
        new HuffmanField(0x3fffdd, 22),
        new HuffmanField(0x3fffde, 22),
        new HuffmanField(0xfffff0, 24),
        new HuffmanField(0x1fffdf, 21),
        new HuffmanField(0x3fffdf, 22),
        new HuffmanField(0x7fffeb, 23),
        new HuffmanField(0x7fffec, 23),
        new HuffmanField(0x1fffe0, 21),
        new HuffmanField(0x1fffe1, 21),
        new HuffmanField(0x3fffe0, 22),
        new HuffmanField(0x1fffe2, 21),
        new HuffmanField(0x7fffed, 23),
        new HuffmanField(0x3fffe1, 22),
        new HuffmanField(0x7fffee, 23),
        new HuffmanField(0x7fffef, 23),
        new HuffmanField(0xfffea, 20),
        new HuffmanField(0x3fffe2, 22),
        new HuffmanField(0x3fffe3, 22),
        new HuffmanField(0x3fffe4, 22),
        new HuffmanField(0x7ffff0, 23),
        new HuffmanField(0x3fffe5, 22),
        new HuffmanField(0x3fffe6, 22),
        new HuffmanField(0x7ffff1, 23),
        new HuffmanField(0x3ffffe0, 26),
        new HuffmanField(0x3ffffe1, 26),
        new HuffmanField(0xfffeb, 20),
        new HuffmanField(0x7fff1, 19),
        new HuffmanField(0x3fffe7, 22),
        new HuffmanField(0x7ffff2, 23),
        new HuffmanField(0x3fffe8, 22),
        new HuffmanField(0x1ffffec, 25),
        new HuffmanField(0x3ffffe2, 26),
        new HuffmanField(0x3ffffe3, 26),
        new HuffmanField(0x3ffffe4, 26),
        new HuffmanField(0x7ffffde, 27),
        new HuffmanField(0x7ffffdf, 27),
        new HuffmanField(0x3ffffe5, 26),
        new HuffmanField(0xfffff1, 24),
        new HuffmanField(0x1ffffed, 25),
        new HuffmanField(0x7fff2, 19),
        new HuffmanField(0x1fffe3, 21),
        new HuffmanField(0x3ffffe6, 26),
        new HuffmanField(0x7ffffe0, 27),
        new HuffmanField(0x7ffffe1, 27),
        new HuffmanField(0x3ffffe7, 26),
        new HuffmanField(0x7ffffe2, 27),
        new HuffmanField(0xfffff2, 24),
        new HuffmanField(0x1fffe4, 21),
        new HuffmanField(0x1fffe5, 21),
        new HuffmanField(0x3ffffe8, 26),
        new HuffmanField(0x3ffffe9, 26),
        new HuffmanField(0xffffffd, 28),
        new HuffmanField(0x7ffffe3, 27),
        new HuffmanField(0x7ffffe4, 27),
        new HuffmanField(0x7ffffe5, 27),
        new HuffmanField(0xfffec, 20),
        new HuffmanField(0xfffff3, 24),
        new HuffmanField(0xfffed, 20),
        new HuffmanField(0x1fffe6, 21),
        new HuffmanField(0x3fffe9, 22),
        new HuffmanField(0x1fffe7, 21),
        new HuffmanField(0x1fffe8, 21),
        new HuffmanField(0x7ffff3, 23),
        new HuffmanField(0x3fffea, 22),
        new HuffmanField(0x3fffeb, 22),
        new HuffmanField(0x1ffffee, 25),
        new HuffmanField(0x1ffffef, 25),
        new HuffmanField(0xfffff4, 24),
        new HuffmanField(0xfffff5, 24),
        new HuffmanField(0x3ffffea, 26),
        new HuffmanField(0x7ffff4, 23),
        new HuffmanField(0x3ffffeb, 26),
        new HuffmanField(0x7ffffe6, 27),
        new HuffmanField(0x3ffffec, 26),
        new HuffmanField(0x3ffffed, 26),
        new HuffmanField(0x7ffffe7, 27),
        new HuffmanField(0x7ffffe8, 27),
        new HuffmanField(0x7ffffe9, 27),
        new HuffmanField(0x7ffffea, 27),
        new HuffmanField(0x7ffffeb, 27),
        new HuffmanField(0xffffffe, 28),
        new HuffmanField(0x7ffffec, 27),
        new HuffmanField(0x7ffffed, 27),
        new HuffmanField(0x7ffffee, 27),
        new HuffmanField(0x7ffffef, 27),
        new HuffmanField(0x7fffff0, 27),
        new HuffmanField(0x3ffffee, 26),
        new HuffmanField(0x3fffffff, 30) // EOS
    };
        static readonly HuffmanNode _root;

        #endregion

        #region constructor

        static Huffman()
        {
            _root = BuildTree();
        }

        #endregion

        #region public

        public static byte[] Encode(string input)
        {
            List<byte> encodedBytes = [];

            int res = 0;
            int remainingBits = 8;

            foreach (char ch in input)
            {
                int code = huffmanTable[ch].Code;
                int codeLength = huffmanTable[ch].Length;

                while (codeLength > 0)
                {
                    //example space left in a byte - 3bits and codeLength is 6bits, so we take only first 3 bits from code, thats why min of both
                    int numberOfBitsToWrite = Math.Min(codeLength, remainingBits);

                    //shift all the bits to left side, to create space for bits to be added on right side, spaceLength is numberOfBitsToWrite
                    res <<= numberOfBitsToWrite;

                    //first extract only required bits from code then OR that required bits with result
                    //example, codeLength - 6bits and numberOfBitsToWrite - 4bits, so we extract 4 MSB by doing rightshift by (6 - 4) bits
                    res |= (code >> (codeLength - numberOfBitsToWrite));

                    codeLength -= numberOfBitsToWrite;
                    remainingBits -= numberOfBitsToWrite;

                    //if all bits are filled, push result byte into result array
                    if (remainingBits == 0)
                    {
                        encodedBytes.Add((byte)res);
                        remainingBits = 8;
                        res = 0;
                    }
                }
            }

            // if some space left in the result byte, append it with 1's, (EOS HuffmanField HPACK)
            if (remainingBits > 0 && remainingBits < 8)
            {
                //create space on the right side
                res <<= remainingBits;

                //example remainingBits - 5, current result byte looks like this - 00000111
                //shift left by 5 -> 11100000, now we need add 1's in all the places of 0's
                //we can do that by ORing it with 00011111 -> (1 << 5) - 1
                res |= (1 << remainingBits) - 1;
                encodedBytes.Add((byte)res);
            }

            return [.. encodedBytes];
        }

        public static string Decode(byte[] encodedBytes)
        {
            HuffmanNode? foundNode = _root;
            string outputString = "";

            for (int i = 0; i < encodedBytes.Length; i++)
            {
                int j = 7;
                while (foundNode != null && j >= 0)
                {
                    // we take each bit to the extreme right of the byte to get its value by ANDing with 1
                    int bit = (encodedBytes[i] >> j) & 1;
                    if (bit == 1)
                    {
                        foundNode = foundNode.Right;
                    }
                    else
                    {
                        foundNode = foundNode.Left;
                    }

                    if (foundNode == null)
                        break;

                    if (foundNode.Left == null && foundNode.Right == null)
                    {
                        outputString += foundNode.Value;
                        foundNode = _root;
                    }
                    j--;
                }

            }

            if (foundNode == null)
            {
                throw new Exception("node not found");
            }

            return outputString;
        }

        #endregion

        #region private

        private static HuffmanNode BuildTree()
        {
            HuffmanNode root = new HuffmanNode(-1, '\0');
            for (int i = 0; i < huffmanTable.Length; i++)
            {
                InsertNode(root, huffmanTable[i], i);
            }

            return root;
        }

        private static void InsertNode(HuffmanNode root, HuffmanField huffman, int idx)
        {
            int code = huffman.Code;
            int length = huffman.Length;

            for (int i = length - 1; i >= 0; i--)
            {
                int bit = (code >> i) & 1;
                HuffmanNode node = new HuffmanNode(bit, i == 0 ? (char)(idx) : '\0');

                if (bit == 0)
                {
                    if (root.Left == null)
                        root.Left = node;

                    root = root.Left;
                }
                else
                {
                    if (root.Right == null)
                        root.Right = node;

                    root = root.Right;
                }
            }
        }

        class HuffmanField
        {
            public int Code { get; set; }
            public int Length { get; set; }

            public HuffmanField(int code, int length)
            {
                Code = code;
                Length = length;
            }
        }

        class HuffmanNode
        {
            int _bit;
            char _value;
            HuffmanNode? _left;
            HuffmanNode? _right;

            public HuffmanNode(int bit, char value)
            {
                _bit = bit;
                _value = value;
                _left = null;
                _right = null;
            }

            public int Bit { get => _bit; set => _bit = value; }
            public char Value { get => _value; set => _value = value; }
            public HuffmanNode? Left { get => _left; set => _left = value; }
            public HuffmanNode? Right { get => _right; set => _right = value; }
        }

        #endregion
    }
}
