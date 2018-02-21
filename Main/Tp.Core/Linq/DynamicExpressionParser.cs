using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using StructureMap;
using Tp.Core;
using Tp.Core.Annotations;
using Tp.Core.Linq;

// ReSharper disable once CheckNamespace

namespace System.Linq.Dynamic
{
    public class DynamicExpressionParser
    {
        private const string EMPTY_EXPRESSION = "{}";

        private static readonly Lazy<DynamicExpressionParser> _parserLazy = Lazy.Create(() => new DynamicExpressionParser(
            DefaultKnownTypes, DefaultExtensionMethods, DefaultSurrogateGenerator, fixIntegerDivision: true));

        public static DynamicExpressionParser Instance => _parserLazy.Value;

        private static readonly Lazy<IReadOnlyDictionary<string, Type>> _defaultKnownTypes =
            Lazy.Create(() => GetKnownTypes(ObjectFactory.Container));

        public static IReadOnlyDictionary<string, Type> DefaultKnownTypes => _defaultKnownTypes.Value;

        [Pure]
        [NotNull]
        public static IReadOnlyDictionary<string, Type> GetKnownTypes(IContainer container) =>
            ObjectFactory.Container
                .GetAllInstances<ITypeProvider>().SelectMany(x => x.GetKnownTypes())
                .ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);

        private static readonly Lazy<IReadOnlyCollection<MethodInfo>> _defaultExtensionMethods =
            Lazy.Create(() => GetExtensionMethods(ObjectFactory.Container));

        public static IReadOnlyCollection<MethodInfo> DefaultExtensionMethods => _defaultExtensionMethods.Value;

        [Pure]
        [NotNull]
        [ItemNotNull]
        public static IReadOnlyCollection<MethodInfo> GetExtensionMethods(IContainer container) =>
            ObjectFactory.Container
                .GetAllInstances<IMethodProvider>()
                .SelectMany(x => x.GetExtensionMethodInfo())
                .ToList();

        private static readonly Lazy<SurrogateGenerator> _defaultSurrogateGenerator =
            Lazy.Create(() => CreateSurrogateGenerator(ObjectFactory.Container));

        public static SurrogateGenerator DefaultSurrogateGenerator => _defaultSurrogateGenerator.Value;

        [Pure]
        [NotNull]
        public static SurrogateGenerator CreateSurrogateGenerator(IContainer container)
        {
            var registeredSurrogates = container
                .GetAllInstances<ISurrogateMethod>()
                .ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);

            return (name, expression) => registeredSurrogates
                .GetValue(name)
                .Select(x => x.GetMethodExpression(expression));
        }

        public DynamicExpressionParser(
            [NotNull] IReadOnlyDictionary<string, Type> knownTypes,
            [NotNull] [ItemNotNull] IReadOnlyCollection<MethodInfo> extensionMethods,
            [CanBeNull] SurrogateGenerator surrogates,
            bool fixIntegerDivision = false)
        {
            _knownTypes = Argument.NotNull(nameof(knownTypes), knownTypes);
            _methodProviders = Argument.NotNull(nameof(extensionMethods), extensionMethods);
            _surrogates = Argument.NotNull(nameof(surrogates), surrogates);
            _fixIntegerDivision = fixIntegerDivision;
        }

        private readonly SurrogateGenerator _surrogates;

        private readonly IReadOnlyCollection<MethodInfo> _methodProviders;

        private readonly IReadOnlyDictionary<string, Type> _knownTypes;

        private readonly bool _fixIntegerDivision;

        [NotNull]
        public Expression Parse(
            [NotNull] Type resultType,
            [NotNull] string expression,
            [CanBeNull] params object[] values)
        {
            var parser = CreateExpressionParser(null, Maybe<Type>.Nothing, expression, values);
            return parser.Parse(resultType);
        }

        [NotNull]
        public LambdaExpression ParseLambda(
            [CanBeNull] ParameterExpression[] parameters,
            [CanBeNull] Type resultType,
            Maybe<Type> baseTypeForNewClass,
            [NotNull] string expression,
            [CanBeNull] params object[] values)
        {
            if (expression == EMPTY_EXPRESSION)
            {
                return Expression.Lambda(Expression.Constant(new object()), parameters);
            }

            var parser = CreateExpressionParser(parameters, baseTypeForNewClass, expression, values);
            return Expression.Lambda(parser.Parse(resultType), parameters);
        }

        [NotNull]
        public LambdaExpression ParseLambda(
            [NotNull] Type itType,
            [CanBeNull] Type resultType,
            [NotNull] string expression,
            [CanBeNull] params object[] values)
        {
            return ParseLambda(new[] { Expression.Parameter(itType, null) }, resultType, Maybe<Type>.Nothing, expression, values);
        }

        [NotNull]
        public Expression<Func<TParameter, TResult>> ParseLambda<TParameter, TResult>(string expression, params object[] values)
        {
            return (Expression<Func<TParameter, TResult>>) ParseLambda(typeof(TParameter), typeof(TResult), expression, values);
        }

        [NotNull]
        public static Type CreateClass(IEnumerable<DynamicProperty> properties, Type baseType = null) =>
            ClassFactory.Instance.GetDynamicClass(properties, baseType);

        [Pure]
        [NotNull]
        [ItemNotNull]
        public IEnumerable<DynamicOrdering> ParseOrdering(
            [NotNull] string ordering,
            [CanBeNull] IReadOnlyList<object> values,
            [CanBeNull] IReadOnlyList<ParameterExpression> parameters)
        {
            var parser = CreateExpressionParser(parameters, Maybe<Type>.Nothing, ordering, values);
            IEnumerable<DynamicOrdering> orderings = parser.ParseOrdering();
            return orderings;
        }

        [NotNull]
        private ExtendedExpressionParser CreateExpressionParser(
            [CanBeNull] IReadOnlyList<ParameterExpression> parameters,
            Maybe<Type> baseTypeForNewClass,
            [NotNull] string expression,
            [CanBeNull] IReadOnlyList<object> values)
        {
            return new ExtendedExpressionParser(parameters, expression, values, _methodProviders, _surrogates, _knownTypes,
                baseTypeForNewClass)
            {
                FixIntegerDivision = _fixIntegerDivision
            };
        }
    }
}
