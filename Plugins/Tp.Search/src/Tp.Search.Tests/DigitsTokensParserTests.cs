using NUnit.Framework;
using Tp.Testing.Common.NUnit;
using hOOt;

namespace Tp.Search.Tests
{
	[TestFixture]
    [Category("PartPlugins1")]
	public class DigitsTokensParserTests
	{
		private DigitsTokensParser _parser;

		[SetUp]
		public void SetUp()
		{
			_parser = new DigitsTokensParser();
		}

		[Test]
		public void Parse_qwerty123asdfg()
		{
			var d = _parser.Parse("qwerty123asdfg");
			d.Count.Should(Be.EqualTo(1), "d.Count.Should(Be.EqualTo(1))");
			d.ContainsKey("123").Should(Be.True, "d.ContainsKey(\"123\").Should(Be.True)");
		}

		[Test]
		public void Parse_qwerty_asdfg()
		{
			var d = _parser.Parse("qwerty asdfg");
			d.Count.Should(Be.EqualTo(0), "d.Count.Should(Be.EqualTo(0))");
		}

		[Test]
		public void Parse_123qwerty_asdfg567()
		{
			var d = _parser.Parse("123qwerty_asdfg567");
			d.Count.Should(Be.EqualTo(2), "d.Count.Should(Be.EqualTo(2))");
			d.ContainsKey("123").Should(Be.True, "d.ContainsKey(\"123\").Should(Be.True)");
			d.ContainsKey("567").Should(Be.True, "d.ContainsKey(\"567\").Should(Be.True)");
		}
	}
}