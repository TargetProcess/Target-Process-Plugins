using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Tp.Core.Annotations;
using Tp.Core.Features;
using Tp.Core.Linq;
using Tp.I18n;

namespace Tp.Core.Expressions.Parsing
{
    // Performance tip: try using nullable returns instead of Maybe because this parsing code is called on hot path

    public class BuildExpressionVisitor : Antlr.ExpressionParserBaseVisitor<Expression>
    {
        private const string ItKeyword = "it";

        private static readonly Expression<Func<int?, int?, double?>> _nullableIntDiv = (x, y) => (1.0 * x) / y;
        private static readonly Expression<Func<int, int, double>> _intDiv = (x, y) => (1.0 * x) / y;
        private static readonly Regex _alphanumeric = new Regex(@"^\w+$", RegexOptions.Compiled);

        private readonly Dictionary<Expression, string> _literals;
        private int _paramCounter;

        [NotNull]
        private readonly IEnumerableMethodStrategy _enumerableMethodStrategy;

        [CanBeNull]
        private readonly Type _resultType;

        private readonly Func<IReadOnlyList<DynamicProperty>, Type> _generateDynamicClassType;

        private readonly ITypeSystem _typeSystem;
        private readonly ISurrogateGenerator _surrogateGenerator;
        private Expression _it;

        public BuildExpressionVisitor(
            [NotNull] ITypeSystem typeSystem,
            [NotNull] Expression it,
            [NotNull] IEnumerableMethodStrategy enumerableMethodStrategy,
            [CanBeNull] Type resultType,
            [NotNull] Func<IReadOnlyList<DynamicProperty>, Type> generateDynamicClassType,
            [NotNull] ISurrogateGenerator surrogateGenerator)
        {
            _resultType = resultType;
            _surrogateGenerator = Argument.NotNull(nameof(surrogateGenerator), surrogateGenerator);
            _typeSystem = Argument.NotNull(nameof(typeSystem), typeSystem);
            _it = Argument.NotNull(nameof(it), it);
            _enumerableMethodStrategy = Argument.NotNull(nameof(enumerableMethodStrategy), enumerableMethodStrategy);
            _generateDynamicClassType = Argument.NotNull(nameof(generateDynamicClassType), generateDynamicClassType);
            _literals = new Dictionary<Expression, string>();
        }

        public IReadOnlyDictionary<Expression, string> Literals => _literals;

        protected override Expression AggregateResult(Expression aggregate, Expression nextResult)
        {
            return nextResult ?? aggregate;
        }

        public override Expression VisitCall(Antlr.ExpressionParser.CallContext context)
        {
            var name = context.functionName.GetText();
            var lazyArguments = Lazy.Create(() => ParseArguments(context.arguments()));
            return ParseCallImpl(context.target, context.functionName, lazyArguments, out var targetType)
                ?? TreatNullableBoolAsBool(context.target, context.functionName, lazyArguments)
                ?? PropagateToNullMethod(context.target, context.functionName, lazyArguments)
                ?? throw new UnknownMethodParseException(GetErrorPos(context), name, targetType);
        }

        private Expression[] ParseArguments(Antlr.ExpressionParser.ArgumentsContext args)
        {
            return args == null
                ? Array.Empty<Expression>()
                : args.expression().Select(exp => exp.Accept(this)).ToArray();
        }

        [CanBeNull]
        private Expression ParseCallImpl(
            [CanBeNull] Antlr.ExpressionParser.ExpressionContext target,
            [CanBeNull] Antlr.ExpressionParser.FieldNameExprContext functionName,
            [NotNull] Lazy<Expression[]> arguments,
            out Type targetType)
        {
            return ParseMethodOrPropertyOrField(target, functionName, false, arguments, out targetType)
                ?? ParseFunctionCall(target, functionName, arguments.Value);
        }

