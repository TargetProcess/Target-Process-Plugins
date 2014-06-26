using System.Collections.Generic;
using System.Linq.Expressions;

namespace System.Linq.Dynamic
{
	public static class DynamicExpressionParser
	{
		private const string EMPTY_EXPRESSION = "{}";

		public static Expression Parse(Type resultType, string expression, params object[] values)
		{
			var parser = new ExpressionParser(null, expression, values);
			return parser.Parse(resultType);
		}

		public static LambdaExpression ParseLambda(Type itType, Type resultType, string expression, params object[] values)
		{
			return ParseLambda(new[] {Expression.Parameter(itType, null)}, resultType, expression, values);
		}

		public static LambdaExpression ParseLambda(ParameterExpression[] parameters, Type resultType, string expression,
		                                           params object[] values)
		{
			if(expression == EMPTY_EXPRESSION)
			{
				return Expression.Lambda(Expression.New(typeof(object)), parameters);
			}

			var parser = new ExpressionParser(parameters, expression, values);
			return Expression.Lambda(parser.Parse(resultType), parameters);
		}

		public static Expression<Func<TParameter, TResult>> ParseLambda<TParameter, TResult>(string expression, params object[] values)
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
	}
}