using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Tp.Core.Expressions.Visitors
{
	class CtorTypeChanger<TBase, TDerived> : ExpressionVisitor where TDerived : TBase
	{
		protected override Expression VisitNew(NewExpression node)
		{
			if (node.Type == typeof(TBase))
			{
				var constructor = FindApropriateCtor(node.Constructor);
				if (node.Members != null)
				{
					return Expression.New(constructor, node.Arguments, node.Members);
				}
				else
				{
					return Expression.New(constructor, node.Arguments);
				}
			}
			else
			{
				return base.VisitNew(node);
			}
		}

		private ConstructorInfo FindApropriateCtor(ConstructorInfo constructor)
		{
			return typeof(TDerived).GetConstructor(constructor.GetParameters().Select(x => x.ParameterType).ToArray());
		}
	}

	class BooleanEvaluator : ExpressionVisitor
	{
		protected override Expression VisitConditional(ConditionalExpression node)
		{
			var newExpr = base.VisitConditional(node);
			if (newExpr.NodeType == ExpressionType.Conditional)
			{
				var cond = (ConditionalExpression)newExpr;
				if (cond.Test.NodeType == ExpressionType.Constant)
				{
					var testResult = (bool)((ConstantExpression)cond.Test).Value;
					return testResult ? cond.IfTrue : cond.IfFalse;
				}
			}
			return newExpr;
		}
		protected override Expression VisitExtension(Expression node)
		{
			return node;
		}
		protected override Expression VisitBinary(BinaryExpression node)
		{
			var visited = base.VisitBinary(node);
			switch (visited.NodeType)
			{
				case ExpressionType.OrElse:
					return VisitBoolBinary(true, (BinaryExpression)visited);
				case ExpressionType.AndAlso:
					return VisitBoolBinary(false, (BinaryExpression)visited);
				default:
					return visited;
			}
		}

		protected override Expression VisitUnary(UnaryExpression node)
		{
			var visited = base.VisitUnary(node);
			if (visited.NodeType == ExpressionType.Not)
			{
				var unary = (UnaryExpression)(visited);
				if (unary.Operand.NodeType == ExpressionType.Constant)
				{
					var boolValue = (bool)((ConstantExpression)unary.Operand).Value;
					return boolValue ? Expression.Constant(false) : Expression.Constant(true);
				}
			}
			return visited;
		}

		private static Expression VisitBoolBinary(bool zero, BinaryExpression binary)
		{
			var booleanConstant = binary.Left as ConstantExpression ?? binary.Right as ConstantExpression;
			if (booleanConstant != null)
			{
				var boolValue = (bool)booleanConstant.Value;
				return boolValue == zero
						   ? Expression.Constant(zero)
						   : (binary.Left.NodeType == ExpressionType.Constant ? binary.Right : binary.Left);
			}
			return binary;
		}
	}
}