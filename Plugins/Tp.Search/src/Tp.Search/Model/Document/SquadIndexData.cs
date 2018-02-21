using System.Collections.Generic;
using System.Linq;
using hOOt;

namespace Tp.Search.Model.Document
{
    public class SquadIndexData : IdsIndexData
    {
        private static readonly int? NoTeamId = null;

        private const string Prefix = "Squad";

        public SquadIndexData(IEnumerable<int?> squadIds) : base(Prefix, squadIds)
        {
        }

        public IEnumerable<int?> SquadIds
        {
            get { return Ids; }
        }

        public static SquadIndexData Sum(SquadIndexData left, SquadIndexData right)
        {
            var ids = IdsIndexData.Sum(left, right).ToList();
            // Exlude no team from teams collection if it contains any team
            if (ids.Count > 1 && ids.Contains(NoTeamId))
            {
                ids = ids.Except(new[] { NoTeamId }).ToList();
            }
            return new SquadIndexData(ids);
        }

        public static SquadIndexData Substract(SquadIndexData left, SquadIndexData right)
        {
            var ids = IdsIndexData.Substract(left, right).ToList();
            // Add no team when ids collention doesn't contains any real team
            if (ids.Count == 0)
            {
                ids.Add(NoTeamId);
            }
            return new SquadIndexData(ids);
        }

        public static SquadIndexData Parse(IndexData indexData)
        {
            return new SquadIndexData(IdsIndexData.Parse(Prefix, indexData));
        }
    }
}