        [CanBeNull]
        private Expression TreatNullableBoolAsBool(
            [CanBeNull] Antlr.ExpressionParser.ExpressionContext target,
            [CanBeNull] Antlr.ExpressionParser.FieldNameExprContext functionName,
            [NotNull] Lazy<Expression[]> arguments)
        {
            var newArgs = Lazy.Create(() => arguments.Value.Select(SharedParserUtils.ConvertNullableBoolToBoolExpression).ToArray());
            return ParseCallImpl(target, functionName, newArgs, out _);
        }

        /// <summary>
        /// Convert <c>Foo(a,b,c)</c>=><c>(a!=null && b!=null)?Foo(a.Value,b.Value):null;</c>
        /// </summary>
        [CanBeNull]
        private Expression PropagateToNullMethod(
            [CanBeNull] Antlr.ExpressionParser.ExpressionContext target,
            [CanBeNull] Antlr.ExpressionParser.FieldNameExprContext functionName,
            [NotNull] Lazy<Expression[]> arguments)
        {
            var newArgs = Lazy.Create(
                () => arguments.Value.Select(a => a.Type.IsNullable() ? Expression.Property(a, "Value") : a).ToArray());
            var methodCall = ParseCallImpl(target, functionName, newArgs, out _);
            if (methodCall is null)
            {
                return null;
            }

            var test = arguments.Value.Where(x => x.Type.IsNullable())
                .Select(x => Expression.NotEqual(x, Expression.Constant(null, x.Type)))
                .ToArray()
                .CombineAnd();
            var needToNullate = methodCall.Type.IsValueType;
            var resultType = needToNullate ? typeof(Nullable<>).MakeGenericType(methodCall.Type) : methodCall.Type;
            var ifTrue = needToNullate ? Expression.Convert(methodCall, resultType) : methodCall;
            var ifFalse = Expression.Constant(null, resultType);

            return Expression.Condition(test, ifTrue, ifFalse);
        }

        /// <summary>
        /// Handles predefined functions provided by type system, e.g IFF, IFNONE etc
        /// </summary>
        [CanBeNull]
        private Expression ParseFunctionCall(
            [CanBeNull] Antlr.ExpressionParser.ExpressionContext target,
            [NotNull] Antlr.ExpressionParser.FieldNameExprContext functionName,
            Expression[] arguments)
        {
            if (target != null)
            {
                return null;
            }

            var function = _typeSystem.FindFunction(functionName.name.Text);
            if (function is null)
            {
                return null;
            }

            var context = new FunctionExpressionContext(arguments, GetErrorPos(functionName), Literals);

            return function.Build(context).Switch(
                err => throw new ParseException(err, GetErrorPos(functionName)),
                r => r);
        }

        [CanBeNull]
        private Expression GenerateMethod(
            StaticTypeOrExpression target, string methodName,
            Lazy<Expression[]> arguments, int errorPos)
        {
            if (target.Expression != null && target.Type != typeof(string))
            {
                var enumerableType = SharedParserUtils.FindGenericType(typeof(IEnumerable<>), target.Type);
                if (enumerableType != null)
                {
                    var elementType = enumerableType.GetGenericArguments()[0];
                    var enumerableMethod = ParseEnumerableMethods(target.Expression, elementType, methodName, arguments);
                    if (enumerableMethod != null)
                    {
                        return enumerableMethod;
                    }
                }
            }

            var method = _typeSystem.FindMethod(target, methodName, arguments.Value);
            if (method != null)
            {
                ValidateMethod(method, target.Type, errorPos);
                return Expression.Call(target.Expression, method, arguments.Value);
            }

            return ParseExtensionMethod(target, methodName, arguments, errorPos, false)
                ?? ParseExtensionMethod(target, methodName, arguments, errorPos, true);
        }

