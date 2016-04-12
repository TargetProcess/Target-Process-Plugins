using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using StructureMap;
using Tp.Core;
using Tp.Core.Linq;

// ReSharper disable once CheckNamespace

namespace System.Linq.Dynamic
{
	public class DynamicExpressionParser
	{
		private readonly Func<string, Expression, Maybe<Expression>> _surrogates;

		private static readonly Dictionary<string, ISurrogateMethod> _registeredSurrogates = ObjectFactory.GetAllInstances<ISurrogateMethod>()
			.ToDictionary(x => x.Name, StringComparer.InvariantCultureIgnoreCase);

		public static readonly DynamicExpressionParser Instance =
			new DynamicExpressionParser
			{
				FixIntegerDivision = true
			};

		public DynamicExpressionParser(
			IEnumerable<MethodInfo> extensionMethods = null,
			Func<string, Expression, Maybe<Expression>> surrogates = null,
			IReadOnlyDictionary<string, Type> knownTypes = null)
		{
			Func<string, Expression, Maybe<Expression>> defaultSurrogate =
				(s, e) => _registeredSurrogates.GetValue(s).Select(x => x.GetMethodExpression(e));

			_surrogates = surrogates == null ? defaultSurrogate : ((s, e) => surrogates(s, e).OrElse(() => defaultSurrogate(s, e)));

			_methodProviders = extensionMethods
				?? ObjectFactory.GetAllInstances<IMethodProvider>().SelectMany(x => x.GetExtensionMethodInfo()).ToArray();
			_knownTypes = knownTypes
				?? ObjectFactory.GetAllInstances<ITypeProvider>()
					.SelectMany(x => x.GetKnownTypes())
					.ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);
		}

		public bool FixIntegerDivision { get; set; }

		private readonly IEnumerable<MethodInfo> _methodProviders;
		private readonly IReadOnlyDictionary<string, Type> _knownTypes;

		private const string EMPTY_EXPRESSION = "{}";

		public Expression Parse(Type resultType, string expression, params object[] values)
		{
			var parser = CreateExpressionParser(null, Maybe<Type>.Nothing, expression, values);
			return parser.Parse(resultType);
		}

		public LambdaExpression ParseLambda(Type itType, Type resultType, Maybe<Type> baseTypeForNewClass, string expression,
			params object[] values)
		{
			return ParseLambda(new[] { Expression.Parameter(itType, null) }, resultType, baseTypeForNewClass, expression, values);
		}

		public LambdaExpression ParseLambda(Type itType, Type resultType, string expression, params object[] values)
		{
			return ParseLambda(itType, resultType, Maybe<Type>.Nothing, expression, values);
		}

		public LambdaExpression ParseLambda(ParameterExpression[] parameters, Type resultType, string expression, params object[] values)
		{
			if (expression == EMPTY_EXPRESSION)
			{
				return Expression.Lambda(Expression.Constant(new object()), parameters);
			}

			var parser = CreateExpressionParser(parameters, Maybe<Type>.Nothing, expression, values);
			return Expression.Lambda(parser.Parse(resultType), parameters);
		}

		private LambdaExpression ParseLambda(ParameterExpression[] parameters, Type resultType, Maybe<Type> baseTypeForNewClass, string expression,
			params object[] values)
		{
			if (expression == EMPTY_EXPRESSION)
			{
				return Expression.Lambda(Expression.Constant(new object()), parameters);
			}

			var parser = CreateExpressionParser(parameters, baseTypeForNewClass, expression, values);
			return Expression.Lambda(parser.Parse(resultType), parameters);
		}

		public Expression<Func<TParameter, TResult>> ParseLambda<TParameter, TResult>(string expression, params object[] values)
		{
			return (Expression<Func<TParameter, TResult>>) ParseLambda(typeof(TParameter), typeof(TResult), expression, values);
		}

		public static Type CreateClass(params DynamicProperty[] properties)
		{
			return ClassFactory.Instance.GetDynamicClass(properties);
		}

		public static Type CreateClass(IEnumerable<DynamicProperty> properties, Type baseType = null)
		{
			return ClassFactory.Instance.GetDynamicClass(properties, baseType);
		}

		public IEnumerable<DynamicOrdering> ParseOrdering(string ordering, object[] values, ParameterExpression[] parameters)
		{
			var parser = CreateExpressionParser(parameters, Maybe<Type>.Nothing, ordering, values);
			IEnumerable<DynamicOrdering> orderings = parser.ParseOrdering();
			return orderings;
		}

		private ExtendedExpressionParser CreateExpressionParser(ParameterExpression[] parameters, Maybe<Type> baseTypeForNewClass,
			string expression, object[] values)
		{
			return new ExtendedExpressionParser(parameters, expression, values, _methodProviders, _surrogates, _knownTypes, baseTypeForNewClass)
			{
				FixIntegerDivision = FixIntegerDivision
			};
		}
	}
}
