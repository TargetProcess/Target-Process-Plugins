using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Tp.Core
{
	public static class Evaluator
	{
		/// <summary>
		///   Performs evaluation & replacement of independent sub-trees
		/// </summary>
		/// <param name="expression"> The root of the expression tree. </param>
		/// <param name="fnCanBeEvaluated"> A function that decides whether a given expression node can be part of the local function. </param>
		/// <returns> A new tree with sub-trees evaluated and replaced. </returns>
		public static Expression PartialEval(Expression expression, Func<Expression, bool> fnCanBeEvaluated)
		{
			return new SubtreeEvaluator(new Nominator(fnCanBeEvaluated).Nominate(expression)).Eval(expression);
		}

		/// <summary>
		///   Performs evaluation & replacement of independent sub-trees
		/// </summary>
		/// <param name="expression"> The root of the expression tree. </param>
		/// <returns> A new tree with sub-trees evaluated and replaced. </returns>
		public static Expression PartialEval(this Expression expression)
		{
			var maybeSimple = SimpleEval(expression);
			if (maybeSimple.HasValue)
				return Expression.Constant(maybeSimple.Value);
			return PartialEval(expression, CanBeEvaluatedLocally);
		}

		private static bool CanBeEvaluatedLocally(Expression expression)
		{
			return Enum.IsDefined(typeof(ExpressionType), expression.NodeType) && expression.NodeType != ExpressionType.Parameter;
		}

		/// <summary>
		///   Evaluates & replaces sub-trees when first candidate is reached (top-down)
		/// </summary>
		private class SubtreeEvaluator : ExpressionVisitor
		{
			private readonly HashSet<Expression> _candidates;

			internal SubtreeEvaluator(HashSet<Expression> candidates)
			{
				_candidates = candidates;
			}

			internal Expression Eval(Expression exp)
			{
				return Visit(exp);
			}

			public override Expression Visit(Expression exp)
			{
				if (exp == null)
				{
					return null;
				}
				return _candidates.Contains(exp) ? Evaluate(exp) : base.Visit(exp);
			}

			protected override Expression VisitMemberInit(MemberInitExpression node)
			{
				var visitAndConvert = VisitAndConvert(node.NewExpression, "VisitMemberInit");
				var readOnlyCollection = Visit(node.Bindings, VisitMemberBinding);
				return node.Update(visitAndConvert, readOnlyCollection);
			}

			protected override Expression VisitExtension(Expression node)
			{
				return node;
			}

			private static Expression Evaluate(Expression e)
			{
				if (e.NodeType == ExpressionType.Constant)
				{
					return e;
				}
				LambdaExpression lambda = Expression.Lambda(e);
				Delegate fn = lambda.Compile();
				return Expression.Constant(fn.DynamicInvoke(null), e.Type);
			}
		}

		/// <summary>
		///   Performs bottom-up analysis to determine which nodes can possibly be part of an evaluated sub-tree.
		/// </summary>
		private class Nominator : ExpressionVisitor
		{
			private readonly Func<Expression, bool> _fnCanBeEvaluated;
			private HashSet<Expression> _candidates;
			private bool _cannotBeEvaluated;

			internal Nominator(Func<Expression, bool> fnCanBeEvaluated)
			{
				_fnCanBeEvaluated = fnCanBeEvaluated;
			}

			internal HashSet<Expression> Nominate(Expression expression)
			{
				_candidates = new HashSet<Expression>();
				Visit(expression);
				return _candidates;
			}

			protected override Expression VisitExtension(Expression node)
			{
				return node;
			}

			protected override Expression VisitMemberInit(MemberInitExpression node)
			{
				var visited = base.VisitMemberInit(node);
				if (_candidates.Contains(node.NewExpression))
				{
					_candidates.Remove(node.NewExpression);
				}
				return visited;
			}

			public override Expression Visit(Expression expression)
			{
				if (expression != null)
				{
					bool saveCannotBeEvaluated = _cannotBeEvaluated;
					_cannotBeEvaluated = false;
					base.Visit(expression);
					if (!_cannotBeEvaluated)
					{
						if (_fnCanBeEvaluated(expression))
						{
							_candidates.Add(expression);
						}
						else
						{
							_cannotBeEvaluated = true;
						}
					}
					_cannotBeEvaluated |= saveCannotBeEvaluated;
				}
				return expression;
			}
		}

		public static Maybe<object> SimpleEval(Expression lambdaContainer)
		{
			Maybe<object> lambda = from methodCallExpression in lambdaContainer.MaybeAs<MethodCallExpression>()
				from @object in methodCallExpression.Object.MaybeAs<ConstantExpression>(nullMeansNothing: false)
				from parameters in GetParameters(methodCallExpression)
				from lambdaExpression in Maybe.Just(methodCallExpression.Method.Invoke(@object == null ? null : @object.Value, parameters.ToArray()))
				select lambdaExpression;

			Maybe<object> maybe = lambda.OrElse(() => Evaluate(lambdaContainer));
			return maybe;
		}

		public static Maybe<IEnumerable<object>> GetParameters(MethodCallExpression methodCallExpression)
		{
			var values = new List<object>();
			foreach (var argument in methodCallExpression.Arguments)
			{
				var value = SimpleEval(argument);
				if (value.HasValue)
					values.Add(value.Value);
				else
				{
					return Maybe.Nothing;
				}
			}
			return values;
		}

		public static Maybe<object> Evaluate(Expression argument)
		{
			var value = argument.MaybeAs<ConstantExpression>().Select(x => x.Value)
				.OrElse(() =>
					from memberExpression in argument.MaybeAs<MemberExpression>()
					from @object in memberExpression.Expression.MaybeAs<ConstantExpression>(nullMeansNothing: false)
					from computedValue in
						memberExpression.Member.MaybeAs<PropertyInfo>().Select(x => x.GetValue(@object.Value, null))
							.OrElse(() => memberExpression.Member.MaybeAs<FieldInfo>().Select(x => x.GetValue(@object.Value)))
					select computedValue
				);
			return value;
		}
	}
}