        [CanBeNull]
        private Expression ParseExtensionMethod(
            StaticTypeOrExpression target,
            string methodName,
            Lazy<Expression[]> arguments,
            int errorPos,
            bool prefixed)
        {
            if (target.IsStatic)
            {
                return null;
            }

            // Try to find extension method
            var newArguments = prefixed ? arguments.Value : arguments.Value.Prepend(target.Expression).ToArray();
            var method = _typeSystem.FindExtensionMethod(methodName, newArguments);
            if (method != null)
            {
                ValidateMethod(method, target.Type, errorPos);
                return Expression.Call(null, method, newArguments);
            }

            return null;
        }

        [CanBeNull]
        private Expression ParseEnumerableMethods(
            [NotNull] Expression instance,
            [NotNull] Type elementType, [NotNull] string methodName,
            [NotNull] Lazy<Expression[]> parseArgumentList)
        {
            var outerIt = _it;
            var innerLambdaParameter = Expression.Parameter(elementType, "Param" + _paramCounter++);
            _it = _enumerableMethodStrategy.BuildRoot(instance, innerLambdaParameter);
            var args = parseArgumentList.Value;
            _it = outerIt;

            var result = _typeSystem.FindEnumerableMethod(elementType, methodName, args);
            if (result.HasValue)
            {
                var (signatureInfo, typeArgs) = result.Value;
                args = args.Length == 0 ? new[] { instance } : new[] { instance, BuildInnerLambda() };
                var methodSourceType = typeof(IQueryable).IsAssignableFrom(instance.Type) ? typeof(Queryable) : typeof(Enumerable);
                return Expression.Call(methodSourceType, signatureInfo.Name, typeArgs, args);
            }

            if (args.Length == 0)
            {
                return null;
            }

            var methodInfo = GetEnumerableMethod(methodName, new[] { elementType, args[0].Type });
            if (methodInfo == null)
            {
                return null;
            }

            var method = Expression.Call(methodInfo, instance, BuildInnerLambda());
            return method;

            LambdaExpression BuildInnerLambda() => Expression.Lambda(args[0], innerLambdaParameter);
        }

        [Pure]
        [CanBeNull]
        private static MethodInfo GetEnumerableMethod(
            [NotNull] string methodName, [NotNull] [ItemNotNull] Type[] types)
        {
            var query =
                from m in SharedParserUtils.EnumerableMethods
                where m.Name.EqualsIgnoreCase(methodName) && m.IsGenericMethod && m.GetGenericArguments().Length == types.Length
                let parameters = m.GetParameters()
                where parameters.Length == 2 && parameters.All(x => x.ParameterType.IsGenericType)
                    && parameters[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                    && parameters[1].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>)
                select m.MakeGenericMethod(types);

            return query.FirstOrDefault();
        }

        private static void ValidateMethod(MethodInfo method, Type targetType, int errorPos)
        {
            if (method.ReturnType == typeof(void))
            {
                throw new ParseException(
                    Res.MethodIsVoid(method.Name, SharedParserUtils.GetTypeName(targetType)),
                    errorPos);
            }
        }

        public override Expression VisitConstant(Antlr.ExpressionParser.ConstantContext context)
        {
            return context.value.Accept(this);
        }

        public override Expression VisitLiteralExpr(Antlr.ExpressionParser.LiteralExprContext context)
        {
            var intNumber = context.INTEGER_NUMBER();
            var floatNumber = context.FLOAT_NUMBER();
            var str = context.STRING();
            var @true = context.TRUE();
            var @false = context.FALSE();
            var @null = context.NULL();

            if (intNumber != null)
            {
                return ParseIntegerLiteral(intNumber.Symbol);
            }

            if (floatNumber != null)
            {
                return ParseRealLiteral(floatNumber.Symbol);
            }

            if (str != null)
            {
                var allText = str.GetText();
                var text = allText
                    // Remove surrounding quotes
                    .Substring(1, allText.Length - 2)
                    // Handle special chars
                    .Unescape();

                // Behavior from old parser, which treats 'a' as char, but 'ab' as string
                if (allText[0] == '\'' && text.Length == 1)
                {
                    return CreateLiteral(text[0], text);
                }

                return CreateLiteral(text, text);
            }

            if (@true != null)
            {
                return SharedParserUtils.TrueLiteral;
            }

            if (@false != null)
            {
                return SharedParserUtils.FalseLiteral;
            }

            if (@null != null)
            {
                return SharedParserUtils.NullLiteral;
            }

            throw new ParseException(Res.UnknownIdentifier(context.GetText()), GetErrorPos(context));
        }

