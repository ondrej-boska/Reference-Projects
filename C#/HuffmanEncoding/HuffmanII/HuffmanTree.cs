using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#nullable enable

namespace Huffman
{
    public class HuffmanNode : IComparable<HuffmanNode>
    {
        public HuffmanNode? left = null;
        public HuffmanNode? right = null;

        public byte? key;
        public long count;

        public int CompareTo(HuffmanNode? other)
        {
            if (other is null) return 1;
            else if (count < other.count) return -1;
            else if (other.count < count) return 1;
            else if (key is not null && other.key is not null) return (int)(key - other.key);
            else return 1;
        }

        bool isLeaf() => left is null && right is null;
    }

    public class HuffmanTree
    {
        public HuffmanNode? root { get; private set; }

        public HuffmanTree() { root = null; }

        public void CreateTreeFromStream(Stream stream)
        {
            Queue<HuffmanNode> leafQueue;
            Queue<HuffmanNode> innerQueue = new();

            long[] frequencies = CountByteFrequencies(stream);
            leafQueue = SortFrequenciesToQueue(frequencies);

            while (leafQueue.Count + innerQueue.Count > 1)
            {
                var node1 = ToDequeueFrom(leafQueue, innerQueue).Dequeue();
                var node2 = ToDequeueFrom(leafQueue, innerQueue).Dequeue();
                innerQueue.Enqueue(new HuffmanNode() { key = null, count = node1.count + node2.count, left = node1, right = node2 });
            }

            if (innerQueue.Count == 0)
            {
                if (leafQueue.Count > 0) root = leafQueue.Dequeue();
                else root = null;
            }
            else root = innerQueue.Dequeue();
        }

        long[] CountByteFrequencies(Stream stream)
        {
            long[] frequencies = new long[256];
            const int bufferSize = 4096;
            byte[] buffer = new byte[bufferSize];
            int bytesRead;

            while ((bytesRead = stream.Read(buffer, 0, bufferSize)) > 0)
            {
                for (int i = 0; i < bytesRead; i++)
                    frequencies[buffer[i]]++;
            }

            return frequencies;
        }

        Queue<HuffmanNode> ToDequeueFrom(Queue<HuffmanNode> leaf, Queue<HuffmanNode> inner)
        {
            if (inner.Count == 0) return leaf;
            else if (leaf.Count == 0) return inner;
            else return (inner.Peek().count < leaf.Peek().count) ? inner : leaf;
        }

        Queue<HuffmanNode> SortFrequenciesToQueue(long[] frequencies)
        {
            List<HuffmanNode> leafList = new();

            for (int i = 0; i < frequencies.Length; i++)
                if (frequencies[i] > 0) leafList.Add(new HuffmanNode() { key = (byte)i, count = frequencies[i] });
            leafList.Sort();
            return new Queue<HuffmanNode>(leafList);
        }

        public void PrintTreePreorder(TextWriter writer)
        {
            HuffmanNode? node = root;
            if (node is null) return;
            Stack<HuffmanNode> stack = new();
            stack.Push(node);

            while (stack.Count > 0)
            {
                node = stack.Pop();
                if (node.key is not null) writer.Write("*" + node.key + ":" + node.count);
                else writer.Write(node.count);

                if (node.right is not null) stack.Push(node.right);
                if (node.left is not null) stack.Push(node.left);

                if (stack.Count > 0) writer.Write(' ');
            }
        }
    }
}
