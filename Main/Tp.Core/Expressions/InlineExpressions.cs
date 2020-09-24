using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DelegateDecompiler;
using Tp.Core.Annotations;

namespace Tp.Core.Expressions
{
    public static class InlineExpressionsExtensions
    {
        [Pure]
        [NotNull]
        public static T Inline<T>([NotNull] this T expression, params object[] inlineEnvironment) where T : Expression
        {
            if (inlineEnvironment.Any(x => x == null))
            {
                throw new ArgumentException("Inline environment element can't be null");
            }

            return (T) new InlineVisitor(inlineEnvironment).Visit(expression);
        }
    }

    public class InlineVisitorCache
    {
        public static readonly InlineVisitorCache Instance = new InlineVisitorCache();

        private readonly ConcurrentDictionary<(Type DeclaringType, string FieldName), Try<LambdaExpression>> _fieldValues =
            new ConcurrentDictionary<(Type DeclaringType, string FieldName), Try<LambdaExpression>>();

        public int FieldCacheSize => _fieldValues.Count;

        public Try<LambdaExpression> GetOrAddField(
            Type declaringType, string fieldName,
            Func<Type, string, Try<LambdaExpression>> impl)
        {
            return _fieldValues.GetOrAdd((declaringType, fieldName), tuple => impl(tuple.DeclaringType, tuple.FieldName));
        }

        private readonly ConcurrentDictionary<(MethodInfo TargetMethod, string InlineMethodName), Try<MethodInfo>> _methods =
            new ConcurrentDictionary<(MethodInfo TargetMethod, string InlineMethodName), Try<MethodInfo>>();

        public int MethodCacheSize => _methods.Count;

        public Try<MethodInfo> GetOrAddMethod(
            MethodInfo calledMethod,
            string inlineMethodName,
            Func<MethodInfo, string, Try<MethodInfo>> impl)
        {
            return _methods.GetOrAdd((calledMethod, inlineMethodName), tuple => impl(tuple.TargetMethod, tuple.InlineMethodName));
        }

    }

    /// <summary>
    /// Replaces expressions calling methods or properties marked with <see cref="InlineableAttribute"/> with:
    /// - expression decompiled from implementation of that method or property,
    /// - expression returned by method with the same name in the declaring type, but returning <see cref="LambdaExpression"/>,
    /// - expression returned by method specified in <see cref="InlineableAttribute.MethodName"/>,
    /// - expression value of the field specified in <see cref="InlineableAttribute.FieldName"/>
    /// </summary>
    internal class InlineVisitor : ExpressionVisitor
    {
        private readonly IEnumerable<object> _inlineEnvironment;

        public InlineVisitor(IEnumerable<object> inlineEnvironment)
        {
            _inlineEnvironment = inlineEnvironment;
        }

        protected override Expression VisitMethodCall(MethodCallExpression target)
        {
            var inlineableAttribute = target.Method.GetCustomAttributeCached<InlineableAttribute>();
            if (inlineableAttribute.HasValue)
            {
                return Visit(InlineExpression(target.Object, target.Method, inlineableAttribute.Value, target.Arguments));
            }
            return base.VisitMethodCall(target);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Member.MemberType == MemberTypes.Property)
            {
                var propertyInfo = (PropertyInfo) node.Member;

                var inlineableAttribute = propertyInfo.GetCustomAttributeCached<InlineableAttribute>();

                if (inlineableAttribute.HasValue)
                {
                    var targetMethod = propertyInfo.GetGetMethod();
                    if (targetMethod.IsAbstract)
                    {
                        var realProperty = node.Expression.Type.GetProperty(node.Member.Name);
                        targetMethod = realProperty.GetGetMethod();
                    }

                    var targetArguments = new List<Expression>().AsReadOnly();

                    return Visit(InlineExpression(node.Expression, targetMethod, inlineableAttribute.Value, targetArguments));
                }
            }

            return base.VisitMember(node);
        }

        private Expression InlineExpression(
            [CanBeNull] Expression thisExpression,
            [NotNull] MethodInfo targetMethod,
            [NotNull] InlineableAttribute attribute,
            [NotNull] [ItemNotNull] IReadOnlyList<Expression> targetArguments)
        {
            var inlineExpression = !string.IsNullOrEmpty(attribute.FieldName)
                ? InlineVisitorCache.Instance.GetOrAddField(targetMethod.DeclaringType, attribute.FieldName, GetFieldValue)
                : InlineVisitorCache.Instance
                    .GetOrAddMethod(targetMethod, attribute.MethodName, FindMethodToInline)
                    .Select(methodInfo =>
                    {
                        var values = GetParameterValuesToInline(targetMethod, targetArguments, methodInfo, _inlineEnvironment);
                        return (LambdaExpression) methodInfo.Invoke(null, values);
                    })
                    .Recover(exception => Try.Create(targetMethod.Decompile));
            var argumentsForInlined = thisExpression == null ? targetArguments : targetArguments.Prepend(thisExpression).ToList();
            return inlineExpression.Value.ReplaceParameters(argumentsForInlined);
        }

