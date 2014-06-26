using System.Collections.Generic;
using Tp.Core;

namespace Tp.Search.Model.Entity
{
	interface IEntityTypeProvider
	{
		Maybe<string> GetEntityTypeName(int? entityTypeId);
		bool IsQueryable(int? entityTypeId);
		bool IsAssignable(int? entityTypeId);
		bool IsTestCase(int? entityTypeId);
		bool IsImpediment(int? entityTypeId);
		IEnumerable<string> NoSquadEntityTypeNames { get; }
		int?[] EntityTypeIds { get; }
	}
}