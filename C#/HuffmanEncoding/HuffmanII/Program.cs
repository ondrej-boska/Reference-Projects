using Huffman;
using System;
using System.IO;

namespace Huffman
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if(args.Length != 1)
            {
                Console.WriteLine("Argument Error");
                return;
            }

            FileStream reader, writer;
            try
            {
                reader = new FileStream(args[0], FileMode.Open, FileAccess.Read);
                writer = new(args[0] + ".huff", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            }
            catch
            {
                Console.WriteLine("File Error");
                return;
            }

            HuffmanEncoder encoder = new(reader);
            encoder.EncodeToFile(writer);

            reader.Dispose();
            writer.Dispose();
        }
    }
}