        private Expression ParseIntegerLiteral(IToken token)
        {
            var text = token.Text;
            if (!ulong.TryParse(text, out ulong value)
                && !ulong.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
            {
                throw ParseError(token, Res.InvalidIntegerLiteral(text));
            }

            if (value <= int.MaxValue) { return CreateLiteral((int) value, text); }

            if (value <= uint.MaxValue) { return CreateLiteral((uint) value, text); }

            if (value <= long.MaxValue) { return CreateLiteral((long) value, text); }

            return CreateLiteral(value, text);
        }

        private Expression ParseRealLiteral(IToken token)
        {
            var text = token.Text;
            object value = null;
            var last = text[text.Length - 1];
            if (last == 'F' || last == 'f')
            {
                var substring = text.Substring(0, text.Length - 1);
                if (float.TryParse(substring, out var f)
                    || float.TryParse(substring, NumberStyles.Any, CultureInfo.InvariantCulture, out f))
                {
                    value = f;
                }
            }
            else
            {
                if (double.TryParse(text, out var d) || double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out d))
                {
                    value = d;
                }
            }

            if (value == null)
            {
                throw ParseError(token, Res.InvalidRealLiteral(text));
            }

            return CreateLiteral(value, text);
        }

        private Expression CreateLiteral(object value, string text)
        {
            var expr = Expression.Constant(value);
            _literals.Add(expr, text);
            return expr;
        }

        public override Expression VisitIndexer(Antlr.ExpressionParser.IndexerContext context)
        {
            var expr = context.target.Accept(this);

            var arg = context.index.Accept(this);

            if (expr.Type.IsArray)
            {
                if (expr.Type.GetArrayRank() != 1 || context.index == null)
                {
                    throw ParseError(context.Start, Res.CannotIndexMultiDimArray);
                }

                var index = SharedParserUtils.PromoteExpression(_literals, arg, typeof(int), true);

                if (index == null)
                {
                    throw ParseError(context.index.Start, Res.InvalidIndex);
                }

                return Expression.ArrayIndex(expr, index);
            }

            var indexer = _typeSystem.FindIndexer(expr.Type, arg);
            if (indexer is null)
            {
                throw ParseError(context.index.Start, Res.NoApplicableIndexer(SharedParserUtils.GetTypeName(expr.Type)));
            }

            return Expression.Call(expr, (MethodInfo) indexer, arg);
        }

        public override Expression VisitConditional(Antlr.ExpressionParser.ConditionalContext context)
        {
            var condition = context.cond.Accept(this);
            var left = context.left.Accept(this);
            var right = context.right.Accept(this);
            return SharedParserUtils.GenerateConditional(_literals, condition, left, right, GetErrorPos(context.cond));
        }

        public override Expression VisitFieldAccess(Antlr.ExpressionParser.FieldAccessContext context)
        {
            var field = context.fieldName.GetText();
            if (context.target == null)
            {
                if (string.Equals(field, ItKeyword, StringComparison.OrdinalIgnoreCase))
                {
                    return _it;
                }
            }

            return ParseMethodOrPropertyOrField(
                    context.target, context.fieldName, true, Lazy.Create(Array.Empty<Expression>()), out var targetType)
                ?? throw new UnknownPropertyOrFieldParseException(GetErrorPos(context.fieldName), field, targetType);
        }

