using System;
using System.Linq.Expressions;

namespace Tp.Core.Expressions.Visitors
{
	class Replacer : ExpressionVisitor
	{
		private readonly Func<Expression, bool> _predicate;
		private readonly Func<Expression, Expression> _with;

		internal Replacer(Func<Expression, bool> predicate, Func<Expression, Expression> with)
		{
			_predicate = predicate;
			_with = with;
		}

		public override Expression Visit(Expression node)
		{
			if (_predicate(node))
				return base.Visit(_with(node));
			return base.Visit(node);
		}

		protected override Expression VisitExtension(Expression node)
		{
			return node;
		}
	}
}