using Huffman;
using System.Reflection.PortableExecutable;
using System.Text;
namespace HuffmanII_UnitTests
{

    public class EncoderReCodExTests
    {
        [Fact]
        public void Simple1()
        {
            FileStream output = new("unittestfile.huff", FileMode.Create);

            FileStream input = new("simple.in", FileMode.Open, FileAccess.Read);
            string correctResult = new StreamReader("C:\\Users\\ondra\\source\\repos\\c#\\HuffmanII\\HuffmanII_UnitTests\\bin\\Debug\\net6.0\\correctoutput\\simple.in.huff").ReadToEnd();

            HuffmanEncoder encoder = new(input);
            encoder.EncodeToFile(output);

            Assert.Equal(correctResult, File.ReadAllText("unittestfile.huff"));

            output.Dispose();
            File.Delete("unittestfile.huff");
        }
        [Fact]
        public void Simple2()
        {
            FileStream output = new("unittestfile.huff", FileMode.Create);

            FileStream input = new("simple2.in", FileMode.Open, FileAccess.Read);
            string correctResult = new StreamReader("C:\\Users\\ondra\\source\\repos\\c#\\HuffmanII\\HuffmanII_UnitTests\\bin\\Debug\\net6.0\\correctoutput\\simple2.in.huff").ReadToEnd();

            HuffmanEncoder encoder = new(input);
            encoder.EncodeToFile(output);

            Assert.Equal(correctResult, File.ReadAllText("unittestfile.huff"));

            output.Dispose();
            File.Delete("unittestfile.huff");
        }
        [Fact]
        public void Simple3()
        {
            FileStream output = new("unittestfile.huff", FileMode.Create);

            FileStream input = new("simple3.in", FileMode.Open, FileAccess.Read);
            string correctResult = new StreamReader("C:\\Users\\ondra\\source\\repos\\c#\\HuffmanII\\HuffmanII_UnitTests\\bin\\Debug\\net6.0\\correctoutput\\simple3.in.huff").ReadToEnd();

            HuffmanEncoder encoder = new(input);
            encoder.EncodeToFile(output);

            Assert.Equal(correctResult, File.ReadAllText("unittestfile.huff"));

            output.Dispose();
            File.Delete("unittestfile.huff");
        }
        [Fact]
        public void Simple4()
        {
            FileStream output = new("unittestfile.huff", FileMode.Create);

            FileStream input = new("simple4.in", FileMode.Open, FileAccess.Read);
            string correctResult = new StreamReader("C:\\Users\\ondra\\source\\repos\\c#\\HuffmanII\\HuffmanII_UnitTests\\bin\\Debug\\net6.0\\correctoutput\\simple4.in.huff").ReadToEnd();

            HuffmanEncoder encoder = new(input);
            encoder.EncodeToFile(output);

            Assert.Equal(correctResult, File.ReadAllText("unittestfile.huff"));

            output.Dispose();
            File.Delete("unittestfile.huff");
        }

        [Fact]
        public void Binary()
        {
            FileStream output = new("unittestfile.huff", FileMode.Create);

            FileStream input = new("binary.in", FileMode.Open, FileAccess.Read);
            string correctResult = new StreamReader("C:\\Users\\ondra\\source\\repos\\c#\\HuffmanII\\HuffmanII_UnitTests\\bin\\Debug\\net6.0\\correctoutput\\binary.in.huff").ReadToEnd();

            HuffmanEncoder encoder = new(input);
            encoder.EncodeToFile(output);

            Assert.Equal(correctResult, File.ReadAllText("unittestfile.huff"));

            output.Dispose();
            File.Delete("unittestfile.huff");
        }
    }