        /// <summary>
        /// Method invocation without arguments could be written without parentheses so
        /// handling of method call and field access is combined into single method
        /// </summary>
        [CanBeNull]
        private Expression ParseMethodOrPropertyOrField(
            [CanBeNull] Antlr.ExpressionParser.ExpressionContext targetContext,
            [NotNull] Antlr.ExpressionParser.FieldNameExprContext methodNameContext,
            bool allowProperties,
            [NotNull] [ItemNotNull] Lazy<Expression[]> arguments,
            out Type targetType)
        {
            var methodName = methodNameContext.GetText();
            var errorPos = GetErrorPos(methodNameContext);

            StaticTypeOrExpression stoe = null;

            if (targetContext != null)
            {
                if (targetContext is Antlr.ExpressionParser.FieldAccessContext fieldAccess && fieldAccess.DOT() == null)
                {
                    var fieldName = fieldAccess.fieldName.GetText();

                    stoe = SharedParserUtils.PredefinedTypesByName.TryGetValue(fieldName, out var matchingType)
                        ? new StaticTypeOrExpression(matchingType)
                        : null;
                }

                stoe = stoe ?? new StaticTypeOrExpression(targetContext.Accept(this));
            }
            else
            {
                stoe = new StaticTypeOrExpression(_it);
            }

            targetType = stoe.Type;
            return MethodOrMember(stoe, methodName, arguments, allowProperties, errorPos);
        }

        [CanBeNull]
        private Expression MethodOrMember(
            [NotNull] StaticTypeOrExpression target,
            [NotNull] string methodName,
            [NotNull] [ItemNotNull] Lazy<Expression[]> arguments,
            bool allowProperties,
            int errorPos)
        {
            var shouldSearchPropertyLike = allowProperties && arguments.Value.Length == 0;

            if (shouldSearchPropertyLike)
            {
                // User didn't pass any arguments to the member reference, e.g. `It.Something`.
                // Let's try to find property or a field first, it's relatively cheap.
                var member = GeneratePropertyOrField(target, methodName);
                if (member != null)
                {
                    return member;
                }

                // Or maybe `It.Something` actually means `It["Something"]`. Should be quite cheap as well.
                if (DynamicDictionary.TryGetExpression(target.Type, target.Expression, methodName, out Expression expression))
                {
                    return expression;
                }

                if (target.Expression != null)
                {
                    var maybeSurrogate = _surrogateGenerator.Generate(methodName, target.Expression);
                    if (maybeSurrogate.HasValue)
                    {
                        return maybeSurrogate.Value;
                    }
                }
            }

            // Next, let's try to find a method with matching arguments.
            // Note that if there's a parameter-less method called `Something()`,
            // and also there's a custom field or extendable domain field named `Something`
            // then `It.Something` will match the method call instead of CF or ED field.
            //
            // It's unknown whether it's a problem
            var method = GenerateMethod(target, methodName, arguments, errorPos);
            if (method != null)
            {
                return method;
            }

            // returns x=>x==null?null:f(x.Value) with cast to Nullable if needed
            if (IsNullableType(target.Type) && target.Expression != null)
            {
                var instance = target.Expression;
                var expression = MethodOrMember(
                    new StaticTypeOrExpression(Expression.Property(instance, nameof(Nullable<int>.Value))),
                    methodName, arguments, allowProperties, errorPos);

                if (expression != null)
                {
                    var protectedExpression = !IsNullableType(expression.Type) && expression.Type.IsValueType
                        ? Expression.Convert(expression, typeof(Nullable<>).MakeGenericType(expression.Type))
                        : expression;

                    var condition = (Expression) Expression.Condition(
                        Expression.Equal(target.Expression, Expression.Constant(null, target.Type)),
                        Expression.Constant(null, protectedExpression.Type), protectedExpression);

                    // attach raw expression to condition to get it name in GetConditionalName()
                    ExtensionsProvider.SetValue(condition, expression);
                    return condition;
                }
            }

            return null;
        }

