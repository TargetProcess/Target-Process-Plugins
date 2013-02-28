// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using StructureMap;
using Tp.Core.Features;

namespace Tp.Core.Expressions
{
	public static class LambdaExtensions
	{
		public static Tuple<Expression<Func<T, object[], TResult>>, object[]> ExtractConstants<T, TResult>(
			this Expression<Func<T, TResult>> lambda)
		{
			return ExtractConstants<Expression<Func<T, object[], TResult>>>(lambda);
		}

		class Hasher:ExpressionVisitor
		{
			internal int _hash;
			private int _count;
			public override Expression Visit(Expression node)
			{
				if (_count > 50)
				{
					return node;
				}
				
				if (node != null)
					_hash = _hash ^ (int) node.NodeType ^ node.Type.GetHashCode();
				_count++;
				return base.Visit(node);
			}
		}


		private static int GetHashCode(LambdaExpression expression)
		{
			if (expression == null) 
				return 0;

			var hasher = new Hasher();
			hasher.Visit(expression);
			return hasher._hash;


		}


		public static readonly Dictionary<LambdaExpression, Delegate> Cache =
			new Dictionary<LambdaExpression, Delegate>(LambdaComparer<LambdaExpression>.Equality(
				(left, right) => new ExpressionComparison(left, right).ExpressionsAreEqual, expression => GetHashCode(expression)
			));

		private static readonly bool IsEnabled;

		static LambdaExtensions()
		{
			IsEnabled = ObjectFactory.GetInstance<ITpFeatureList>().IsEnabled(TpFeature.CachedCompile);
		}

		public static Func<T, TResult> CacheCompile<T, TResult>(this Expression<Func<T, TResult>> lambda)
		{
			if (IsEnabled)
			{

				Expression<Func<T, object[], TResult>> lambda2;
				object[] args;
				lambda.ExtractConstants().Decompose(out lambda2, out args);

				Func<T, object[], TResult> @delegate;
				lock (Cache)
				{
					@delegate = (Func<T, object[], TResult>)Cache.GetOrAdd(lambda2, x => lambda2.Compile());
				}


				return x => @delegate(x, args);
			}
			return lambda.Compile();
		}


		private static Tuple<T, object[]> ExtractConstants<T>(LambdaExpression lambda) where T : LambdaExpression
		{
			var constants = new List<object>();
			var constantsParameter = Expression.Parameter(typeof (object[]));
			var visitor = new ConstatnReplacer(constants, constantsParameter);
			var newParamters = new ParameterExpression[lambda.Parameters.Count + 1];
			lambda.Parameters.CopyTo(newParamters, 0);
			newParamters[lambda.Parameters.Count] = constantsParameter;
			var newLambda = Expression.Lambda(((LambdaExpression) visitor.Visit(lambda)).Body, newParamters);
			return Tuple.Create((T) newLambda, constants.ToArray());
		}

		private class ConstatnReplacer : ExpressionVisitor
		{
			private readonly List<object> _constants;
			private readonly ParameterExpression _constantsParameter;

			public ConstatnReplacer(List<object> constants, ParameterExpression constantsParameter)
			{
				_constants = constants;
				_constantsParameter = constantsParameter;
			}
			protected override Expression VisitConstant(ConstantExpression node)
			{
				_constants.Add(node.Value);
				return Expression.Convert(Expression.ArrayAccess(_constantsParameter,
				                                                 Expression.Constant(
					                                                 _constants.Count - 1)),
				                          node.Type);
			}
		}
	}
}