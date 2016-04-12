using System.IO;
using System.Linq;
using NUnit.Framework;
using Tp.Testing.Common.NUnit;
using hOOt;

namespace Tp.Search.Tests
{
	[TestFixture]
    [Category("PartPlugins1")]
    public class HootTests
    {
			[Test]
			public void Project()
			{
				var h = new Hoot(Directory.GetCurrentDirectory(), "Fake", _ => { }, _ => { }, new CharacterTokensParser());
				const string key = "project";
				var d = h.GenerateWordFreq(key);
				d.ContainsKey(key).Should(Be.True, "d.ContainsKey(key).Should(Be.True)");
				h.Shutdown();
			}

			[Test]
			public void SomeLittleShit()
			{
				const string key = "squadnull";
				const string hootFileName = "Fake";
				var hootPath = Directory.GetCurrentDirectory();

				foreach (var file in Directory.GetFiles(hootPath, hootFileName + ".*", SearchOption.TopDirectoryOnly))
				{
					File.Delete(file);
				}

				var h = new Hoot(hootPath, hootFileName, _ => { }, _ => { }, new CharacterTokensParser());
				for (int i = 0; i < 11000; i++)
				{
					h.Index(i, key);
				}
				var rowsBeforeOptimize = h.FindRows(key);
				rowsBeforeOptimize.Count().Should(Be.EqualTo(11000), "rowsBeforeOptimize.Count().Should(Be.EqualTo(11000))");

				h.OptimizeIndex(true);
				var rowsAfterOptimize = h.FindRows(key);

				h.Shutdown();

				foreach (var file in Directory.GetFiles(hootPath, hootFileName + ".*", SearchOption.TopDirectoryOnly))
				{
					File.Delete(file);
				}

				rowsAfterOptimize.Count().Should(Be.EqualTo(11000), "rowsAfterOptimize.Count().Should(Be.EqualTo(11000))");
			}

			[Test]
			public void ProjectSpacebarSquad()
			{
				var h = new Hoot(Directory.GetCurrentDirectory(), "Fake", _ => { }, _ => { }, new CharacterTokensParser());
				const string key = "project squad";
				var d = h.GenerateWordFreq(key);
				d.ContainsKey("project").Should(Be.True, "d.ContainsKey(\"project\").Should(Be.True)");
				d.ContainsKey("squad").Should(Be.True, "d.ContainsKey(\"squad\").Should(Be.True)");
				h.Shutdown();
			}

			[Test]
			public void ProjectWithSpacebars()
			{
				var h = new Hoot(Directory.GetCurrentDirectory(), "Fake", _ => { }, _ => { }, new CharacterTokensParser());
				const string key = "project                    ";
				var d = h.GenerateWordFreq(key);
				d.ContainsKey("project").Should(Be.True, "d.ContainsKey(\"project\").Should(Be.True)");
				h.Shutdown();
			}
    }
}