        [CanBeNull]
        private Expression GeneratePropertyOrField(
            [NotNull] StaticTypeOrExpression target,
            [NotNull] string name)
        {
            var foundProperty = _typeSystem.FindProperty(target.Type, target.Expression, name);
            if (foundProperty is null)
            {
                return null;
            }

            if (foundProperty is PropertyInfo property)
            {
                return Expression.Property(target.Expression, property);
            }

            return Expression.Field(target.Expression, (FieldInfo) foundProperty);
        }

        public override Expression VisitBinary(Antlr.ExpressionParser.BinaryContext context)
        {
            var left = context.left.Accept(this);
            var right = context.right.Accept(this);

            var op = context.PLUS() ?? context.MINUS() ?? context.MULT() ?? context.DIV() ?? context.AMPERSAND();
            if (context.PLUS() != null)
            {
                if (left.Type == typeof(string) || right.Type == typeof(string))
                {
                    return SharedParserUtils.GenerateStringConcat(left, right);
                }

                SharedParserUtils.CheckAndPromoteOperands(
                    _literals, typeof(IAddSignatures), op.GetText(), ref left, ref right,
                    GetErrorPos(op));
            }
            else if (context.MINUS() != null)
            {
                SharedParserUtils.CheckAndPromoteOperands(
                    _literals, typeof(ISubtractSignatures), op.GetText(), ref left, ref right,
                    GetErrorPos(op));
            }
            else
            {
                SharedParserUtils.CheckAndPromoteOperands(
                    _literals, typeof(IArithmeticSignatures), op.GetText(), ref left, ref right,
                    GetErrorPos(op));
            }

            if (context.PLUS() != null)
            {
                return Expression.Add(left, right);
            }

            if (context.MINUS() != null)
            {
                return Expression.Subtract(left, right);
            }

            if (context.MULT() != null)
            {
                return Expression.Multiply(left, right);
            }

            if (context.DIV() != null)
            {
                if (left.Type == typeof(int?) && right.Type == typeof(int?))
                {
                    return _nullableIntDiv.ApplyLambdaParameterExpressions(left, right);
                }

                if (left.Type == typeof(int) && right.Type == typeof(int))
                {
                    return _intDiv.ApplyLambdaParameterExpressions(left, right);
                }

                return Expression.Divide(left, right);
            }

            if (context.AMPERSAND() != null)
            {
                return SharedParserUtils.GenerateStringConcat(left, right);
            }

            throw new ParseException(Res.UnknownIdentifier(op.GetText()), GetErrorPos(op));
        }

