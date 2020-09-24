using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Tp.Core.Annotations;
using Tp.Core.Linq;
using Tp.I18n;

namespace Tp.Core.Expressions.Parsing
{
    /// <summary>
    /// Represents special types, functions, and extension methods available in our DSL. E.g: Min, Max, Avg,
    /// </summary>
    public interface ITypeSystem
    {
        // nullable return values are used instead of maybe because this code is called on hot paths

        [NotNull]
        IReadOnlyDictionary<string, Type> KnownTypes { get; }

        [CanBeNull]
        MethodBase FindIndexer([NotNull] Type target, [NotNull] Expression argument);

        [CanBeNull]
        MemberInfo FindProperty([NotNull] Type target, [CanBeNull] Expression instance, [NotNull] string name);

        [CanBeNull]
        MethodInfo FindMethod(
            [NotNull] StaticTypeOrExpression target,
            [NotNull] string methodName,
            [NotNull] [ItemNotNull] Expression[] arguments);

        [CanBeNull]
        (MethodBase signature, Type[] typeArguments)? FindEnumerableMethod(
            [NotNull] Type elementType,
            [NotNull] string methodName,
            [NotNull] [ItemNotNull] Expression[] arguments);

        [CanBeNull]
        MethodInfo FindExtensionMethod(
            [NotNull] string methodName,
            [NotNull] [ItemNotNull] Expression[] arguments);

        [CanBeNull]
        FunctionInfo FindFunction([NotNull] string functionName);
    }

    public class FunctionParameterInfo
    {
        public FunctionParameterInfo(
            [NotNull] string name,
            [NotNull] string description)
        {
            Name = Argument.NotNullOrEmpty(nameof(name), name);
            Description = Argument.NotNull(nameof(description), description);
        }

        public string Name { get; }

        public string Description { get; }
    }

    public class FunctionInfo
    {
        [NotNull]
        private readonly Func<FunctionExpressionContext, Either<IFormattedMessage, Expression>> _buildExpression;

        public FunctionInfo(
            [NotNull] string name,
            [NotNull] string description,
            [NotNull] [ItemNotNull] IReadOnlyList<FunctionParameterInfo> parameters,
            [NotNull] Func<FunctionExpressionContext, Either<IFormattedMessage, Expression>> buildExpression)
        {
            Name = Argument.NotNullOrEmpty(nameof(name), name);
            Description = Argument.NotNull(nameof(description), description);
            Parameters = Argument.NotNull(nameof(parameters), parameters);
            _buildExpression = Argument.NotNull(nameof(buildExpression), buildExpression);
        }

        public string Name { get; }

        public string Description { get; }

        public IReadOnlyList<FunctionParameterInfo> Parameters { get; }

        public Either<IFormattedMessage, Expression> Build(
            FunctionExpressionContext context)
        {
            return _buildExpression(context);
        }
    }

    public class FunctionExpressionContext
    {
        public FunctionExpressionContext(
            [NotNull] [ItemNotNull] Expression[] arguments,
            int errorPosition,
            [NotNull] IReadOnlyDictionary<Expression, string> literals)
        {
            Arguments = Argument.NotNull(nameof(arguments), arguments);
            ErrorPosition = errorPosition;
            Literals = Argument.NotNull(nameof(literals), literals);
        }

        public Expression[] Arguments { get; }
        public int ErrorPosition { get; }
        public IReadOnlyDictionary<Expression, string> Literals { get; }
    }

    public class TypeSystem : ITypeSystem
    {
        [NotNull]
        private readonly Func<IReadOnlyDictionary<Expression, string>> _getLiterals;

        [NotNull]
        [ItemNotNull]
        private readonly IReadOnlyCollection<MethodInfo> _allowedExtensionMethods;

        public TypeSystem(
            [NotNull] Func<IReadOnlyDictionary<Expression, string>> getLiterals,
            [NotNull] IReadOnlyDictionary<string, Type> knownTypes,
            [NotNull] [ItemNotNull] IReadOnlyCollection<MethodInfo> allowedExtensionMethods)
        {
            _getLiterals = Argument.NotNull(nameof(getLiterals), getLiterals);
            KnownTypes = Argument.NotNull(nameof(knownTypes), knownTypes);
            _allowedExtensionMethods = Argument.NotNull(nameof(allowedExtensionMethods), allowedExtensionMethods);
        }

        public IReadOnlyDictionary<string, Type> KnownTypes { get; }

        public MethodBase FindIndexer(Type target, Expression argument)
        {
            var indexer = SharedParserUtils.FindIndexer(_getLiterals(), target, new[] { argument });
            if (indexer.IsSingle)
            {
                return indexer.GetSingleOrThrow();
            }

            return null;
        }

        public MemberInfo FindProperty(Type target, Expression instance, string name)
        {
            return SharedParserUtils.FindPropertyOrField(target, name, instance == null);
        }

        public MethodInfo FindMethod(StaticTypeOrExpression target, string methodName, Expression[] arguments)
        {
            var methodResult = SharedParserUtils.GetAppropriateMethod(_getLiterals(), target.Type, methodName, target.IsStatic, arguments);
            if (methodResult.IsSingle)
            {
                return (MethodInfo) methodResult.GetSingleOrThrow();
            }

            return null;
        }

        public (MethodBase signature, Type[] typeArguments)? FindEnumerableMethod(
            Type elementType, string methodName, Expression[] arguments)
        {
            if (methodName.EqualsIgnoreCase("Avg"))
            {
                methodName = nameof(Enumerable.Average);
            }

            var signatureInfo = SharedParserUtils.GetAppropriateMethod(
                _getLiterals(), typeof(IEnumerableAggregateSignatures), methodName,
                false, arguments);

            if (signatureInfo.IsSingle)
            {
                var signature = signatureInfo.GetSingleOrThrow();
                var typeArgs = signature.Name == nameof(IEnumerableAggregateSignatures.Min)
                    || signature.Name == nameof(IEnumerableAggregateSignatures.Max)
                        ? new[] { elementType, arguments[0].Type }
                        : new[] { elementType };

                return (signature, typeArgs);
            }

            return null;
        }

        public MethodInfo FindExtensionMethod(
            string methodName,
            Expression[] arguments)
        {
            var methodCandidateInfo = SharedParserUtils.FindBestMethod(_getLiterals(), _allowedExtensionMethods, methodName, arguments);
            if (methodCandidateInfo.IsSingle)
            {
                return methodCandidateInfo.GetSingleOrThrow();
            }

            return null;
        }

        private static readonly IReadOnlyDictionary<string, FunctionInfo> _functions = new FunctionInfo[]
            {
                new Iif(),
                new IfNone()
            }
            .ToDictionaryOfKnownCapacity(x => x.Name, StringComparer.OrdinalIgnoreCase);

        public FunctionInfo FindFunction(string functionName)
        {
            return _functions.TryGetValue(functionName, out var f) ? f : null;
        }
    }
}
