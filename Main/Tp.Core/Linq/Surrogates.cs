using System.Linq.Expressions;
using Tp.Core;
using Tp.Core.Annotations;
using Tp.Core.Linq;

// ReSharper disable once CheckNamespace
namespace System.Linq.Dynamic
{
    public interface ISurrogateGenerator
    {
        /// <summary>
        /// If this surrogate generator depends on some environment,
        /// return a unique key for this environment.
        ///
        /// For example, custom fields may return their current actual version.
        /// </summary>
        /// <remarks>
        /// Should be as short as possible, see <see cref="ExpressionParserCache.BuildCacheKey"/> for details.
        /// </remarks>
        string GetCacheKey();

        [Pure]
        Maybe<Expression> Generate([NotNull] string name, [CanBeNull] Expression target);
    }

    public class LambdaSurrogateGenerator : ISurrogateGenerator
    {
        [NotNull]
        private readonly Func<string, Expression, Maybe<Expression>> _generate;
        private readonly Func<string> _getCacheKey;

        public LambdaSurrogateGenerator(
            [NotNull] Func<string> getCacheKey,
            [NotNull] Func<string, Expression, Maybe<Expression>> generate)
        {
            _getCacheKey = Argument.NotNull(nameof(getCacheKey), getCacheKey);
            _generate = Argument.NotNull(nameof(generate), generate);
        }

        public string GetCacheKey() => _getCacheKey();

        public Maybe<Expression> Generate(string name, Expression target) => _generate(name, target);
    }

    public static class SurrogateGeneratorHelper
    {
        public static readonly ISurrogateGenerator NoSurrogates =
            new LambdaSurrogateGenerator(() => "ns", (name, target) => Maybe<Expression>.Nothing);

        private class FallbackImpl : ISurrogateGenerator
        {
            private readonly ISurrogateGenerator _first;
            private readonly ISurrogateGenerator _second;

            public FallbackImpl(ISurrogateGenerator first, ISurrogateGenerator second)
            {
                _first = first;
                _second = second;
            }

            public string GetCacheKey() => "(" + _first.GetCacheKey() + "||" + _second.GetCacheKey() + ")";

            public Maybe<Expression> Generate(string name, Expression target)
            {
                var maybeFirst = _first.Generate(name, target);
                return maybeFirst.HasValue ? maybeFirst : _second.Generate(name, target);
            }
        }

        /// <summary>
        /// Builds a new instance of <see cref="ISurrogateGenerator"/> which uses <see cref="generator"/> by default,
        /// and if it doesn't provide a value, uses <see cref="fallback"/>.
        /// </summary>
        [Pure]
        [NotNull]
        public static ISurrogateGenerator FallbackTo(this ISurrogateGenerator generator, ISurrogateGenerator fallback) =>
            new FallbackImpl(generator, fallback);
    }
}
