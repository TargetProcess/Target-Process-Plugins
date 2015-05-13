// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using Tp.Core;
using Tp.Core.Expressions;
using Tp.Core.Expressions.Visitors;

// ReSharper disable once CheckNamespace
namespace System.Linq.Expressions
{
	public static class ExpressionExtensions
	{
		public static Maybe<T> TryEval<T>(this Expression expression)
		{
			expression = expression.PartialEval();
			var constantExpression = expression as ConstantExpression;
			return (constantExpression != null && constantExpression.Value is T) ? Maybe.Return((T)constantExpression.Value) : Maybe.Nothing;
		}

		public static Expression<Func<TArg0, TResult>> Expr<TArg0, TResult>(this Expression<Func<TArg0, TResult>> expr)
		{
			return expr;
		}

		public static Expression<Func<TArg0, TArg1, TResult>> Expr<TArg0, TArg1, TResult>(this Expression<Func<TArg0, TArg1, TResult>> expr)
		{
			return expr;
		}

		public static bool IsConstantNull(this Expression e)
		{
			return IsConstant(e, null);
		}

		public static bool IsConstant(this Expression e, object constant)
		{
			return e.NodeType == ExpressionType.Constant && Equals(((ConstantExpression)e).Value, constant);
		}

		/// <summary>
		/// Convert a Lambda expression to typed one, adding Convert calls if neccessary
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="e"></param>
		/// <returns></returns>
		public static Expression<Func<T, TResult>> Convert<T, TResult>(this LambdaExpression e)
		{
			return TryConvert<T, TResult>(e).Value;
		}

		public static Maybe<Expression<Func<T, TResult>>> TryConvert<T, TResult>(this LambdaExpression e)
		{
			var maybeLambdaWithParamterConverted = TryConvertLambdaParameter<T>(e);
			return maybeLambdaWithParamterConverted.Bind<LambdaExpression, Expression<Func<T, TResult>>>(lambdaWithParamterConverted =>
			{
				var result = lambdaWithParamterConverted;

				var needResultCast = e.Body.Type != typeof(TResult);
				if (needResultCast && !(typeof(TResult).IsAssignableFrom(e.Body.Type) || e.Body.Type.IsAssignableFrom(typeof(TResult))))
				{
					return Maybe.Nothing;
				}

				if (needResultCast)
				{
					var newBody = Expression.Convert(result.Body, typeof(TResult));
					result = Expression.Lambda(newBody, result.Parameters);
				}

				// e.Body.Type == TResult but e.ReturnType is some supertype of TResult
				if (result.ReturnType != typeof(TResult))
				{
					return Expression.Lambda<Func<T, TResult>>(result.Body, result.Parameters);
				}

				return (Expression<Func<T, TResult>>)result;
			});
		}

		public static Maybe<LambdaExpression> TryConvertLambdaParameter<T>(this LambdaExpression e)
		{
			var oldParameter = e.Parameters[0];

			var needParameterCast = oldParameter.Type != typeof(T);
			if (needParameterCast && !(typeof(T).IsAssignableFrom(oldParameter.Type) || oldParameter.Type.IsAssignableFrom(typeof(T))))
			{
				return Maybe.Nothing;
			}

			if (needParameterCast)
			{
				var newParameter = Expression.Parameter(typeof(T), oldParameter.Name);
				var newBody = e.Body.Replace(oldParameter, Expression.Convert(newParameter, oldParameter.Type));
				return Expression.Lambda(newBody, newParameter);
			}

			return e;
		}

		public static Expression<Func<TNewParam, TResult>> ReplaceLambdaParameter<TNewParam, TResult>(this LambdaExpression e, LambdaExpression replacement)
		{
			ParameterExpression oldParameter = e.Parameters[0];
			var newParameter = replacement.Parameters[0];

			var newBody = e.Body.Replace(x => x == oldParameter, replacement.Body);
			var result = Expression.Lambda(newBody, newParameter);

			return (Expression<Func<TNewParam, TResult>>)result;
		}

		public static Expression<Func<T, TResult>> MakeSingleParameter<T, TResult>(this Expression<Func<T, T, TResult>> selector)
		{
			var what = selector.Parameters[1];
			var with = selector.Parameters[0];
			var newBody = selector.Body.Replace(x => x == what, with);
			return Expression.Lambda<Func<T, TResult>>(newBody, with);
		}

		public static T Replace<T>(this T @in, Expression what, Expression with) where T : Expression
		{
			return Replace(@in, x => x == what, with);
		}
		
		public static T Replace<T>(this T @in, Func<Expression, bool> predicate, Expression with) where T : Expression
		{
			return Replace(@in, e => predicate(e) ? Maybe.Just(with) : Maybe.Nothing);
		}
		public static T Replace<T>(this T @in, Func<Expression, Maybe<Expression>> replacement) where T : Expression
		{
			return (T)new Replacer(replacement).Visit(@in);
		}

		public static Expression ApplyExpression(this LambdaExpression lambda, params Expression[] arguments)
		{
			return lambda.Apply(arguments.AsEnumerable());
		}

