using System;
using System.Linq.Expressions;
using Tp.Core.Annotations;

namespace Tp.Core.Expressions.Visitors
{
    internal class Replacer : ExpressionVisitor
    {
        private readonly Func<Expression, Maybe<Expression>> _replacement;

        internal Replacer(
            [NotNull] Func<Expression, Maybe<Expression>> replacement)
        {
            _replacement = Argument.NotNull(nameof(replacement), replacement);
        }

        public override Expression Visit(Expression node)
        {
            return _replacement(node).GetOrElse(() => base.Visit(node));
        }

        protected override Expression VisitExtension(Expression node)
        {
            return _replacement(node).GetOrDefault(node);
        }
    }
}
