using System.Collections.Generic;
using System.Linq;
using hOOt;

namespace Tp.Search.Model.Document
{
	public class ProjectIndexData : IdsIndexData
	{
		public static readonly ProjectIndexData Empty = new ProjectIndexData(Enumerable.Empty<int?>());

		private const string Prefix = "Project";

		public ProjectIndexData(IEnumerable<int?> projectIds) : base(Prefix, projectIds)
		{
		}

		public IEnumerable<int?> ProjectIds
		{
			get { return Ids; }
		}

		public static ProjectIndexData Sum(ProjectIndexData left, ProjectIndexData right)
		{
			return new ProjectIndexData(IdsIndexData.Sum(left, right));
		}

		public static ProjectIndexData Substract(ProjectIndexData left, ProjectIndexData right)
		{
			return new ProjectIndexData(IdsIndexData.Substract(left, right));
		}

		public static ProjectIndexData Parse(IndexData indexData)
		{
			return new ProjectIndexData(IdsIndexData.Parse(Prefix, indexData));
		}
	}
}