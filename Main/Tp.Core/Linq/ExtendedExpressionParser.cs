using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using StructureMap.TypeRules;
using Tp.Core;
using Tp.Core.Annotations;

// ReSharper disable once CheckNamespace

namespace System.Linq.Dynamic
{
    public delegate Maybe<Expression> SurrogateGenerator([NotNull] string name, [CanBeNull] Expression target);

    internal class ExtendedExpressionParser : ExpressionParser
    {
        private readonly SurrogateGenerator _surrogateGenerator;
        private readonly IReadOnlyDictionary<string, Type> _knownTypes;
        private readonly Maybe<Type> _baseTypeForNewClass;
        private readonly IReadOnlyCollection<MethodInfo> _allowedExtensionMethods;

        public ExtendedExpressionParser(
            [CanBeNull] IReadOnlyList<ParameterExpression> parameters,
            [NotNull] string expression,
            [CanBeNull] IReadOnlyList<object> values,
            [NotNull] [ItemNotNull] IReadOnlyCollection<MethodInfo> allowedExtensionMethods,
            [NotNull] SurrogateGenerator surrogateGenerator,
            [NotNull] IReadOnlyDictionary<string, Type> knownTypes,
            Maybe<Type> baseTypeForNewClass)
            : base(parameters, expression, values)
        {
            _allowedExtensionMethods = Argument.NotNull(nameof(allowedExtensionMethods), allowedExtensionMethods);
            _surrogateGenerator = Argument.NotNull(nameof(surrogateGenerator), surrogateGenerator);
            _knownTypes = Argument.NotNull(nameof(knownTypes), knownTypes);
            _baseTypeForNewClass = baseTypeForNewClass;
        }

        [Pure]
        private Maybe<Expression> ExtensionMethod(
            [CanBeNull] Expression instance,
            [NotNull] string methodName,
            [NotNull] Lazy<Expression[]> args, int errorPos, bool prefixed)
        {
            var newArguments = prefixed ? args.Value : new[] { instance }.Concat(args.Value).ToArray();

            var methodCandidateInfo = FindBestMethod(_allowedExtensionMethods, methodName, newArguments);

            if (methodCandidateInfo.IsSingle)
            {
                return Expression.Call(methodCandidateInfo.GetSingleOrThrow(), newArguments);
            }

            if (methodCandidateInfo.HasSeveral)
            {
                throw new ParseException("There are several methods named {methodName}.".Localize(new { methodName }), errorPos);
            }

            return Maybe.Nothing;
        }

        protected override Maybe<Expression> GenerateMethodCall(
            Type type, Expression instance, int errorPos, string id,
            Lazy<Expression[]> argumentList)
        {
            return GenerateMethodCallImpl(type, instance, errorPos, id, argumentList)
                .OrElse(() => TreatNullableBoolAsBool(type, instance, errorPos, id, argumentList))
                .OrElse(() => PropagateToNullMethod(type, instance, errorPos, id, argumentList));
        }

        private Maybe<Expression> TreatNullableBoolAsBool(Type type, Expression instance, int errorPos, string id,
            Lazy<Expression[]> argumentList)
        {
            var newArgs = argumentList.Value
                .Select(ConvertNullableBoolToBoolExpression)
                .ToArray();
            return GenerateMethodCallImpl(type, instance, errorPos, id, Lazy.Create(newArgs));
        }

        /// <summary>
        /// Convert <c>Foo(a,b,c)</c>=><c>(a!=null && b!=null)?Foo(a.Value,b.Value):null;</c>
        /// </summary>
        [Pure]
        private Maybe<Expression> PropagateToNullMethod(
            [NotNull] Type type,
            [CanBeNull] Expression instance,
            int errorPos,
            [NotNull] string id,
            [NotNull] Lazy<Expression[]> argumentList)
        {
            var newArgs = argumentList.Value.Select(a => a.Type.IsNullable() ? Expression.Property(a, "Value") : a).ToArray();
            var methodCall = GenerateMethodCallImpl(type, instance, errorPos, id, Lazy.Create(newArgs));
            return methodCall.Select<Expression, Expression>(e =>
            {
                var test = argumentList.Value.Where(x => x.Type.IsNullable())
                    .Select(x => Expression.NotEqual(x, Expression.Constant(null, x.Type)))
                    .ToArray()
                    .CombineAnd();
                var needToNullate = e.Type.IsValueType;
                var resultType = needToNullate ? typeof(Nullable<>).MakeGenericType(e.Type) : e.Type;
                var ifTrue = needToNullate ? Expression.Convert(e, resultType) : e;
                var ifFalse = Expression.Constant(null, resultType);

                return Expression.Condition(test, ifTrue, ifFalse);
            });
        }

        [Pure]
        private Maybe<Expression> GenerateMethodCallImpl(
            [NotNull] Type type, [CanBeNull] Expression instance,
            int errorPos, [NotNull] string id,
            [NotNull] Lazy<Expression[]> argumentList)
        {
            return base.GenerateMethodCall(type, instance, errorPos, id, argumentList)
                .OrElse(() => ExtensionMethod(instance, id, argumentList, errorPos, prefixed: false))
                .OrElse(() => ExtensionMethod(instance, id, argumentList, errorPos, prefixed: true))
                .OrElse(() => SurrogateExpression(instance, id));
        }

        [Pure]
        private Maybe<Expression> SurrogateExpression(
            [CanBeNull] Expression instance, [NotNull] string name)
        {
            return _surrogateGenerator(name, instance);
        }

        protected override Try<Expression> TryParseMemberAccess(
            Type type, Expression instance, TokenId nextToken, int errorPos, string name)
        {
            if (name.EqualsIgnoreCase("as"))
            {
                return Try.Create(() => ParseAs(instance));
            }
            return base.TryParseMemberAccess(type, instance, nextToken, errorPos, name);
        }

        [Pure]
        [NotNull]
        private Expression ParseAs([NotNull] Expression instance)
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
