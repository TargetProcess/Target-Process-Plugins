using System;
using System.Linq.Expressions;

namespace Tp.Core.Expressions.Visitors
{
	class ProtectFromNullReferenceVisitor : ExpressionVisitor
	{
		protected override Expression VisitMember(MemberExpression memberExpression)
		{
			var expression = Visit(memberExpression.Expression);
			var resultType = memberExpression.Member.ResultType();
			if (expression.Type.IsValueType)
			{
				return memberExpression;
			}
			else
			{

				Expression condition = Expression.ReferenceEqual(expression, Expression.Constant(expression.Type.DefaultValue()));
				var conditionalExpression = Expression.Condition(
					condition,
					Expression.Constant(resultType.DefaultValue(), resultType),
					memberExpression);
				return conditionalExpression;
			}

		}
	}
}