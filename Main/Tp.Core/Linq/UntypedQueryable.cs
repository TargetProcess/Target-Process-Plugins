using System.Linq.Expressions;
using Tp.Core;

// ReSharper disable once CheckNamespace
namespace System.Linq.Dynamic
{
    public static class UntypedQueryable
    {
        public static IQueryable Where(this IQueryable source, LambdaExpression predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return WhereRelaxed(source, predicate);
        }
        
        public static IQueryable WhereRelaxed(this IQueryable source, LambdaExpression predicate)
        {
            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), nameof(Queryable.Where),
                    new[] { source.ElementType },
                    source.Expression, Expression.Quote(predicate)));
        }

        public static IQueryable Select(this IQueryable source, LambdaExpression selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), nameof(Queryable.Select),
                    new[] { source.ElementType, selector.Body.Type },
                    source.Expression, Expression.Quote(selector)));
        }

        public static IQueryable Take(this IQueryable source, int count)
        {
            Argument.NotNull(nameof(source), source);
            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), nameof(Queryable.Take),
                    new[] { source.ElementType },
                    source.Expression, Expression.Constant(count)));
        }

        public static IQueryable Skip(this IQueryable source, int count)
        {
            Argument.NotNull(nameof(source), source);
            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), nameof(Queryable.Skip),
                    new[] { source.ElementType },
                    source.Expression, Expression.Constant(count)));
        }

        public static bool Any(this IQueryable source)
        {
            Argument.NotNull(nameof(source), source);
            return (bool) source.Provider.Execute(
                Expression.Call(
                    typeof(Queryable), nameof(Queryable.Any),
                    new[] { source.ElementType }, source.Expression));
        }

        public static int Count(this IQueryable source)
        {
            Argument.NotNull(nameof(source), source);
            return (int) source.Provider.Execute(
                Expression.Call(
                    typeof(Queryable), nameof(Queryable.Count),
                    new[] { source.ElementType }, source.Expression));
        }
    }
}
