using System.Linq.Expressions;
using Tp.Core.Annotations;
using Tp.Core.Linq;

namespace Tp.Core.Expressions.Parsing
{
    public interface IEnumerableMethodStrategy
    {
        /// <remarks>
        /// Should be as short as possible, see <see cref="ExpressionParserCache.BuildCacheKey"/> for details.
        /// </remarks>
        string CacheKey { get; }

        /// <summary>
        /// Describes a custom mechanism to build the root expression (also called "it") when parsing enumerable methods like `.Select()`.
        /// At the moment of writing, it is needed only for extendable domain, because all ED entities are represented as `AssignableDto`,
        /// and we need to wrap them in custom type marker expressions.
        /// </summary>
        /// <remarks>
        /// For example, when parsing `Capabilities.Select(SomeCapabilityValue)`,
        /// the parser should produce something like `Capabilities.Select(assignable => ExtendableMarker(assignable, "Capability").SomeCapabilityValue)`.
        ///
        /// Implementation should know how to turn `(Capabilities, assignable)` pair into `ExtendableMarker(assignable, "Capability")`.
        /// </remarks>
        /// <param name="enumerable">Expression representing the accessed collection.</param>
        /// <param name="lambdaParameter">
        /// Expression created by parser which will be used as parameter of lambda expression passed as argument to enumerable method call.
        /// </param>
        [Pure]
        [NotNull]
        Expression BuildRoot([NotNull] Expression enumerable, [NotNull] ParameterExpression lambdaParameter);
    }

    public class DefaultEnumerableMethodStrategy : IEnumerableMethodStrategy
    {
        public static readonly DefaultEnumerableMethodStrategy Instance = new DefaultEnumerableMethodStrategy();

        private DefaultEnumerableMethodStrategy()
        {
        }

        public string CacheKey => "d";

        public Expression BuildRoot(Expression enumerable, ParameterExpression lambdaParameter) => lambdaParameter;
    }
}