        public override Expression VisitRelational(Antlr.ExpressionParser.RelationalContext context)
        {
            var left = context.left.Accept(this);
            if (context.IN() != null)
            {
                var arguments = ParseArguments(context.arguments());
                return SharedParserUtils.GenerateIn(left, arguments);
            }

            var right = context.right.Accept(this);
            var equalityToken = context.EQUAL() ?? context.NOT_EQUAL();

            var op = equalityToken ?? context.GREATER() ?? context.LESS() ?? context.GREATER_OR_EQUAL() ?? context.LESS_OR_EQUAL();

            if (equalityToken != null && !left.Type.IsValueType && !right.Type.IsValueType)
            {
                if (left.Type.IsAssignableFrom(right.Type))
                {
                    right = Expression.Convert(right, left.Type);
                }
                else if (right.Type.IsAssignableFrom(left.Type))
                {
                    left = Expression.Convert(left, right.Type);
                }
                else
                {
                    throw SharedParserUtils.IncompatibleOperandsError(equalityToken.GetText(), left, right, GetErrorPos(equalityToken));
                }
            }
            else if (SharedParserUtils.IsEnumType(left.Type) || SharedParserUtils.IsEnumType(right.Type))
            {
                PromoteLeftOrRight(ref left, ref right, op);
            }
            else if (left.Type == typeof(char) || right.Type == typeof(char))
            {
                PromoteLeftOrRight(ref left, ref right, op);
            }
            else if ((SharedParserUtils.IsEntityCustomValue(left.Type) || SharedParserUtils.IsEntityCustomValue(right.Type))
                && TpFeature.AllowToFilterByEntityCustomFieldDirectly.IsEnabled())
            {
                PromoteLeftOrRight(ref left, ref right, op);
            }
            else
            {
                SharedParserUtils.CheckAndPromoteOperands(_literals,
                    equalityToken != null ? typeof(IEqualitySignatures) : typeof(IRelationalSignatures),
                    op.GetText(), ref left, ref right, GetErrorPos(op));
            }

            if (context.GREATER() != null)
            {
                if (left.Type == typeof(string))
                {
                    return Expression.GreaterThan(
                        SharedParserUtils.GenerateStaticMethodCall(GetErrorPos(op), nameof(string.Compare), left, right),
                        Expression.Constant(0));
                }

                return Expression.GreaterThan(left, right);
            }

            if (context.LESS() != null)
            {
                if (left.Type == typeof(string))
                {
                    return Expression.LessThan(
                        SharedParserUtils.GenerateStaticMethodCall(GetErrorPos(op), nameof(string.Compare), left, right),
                        Expression.Constant(0));
                }

                return Expression.LessThan(left, right);
            }

            if (context.GREATER_OR_EQUAL() != null)
            {
                if (left.Type == typeof(string))
                {
                    return Expression.GreaterThanOrEqual(
                        SharedParserUtils.GenerateStaticMethodCall(GetErrorPos(op), nameof(string.Compare), left, right),
                        Expression.Constant(0));
                }

                return Expression.GreaterThanOrEqual(left, right);
            }

            if (context.LESS_OR_EQUAL() != null)
            {
                if (left.Type == typeof(string))
                {
                    return Expression.LessThanOrEqual(
                        SharedParserUtils.GenerateStaticMethodCall(GetErrorPos(op), nameof(string.Compare), left, right),
                        Expression.Constant(0));
                }

                return Expression.LessThanOrEqual(left, right);
            }

            if (context.EQUAL() != null)
            {
                return Expression.Equal(left, right);
            }

            if (context.NOT_EQUAL() != null)
            {
                return Expression.NotEqual(left, right);
            }

            throw new ParseException(Res.UnknownIdentifier(op.GetText()), GetErrorPos(op));
        }

        public override Expression VisitUnary(Antlr.ExpressionParser.UnaryContext context)
        {
            var right = context.right.Accept(this);

            if (context.MINUS() != null)
            {
                return Expression.Negate(right);
            }

            if (context.LOGICAL_NOT() != null)
            {
                return Expression.Not(right);
            }

            throw new ParseException(Res.UnknownIdentifier(context.GetText()), GetErrorPos(context));
        }

        public override Expression VisitCast(Antlr.ExpressionParser.CastContext context)
        {
            var castTypeName = context.fieldNameExpr().name;
            var castType = _typeSystem.KnownTypes
                .GetValue(castTypeName.Text)
                .GetOrThrow(() => ParseError(castTypeName, System.Linq.Dynamic.Res.UnknownType(castTypeName.Text)));

            var target = context.target.Accept(this);
            return Expression.TypeAs(target, castType);
        }

        public override Expression VisitParenthesis(Antlr.ExpressionParser.ParenthesisContext context)
        {
            return context.expression().Accept(this);
        }

