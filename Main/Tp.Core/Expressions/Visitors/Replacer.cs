using System;
using System.Linq.Expressions;

namespace Tp.Core.Expressions.Visitors
{
	class Replacer : ExpressionVisitor
	{
		private readonly Func<Expression, Maybe<Expression>> _replacement;

		internal Replacer(Func<Expression, Maybe<Expression>> replacement)
		{
			_replacement = replacement;
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