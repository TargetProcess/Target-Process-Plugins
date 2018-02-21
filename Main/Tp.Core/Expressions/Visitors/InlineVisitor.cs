using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DelegateDecompiler;

namespace Tp.Core.Expressions
{
    internal class InlineVisitor : ExpressionVisitor
    {
        private readonly IEnumerable<object> _inlineEnvironment;

        public InlineVisitor(IEnumerable<object> inlineEnvironment)
        {
            _inlineEnvironment = inlineEnvironment;
        }

        protected override Expression VisitMethodCall(MethodCallExpression target)
        {
            var inlinableAttribute = target.Method.GetCustomAttribute<InlineableAttribute>();
            if (inlinableAttribute.HasValue)
            {
                return Visit(InlineExpression(target.Object, target.Method, inlinableAttribute, target.Arguments));
            }
            return base.VisitMethodCall(target);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Member.MemberType == MemberTypes.Property)
            {
                var propertyInfo = ((PropertyInfo) node.Member);

                var inlinableAttribute = propertyInfo.GetCustomAttribute<InlineableAttribute>();

                if (inlinableAttribute.HasValue)
                {
                    MethodInfo targetMethod = propertyInfo.GetGetMethod();
                    if (targetMethod.IsAbstract)
                    {
                        PropertyInfo realProperty = node.Expression.Type.GetProperty(node.Member.Name);
                        targetMethod = realProperty.GetGetMethod();
                    }

                    ReadOnlyCollection<Expression> targetArguments = new List<Expression>().AsReadOnly();

                    return Visit(InlineExpression(node.Expression, targetMethod, inlinableAttribute, targetArguments));
                }
            }
            return base.VisitMember(node);
        }

        private Expression InlineExpression(Expression thisExpression, MethodInfo targetMethod,
            Maybe<InlineableAttribute> inlinableAttribute,
            ReadOnlyCollection<Expression> targetArguments)
        {
            var inlineExpression = FindMethodToInline(targetMethod, inlinableAttribute.Value.InlineMethodName)
                .Select(methodInfo =>
                {
                    var values = GetParameterValuesToInline(targetMethod, targetArguments, methodInfo, _inlineEnvironment);
                    return (LambdaExpression) methodInfo.Invoke(null, values);
                })
                .Recover(exception => Try.Create(targetMethod.Decompile).Recover(_ => new Failure<LambdaExpression>(exception)));

            var argumentsForInlined = thisExpression == null ? targetArguments : new[] { thisExpression }.Concat(targetArguments);

            return inlineExpression.Value.Splice(argumentsForInlined);
        }

        private static object[] GetParameterValuesToInline(MethodInfo targetMethod, ReadOnlyCollection<Expression> targetArguments,
            MethodInfo methodToInline, IEnumerable<object> inlineEnvironments)
        {
            var targetParameters =
                targetMethod.GetParameters().Select((parameter, index) => new { value = targetArguments[index], parameter }).ToArray();
            var inlineParameters = methodToInline.GetParameters().Select((parameter, index) => new { index, parameter }).ToArray();

            var values = from inlineParam in inlineParameters
                join targetParam in targetParameters on inlineParam.parameter.Name equals targetParam.parameter.Name
                into targetParameterTmpCollection
                from targetParameter in targetParameterTmpCollection.DefaultIfEmpty()
                let value = targetParameter == null
                    ? GetValueFromEnvironment(inlineParam.parameter.ParameterType, inlineEnvironments, methodToInline)
                    : GetValueByExpression(targetParameter.value, methodToInline)
                orderby inlineParam.index
                select value;

            return values.ToArray();
        }

        private static object GetValueByExpression(Expression expression, MethodInfo methodToInline)
        {
            var maybeConst = expression as ConstantExpression;
            if (maybeConst == null)
            {
                throw new NotSupportedException(
                    "Only constant arguments can be passed to inline method {0}.{1}".Fmt(methodToInline.DeclaringType,
                        methodToInline.Name));
            }

            return maybeConst.Value;
        }

        private static object GetValueFromEnvironment(Type parameterType, IEnumerable<object> inlineEnvironments, MethodInfo methodToInline)
        {
            var values = inlineEnvironments.Where(parameterType.IsInstanceOfType).ToArray();
            if (values.Length == 0)
            {
                throw new InvalidOperationException("There is no overload of inlinable method {0}.{1}.".Fmt(methodToInline.DeclaringType,
                    methodToInline.Name));
            }

            if (values.Length > 1)
            {
                throw new InvalidOperationException(
                    "It's more then 1 elemnt of type {0} in inline environment. Inline method {1}.{2} can't be called.".Fmt(parameterType,
                        methodToInline.DeclaringType, methodToInline.Name));
            }

            return values.Single();
        }


        private static Try<MethodInfo> FindMethodToInline(MemberInfo member, string inlineMethodName)
        {
            Func<MethodInfo, bool> matchByParameters;
            Func<MethodInfo, bool> matchByGenericArguments;
            var method = member as MethodInfo;
            if (method == null)
            {
                matchByParameters = methodInfo => methodInfo.GetParameters().Length == 0;
                matchByGenericArguments = x => true;
            }
            else
            {
                var targetParams = method.GetParameters()
                    .Select(x => new { x.Name, x.ParameterType }).ToArray();

                matchByParameters = candidateMethod => candidateMethod
                    .GetParameters()
                    .Where(x => !x.GetCustomAttribute<InlineEnvironmentAttribute>().HasValue)
                    .Select(candidateParam => new { candidateParam.Name, candidateParam.ParameterType })
                    .All(targetParams.Contains);

                if (method.IsGenericMethod)
                {
                    var genericArgumentsCount = method.GetGenericArguments().Length;
                    matchByGenericArguments = x => x.GetGenericArguments().Length == genericArgumentsCount;
                }
                else
                {
                    matchByGenericArguments = x => !x.IsGenericMethod;
                }
            }


            var methodName = inlineMethodName ?? member.Name;

            var candidates = member.DeclaringType
                .GetMethods()
                .Where(x => x.Name == methodName && typeof(Expression).IsAssignableFrom(x.ReturnType))
                .Where(matchByParameters)
                .Where(matchByGenericArguments)
                .ToArray();

            if (candidates.Length > 1)
            {
                return
                    new Failure<MethodInfo>(
                        new InvalidOperationException("It's more than 1 overload of inlinable method {0}.{1}.".Fmt(member.DeclaringType,
                            method.Name)));
            }

            if (candidates.Length == 0)
            {
                return
                    new Failure<MethodInfo>(
                        new InvalidOperationException("There is no overload of inlinable method {0}.{1}.".Fmt(member.DeclaringType,
                            method.Name)));
            }

            return new Success<MethodInfo>(candidates.Single());
        }
    }
}
