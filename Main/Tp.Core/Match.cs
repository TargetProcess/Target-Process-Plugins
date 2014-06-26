using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Tp.Core.Expressions;

namespace Tp.Core
{
	public static class PatternMatching
	{
		public static CaseClause<T, object> Match<T>(this T o)
		{
			return new CaseClause<T, object>(o);
		}

		public static CaseClause<T, TU> Match<T, TU>(this T o, TU typeMarker)
		{
			return new CaseClause<T, TU>(o);
		}

		public static T InlineMatch<T>(this T e) where T : Expression
		{
			var visitor = new ReplaceVisitor();
			return (T) visitor.Visit(e);
		}

		public static IQueryable<T> InlineMatch<T>(this IQueryable<T> e)
		{
			var visitor = new ReplaceVisitor();
			return e.Provider.CreateQuery<T>(visitor.Visit(e.Expression));
		}


		public class CaseClause<T, TResult>
		{
			private Maybe<TResult> _result;
			private readonly T _root;

			internal CaseClause(T root)
			{
				_root = root;
				_result = Maybe.Nothing;
			}

			public CaseClause<T, TResult> Case(Func<T, bool> @case, TResult result)
			{
				if (!_result.HasValue && @case(_root))
					_result = Maybe.Just(result);
				return this;
			}

			public CaseClause<T, TResult> Case(Func<T, bool> @case, Func<T, TResult> func)
			{
				if (!_result.HasValue && @case(_root))
					_result = Maybe.Just(func(_root));
				return this;
			}

			public CaseClause<T, TResult> Case<T1>(Func<T1, TResult> func)
			{
				return Case(x => x is T1, x => func((T1)(object)x));
			}

			public TResult End(Func<T, TResult> @default)
			{
				if (@default == null)
					@default = _ => default(TResult);
				var r = End();
				return r.HasValue ? r.Value : @default(_root);
			}

			public Maybe<TResult> End()
			{
				return _result;
			}
		}


		internal sealed class ReplaceVisitor : ExpressionVisitor
		{
			private Expression _argument;
			public Expression _elseExpression;

			protected override Expression VisitMethodCall(MethodCallExpression node)
			{
				return node.Method
					.Match(default(Expression))
					.Case(IsEndMethod, _ => VisitEndMethod(node))
					.Case(IsCaseWithTypeMethod, _ => VisitForWithType(node))
					.Case(IsCaseMethod, _ => VisitForWithCondition(node))
					.End(_ => base.VisitMethodCall(node));
			}

			private bool IsCaseWithTypeMethod(MethodInfo method)
			{
				return IsCaseMethod(method) && method.IsGenericMethod;
			}

			private Expression VisitEndMethod(MethodCallExpression node)
			{
				MethodCallExpression matchCall =
					ExpressionEnumerable.All(node.Object).OfType<MethodCallExpression>().First(
						x => x.NodeType == ExpressionType.Call && IsMatchMethod(x.Method));

				_argument = matchCall.Arguments[0];

				if (node.Arguments.Count == 0)
				{
					throw new NotSupportedException("Could not inline Match expression without default End value");
				}

				var endLambda = node.Arguments[0] as LambdaExpression;

				Expression newBody = endLambda.ApplyExpression(_argument);

				_elseExpression = newBody;

				base.VisitMethodCall(node);
				return _elseExpression;
			}

			private Expression VisitForWithType(MethodCallExpression node)
			{
				var toType = node.Method.GetGenericArguments()[0];
				if (_argument.Type.IsAssignableFrom(toType) || toType.IsAssignableFrom(_argument.Type))
				{

					var conditionBody = Expression.TypeIs(_argument, toType);
					var casted = Expression.Convert(_argument, toType);

					var lambda = node.Arguments[0] as LambdaExpression;

					var funcBody = lambda.ApplyExpression(casted);

					_elseExpression = Expression.Condition(conditionBody, funcBody, _elseExpression);

				}
				return base.VisitMethodCall(node);
			}

