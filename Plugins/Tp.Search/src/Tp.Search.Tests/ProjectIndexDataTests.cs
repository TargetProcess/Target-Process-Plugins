using NUnit.Framework;
using Tp.Search.Model.Document;
using Tp.Testing.Common.NUnit;

namespace Tp.Search.Tests
{
	[TestFixture]
	[Category("PartPlugins1")]
	public class ProjectIndexDataTests
	{
		[Test]
		public void ProjectDataSumTest()
		{
			var left = new ProjectIndexData(new int?[]{1,2});
			var right = new ProjectIndexData(new int?[]{1,2,3});
			var result = ProjectIndexData.Sum(left, right);
			result.ProjectIds.Should(Be.EquivalentTo(new[]{1,2,3}));
		}

		[Test]
		public void ProjectDataSubstractTest()
		{
			var left = new ProjectIndexData(new int?[] { 1, 2 });
			var right = new ProjectIndexData(new int?[] { 1, 2, 3 });
			var result = ProjectIndexData.Substract(right, left);
			result.ProjectIds.Should(Be.EquivalentTo(new[] { 3 }));
		}
	}
}