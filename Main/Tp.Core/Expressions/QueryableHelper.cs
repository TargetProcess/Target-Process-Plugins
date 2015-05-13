using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Tp.Core.Annotations;

namespace Tp.Core.Expressions
{
	public static class QueryableHelper
	{
		[Pure]
		public static IQueryable<T> Replace<T, T1, T2>(this IQueryable<T> queryable, Expression<Func<T1, T2>> what,
													   LambdaExpression with)
		{
			Expression e = queryable.Expression;

			var newE = ReplaceLambda(e, what, with);

			return queryable.Provider.CreateQuery<T>(newE);
		}


		[Pure]
		public static IQueryable<T> ProtectFromNullReference<T>(this IQueryable<T> queryable)
		{
			Expression e = queryable.Expression;

			var newE = e.ProtectFromNullReference();

			return queryable.Provider.CreateQuery<T>(newE);
		}

		[Pure]
		public static IQueryable<T> Replace<T>(this IQueryable<T> queryable, LambdaExpression what, LambdaExpression with)
		{
			Expression e = queryable.Expression;

			var newE = ReplaceLambda(e, what, with);

			return queryable.Provider.CreateQuery<T>(newE);
		}


		[Pure]
		public static Expression<Func<T, T1>> Replace<T, T1, T3>(this Expression<Func<T, T1>> e, Expression<Func<T, T3>> what, LambdaExpression with)
		{
			var r = new LambdaReplacer(what, with);

			var newE = r.Visit(e);
			return (Expression<Func<T, T1>>)newE;
		}

		private static Expression ReplaceLambda(Expression e, LambdaExpression what, LambdaExpression with)
		{
			var r = new LambdaReplacer(what, with);

			var newE = r.Visit(e);
			return newE;
		}


		class LambdaReplacer : ExpressionVisitor
		{
			private readonly MethodCallExpression _what;
			private readonly LambdaExpression _with;

			public LambdaReplacer(LambdaExpression what, LambdaExpression with)
			{
				_what = what.Body as MethodCallExpression;
				_with = with;
			}

			protected override Expression VisitMethodCall(MethodCallExpression node)
			{
				if (AreTheSameMethod(node.Method, _what.Method))
					if (IsTheSameObject(node.Object, _what.Object))
						return LambdaSubstituter.ReplaceParameters(_with, node.Arguments);
				return base.VisitMethodCall(node);
			}

			private bool AreTheSameMethod(MethodInfo m1, MethodInfo m2)
			{
				return m1.Name == m2.Name && m1.ReturnType == m2.ReturnType && m1.DeclaringType == m2.DeclaringType;
			}

			private static bool IsTheSameObject(Expression expression, Expression other)
			{
				bool isTheSameObject = new ExpressionComparison(Evaluator.PartialEval(expression), Evaluator.PartialEval(other)).ExpressionsAreEqual;
				return isTheSameObject;
			}
		}
	}
}