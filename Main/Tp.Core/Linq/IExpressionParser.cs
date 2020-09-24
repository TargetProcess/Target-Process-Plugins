using System.Collections.Generic;
using System.Linq.Expressions;
using Tp.Core;
using Tp.Core.Annotations;

// ReSharper disable once CheckNamespace
namespace System.Linq.Dynamic
{
    public interface IExpressionParser
    {
        /// <summary>
        /// Used as a part of caching key by <see cref="Tp.Core.Linq.ExpressionParserCache"/>.
        /// Should be as short as possible.
        /// </summary>
        string GetOptionsCacheKey();

        [NotNull]
        [Pure]
        Expression Parse(
            [NotNull] Expression it,
            [CanBeNull] Type resultType,
            Maybe<Type> baseTypeForNewClass,
            [NotNull] string expression);

        [NotNull]
        [Pure]
        IReadOnlyList<DynamicOrdering> ParseOrdering(
            [NotNull] string ordering,
            [NotNull] Expression it);
    }
}
