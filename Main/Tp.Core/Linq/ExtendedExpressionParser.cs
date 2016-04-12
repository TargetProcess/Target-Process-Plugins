using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using StructureMap.TypeRules;
using Tp.Core;
using Tp.Core.Annotations;

// ReSharper disable once CheckNamespace

namespace System.Linq.Dynamic
{
	internal class ExtendedExpressionParser : ExpressionParser
	{
		private readonly Func<string, Expression, Maybe<Expression>> _surrogateGenerator;
		private readonly IReadOnlyDictionary<string, Type> _knownTypes;
		private readonly Maybe<Type> _baseTypeForNewClass;
		private readonly IEnumerable<MethodInfo> _allowedExtensionMethods;

		public ExtendedExpressionParser(IReadOnlyList<ParameterExpression> parameters,
			string expression,
			object[] values,
			[NotNull] IEnumerable<MethodInfo> allowedExtensionMethods,
			[NotNull] Func<string, Expression, Maybe<Expression>> surrogateGenerator,
			[NotNull] IReadOnlyDictionary<string, Type> knownTypes,
			Maybe<Type> baseTypeForNewClass)
			: base(parameters, expression, values)
		{
			_allowedExtensionMethods = allowedExtensionMethods;
			_surrogateGenerator = surrogateGenerator;
			_knownTypes = knownTypes;
			_baseTypeForNewClass = baseTypeForNewClass;
		}

		private Maybe<Expression> ExtensionMethod(Expression instance, string methodName, Lazy<Expression[]> args, int errorPos, bool prefixed)
		{
			MethodBase method;
			Expression[] newArguments = prefixed ? args.Value : new[] { instance }.Concat(args.Value).ToArray();
			int count = FindBestMethod(_allowedExtensionMethods, methodName, newArguments, out method);
			if (count == 1)
			{
				return Expression.Call((MethodInfo) method, newArguments);
			}
			if (count > 1)
			{
				throw new ParseException("There are several methods named {methodName}.".Localize(new { methodName }), errorPos);
			}
			return Maybe.Nothing;
		}

		protected override Maybe<Expression> GenerateMethodCall(Type type, Expression instance, int errorPos, string id,
			Lazy<Expression[]> argumentList)
		{
			return GenerateMethodCallImpl(type, instance, errorPos, id, argumentList)
				.OrElse(() => PropagateToNullMethod(type, instance, errorPos, id, argumentList));
		}

		/// <summary>
		/// Convert <c>Foo(a,b,c)</c>=><c>(a!=null && b!=null)?Foo(a.Value,b.Value):null;</c>
		/// </summary>
		private Maybe<Expression> PropagateToNullMethod(Type type, Expression instance, int errorPos, string id, Lazy<Expression[]> argumentList)
		{

			var newArgs = argumentList.Value.Select(a => a.Type.IsNullable() ? Expression.Property(a, "Value") : a).ToArray();
			var methodCall = GenerateMethodCallImpl(type, instance, errorPos, id, Lazy.Create(newArgs));
			return methodCall.Select(e =>
			{

				var test = argumentList.Value.Where(x => x.Type.IsNullable())
					.Select(x => Expression.NotEqual(x, Expression.Constant(null, x.Type)))
					.CombineAnd();

				var needToNullate = e.Type.IsValueType;

				var resultType = needToNullate ? typeof(Nullable<>).MakeGenericType(e.Type) : e.Type;

				var ifTrue = needToNullate ? Expression.Convert(e, resultType) : e;

				var ifFalse = Expression.Constant(null, resultType);


				return (Expression)Expression.Condition(test, ifTrue, ifFalse);
			});

		}

	private Maybe<Expression> GenerateMethodCallImpl(Type type, Expression instance, int errorPos, string id, Lazy<Expression[]> argumentList)
		{
			return base.GenerateMethodCall(type, instance, errorPos, id, argumentList)
				.OrElse(() => ExtensionMethod(instance, id, argumentList, errorPos, prefixed: false))
				.OrElse(() => ExtensionMethod(instance, id, argumentList, errorPos, prefixed: true))
				.OrElse(() => SurrogateExpression(instance, id));
		}

		private Maybe<Expression> SurrogateExpression(Expression instance, string name)
		{
			return _surrogateGenerator(name, instance);
		}

		protected override Try<Expression> TryParseMemberAccess(Type type, Expression instance, TokenId nextToken, int errorPos, string name)
		{
			if (name.EqualsIgnoreCase("as"))
			{
				return Try.Create(() => ParseAs(instance));
			}
			return base.TryParseMemberAccess(type, instance, nextToken, errorPos, name);
		}

		private Expression ParseAs(Expression instance)
		{
			ValidateToken(TokenId.LessThan, Res.AngleBracketsExpected);
			NextToken();
			var castTypeName = GetIdentifier();
			NextToken();
			ValidateToken(TokenId.GreaterThan, Res.AngleBracketsExpected);
			NextToken();

			var castType = _knownTypes.GetValue(castTypeName).GetOrThrow(() => ParseError(Res.UnknownType(castTypeName)));
			return Expression.TypeAs(instance, castType);
		}

		protected override Type GenerateDynamicClassType(IReadOnlyList<DynamicProperty> properties)
		{
			return DynamicExpressionParser.CreateClass(properties, _baseTypeForNewClass.GetOrDefault());
		}
	}
}
