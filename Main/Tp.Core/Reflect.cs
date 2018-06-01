using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Tp.Core;

public partial class Reflect
{
    public static PropertyInfo GetIndexer<T, TIndexer>()
    {
        return (from propertyInfo in typeof(T).GetProperties()
            let parameterInfos = propertyInfo.GetIndexParameters()
            where parameterInfos.Length > 0 && parameterInfos[0].ParameterType == typeof(TIndexer)
            select propertyInfo).FirstOrDefault();
    }

    public static TResult InvokeGeneric<TResult>(Expression<Func<TResult>> genericCall, params Type[] types)
    {
        var calls = genericCall.TraversePreOrder().OfType<MethodCallExpression>().ToArray();
        if (calls.Length != 1)
        {
            throw new ArgumentException($"Expect 1 method call in expression, was {calls.Length}", nameof(genericCall));
        }        
        var call = calls[0];

        var targetObject = call.Object?.PartialEval().MaybeAs<ConstantExpression>()
            .Select(c => c.Value)
            .GetOrThrow(() => new ArgumentException("Method call target should be constant value", nameof(genericCall)));

        var arguments = call.Arguments
            .Select(a => a.PartialEval().MaybeAs<ConstantExpression>().GetOrThrow(() => new ArgumentException("Method call arguments should be constant values", nameof(genericCall))))
            .Select(c => c.Value)
            .ToArray();

        var targetMethod = call.Method.GetGenericMethodDefinition().MakeGenericMethod(types);
        return (TResult)targetMethod.Invoke(targetObject, arguments);
    }
}

// ReSharper restore CheckNamespace
