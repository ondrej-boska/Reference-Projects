using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huffman
{
    public class HuffmanEncoder
    {
        readonly static byte[] header = { 0x7B, 0x68, 0x75, 0x7C, 0x6D, 0x7D, 0x66, 0x66 };

        HuffmanTree tree;
        FileStream readFile;

        (byte[] encoding, int encodingLength)[] encodingCache;

        public HuffmanEncoder(FileStream stream)
        {
            readFile = stream;
            tree = new HuffmanTree();
            tree.CreateTreeFromStream(stream);
            encodingCache = new (byte[], int)[256];
            FillEncodingCache();
        }

        public void EncodeToFile(Stream writeFile)
        {
            using BinaryWriter binaryWriter = new(writeFile);
            using BitWriter bitWriter = new(writeFile);
            readFile.Seek(0, SeekOrigin.Begin);

            writeFile.Write(header);
            WriteTreeToFile(binaryWriter);
            WriteEncodingToFile(bitWriter);
        }

        void WriteTreeToFile(BinaryWriter writer)
        {
            var node = tree.root;
            if (node is null) return;
            Stack<HuffmanNode> stack = new();
            stack.Push(node);

            while (stack.Count > 0)
            {
                node = stack.Pop();

                if (node.key is not null)
                    WriteLeafNodeToFile(writer, node);
                else
                {
                    WriteInnerNodeToFile(writer, node);
                    stack.Push(node.right!);
                    stack.Push(node.left!);
                }                
            }

            writer.Write((long)0); // write the footer - 8 empty bytes
        }

        void WriteInnerNodeToFile(BinaryWriter writer, HuffmanNode node)
        {
            long toWrite = 0;
            toWrite |= node.count << 1;
            toWrite &= ~((long)0xFF << 56); // set the first byte to zero

            writer.Write(toWrite);
        }

        void WriteLeafNodeToFile(BinaryWriter writer, HuffmanNode node)
        {
            if (node.key is null) return;

            long toWrite = 1;
            toWrite |= node.count << 1;
            toWrite = toWrite & ~((long)0xFF << 56) | ((long)node.key << 56); // write the value of node's key to the first byte

            writer.Write(toWrite);
        }

        void WriteEncodingToFile(BitWriter writer)
        {
            const int bufferSize = 4096;
            byte[] readBuffer = new byte[bufferSize];
            int bytesRead;

            while ((bytesRead = readFile.Read(readBuffer, 0, bufferSize)) > 0)
            {
                for (int i = 0; i < bytesRead; i++)
                {
                    var currByteEncoding = encodingCache[readBuffer[i]];
                    writer.Write(currByteEncoding.encoding, currByteEncoding.encodingLength);
                }
            }
        }

        struct StackNode
        {
            public HuffmanNode node;
            public int depth;
            public bool isRightSon;

            public StackNode(HuffmanNode node, int depth, bool isRightSon)
            {
                this.node = node;
                this.depth = depth;
                this.isRightSon = isRightSon;
            }
        }

        void FillEncodingCache()
        {
            byte[] encoding = new byte[32];

            if (tree.root is null) return;

            Stack<StackNode> stack = new();
            if (tree.root.right is not null) stack.Push(new(tree.root.right, 0, true));
            if (tree.root.left is not null) stack.Push(new(tree.root.left, 0, false));

            while (stack.Count > 0)
            {
                var poppedItem = stack.Pop();

                int currBytePos = poppedItem.depth / 8;
                int currBitPos = poppedItem.depth % 8;

                if (poppedItem.isRightSon) encoding[currBytePos] |= (byte) (1 << currBitPos);
                else encoding[currBytePos] &= (byte) ~(1 << currBitPos);

                if (poppedItem.node.key is not null)
                    encodingCache[poppedItem.node.key.Value] = (DeepCopyByteArray(encoding, (poppedItem.depth / 8) + 1), poppedItem.depth + 1);
                else
                {
                    stack.Push(new(poppedItem.node.right!, poppedItem.depth + 1, true));
                    stack.Push(new(poppedItem.node.left!, poppedItem.depth + 1, false));
                }
            }
        }

        byte[] DeepCopyByteArray(byte[] source, int? index)
        {
            if (index is null) index = source.Length;
            byte[] copy = new byte[index.Value];
            for (int i = 0; i < copy.Length; i++)
                copy[i] = source[i];
            return copy;
        }
    }

    public class BitWriter : IDisposable
    {
        Stream writeStream;
        byte toWrite = 0;
        int currBitIndex = 0;
        bool flushed = false;

        public BitWriter(Stream stream)
        {
            writeStream = stream;
        }

        void Write(byte input, int length)
        {
            for (int i = length; i < 8; i++)
                input &= (byte)~(1 << i);

            toWrite |= (byte)(input << currBitIndex);
            int written = Math.Min(8 - currBitIndex, length);
            currBitIndex += written;
            flushed = false;

            if (currBitIndex == 8)
            {
                FlushByte();
                if (written != length)
                {
                    toWrite |= (byte)(input >> written);
                    currBitIndex = Math.Min(8, length) - written;
                }
                else flushed = true;
            }
        }

        public void Write(byte[] input, int length)
        {
            int byteIndex = 0;
            while (length > 0)
            {
                Write(input[byteIndex], Math.Min(length, 8));
                length -= 8;
                ++byteIndex;
            }
        }

        void FlushByte()
        {
            writeStream.Write(new byte[] { toWrite });
            toWrite = 0;
            currBitIndex = 0;
        }

        public void Dispose()
        {
            if (!flushed) FlushByte();
        }
    }
}