			private Expression VisitForWithCondition(MethodCallExpression node)
			{
				var cond = node.Arguments[0] as LambdaExpression;
				var result = node.Arguments[1];
				if (result is LambdaExpression)
				{
					var endLambda = (LambdaExpression) result;
					result = endLambda.ApplyExpression(_argument);
				}

				var newCondition = cond.ApplyExpression(_argument);

				_elseExpression = Expression.Condition(newCondition, result, _elseExpression);

				return base.VisitMethodCall(node);
			}

			private bool IsCaseMethod(MethodInfo method)
			{
				return IsMethodFromCase(method) && method.Name == "Case";
			}

			private bool IsMatchMethod(MethodInfo method)
			{
				if (method.IsGenericMethod)
				{
					return method.DeclaringType == typeof (PatternMatching) && method.Name == "Match";
				}
				return false;
			}

			private bool IsEndMethod(MethodInfo method)
			{
				return IsMethodFromCase(method) && method.Name=="End";
			}

			private static bool IsMethodFromCase(MethodInfo method)
			{
				return method.DeclaringType.IsGenericType && method.DeclaringType.GetGenericTypeDefinition() == typeof(CaseClause<object, object>).GetGenericTypeDefinition();
			}
		}

		public static CaseE<TSource,TResult> MatchE<TSource, TResult>()
		{
			return new CaseE<TSource, TResult>();
		}

		public class CaseE<T, TResult>
		{
			internal static readonly MethodInfo ForMethodInfo = Reflect.GetMethod<CaseClause<T, TResult>, CaseClause<T, TResult>>(x => x.Case(y => true, y => default(TResult)));
			static readonly MethodInfo EndMethodInfo = Reflect.GetMethod<CaseClause<T, TResult>, TResult>(x => x.End(y => default(TResult)));


			private readonly CaseE<T, TResult> _previousCase;
			private readonly Expression<Func<T, bool>> _func;
			private readonly Expression<Func<T, TResult>> _func1;


			internal CaseE(CaseE<T, TResult> previousCase, Expression<Func<T, bool>> func, Expression<Func<T, TResult>> func1)
			{
				_previousCase = previousCase;
				_func = func;
				_func1 = func1;
			}

			internal CaseE()
			{
			}

			public CaseE<T, TResult> Case(Expression<Func<T, bool>> @case, Expression<Func<T, TResult>> then)
			{
				return new CaseE<T, TResult>(this, @case, then);
			}


			public CaseE<T, TResult> Case<T1>(Expression<Func<T1, TResult>> func1)
			{
				Expression<Func<T, TResult>> expression = x => func1.Apply((T1)(object)x);
				return new CaseE<T, TResult>(this, x => x is T1, expression.InlineApply());
			}

			public CaseE<T, TResult> Case(Type t1, LambdaExpression expression)
			{
				ParameterExpression p = Expression.Parameter(typeof (T));
				Expression<Func<T, bool>> lambda = Expression.Lambda<Func<T, bool>>(Expression.TypeIs(p, t1), p);


				ParameterExpression p1 = Expression.Parameter(typeof (T));
				var selector = expression.Body.Replace(expression.Parameters[0],
				                                  Expression.Convert(p1, t1));
				var selectorLambda = Expression.Lambda(Expression.Convert(selector, typeof(TResult)), p1);


//				Expression<Func<T, TResult>> expression = x => func1.Apply((T1)(object)x);
				return new CaseE<T, TResult>(this, lambda, (Expression<Func<T, TResult>>) selectorLambda);
			}

			public Expression<Func<T, TResult>> End(Expression<Func<T, TResult>> func)
			{
				var cases = AllCases().Reverse();

				Expression<Func<T, CaseClause<T,TResult>>> start = x => x.Match(default(TResult));

				Expression e = Expression.Call(
					cases.Aggregate(start.Body, (current, @case) => Expression.Call(current, ForMethodInfo, @case._func, @case._func1)),
					EndMethodInfo, func);
				var result = Expression.Lambda<Func<T, TResult>>(e, start.Parameters);
				return result;

			}

			private IEnumerable<CaseE<T, TResult>> AllCases()
			{
				for (var @case = this; @case._previousCase!=null; @case=@case._previousCase)
				{
					yield return @case;
				}
			}

		}
	}
}