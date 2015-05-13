using System;
using System.Linq.Expressions;

namespace Tp.Core.Expressions.Visitors
{
	class ProtectFromNullReferenceVisitor : ExpressionVisitor
	{
		protected override Expression VisitMember(MemberExpression memberExpression)
		{
			if (memberExpression.Expression != null)
			{
				return ProtectFromNull(memberExpression, memberExpression.Expression);
			}
			return base.VisitMember(memberExpression);
		}
		protected override Expression VisitIndex(IndexExpression node)
		{
			return ProtectFromNull(node, node.Object);
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			if (node.Method.IsSpecialName && node.Method.Name == "get_Item")
			{
				return ProtectFromNull(node, node.Object);
			}
			return base.VisitMethodCall(node);
		}

		protected override Expression VisitUnary(UnaryExpression node)
		{
			if (node.NodeType == ExpressionType.Convert && node.Type.IsValueType && !node.Operand.Type.IsValueType)
			{
				return ProtectFromNull(node, node.Operand);
			}

			return base.VisitUnary(node);
		}

		private Expression ProtectFromNull(Expression memberExpression, Expression target)
		{
			var targetType = target.Type;
			if (targetType.IsValueType)
			{
				return memberExpression;
			}
			else
			{
				var condition = Expression.ReferenceEqual(Visit(target), Expression.Constant(null, target.Type));

				var @true = Expression.Constant(memberExpression.Type.DefaultValue(), memberExpression.Type);
				var @false = memberExpression;
				return Expression.Condition(condition, @true, @false);
			}
		}


	}
}
