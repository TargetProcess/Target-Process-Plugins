using System.Collections;
using System.Linq;
using System.Text;

namespace Tp.Search.Model.Document
{
	class IndexDataFactory : IIndexDataFactory
	{
		public string CreateProjectData(int? projectId)
		{
			return EncodeStringId(projectId, "Project");
		}

		public string CreateEntityStateData(int entityStateId)
		{
			return EncodeStringId(entityStateId, "Entitystate");
		}

		public string CreateSquadData(int? squadId)
		{
			return EncodeStringId(squadId, "Squad");
		}

		public string CreateImpedimentData(bool? isPrivate, int? ownerId, int? responsibleId)
		{
			var parts = new[]
				{
					isPrivate.GetValueOrDefault() ? string.Empty : "public",
					ownerId.HasValue ? EncodeStringId(ownerId.Value, "Owner") : string.Empty,
					responsibleId.HasValue ? EncodeStringId(responsibleId.Value, "Responsible") : string.Empty
				};
			return string.Join(" ", parts.Where(x => !string.IsNullOrEmpty(x)));
		}

		private static string EncodeStringId(int? id, string prefix)
		{
			if (id == null)
			{
				return prefix + "null";
			}
			var array = new BitArray(new[] { id.Value });
			var b = new StringBuilder();
			foreach (bool f in array)
			{
				b.Append(f ? "t" : "f");
			}
			return prefix + b;
		}
	}
}