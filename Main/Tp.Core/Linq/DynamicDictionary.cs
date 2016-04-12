using System.Collections.Generic;
using System.Linq.Expressions;
using Tp.Core;
using Tp.Core.Annotations;

// ReSharper disable CheckNamespace

namespace System.Linq.Dynamic
// ReSharper restore CheckNamespace
{
	internal class DynamicDictionary
	{
		public static bool TryGetExpression(Type type, Expression instance, int errorPos, string name,
			out Expression expression)
		{
			var dictInterface = TypeIsDictionaryWithStringKey(type)
				? type
				: type.GetInterfaces().FirstOrDefault(
					x =>
						x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDictionary<,>) &&
							x.GetGenericArguments().First() == typeof(string));

			if (dictInterface != null)
			{
				expression = GenerateDictionaryMemberAccess(instance, name, dictInterface.GetGenericArguments()[1]);
				return true;
			}

			expression = null;
			return false;
		}

		public static bool TryGetAlias(Expression expr, int exprPos, out string name)
		{
			var methodExpression = expr as MethodCallExpression;

			if (methodExpression != null)
			{
				var declaringType = methodExpression.Method.DeclaringType;
				if (declaringType != null && declaringType.IsGenericType
					&& declaringType.GetGenericTypeDefinition() == typeof(DictionaryValueAccessor<>))
				{
					var constExpression = methodExpression.Arguments[1] as ConstantExpression;
					if (constExpression != null)
					{
						name = constExpression.Value as string;
						return true;
					}
				}
			}

			name = null;
			return false;
		}

		private static Expression GenerateDictionaryMemberAccess(Expression instance, string name, Type valueType)
		{
			var expression =
				Expression.Call(typeof(DictionaryValueAccessor<>).MakeGenericType(valueType).GetMethod("GetDictionaryValue"),
					instance,
					Expression.Constant(name));

			return expression;
		}

		private static bool TypeIsDictionaryWithStringKey(Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>) &&
				type.GetGenericArguments().First() == typeof(string);
		}

		public static Maybe<string> GetAlias(Expression expr, int exprPos)
		{
			string name;
			if (!TryGetAlias(expr, exprPos, out name))
			{
				return Maybe.Nothing;
			}
			return Maybe.Just(name);
		}
	}

	internal class DictionaryValueAccessor<T>
	{
		[UsedImplicitly]
		public static T GetDictionaryValue(IDictionary<string, T> dictionary, string key)
		{
			T value;
			if (dictionary.TryGetValue(key, out value))
				return value;
			return default(T);
		}
	}
}
