using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Tp.Core.Expressions
{
	public static class Quote
	{
		public static Expression Splice(this LambdaExpression e, params Expression[] newExpr)
		{
			return LambdaSubstituter.ReplaceParameters(e, newExpr);
		}

		public static Expression Splice(this LambdaExpression e, IEnumerable<Expression> newExpr)
		{
			return LambdaSubstituter.ReplaceParameters(e, newExpr);
		}
	}

	public class LambdaSubstituter : ExpressionVisitor
	{
		public static Expression ReplaceParameters(LambdaExpression @in, params Expression[] with)
		{
			return ReplaceParameters(@in, (IEnumerable<Expression>) with);
		}

		public static Expression ReplaceParameters(LambdaExpression @in, IEnumerable<Expression> with)
		{
			Expression e = @in.Body;

			var withList = with.ToList();

			if (withList.Count() == @in.Parameters.Count)
			{
				e = @in.Parameters.Zip(withList, (parameter, replace) => new { parameter, replace })
					.Aggregate(e, (current, expression) => Rewrite(current, expression.parameter, expression.replace));
			}
			else
			{
				foreach (var parameter in @in.Parameters)
				{
					ParameterExpression parameter1 = parameter;
					foreach (var withParameter in withList.Where(withParameter => parameter1.Type == withParameter.Type))
					{
						e = Rewrite(e, parameter, withParameter);
						break;
					}
				}
			}

			return e;
		}

		internal static Expression Rewrite(Expression @in, ParameterExpression what, Expression with)
		{
			if (what.Type != with.Type && !what.Type.IsAssignableFrom(with.Type))
				with = Expression.Convert(with, what.Type);
			var visitor = new LambdaSubstituter(what, with);
			return visitor.Visit(@in);
		}

		private readonly ParameterExpression _what;
		private readonly Expression _with;

		private LambdaSubstituter(ParameterExpression what, Expression with)
		{
			_what = what;
			_with = with;
		}

		protected override Expression VisitParameter(ParameterExpression node)
		{
			if (node == _what)
				return _with;
			return base.VisitParameter(node);
		}
	}
}
