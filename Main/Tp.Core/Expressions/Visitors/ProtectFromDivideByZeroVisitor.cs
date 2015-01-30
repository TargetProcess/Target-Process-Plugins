using System.Linq.Expressions;

namespace Tp.Core.Expressions.Visitors
{
	class ProtectFromDivideByZeroVisitor : ExpressionVisitor
	{
		protected override Expression VisitBinary(BinaryExpression node)
		{
			if (node.NodeType == ExpressionType.Divide && node.IsLiftedToNull)
			{
				var right = Visit(node.Right);

				var equality = Expression.Equal(right, Expression.Convert(Expression.Constant(0), right.Type));
				var @true = Expression.Constant(null, node.Type);
				return Expression.Condition(equality, @true, base.VisitBinary(node));
			}
			return base.VisitBinary(node);
		}
	}
}