        /// <remarks>
        /// <see cref="InlineableAttribute"/> may point to a field containing actual expression to inline
        /// instead of the called method, which in this case serves only as an alias.
        /// </remarks>
        /// <param name="declaringType">Type where alias method and actual field are declared</param>
        /// <param name="fieldName">Name of the field containing expression value to inline instead of the method call.</param>
        private static Try<LambdaExpression> GetFieldValue(Type declaringType, string fieldName)
        {
            var field = declaringType
                .GetFields()
                .FirstOrDefault(x => x.Name == fieldName && x.IsStatic);

            if (field is null)
            {
                return new Failure<LambdaExpression>(new InvalidOperationException(
                    $"Unable to find target inline field '{fieldName}'"));
            }

            if (!typeof(LambdaExpression).IsAssignableFrom(field.FieldType))
            {
                return new Failure<LambdaExpression>(new InvalidOperationException(
                    $"Target inline field '{fieldName}' should be assignable to {nameof(LambdaExpression)}. Actual type: {field.FieldType}"));
            }

            var expression = (LambdaExpression) field.GetValue(null);
            return new Success<LambdaExpression>(expression);
        }

        /// <remarks>
        /// <see cref="InlineableAttribute"/> may point to a method returning an expression which should be inlined
        /// instead of a call to original <see cref="method"/>.
        /// </remarks>
        /// <param name="method">Method which was actually called in expression</param>
        /// <param name="inlineMethodName">
        /// The name of the method returning an expression which should be inlined instead of the original method call.
        /// Optional, visitor searches for the method with the same name by default.
        /// </param>
        /// <returns></returns>
        private static Try<MethodInfo> FindMethodToInline(
            [NotNull] MethodInfo method,
            [CanBeNull] string inlineMethodName)
        {
            Func<MethodInfo, bool> matchByGenericArguments;
            var targetParams = method.GetParameters()
                .Select(x => new { x.Name, x.ParameterType }).ToArray();

            Func<MethodInfo, bool> matchByParameters = candidateMethod => candidateMethod
                .GetParameters()
                .Where(x => !x.GetCustomAttributeCached<InlineEnvironmentAttribute>().HasValue)
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


            var methodName = inlineMethodName ?? method.Name;

            var candidates = method.DeclaringType
                .GetMethods()
                .Where(x => x.Name == methodName && typeof(Expression).IsAssignableFrom(x.ReturnType))
                .Where(matchByParameters)
                .Where(matchByGenericArguments)
                .ToArray();

            if (candidates.Length > 1)
            {
                return
                    new Failure<MethodInfo>(
                        new InvalidOperationException(string.Format("It's more than 1 overload of inlineable method {0}.{1}.", method.DeclaringType, method?.Name)));
            }

            if (candidates.Length == 0)
            {
                return
                    new Failure<MethodInfo>(
                        new InvalidOperationException(string.Format("There is no overload of inlineable method {0}.{1}.", method.DeclaringType, method?.Name)));
            }

            return new Success<MethodInfo>(candidates.Single());
        }

        [NotNull]
        private static object[] GetParameterValuesToInline(
            [NotNull] MethodInfo targetMethod,
            [NotNull] [ItemNotNull] IReadOnlyList<Expression> targetArguments,
            [NotNull] MethodInfo methodToInline,
            [NotNull] [ItemNotNull] IEnumerable<object> inlineEnvironments)
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

        [CanBeNull]
        private static object GetValueByExpression(
            [CanBeNull] Expression expression,
            [NotNull] MethodInfo methodToInline)
        {
            if (expression is ConstantExpression maybeConst)
            {
                return maybeConst.Value;
            }

            throw new NotSupportedException(
                string.Format("Only constant arguments can be passed to inline method {0}.{1}", methodToInline.DeclaringType, methodToInline.Name));
        }

        [NotNull]
        private static object GetValueFromEnvironment(
            [NotNull] Type parameterType,
            [NotNull] [ItemNotNull] IEnumerable<object> inlineEnvironments,
            [NotNull] MethodInfo methodToInline)
        {
            var values = inlineEnvironments.Where(parameterType.IsInstanceOfType).ToArray();
            if (values.Length == 0)
            {
                throw new InvalidOperationException(string.Format("There is no overload of inlineable method {0}.{1}.", methodToInline.DeclaringType, methodToInline.Name));
            }

            if (values.Length > 1)
            {
                throw new InvalidOperationException(
                    string.Format("It's more then 1 element of type {0} in inline environment. Inline method {1}.{2} can't be called.", parameterType, methodToInline.DeclaringType, methodToInline.Name));
            }

            return values.Single();
        }

    }

    public class InlineEnvironmentAttribute : Attribute
    {
    }

    /// <remarks>
    /// See <see cref="InlineVisitor"/> for details.
    ///
    /// When put on a method requires the presence of that's method overload with the same params wrapped into Expression&lt;Func&lt;,&gt;&gt;.
    /// For example:
    /// <code>
    /// [Inlineable]
    /// public static TOut Method1(this TIn item)
    /// {
    ///     return Method1().Apply(item);
    /// }
    ///
    /// public static Expression&lt;Func&lt;TIn, TOut&gt;&gt; Method1()
    /// {
    ///     //return expression here
    /// }
    /// </code>
    /// </remarks>
    public class InlineableAttribute : Attribute
    {
        public string MethodName { get; set; }

        public string FieldName { get; set; }
    }
}
