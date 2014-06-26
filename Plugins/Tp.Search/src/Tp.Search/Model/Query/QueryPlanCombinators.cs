using System;
using Tp.Core;

namespace Tp.Search.Model.Query
{
	static class QueryPlanCombinators
	{
		public static Maybe<QueryPlan> And(this Maybe<QueryPlan> left, Maybe<QueryPlan> right)
		{
			return Op(left, right, Operation.And);
		}

		public static Maybe<QueryPlan> Or(this Maybe<QueryPlan> left, Maybe<QueryPlan> right)
		{
			return Op(left, right, Operation.Or);
		}

		private static Maybe<QueryPlan> Op(Maybe<QueryPlan> left, Maybe<QueryPlan> right, Operation operation)
		{
			if (left.HasValue && right.HasValue)
			{
				switch (operation)
				{
					case Operation.And:
						return Maybe.Return(QueryPlan.And(left.Value, right.Value));
					case Operation.Or:
						return Maybe.Return(QueryPlan.Or(left.Value, right.Value));
					default:
						throw new ArgumentException("{0} is not supported.".Fmt(operation));
				}
			}
			return left.HasValue ? left : right;
		}

		private enum Operation
		{
			And,
			Or
		}
	}
}