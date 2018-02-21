using System.Linq.Dynamic;
using Tp.Core.Annotations;

namespace Tp.Core.Linq
{
    public static class SurrogateGeneratorHelper
    {
        /// <summary>
        /// Returns an instance of <see cref="SurrogateGenerator"/> which doesn't return any surrogates.
        /// </summary>
        public static readonly SurrogateGenerator NoSurrogates = (name, target) => Maybe.Nothing;

        /// <summary>
        /// Builds a new instance of <see cref="SurrogateGenerator"/> which uses <see cref="generator"/> by default, 
        /// and if it doesn't provide a value, uses <see cref="fallback"/>.
        /// </summary>
        [Pure]
        [NotNull]
        public static SurrogateGenerator FallbackTo(this SurrogateGenerator generator, SurrogateGenerator fallback) => 
            (name, target) => generator(name, target).OrElse(() => fallback(name, target));
    }
}
