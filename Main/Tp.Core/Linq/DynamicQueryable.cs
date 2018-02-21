using System.Linq.Expressions;
using Tp.Core;

// ReSharper disable CheckNamespace

namespace System.Linq.Dynamic
// ReSharper restore CheckNamespace
{
    public static class DynamicQueryable
    {
        public static IQueryable<T> Where<T>(this IQueryable<T> source, string predicate, params object[] values)
        {
            return (IQueryable<T>) Where((IQueryable) source, predicate, values);
        }

        public static IQueryable<T> WhereRelaxed<T>(this IQueryable<T> source, string predicate, params object[] values)
        {
            return (IQueryable<T>) WhereRelaxed((IQueryable) source, predicate, values);
        }

        public static IQueryable<T> WhereRelaxed<T>(this IQueryable<T> source, LambdaExpression predicate)
        {
            IQueryable q = source;
            return (IQueryable<T>) WhereRelaxed(q, predicate);
        }

        public static IQueryable Where(this IQueryable source, string predicate, params object[] values)
        {
            Argument.NotNull(nameof(source), source);
            Argument.NotNull(nameof(predicate), predicate);
            return WhereRelaxed(source, predicate, values);
        }

        public static IQueryable WhereRelaxed(this IQueryable source, string predicate, params object[] values)
        {
            if (predicate.IsNullOrEmpty())
                return source;
            LambdaExpression lambda = DynamicExpressionParser.Instance.ParseLambda(source.ElementType, typeof(bool), predicate, values);
            return WhereRelaxed(source, lambda);
        }

        private static IQueryable WhereRelaxed(this IQueryable source, LambdaExpression predicate)
        {
            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), "Where",
                    new[] { source.ElementType },
                    source.Expression, Expression.Quote(predicate)));
        }

        public static IQueryable Select(this IQueryable source, string selector, params object[] values)
        {
            Argument.NotNull(nameof(source), source);
            Argument.NotNull(nameof(selector), selector);
            LambdaExpression lambda = DynamicExpressionParser.Instance.ParseLambda(source.ElementType, null, selector, values);
            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), "Select",
                    new[] { source.ElementType, lambda.Body.Type },
                    source.Expression, Expression.Quote(lambda)));
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string ordering, params object[] values)
        {
            return (IQueryable<T>) OrderBy((IQueryable) source, ordering, values);
        }

        public static IQueryable OrderBy(this IQueryable source, string ordering, params object[] values)
        {
            Argument.NotNull(nameof(source), source);
            Argument.NotNull(nameof(ordering), ordering);
            var parameters = new[]
            {
                Expression.Parameter(source.ElementType, null)
            };
            var orderings = DynamicExpressionParser.Instance.ParseOrdering(ordering, values, parameters);
            Expression queryExpr = source.Expression;
            string methodAsc = "OrderBy";
            string methodDesc = "OrderByDescending";
            foreach (DynamicOrdering o in orderings)
            {
                queryExpr = Expression.Call(
                    typeof(Queryable), o.Ascending ? methodAsc : methodDesc,
                    new[] { source.ElementType, o.Selector.Type },
                    queryExpr, Expression.Quote(Expression.Lambda(o.Selector, parameters)));
                methodAsc = "ThenBy";
                methodDesc = "ThenByDescending";
            }
            return source.Provider.CreateQuery(queryExpr);
        }

        public static IQueryable Take(this IQueryable source, int count)
        {
            Argument.NotNull(nameof(source), source);
            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), "Take",
                    new[] { source.ElementType },
                    source.Expression, Expression.Constant(count)));
        }

        public static IQueryable Skip(this IQueryable source, int count)
        {
            Argument.NotNull(nameof(source), source);
            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), "Skip",
                    new[] { source.ElementType },
                    source.Expression, Expression.Constant(count)));
        }

        public static IQueryable GroupBy(this IQueryable source, string keySelector, string elementSelector,
            params object[] values)
        {
            Argument.NotNull(nameof(source), source);
            Argument.NotNull(nameof(keySelector), keySelector);
            Argument.NotNull(nameof(elementSelector), elementSelector);
            LambdaExpression keyLambda = DynamicExpressionParser.Instance.ParseLambda(source.ElementType, null, keySelector, values);
            LambdaExpression elementLambda = DynamicExpressionParser.Instance.ParseLambda(source.ElementType, null, elementSelector, values);
            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), "GroupBy",
                    new[] { source.ElementType, keyLambda.Body.Type, elementLambda.Body.Type },
                    source.Expression, Expression.Quote(keyLambda), Expression.Quote(elementLambda)));
        }

        public static bool Any(this IQueryable source)
        {
            Argument.NotNull(nameof(source), source);
            return (bool) source.Provider.Execute(
                Expression.Call(
                    typeof(Queryable), "Any",
                    new[] { source.ElementType }, source.Expression));
        }

        public static int Count(this IQueryable source)
        {
            Argument.NotNull(nameof(source), source);
            return (int) source.Provider.Execute(
                Expression.Call(
                    typeof(Queryable), "Count",
                    new[] { source.ElementType }, source.Expression));
        }
    }
}
