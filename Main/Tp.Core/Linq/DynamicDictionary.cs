using System.Collections.Generic;
using System.Linq.Expressions;
using Tp.Core;
using Tp.Core.Annotations;

// ReSharper disable CheckNamespace

namespace System.Linq.Dynamic
// ReSharper restore CheckNamespace
{
    internal static class DynamicDictionary
    {
        public static bool TryGetExpression(Type type, Expression instance, string name,
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

        public static bool TryGetAlias(Expression expr, out string name)
        {
            if (expr is MethodCallExpression methodExpression)
            {
                var declaringType = methodExpression.Method.DeclaringType;
                if (declaringType != null && declaringType.IsGenericType
                    && declaringType.GetGenericTypeDefinition() == typeof(DictionaryValueAccessor<>))
                {
                    if (methodExpression.Arguments[1] is ConstantExpression constExpression)
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
                Expression.Call(typeof(DictionaryValueAccessor<>).MakeGenericType(valueType).GetMethod(nameof(DictionaryValueAccessor<int>.GetDictionaryValue)),
                    instance,
                    Expression.Constant(name));

            return expression;
        }

        private static bool TypeIsDictionaryWithStringKey(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>) &&
                type.GetGenericArguments().First() == typeof(string);
        }

        public static Maybe<string> GetAlias(Expression expr)
        {
            if (!TryGetAlias(expr, out string name))
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
            return dictionary.TryGetValue(key, out T value) ? value : default;
        }
    }
}
