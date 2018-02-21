using System;
using System.Collections.Generic;
using System.Linq;
using Tp.Search.Model.Document;
using hOOt;

namespace Tp.Search.Model.Query
{
    class QueryPlan
    {
        private readonly Func<WAHBitArray> _evalCore;
        private readonly HashSet<ILock> _locks;

        public QueryPlan(Func<WAHBitArray> evalCore)
        {
            _evalCore = evalCore;
            _locks = new HashSet<ILock>(new LockEqComparer());
        }

        public static QueryPlan And(QueryPlan left, QueryPlan right)
        {
            var result = new QueryPlan(() => left._evalCore().And(right._evalCore()));
            CopyLocks(result, left, right);
            return result;
        }

        public static QueryPlan Or(QueryPlan left, QueryPlan right)
        {
            var result = new QueryPlan(() => left._evalCore().Or(right._evalCore()));
            CopyLocks(result, left, right);
            return result;
        }

        public void AddLock(ILock @lock)
        {
            _locks.Add(@lock);
        }

        public QueryPlanResult Eval()
        {
            List<LockToken> locks = _locks.OrderBy(i => i.Token).Select(o => o.Lock()).ToList();
            try
            {
                WAHBitArray result = _evalCore();
                return new QueryPlanResult(result, locks.ToDictionary(l => l.TypeToken, l => l.Version));
            }
            finally
            {
                locks.Reverse();
                locks.ForEach(l => l.Token.Dispose());
            }
        }

        private class LockEqComparer : IEqualityComparer<ILock>
        {
            public bool Equals(ILock x, ILock y)
            {
                return x.Token == y.Token;
            }

            public int GetHashCode(ILock obj)
            {
                return obj.Token.GetHashCode();
            }
        }

        private static void CopyLocks(QueryPlan into, params QueryPlan[] src)
        {
            foreach (var @lock in src.SelectMany(s => s._locks))
            {
                @into.AddLock(@lock);
            }
        }
    }
}
