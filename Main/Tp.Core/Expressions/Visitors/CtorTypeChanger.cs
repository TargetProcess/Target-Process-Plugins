using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Tp.Core.Expressions.Visitors
{
	class CtorTypeChanger : ExpressionVisitor
	{
		private readonly Type _baseType;
		private readonly Type _derivedType;

		public CtorTypeChanger(Type baseType, Type derivedType)
		{
			_baseType = baseType;
			_derivedType = derivedType;
		}

		protected override Expression VisitNew(NewExpression node)
		{
			if (node.Type == _baseType)
			{
				return ReplaceConstructor(node, _derivedType);
			}
			if (node.Type.IsConstructedGenericType && _baseType.IsGenericType && node.Type.GetGenericTypeDefinition() == _baseType)
			{
				return ReplaceConstructor(node, _derivedType.MakeGenericType(node.Type.GenericTypeArguments));
			}
			return base.VisitNew(node);
		}

		private Expression CreateNewConstructor(NewExpression node, ConstructorInfo constructor)
		{
			if (node.Members != null)
			{
				return Expression.New(constructor, node.Arguments, node.Members);
			}
			else
			{
				return Expression.New(constructor, node.Arguments);
			}
		}

		private Expression ReplaceConstructor(NewExpression node, Type type)
		{
			var constructor = FindApropriateCtor(node.Constructor, type);
			return CreateNewConstructor(node, constructor);
		}

		private ConstructorInfo FindApropriateCtor(ConstructorInfo constructor, Type type)
		{
			return type.GetConstructor(constructor.GetParameters().Select(x => x.ParameterType).ToArray());
		}
	}

	class CtorTypeChanger<TBase, TDerived> : CtorTypeChanger
	{
		public CtorTypeChanger() : base(typeof(TBase), typeof(TDerived))
		{
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