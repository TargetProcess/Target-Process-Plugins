using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using StructureMap;
using Tp.Core;

// ReSharper disable once CheckNamespace
namespace System.Linq.Dynamic
{
	public class DynamicExpressionParser
	{
		private readonly Func<string, Expression, Maybe<Expression>> _surrogates;
		public static DynamicExpressionParser Instance = new DynamicExpressionParser();

		public DynamicExpressionParser(IEnumerable<MethodInfo> extensionMethods = null, Func<string, Expression, Maybe<Expression>> surrogates = null)
		{
			if (surrogates == null)
			{
				_surrogates = (s, expression) => Maybe.Nothing;
			}
			else
			{
				_surrogates = surrogates;
			}
			_methodProviders = extensionMethods ??  ObjectFactory.GetAllInstances<IMethodProvider>().SelectMany(x=>x.GetExtensionMethodInfo());
		}

		private readonly IEnumerable<MethodInfo> _methodProviders;

		private const string EMPTY_EXPRESSION = "{}";

		public Expression Parse(Type resultType, string expression, params object[] values)
		{
			var parser = new ExtenedExpressionParser(null, expression, values, _methodProviders, _surrogates);
			return parser.Parse(resultType);
		}

		public LambdaExpression ParseLambda(Type itType, Type resultType, string expression, params object[] values)
		{
			return ParseLambda(new[] {Expression.Parameter(itType, null)}, resultType, expression, values);
		}

		public LambdaExpression ParseLambda(ParameterExpression[] parameters, Type resultType, string expression, params object[] values)
		{
			if(expression == EMPTY_EXPRESSION)
			{
				return Expression.Lambda(Expression.Constant(new object()), parameters);
			}

			var parser = new ExtenedExpressionParser(parameters, expression, values, _methodProviders, _surrogates);
			return Expression.Lambda(parser.Parse(resultType), parameters);
		}

		public Expression<Func<TParameter, TResult>> ParseLambda<TParameter, TResult>(string expression, params object[] values)
		{
			return (Expression<Func<TParameter, TResult>>) ParseLambda(typeof (TParameter), typeof (TResult), expression, values);
		}

		public static Type CreateClass(params DynamicProperty[] properties)
		{
			return ClassFactory.Instance.GetDynamicClass(properties);
		}

		public static Type CreateClass(IEnumerable<DynamicProperty> properties, Type baseType=null)
		{
			return ClassFactory.Instance.GetDynamicClass(properties, baseType);
		}

		internal IEnumerable<DynamicOrdering> ParseOrdering(string ordering, object[] values, ParameterExpression[] parameters)
		{
			var parser = new ExtenedExpressionParser(parameters, ordering, values, _methodProviders, _surrogates);
			IEnumerable<DynamicOrdering> orderings = parser.ParseOrdering();
			return orderings;
		}
	}
}