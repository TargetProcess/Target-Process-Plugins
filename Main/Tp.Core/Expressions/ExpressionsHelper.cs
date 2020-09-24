using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using Tp.Core.Annotations;

namespace Tp.Core.Expressions
{
    public static class ExpressionsHelper
    {
        public static MemberInitExpression GenerateMemberInit(Dictionary<string, Expression> propertyInitializers)
        {
            var @class = ClassFactory.Instance.GetDynamicClass(propertyInitializers.Select(x => new DynamicProperty(x.Key, x.Value.Type)));
            return GenerateMemberInit(@class, propertyInitializers, true);
        }

        public static MemberInitExpression GenerateMemberInit(Type resultType, Dictionary<string, Expression> propertyInitializers,
            bool ignoreDefaults)
        {
            var bindings =
                resultType.GetProperties(BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty)
                    .Select(property => property.GetBaseDefinition())
                    .Where(p => p.CanWrite)
                    .Select(property => new
                    {
                        property,
                        expression = propertyInitializers.GetValue(property.Name)
                            .GetOrElse(
                                () =>
                                    ignoreDefaults
                                        ? null
                                        : Expression.Constant(property.ResultType().DefaultValue(), property.ResultType()))
                    })
                    .Where(arg => arg.expression != null)
                    .Select(arg => Expression.Bind(arg.property, arg.expression));

            return Expression.MemberInit(Expression.New(resultType), bindings);
        }


        [Pure]
        [NotNull]
        public static Expression<Func<TArg0, TResult>> Expr<TArg0, TResult>([NotNull] Expression<Func<TArg0, TResult>> expr)
        {
            return expr;
        }

        [Pure]
        [NotNull]
        public static Expression<Func<TArg0, TArg1, TResult>> Expr<TArg0, TArg1, TResult>(
            [NotNull] Expression<Func<TArg0, TArg1, TResult>> expr)
        {
            return expr;
        }
    }
}