    public class BitWriterTest
    {
        [Theory]
        [InlineData(new byte[] { 0b10101100 }, 8, new byte[] { 0b10101100 })]
        [InlineData(new byte[] { 0b10101100 }, 5, new byte[] { 0b00001100 })]
        [InlineData(new byte[] { 0b10101100, 0b11010001 }, 16, new byte[] { 0b10101100, 0b11010001 })]
        [InlineData(new byte[] { 0b10101100, 0b11010001 }, 12, new byte[] { 0b10101100, 0b00000001 })]
        public void TestBitWriter(byte[] toWrite, int length, byte[] correct)
        {
            MemoryStream output = new();
            BitWriter bitWriter = new(output);
            bitWriter.Write(toWrite, length);
            bitWriter.Dispose();

            output.Seek(0, SeekOrigin.Begin);
            byte[] wasRead = new byte[correct.Length];
            output.Read(wasRead, 0, correct.Length);
            Assert.Equal(correct, wasRead);
        }
    } 

    public class HuffmanNodeTest
    {
        [Fact]
        public void LeafNodesSort()
        {
            List<HuffmanNode> toSort = new()
            {
                new() { key = 0, count = 1 },
                new() { key = 5, count = 2 },
                new() { key = 11, count = 3 },
                new() { key = 8, count = 2 },
                new() { key = 20, count = 1 },
                new() { key = 1, count = 10 },
                new() { key = 12, count = 3 },
                new() { key = 13, count = 2 }
            };

            toSort.Sort();

            List<HuffmanNode> correctOrder = new List<HuffmanNode>
                {
                    new() { key = 0, count = 1 },
                    new() { key = 20, count = 1 },
                    new() { key = 5, count = 2 },
                    new() { key = 8, count = 2 },
                    new() { key = 13, count = 2 },
                    new() { key = 11, count = 3 },
                    new() { key = 12, count = 3 },
                    new() { key = 1, count = 10 }
                };

            Assert.Equal(correctOrder, toSort);
        }
    }

    public class HuffmanTreeTest
    {
        [Fact]
        public void CharsTest()
        {
            MemoryStream input = new(Encoding.ASCII.GetBytes("aaabbc"));
            HuffmanTree tree = new();
            tree.CreateTreeFromStream(input);
            StringWriter output = new();
            tree.PrintTreePreorder(output);

            string correctResult = "6 *97:3 3 *99:1 *98:2";

            Assert.Equal(correctResult, output.ToString(), false, true, true);
        }

        [Fact]
        public void BinaryTest()
        {
            MemoryStream input = new(new byte[] { 0, 3, 8, 3, 9, 20, 128, 255, 255, 1, 1, 1, 0, 0, 0, 3, 1, 21 });
            HuffmanTree tree = new();
            tree.CreateTreeFromStream(input);
            StringWriter output = new();
            tree.PrintTreePreorder(output);

            string correctResult = "18 8 *0:4 *1:4 10 4 2 *8:1 *9:1 2 *20:1 *21:1 6 *3:3 3 *128:1 *255:2";

            Assert.Equal(correctResult, output.ToString(), false, true, true);
        }

        [Fact]
        public void EmptyInput()
        {
            MemoryStream input = new(new byte[] { });
            HuffmanTree tree = new();
            tree.CreateTreeFromStream(input);
            StringWriter output = new();
            tree.PrintTreePreorder(output);

            string correctResult = "";

            Assert.Equal(correctResult, output.ToString(), false, true);
        }

        [Fact]
        public void OneByteOnly()
        {
            MemoryStream input = new(Encoding.ASCII.GetBytes("a"));
            HuffmanTree tree = new();
            tree.CreateTreeFromStream(input);
            StringWriter output = new();
            tree.PrintTreePreorder(output);

            string correctResult = "*97:1";

            Assert.Equal(correctResult, output.ToString(), false, true);
        }

        [Fact]
        public void TwoBytesOnly()
        {
            MemoryStream input = new(Encoding.ASCII.GetBytes("ab"));
            HuffmanTree tree = new();
            tree.CreateTreeFromStream(input);
            StringWriter output = new();
            tree.PrintTreePreorder(output);

            string correctResult = "2 *97:1 *98:1";

            Assert.Equal(correctResult, output.ToString(), false, true);
        }
    }
}