        public override Expression VisitLogical(Antlr.ExpressionParser.LogicalContext context)
        {
            var left = context.left.Accept(this);
            var right = context.right.Accept(this);

            var op = context.LOGICAL_AND() ?? context.LOGICAL_OR();

            SharedParserUtils.CheckAndPromoteOperands(
                _literals,
                typeof(ILogicalSignatures), op.GetText(), ref left, ref right, GetErrorPos(op));

            if (context.LOGICAL_AND() != null)
            {
                return Expression.AndAlso(left, right);
            }

            if (context.LOGICAL_OR() != null)
            {
                return Expression.OrElse(left, right);
            }

            throw new ParseException(Res.UnknownIdentifier(op.GetText()), GetErrorPos(op));
        }

        public override Expression VisitObjectExpr(Antlr.ExpressionParser.ObjectExprContext context)
        {
            if (context.objectPropertyExpr() == null || context.objectPropertyExpr().Length == 0)
            {
                return CreateMemberInit(Array.Empty<DynamicProperty>(), Array.Empty<Expression>());
            }

            var expressions = new List<Expression>();
            var properties = new List<DynamicProperty>();

            foreach (var propertyContext in context.objectPropertyExpr())
            {
                var expressionContext = propertyContext.expression();
                var expr = expressionContext.Accept(this);
                string propName;
                if (propertyContext.alias != null)
                {
                    propName = propertyContext.alias.Text;
                }
                else
                {
                    propName = SharedParserUtils.GetPropertyName(expr, -1);
                    if (propName is null)
                    {
                        var text = expressionContext.GetText();
                        propName = _alphanumeric.IsMatch(text) ? text : null;
                    }

                    if (propName is null)
                    {
                        throw ParseError(propertyContext.alias, Res.MissingAsClause);
                    }
                }

                expressions.Add(expr);
                properties.Add(new DynamicProperty(propName, expr.Type));
            }

            return CreateMemberInit(properties, expressions);
        }

        private Expression CreateMemberInit(
            [NotNull] [ItemNotNull] IReadOnlyList<DynamicProperty> properties,
            [NotNull] [ItemNotNull] IReadOnlyList<Expression> expressions)
        {
            var type = _generateDynamicClassType(properties);

            var bindings = properties
                .Select((property, index) => Expression.Bind(type.GetProperty(property.Name), expressions[index]))
                .ToArray<MemberBinding>();

            return Expression.MemberInit(Expression.New(type), bindings);
        }

        [NotNull]
        public override Expression Visit(IParseTree tree)
        {
            var expression = tree.Accept(this);

            if (_resultType != null)
            {
                var promotedBody = SharedParserUtils.PromoteExpression(
                    _literals, expression, _resultType, true);
                if (promotedBody == null)
                {
                    throw new ParseException(System.Linq.Dynamic.Res.ExpressionTypeMismatch(SharedParserUtils.GetTypeName(_resultType)), 0);
                }

                return promotedBody;
            }

            return expression;
        }

        private void PromoteLeftOrRight(ref Expression left, ref Expression right, ITerminalNode op)
        {
            if (left.Type != right.Type)
            {
                Expression e;
                if ((e = SharedParserUtils.PromoteExpression(_literals, right, left.Type, true)) != null)
                {
                    right = e;
                }
                else if ((e = SharedParserUtils.PromoteExpression(_literals, left, right.Type, true)) != null)
                {
                    left = e;
                }
                else
                {
                    throw SharedParserUtils.IncompatibleOperandsError(op.GetText(), left, right, GetErrorPos(op));
                }
            }
        }

        internal static int GetErrorPos([CanBeNull] IToken token)
        {
            return token?.StartIndex ?? 0;
        }

        private static int GetErrorPos(ParserRuleContext ctx)
        {
            return GetErrorPos(ctx.Start);
        }

        private static int GetErrorPos(ITerminalNode node)
        {
            return GetErrorPos(node.Symbol);
        }

        private Exception ParseError(IToken errorToken, IFormattedMessage message)
        {
            return new ParseException(message, errorToken?.StartIndex ?? 0);
        }

        [Pure]
        private static bool IsNullableType([NotNull] Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}
