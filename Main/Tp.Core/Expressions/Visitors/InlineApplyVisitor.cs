using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Tp.Core.Expressions.Visitors
{
	class InlineApplyVisitor : ExpressionVisitor
	{
		private static readonly HashSet<MethodInfo> ApplyMethods =
			new HashSet<MethodInfo>(
				from x in typeof(ExpressionExtensions).GetMethods()
				where x.Name == "Apply" && x.IsGenericMethod
				select x.GetGenericMethodDefinition());

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			if (node.Method.IsGenericMethod && ApplyMethods.Contains(node.Method.GetGenericMethodDefinition()))
			{
				var lambdaContainer = node.Arguments[0];

				var maybe = Evaluator.SimpleEval(lambdaContainer);


				return maybe.OfType<LambdaExpression>()
					.GetOrThrow(() => new Exception("Could not Simplify expression " + node))
					.Apply(node.Arguments.Skip(1));
			}
			return base.VisitMethodCall(node);
		}

		protected override Expression VisitExtension(Expression node)
		{
			return node;
		}
	}
}
