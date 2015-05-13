using NUnit.Framework;
using StructureMap;
using Tp.Search.Model.Query;
using Tp.Search.StructureMap;
using Tp.Search.Tests.Registry;
using Tp.Testing.Common.NUnit;

namespace Tp.Search.Tests
{
	[TestFixture]
    [Category("PartPlugins1")]
	public class QueryParserTests
	{
		[SetUp]
		public void SetUp()
		{
			ObjectFactory.Initialize(e =>
			{
				e.AddRegistry(new PluginRegistry());
				e.AddRegistry(new TestRegistry());
			});
		}

		[Test]
		public void Digits0()
		{
			var queryParserResult = QueryParser().Parse("\"22test\"");
			queryParserResult.Words.Should(Be.EqualTo("+test"));
			queryParserResult.Numbers.Should(Be.EqualTo("+22"));
		}

		[Test]
		public void SingleSymbol()
		{
			var queryParserResult = QueryParser().Parse("2 t ");
			queryParserResult.Words.Should(Be.EqualTo(string.Empty));
			queryParserResult.Numbers.Should(Be.EqualTo("+2"));
		}

		[Test]
		public void Digits1()
		{
			var queryParserResult = QueryParser().Parse("\"22 local\"");
			queryParserResult.Words.Should(Be.EqualTo("+local"));
			queryParserResult.Numbers.Should(Be.EqualTo("+22"));
		}

		[Test]
		public void Digits2()
		{
			var queryParserResult = QueryParser().Parse("\"local@time\"");
			queryParserResult.Words.Should(Be.EqualTo("+local +time"));
			queryParserResult.Numbers.Should(Be.EqualTo(string.Empty));
		}

		[Test]
		public void Digits6()
		{
			var queryParserResult = QueryParser().Parse("\"zag zag. \"");
			queryParserResult.Words.Should(Be.EqualTo("+zag"));
			queryParserResult.Numbers.Should(Be.EqualTo(string.Empty));
		}

		[Test]
		public void Digits3()
		{
			var queryParserResult = QueryParser().Parse(@"""заголовки - h
[11:52:54 AM] Maryia Shchamialiova""");
			queryParserResult.Words.Should(Be.EqualTo("+заголовки +am +maryia +shchamialiova"));
			queryParserResult.Numbers.Should(Be.EqualTo("+115254"));
		}

		[Test]
		public void Digits4()
		{
			var queryParserResult = QueryParser().Parse(@"""если ты заголовки в колонках сделаешь h,,,, то не придется юзать классв special_color title""");
			queryParserResult.Words.Should(Be.EqualTo("+если +ты +заголовки +колонках +сделаешь +то +не +придется +юзать +классв +special +color +title"));
			queryParserResult.Numbers.Should(Be.EqualTo(string.Empty));
		}

		[Test]
		public void Digits7()
		{
			var queryParserResult = QueryParser().Parse(@"lead [lead]");
			queryParserResult.Words.Should(Be.EqualTo("*lead*"));
			queryParserResult.Numbers.Should(Be.EqualTo(string.Empty));
		}

		[Test]
		public void Digits8()
		{
			var queryParserResult = QueryParser().Parse(@"lead [lead*");
			queryParserResult.Words.Should(Be.EqualTo("*lead*"));
			queryParserResult.Numbers.Should(Be.EqualTo(string.Empty));
		}

		[Test]
		public void Digits10()
		{
			var queryParserResult = QueryParser().Parse(@"lead [lead?");
			queryParserResult.Words.Should(Be.EqualTo("*lead* *lead?"));
			queryParserResult.Numbers.Should(Be.EqualTo(string.Empty));
		}

		[Test]
		public void Digits11()
		{
			var queryParserResult = QueryParser().Parse("+bla.zag.");
			queryParserResult.Words.Should(Be.EqualTo("+bla +zag"));
			queryParserResult.Numbers.Should(Be.EqualTo(string.Empty));
		}

		[Test]
		public void Digits5()
		{
			var queryParserResult = QueryParser().ParseIntoWords(@"""заголовки - h
[11:52:54 AM] Maryia Shchamialiova"" еще ?чуть +не мандатори ""теперь снова мандатори""");
			queryParserResult.Should(Be.EquivalentTo("+заголовки +am +maryia +shchamialiova *еще* ?чуть +не *мандатори* +теперь +снова +мандатори +115254".Split(' ')));
		}

		[Test]
		public void Star()
		{
			var queryParserResult = QueryParser().Parse("*zag:xxx");
			queryParserResult.Words.Should(Be.EqualTo("*zag +xxx"));
			queryParserResult.Numbers.Should(Be.EqualTo(string.Empty));
		}

		[Test]
		public void SomeRandomStuff()
		{
			var queryParserResult = QueryParser().Parse("22zag:xxx:yyy");
			queryParserResult.Words.Should(Be.EqualTo("*zag* +xxx +yyy"));
			queryParserResult.Numbers.Should(Be.EqualTo("+22"));
		}

		[Test]
		public void SomeRandomStuff2()
		{
			var queryParserResult = QueryParser().Parse("\"zag:xxx:yyy\"");
			queryParserResult.Words.Should(Be.EqualTo("+zag +xxx +yyy"));
			queryParserResult.Numbers.Should(Be.EqualTo(string.Empty));
		}

		[Test]
		public void Test0()
		{
			var queryParserResult = QueryParser().Parse("!#$*%^f&#^$*% xxx");
			queryParserResult.Words.Should(Be.EqualTo("*xxx*"));
			queryParserResult.Numbers.Should(Be.EqualTo(string.Empty));
		}

		[Test]
		public void Test1()
		{
			var queryParserResult = QueryParser().Parse("!#$*%^ffff&#^$*% xxx");
			queryParserResult.Words.Should(Be.EqualTo("*ffff* *xxx*"));
			queryParserResult.Numbers.Should(Be.EqualTo(string.Empty));
		}

		[Test]
		public void Test2()
		{
			var queryParserResult = QueryParser().Parse("( like");
			queryParserResult.Words.Should(Be.EqualTo("*like*"));
			queryParserResult.Numbers.Should(Be.EqualTo(string.Empty));
		}

		[Test]
		public void Test3()
		{
			var queryParserResult = QueryParser().Parse("like.");
			queryParserResult.Words.Should(Be.EqualTo("like*"));
			queryParserResult.Numbers.Should(Be.EqualTo(string.Empty));
		}

		[Test]
		public void Test4()
		{
			var queryParserResult = QueryParser().Parse("+like -zag");
			queryParserResult.Words.Should(Be.EqualTo("+like -zag"));
			queryParserResult.Numbers.Should(Be.EqualTo(string.Empty));
		}

		[Test]
		public void Test5()
		{
			var queryParserResult = QueryParser().Parse("story1");
			queryParserResult.Words.Should(Be.EqualTo("*story*"));
			queryParserResult.Numbers.Should(Be.EqualTo("+1"));
		}

		[Test]
		public void Test6()
		{
			var queryParserResult = QueryParser().Parse("US2.2");
			queryParserResult.Words.Should(Be.EqualTo("*us*"));
			queryParserResult.Numbers.Should(Be.EqualTo("+22"));
		}
		private QueryParser QueryParser()
		{
			return ObjectFactory.GetInstance<QueryParser>();
		}
	}
}
