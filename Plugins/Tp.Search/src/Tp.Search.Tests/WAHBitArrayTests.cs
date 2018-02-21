using System.Linq;
using NUnit.Framework;
using Tp.Testing.Common.NUnit;
using hOOt;

namespace Tp.Search.Tests
{
    [TestFixture]
    [Category("PartPlugins1")]
    public class WAHBitArrayTests : SearchTestBase
    {
        [Test]
        public void TestWAHBitArray1()
        {
            var b = new WAHBitArray();
            int count = 31;
            for (int i = 0; i < count; i++)
            {
                b.Set(i, true);
            }
            var expected = b.GetBitIndexes().ToArray();
            var b2 = new WAHBitArray(WAHBitArray.TYPE.Compressed_WAH, b.GetCompressed());
            var actual = b2.GetBitIndexes().ToArray();
            expected.Should(Be.EqualTo(actual), "expected.Should(Be.EqualTo(actual))");
        }

        [Test]
        public void TestWAHBitArray2()
        {
            var b = new WAHBitArray();
            int count = 64;
            for (int i = 0; i < 5; i++)
            {
                b.Set(i, false);
            }
            for (int i = 5; i < count + 5; i++)
            {
                b.Set(i, true);
            }
            var expected = b.GetBitIndexes().ToArray();
            var b2 = new WAHBitArray(WAHBitArray.TYPE.Compressed_WAH, b.GetCompressed());
            var actual = b2.GetBitIndexes().ToArray();
            expected.Should(Be.EqualTo(actual), "expected.Should(Be.EqualTo(actual))");
        }

        [Test]
        public void TestWAHBitArray3()
        {
            var b = new WAHBitArray();
            int count = 25;
            for (int i = 0; i < 5; i++)
            {
                b.Set(i, false);
            }
            for (int i = 5; i < count + 5; i++)
            {
                b.Set(i, true);
            }
            for (int i = 30; i < 64; i++)
            {
                b.Set(i, i == 35);
            }
            for (int i = 64; i < 65; i++)
            {
                b.Set(i, true);
            }
            var expected = b.GetBitIndexes().ToArray();
            var b2 = new WAHBitArray(WAHBitArray.TYPE.Compressed_WAH, b.GetCompressed());
            var actual = b2.GetBitIndexes().ToArray();
            expected.Should(Be.EqualTo(actual), "expected.Should(Be.EqualTo(actual))");
        }

        [Test]
        public void TestWAHBitArray4()
        {
            var b = new WAHBitArray();
            int count = 25;
            for (int i = 0; i < 5; i++)
            {
                b.Set(i, true);
            }
            for (int i = 5; i < count + 5; i++)
            {
                b.Set(i, false);
            }
            for (int i = 30; i < 64; i++)
            {
                b.Set(i, i != 35);
            }
            for (int i = 64; i < 100; i++)
            {
                b.Set(i, true);
            }
            var expected = b.GetBitIndexes().ToArray();
            var b2 = new WAHBitArray(WAHBitArray.TYPE.Compressed_WAH, b.GetCompressed());
            var actual = b2.GetBitIndexes().ToArray();
            expected.Should(Be.EqualTo(actual), "expected.Should(Be.EqualTo(actual))");
        }
    }
}
