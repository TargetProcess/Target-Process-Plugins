using System.Linq.Expressions;

namespace System.Linq.Dynamic
{
	public static class UntypedQueryable
	{
		public static IQueryable Where(this IQueryable source, LambdaExpression predicate)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));
			if (predicate == null) throw new ArgumentNullException(nameof(predicate));

			return source.Provider.CreateQuery(
				Expression.Call(
					typeof(Queryable), "Where",
					new[] { source.ElementType },
					source.Expression, Expression.Quote(predicate)));
		}

		public static IQueryable Select(this IQueryable source, LambdaExpression selector)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));
			if (selector == null) throw new ArgumentNullException(nameof(selector));
			return source.Provider.CreateQuery(
				Expression.Call(
					typeof(Queryable), "Select",
					new[] { source.ElementType, selector.Body.Type },
					source.Expression, Expression.Quote(selector)));
		}
	}
}
