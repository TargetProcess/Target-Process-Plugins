using System;
using System.Linq.Expressions;
using Tp.Core.Annotations;

namespace Tp.Core.Expressions.Visitors
{
    [PerformanceCritical]
    public class Replacer : ExpressionVisitor
    {
        private readonly Func<Expression, Maybe<Expression>> _replacement;

        internal Replacer(
            [NotNull] Func<Expression, Maybe<Expression>> replacement)
        {
            _replacement = Argument.NotNull(nameof(replacement), replacement);
        }

        [PerformanceCritical]
        public override Expression Visit(Expression node)
        {
            var maybeReplacement = _replacement(node);
            return maybeReplacement.HasValue ? maybeReplacement.Value : base.Visit(node);
        }

        protected override Expression VisitExtension(Expression node)
        {
            return _replacement(node).GetOrDefault(node);
        }
    }
}
