using System;
using System.Linq.Expressions;
using Tp.Core.Annotations;

namespace Tp.Core.Linq
{
    public interface ISurrogateMethod
    {
        string Name { get; }

        /// <remarks>
        /// Expressions returned from this method MAY BE cached,
        /// so if the result value depends on environment,
        /// a call/lambda expression should be returned instead of a constant expression.
        /// </remarks>
        /// <param name="target">Which object this method is called on</param>
        MethodCallExpression GetMethodExpression([CanBeNull] Expression target);
    }

    public class TodaySurrogate : ISurrogateMethod
    {
        private static readonly MethodCallExpression _expression =
            Expression.Call(Reflect.GetMethod(() => Today()));

        private static DateTime Today() => CurrentDate.Value;

        public string Name => "Today";

        /// <remarks>
        /// Returns invocation of static method instead of a simple Expression.Constant to make this expression cache-able.
        /// </remarks>
        public MethodCallExpression GetMethodExpression(Expression target) => _expression;
    }
}
