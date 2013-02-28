// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Tp.Core.Expressions
{
	public static class ExpressionExtensions
	{
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
			return e.NodeType == ExpressionType.Constant && Equals(((ConstantExpression)e).Value,constant);
		}

		public static Expression<Func<T, TResult>> Convert<T,TResult>(this LambdaExpression e)
		{
			// Func<T,TR> f;
			// Func<T1,TR1> f1 = x=> (TR1)(f((T1)x))

			bool needFinalCast = e.Body.Type != typeof (TResult);

			ParameterExpression oldParameter = e.Parameters[0];

			bool needParameterCast = oldParameter.Type != typeof (T);

			LambdaExpression result = needFinalCast ? Expression.Lambda(Expression.Convert(e.Body, typeof (TResult)), e.Parameters) : e;

			if (needParameterCast)
			{
				var newParameter = Expression.Parameter(typeof (T), oldParameter.Name);
				var newBody = result.Body.Replace(x => x == oldParameter, Expression.Convert(newParameter, oldParameter.Type));
				result = Expression.Lambda(newBody, newParameter);
			}

			return (Expression<Func<T, TResult>>) result;
		}

		public static Expression<Func<TNewParam, TResult>> ReplaceLambdaParameter<TNewParam, TResult>(
			this LambdaExpression e, LambdaExpression replacement)
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
			return (T)new Replacer(e=>e==what, e => with).Visit(@in);
		}

		public static T Replace<T>(this T @in, Func<Expression, bool> predicate, Expression with) where T : Expression
		{
			return (T)new Replacer(predicate, e => with).Visit(@in);
		}
		public static T Replace<T>(this T @in, Func<Expression, bool> predicate, Func<Expression, Expression> with) where T : Expression
		{
			return (T)new Replacer(predicate, with).Visit(@in);
		}

		public static Expression ApplyExpression(this LambdaExpression lambda, params Expression[] arguments)
		{
			return lambda.Apply(arguments.AsEnumerable());
			
		}

		private static Expression Apply(this LambdaExpression lambda, IEnumerable<Expression> arguments)
		{
			arguments = arguments.ToList();

			if (lambda.Parameters.Count() != arguments.Count())
			{
				throw new ArgumentException("Expected {0} parameters, given {1} arguments".Fmt(lambda.Parameters.Count(),
				                                                                               arguments.Count()));

			}
			
			return lambda.Parameters.Zip(arguments, (parameter, argument) => new {parameter,argument}).Aggregate(lambda.Body, (body,pair)=>body.Replace(x=>x == pair.parameter, pair.argument) );
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

		public static T InlineApply<T>(this T expression) where T:Expression
		{
			var invoker = new InlineApplyVisitor();
			Expression visit = invoker.Visit(expression);
			return (T) visit;
		}

		public static T Inline<T>(this T expression, params object[] inlineEnvironment) where T : Expression
		{
			if (inlineEnvironment.Any(x => x == null))
			{
				throw new ArgumentException("Inline environment element can't be null");
			}

			return (T)new InlineVisitor(inlineEnvironment).Visit(expression);
		}

		private class InlineVisitor : ExpressionVisitor
		{
			private readonly IEnumerable<object> _inlineEnvironment;

			public InlineVisitor(IEnumerable<object> inlineEnvironment)
			{
				_inlineEnvironment = inlineEnvironment;
			}

			protected override Expression VisitMethodCall(MethodCallExpression target)
			{
				var inlinableAttribute = target.Method.GetCustomAttribute<InlineableAttribute>();
				if (inlinableAttribute.HasValue)
				{
					var method = FindMethodToInline(target.Method, inlinableAttribute.Value.InlineMethodName);
					var values = GetParameterValuesToInline(target, method, _inlineEnvironment);
					var inlineExpression = (LambdaExpression)method.Invoke(null, values);
					return inlineExpression.Splice(target.Arguments);
				}

				return base.VisitMethodCall(target);
			}

			private static object[] GetParameterValuesToInline(MethodCallExpression target, MethodInfo methodToInline, IEnumerable<object> inlineEnvironments)
			{
				var targetParameters = target.Method.GetParameters().Select((parameter, index) => new { value = target.Arguments[index], parameter }).ToArray();
				var inlineParameters = methodToInline.GetParameters().Select((parameter, index) => new {index, parameter}).ToArray();

				var values = from inlineParam in inlineParameters
						 join targetParam in targetParameters on inlineParam.parameter.Name equals targetParam.parameter.Name
						 into targetParameterTmpCollection
						 from targetParameter in targetParameterTmpCollection.DefaultIfEmpty()
						 let value = targetParameter == null
							? GetValueFromEnvironment(inlineParam.parameter.ParameterType, inlineEnvironments, methodToInline)
							: GetValueByExpression(targetParameter.value, methodToInline)
					     orderby inlineParam.index
						 select value;

				return values.ToArray();
			}

			private static object GetValueByExpression(Expression expression, MethodInfo methodToInline)
			{
				var maybeConst = expression as ConstantExpression;
				if (maybeConst == null)
				{
					throw new NotSupportedException("Only constant arguments can be passed to inline method {0}.{1}".Fmt(methodToInline.DeclaringType, methodToInline.Name));
				}

				return maybeConst.Value;
			}

			private static object GetValueFromEnvironment(Type parameterType, IEnumerable<object> inlineEnvironments, MethodInfo methodToInline)
			{
				var values = inlineEnvironments.Where(parameterType.IsInstanceOfType).ToArray();
				if (values.Length == 0)
				{
					throw new InvalidOperationException("There is no overload of inlinable method {0}.{1}.".Fmt(methodToInline.DeclaringType, methodToInline.Name));
				}

				if (values.Length > 1)
				{
					throw new InvalidOperationException("It's more then 1 elemnt of type {0} in inline environment. Inline method {1}.{2} can't be called.".Fmt(parameterType, methodToInline.DeclaringType, methodToInline.Name));
				}

				return values.Single();
			}

			private static MethodInfo FindMethodToInline(MethodInfo method, string inlineMethodName)
			{
				var targetParams = method.GetParameters()
					.Select(x => new {x.Name, x.ParameterType}).ToArray();

				Func<MethodInfo, bool> matchByParameters = candidateMethod => candidateMethod
					.GetParameters()
					.Where(x => !x.GetCustomAttribute<InlineEnvironmentAttribute>().HasValue)
					.Select(candidateParam => new { candidateParam.Name, candidateParam.ParameterType })
					.All(targetParams.Contains);

				var methodName = inlineMethodName ?? method.Name;

				var candidates = method.DeclaringType
					.GetMethods()
					.Where(x => x.Name == methodName && typeof(Expression).IsAssignableFrom(x.ReturnType))
					.Where(matchByParameters)
					.ToArray();

				if (candidates.Length > 1)
				{
					throw new InvalidOperationException("It's more than 1 overload of inlinable method {0}.{1}.".Fmt(method.DeclaringType, method.Name));
				}

				if (candidates.Length == 0)
				{
					throw new InvalidOperationException("There is no overload of inlinable method {0}.{1}.".Fmt(method.DeclaringType, method.Name));
				}

				return candidates.Single();
			}
		}

		public static Expression EvalBooleanConstants(this Expression expression)
		{
			return new BooleanEvaluator().Visit(expression);
		}

		private class BooleanEvaluator : ExpressionVisitor
		{
			protected override Expression VisitConditional(ConditionalExpression node)
			{
				var newExpr = base.VisitConditional(node);
				if (newExpr.NodeType == ExpressionType.Conditional)
				{
					var cond = (ConditionalExpression)newExpr;
					if (cond.Test.NodeType == ExpressionType.Constant)
					{
						var testResult = (bool)((ConstantExpression) cond.Test).Value;
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
						return VisitBoolBinary(true, (BinaryExpression) visited);
					case ExpressionType.AndAlso:
						return VisitBoolBinary(false, (BinaryExpression) visited);
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
						var boolValue = (bool)((ConstantExpression) unary.Operand).Value;
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
					var boolValue = (bool) booleanConstant.Value;
					return boolValue == zero
						       ? Expression.Constant(zero)
						       : (binary.Left.NodeType == ExpressionType.Constant ? binary.Right : binary.Left);
				}
				return binary;
			}
		}

		private class InlineApplyVisitor : ExpressionVisitor
		{
			private static readonly HashSet<MethodInfo> ApplyMethods =
				new HashSet<MethodInfo>(
					from x in typeof (ExpressionExtensions).GetMethods()
					where x.Name == "Apply" && x.IsGenericMethod
					select x.GetGenericMethodDefinition());

			protected override Expression VisitMethodCall(MethodCallExpression node)
			{
				if (node.Method.IsGenericMethod && ApplyMethods.Contains(node.Method.GetGenericMethodDefinition()))
				{
					var lambdaContainer = node.Arguments[0];

					var maybe = Evaluator.SimpleEval(lambdaContainer);


					return maybe.Bind(x=>x.MaybeAs<LambdaExpression>())
						.FailIfNothing(()=>new Exception("Could not Simplify expression "+node))
						.Apply(node.Arguments.Skip(1));

				}
				return base.VisitMethodCall(node);
			}
		}
		
		private class Replacer : ExpressionVisitor
		{
			private readonly Func<Expression, bool> _predicate;
			private readonly Func<Expression, Expression> _with;

			internal Replacer(Func<Expression, bool> predicate, Func<Expression, Expression> with)
			{
				_predicate = predicate;
				_with = with;
			}

			public override Expression Visit(Expression node)
			{
				if (_predicate(node))
					return base.Visit(_with(node));
				return base.Visit(node);
			}

			protected override Expression VisitExtension(Expression node)
			{
				return node;
			}
		}

		public static T ProtectFromNullReference<T>(this T expression) where T : Expression
		{
			return (T) (new ProtectFromNullReferenceVisitor().Visit(expression));
		}

		private class ProtectFromNullReferenceVisitor : ExpressionVisitor
		{
			protected override Expression VisitMember(MemberExpression memberExpression)
			{
				var expression = Visit(memberExpression.Expression);
				var resultType = memberExpression.Member.ResultType();



				var conditionalExpression = Expression.Condition(
					Expression.ReferenceEqual(expression, Expression.Constant(null, expression.Type)),
					Expression.Constant(resultType.DefaultValue(), resultType),
					memberExpression);
				return conditionalExpression;
			}
		}

		public static IEnumerable<Expression> ToEnumerable(this Expression expression)
		{
			return new ExpressionCollection(expression);
		}
	}


	public class InlineableAttribute : Attribute
	{
		private readonly string _inlineMethodName;

		public InlineableAttribute(string inlineMethodName = null)
		{
			_inlineMethodName = inlineMethodName;
		}

		public string InlineMethodName
		{
			get { return _inlineMethodName; }
		}
	}

	public class InlineEnvironmentAttribute : Attribute
	{
	}
}