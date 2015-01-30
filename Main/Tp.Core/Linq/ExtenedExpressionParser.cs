using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Tp.Core;
using Tp.Core.Annotations;

namespace System.Linq.Dynamic
{
	class ExtenedExpressionParser : ExpressionParser
	{
		readonly Func<string, Expression, Maybe<Expression>> _surrogateGenerator;
		private readonly IEnumerable<MethodInfo> _allowedExtensionMethods;
		
		public ExtenedExpressionParser(ParameterExpression[] parameters, string expression, object[] values,
			[NotNull] IEnumerable<MethodInfo> allowedExtensionMethods, [NotNull] Func<string,Expression,Maybe<Expression>> surrogateGenerator)
			: base(parameters, expression, values)
		{
			_allowedExtensionMethods = allowedExtensionMethods;
			_surrogateGenerator = surrogateGenerator;
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
				throw new ParseException("There are several methods named '{0}'".Fmt(methodName), errorPos);
			}
			return Maybe.Nothing;
		}

		protected override Maybe<Expression> GenerateMethodCall(Type type, Expression instance, int errorPos, string id,
			Lazy<Expression[]> argumentList)
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
	}
}