		internal static Expression Apply(this LambdaExpression lambda, IEnumerable<Expression> arguments)
		{
			arguments = arguments.ToList();

			if (lambda.Parameters.Count() != arguments.Count())
			{
				throw new ArgumentException("Expected {0} parameters, given {1} arguments".Fmt(lambda.Parameters.Count(),
																							   arguments.Count()));

			}

			return lambda.Parameters.Zip(arguments, (parameter, argument) => new { parameter, argument }).Aggregate(lambda.Body, (body, pair) => body.Replace(x => x == pair.parameter, pair.argument));
		}

		public static TResult Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, T16 t16)
		{
			return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16);
		}
		public static TResult Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15)
		{
			return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15);
		}
		public static TResult Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14)
		{
			return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14);
		}
		public static TResult Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13)
		{
			return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13);
		}
		public static TResult Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12)
		{
			return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12);
		}
		public static TResult Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11)
		{
			return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11);
		}
		public static TResult Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10)
		{
			return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10);
		}
		public static TResult Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9)
		{
			return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8, t9);
		}
		public static TResult Apply<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8)
		{
			return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8);
		}
		public static TResult Apply<T1, T2, T3, T4, T5, T6, T7, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
		{
			return expression.Compile()(t1, t2, t3, t4, t5, t6, t7);
		}
		public static TResult Apply<T1, T2, T3, T4, T5, T6, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
		{
			return expression.Compile()(t1, t2, t3, t4, t5, t6);
		}
		public static TResult Apply<T1, T2, T3, T4, T5, TResult>(this Expression<Func<T1, T2, T3, T4, T5, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
		{
			return expression.Compile()(t1, t2, t3, t4, t5);
		}
		public static TResult Apply<T1, T2, T3, T4, TResult>(this Expression<Func<T1, T2, T3, T4, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4)
		{
			return expression.Compile()(t1, t2, t3, t4);
		}
		public static TResult Apply<T1, T2, T3, TResult>(this Expression<Func<T1, T2, T3, TResult>> expression, T1 t1, T2 t2, T3 t3)
		{
			return expression.Compile()(t1, t2, t3);
		}
		public static TResult Apply<T1, T2, TResult>(this Expression<Func<T1, T2, TResult>> expression, T1 t1, T2 t2)
		{
			return expression.Compile()(t1, t2);
		}
		public static TResult Apply<T1, TResult>(this Expression<Func<T1, TResult>> expression, T1 t1)
		{
			return expression.Compile()(t1);
		}
		public static TResult Apply<TResult>(this Expression<Func<TResult>> expression)
		{
			return expression.Compile()();
		}

		public static Expression<Func<TResult>> PartialApply<T1, TResult>(this Expression<Func<T1, TResult>> expression, T1 paramValue)
		{
			var lambda = PartialApplyLambda(expression, paramValue);
			return (Expression<Func<TResult>>)lambda;
		}
		public static Expression<Func<T1, TResult>> PartialApply<T1, T2, TResult>(this Expression<Func<T1, T2, TResult>> expression, T2 paramValue)
		{
			var lambda = PartialApplyLambda(expression, paramValue);
			return (Expression<Func<T1, TResult>>)lambda;
		}
		public static Expression<Func<T1, T2, TResult>> PartialApply<T1, T2, T3, TResult>(this Expression<Func<T1, T2, T3, TResult>> expression, T3 paramValue)
		{
			var lambda = PartialApplyLambda(expression, paramValue);
			return (Expression<Func<T1, T2, TResult>>)lambda;
		}
		public static Expression<Func<T1, T2, T3, TResult>> PartialApply<T1, T2, T3, T4, TResult>(this Expression<Func<T1, T2, T3, T4, TResult>> expression, T4 paramValue)
		{
			var lambda = PartialApplyLambda(expression, paramValue);
			return (Expression<Func<T1, T2, T3, TResult>>)lambda;
		}
		public static Expression<Func<T1, T2, T3, T4, TResult>> PartialApply<T1, T2, T3, T4, T5, TResult>(this Expression<Func<T1, T2, T3, T4, T5, TResult>> expression, T5 paramValue)
		{
			var lambda = PartialApplyLambda(expression, paramValue);
			return (Expression<Func<T1, T2, T3, T4, TResult>>)lambda;
		}
		public static Expression<Func<T1, T2, T3, T4, T5, TResult>> PartialApply<T1, T2, T3, T4, T5, T6, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, TResult>> expression, T6 paramValue)
		{
			var lambda = PartialApplyLambda(expression, paramValue);
			return (Expression<Func<T1, T2, T3, T4, T5, TResult>>)lambda;
		}
		public static Expression<Func<T1, T2, T3, T4, T5, T6, TResult>> PartialApply<T1, T2, T3, T4, T5, T6, T7, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, TResult>> expression, T7 paramValue)
		{
			var lambda = PartialApplyLambda(expression, paramValue);
			return (Expression<Func<T1, T2, T3, T4, T5, T6, TResult>>)lambda;
		}
		public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, TResult>> PartialApply<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>> expression, T8 paramValue)
		{
			var lambda = PartialApplyLambda(expression, paramValue);
			return (Expression<Func<T1, T2, T3, T4, T5, T6, T7, TResult>>)lambda;
		}
		public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>> PartialApply<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression, T9 paramValue)
		{
			var lambda = PartialApplyLambda(expression, paramValue);
			return (Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>>)lambda;
		}
		public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> PartialApply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression, T10 paramValue)
		{
			var lambda = PartialApplyLambda(expression, paramValue);
			return (Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>>)lambda;
		}
		public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> PartialApply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expression, T11 paramValue)
		{
			var lambda = PartialApplyLambda(expression, paramValue);
			return (Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>>)lambda;
		}
		public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> PartialApply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> expression, T12 paramValue)
		{
			var lambda = PartialApplyLambda(expression, paramValue);
			return (Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>>)lambda;
		}
		public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> PartialApply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>> expression, T13 paramValue)
		{
			var lambda = PartialApplyLambda(expression, paramValue);
			return (Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>>)lambda;
		}
		public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>> PartialApply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>> expression, T14 paramValue)
		{
			var lambda = PartialApplyLambda(expression, paramValue);
			return (Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>>)lambda;
		}
		public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>> PartialApply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>> expression, T15 paramValue)
		{
			var lambda = PartialApplyLambda(expression, paramValue);
			return (Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>>)lambda;
		}

		private static LambdaExpression PartialApplyLambda<TParam2>(LambdaExpression expression, TParam2 paramValue)
		{
			var lastParametr = expression.Parameters.Last();
			var constant = Expression.Constant(paramValue, lastParametr.Type);
			var result = expression.Body.Replace(lastParametr, constant);
			var lambda = Expression.Lambda(result, expression.Parameters.Take(expression.Parameters.Count - 1));
			return lambda;
		}

		public static T InlineApply<T>(this T expression) where T : Expression
		{
			var invoker = new InlineApplyVisitor();
			Expression visit = invoker.Visit(expression);
			return (T)visit;
		}
		public static T Inline<T>(this T expression, params object[] inlineEnvironment) where T : Expression
		{
			if (inlineEnvironment.Any(x => x == null))
			{
				throw new ArgumentException("Inline environment element can't be null");
			}

			return (T)new InlineVisitor(inlineEnvironment).Visit(expression);
		}

		public static Expression EvalBooleanConstants(this Expression expression)
		{
			return new BooleanEvaluator().Visit(expression);
		}

		public static T ProtectFromNullReference<T>(this T expression) where T : Expression
		{
			return (T)(new ProtectFromNullReferenceVisitor().Visit(expression));
		}
		public static T ProtectFromDivideByZero<T>(this T expression) where T : Expression
		{
			return (T)(new ProtectFromDivideByZeroVisitor().Visit(expression));
		}
		public static T FixFloatEqualityComparison<T>(this T expression, float delta) where T : Expression
		{
			return (T)(new FixFloatComparisonVisitor(delta).Visit(expression));
		}


		public static Expression<Func<T, bool>> CombineAnd<T>(this Expression<Func<T, bool>> head, params Expression<Func<T, bool>>[] tail)
		{
			return Combine(head, tail, Expression.AndAlso);
		}

		public static Expression<Func<T, bool>> CombineOr<T>(this Expression<Func<T, bool>> head, params Expression<Func<T, bool>>[] tail)
		{
			return Combine(head, tail, Expression.OrElse);
		}

		public static Expression CombineOr(this IEnumerable<Expression> expressions)
		{
			var result = expressions.Aggregate(Expression.OrElse);
			return result;
		}

		public static Expression CombineAnd(this IEnumerable<Expression> expressions)
		{
			var result = expressions.Aggregate(Expression.AndAlso);
			return result;
		}

		private static Expression<Func<T, bool>> Combine<T>(Expression<Func<T, bool>> head, IEnumerable<Expression<Func<T, bool>>> tail, Func<Expression, Expression, BinaryExpression> combiner)
		{
			var result = tail.Aggregate(head, (soFar, element) =>
			{
				var lambda = LambdaSubstituter.ReplaceParameters(element, soFar.Parameters);
				return (Expression<Func<T, bool>>)Expression.Lambda(combiner(soFar.Body, lambda), soFar.Parameters);
			});

			return result;
		}

		public static Expression ChangeCtorType<TBase, TDerived>(this Expression lambda) where TDerived : TBase
		{
			return new CtorTypeChanger<TBase, TDerived>().Visit(lambda);
		}

		public static Expression ChangeCtorType(this Expression lambda, Type baseType, Type derivedType)
		{
			return new CtorTypeChanger(baseType, derivedType).Visit(lambda);
		}


		public static IEnumerable<Expression> TraversePreOrder(this Expression expression)
		{
			var visitor = new PreOrderTraverseVisitor();
			return visitor.Traverse(expression);
		}
	}
}