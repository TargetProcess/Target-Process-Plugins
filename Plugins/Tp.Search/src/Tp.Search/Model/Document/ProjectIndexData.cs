using System.Collections.Generic;
using System.Linq;
using hOOt;

namespace Tp.Search.Model.Document
{
	public class ProjectIndexData
	{
		public static readonly ProjectIndexData Empty = new ProjectIndexData(Enumerable.Empty<int?>());

		private readonly IEnumerable<int?> _projectIds;

		public ProjectIndexData(IEnumerable<int?> projectIds)
		{
			_projectIds = projectIds;
		}

		public IEnumerable<int?> ProjectIds
		{
			get { return _projectIds; }
		}

		public override string ToString()
		{
			return IndexDataStringServices.OfParts(_projectIds.Select(x => IndexDataStringServices.EncodeStringId(x, "Project")));
		}

		public static ProjectIndexData Sum(ProjectIndexData left, ProjectIndexData right)
		{
			var result = left.ProjectIds.Concat(right.ProjectIds).Distinct().ToList();
			return new ProjectIndexData(result);
		}

		public static ProjectIndexData Substract(ProjectIndexData left, ProjectIndexData right)
		{
			var result = left.ProjectIds.Except(right.ProjectIds).Distinct().ToList();
			return new ProjectIndexData(result);
		}

		public static ProjectIndexData Parse(IndexData indexData)
		{
			var ids = indexData.Words.Select(x => IndexDataStringServices.DecodeStringId(x, "Project")).ToList();
			return new ProjectIndexData(ids);
		}
	}
}