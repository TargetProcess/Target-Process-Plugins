using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using Tp.Core;
using Tp.Core.Annotations;
using Tp.I18n;

// ReSharper disable once CheckNamespace

namespace System.Linq.Dynamic
{
    internal partial class ExpressionParser
    {
        private static Dictionary<string, object> _keywords;

        private readonly Dictionary<string, object> _symbols;
        private readonly Dictionary<Expression, string> _literals;
        private readonly string _text;
        private readonly int _textLen;

        private IDictionary<string, object> _externals;
        private ParameterExpression _it;
        private int _textPos;
        private char _ch;
        private Token _token;
        private int _paramCounter;

        public bool FixIntegerDivision { get; set; }

        public ExpressionParser(
            [CanBeNull] IReadOnlyList<ParameterExpression> parameters,
            [NotNull] string expression,
            [CanBeNull] IReadOnlyList<object> values)
        {
            Argument.NotNull(nameof(expression), expression);

            if (_keywords == null)
            {
                _keywords = CreateKeywords();
            }

            _symbols = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            _literals = new Dictionary<Expression, string>();

            if (parameters != null)
            {
                ProcessParameters(parameters);
            }

            if (values != null)
            {
                ProcessValues(values);
            }

            _text = expression;
            _textLen = _text.Length;
            SetTextPos(0);
            NextToken();
        }

        private void ProcessParameters(IReadOnlyList<ParameterExpression> parameters)
        {
            foreach (var parameterExpression in parameters)
            {
                if (!string.IsNullOrEmpty(parameterExpression.Name))
                {
                    AddSymbol(parameterExpression.Name, parameterExpression);
                }
            }

            if (parameters.Count == 1 && string.IsNullOrEmpty(parameters[0].Name))
            {
                _it = parameters[0];
            }
        }

        private void ProcessValues(IReadOnlyList<object> values)
        {
            for (var index = 0; index < values.Count; index++)
            {
                var value = values[index];
                if (index == values.Count - 1 && value is IDictionary<string, object>)
                {
                    _externals = (IDictionary<string, object>) value;
                }
                else
                {
                    AddSymbol("@" + index.ToString(CultureInfo.InvariantCulture), value);
                }
            }
        }

        private void AddSymbol(string name, object value)
        {
            if (_symbols.ContainsKey(name))
            {
                throw ParseError(Res.DuplicateIdentifier(name));
            }
            _symbols.Add(name, value);
        }

        public Expression Parse(Type resultType)
        {
            int exprPos = _token.Position;
            Expression expr = ParseExpression();
            if (resultType != null)
            {
                expr = PromoteExpression(expr, resultType, true);
                if (expr == null)
                {
                    throw ParseError(exprPos, Res.ExpressionTypeMismatch(GetTypeName(resultType)));
                }
            }
            ValidateToken(TokenId.End, Res.SyntaxError);
            return expr;
        }

        public IEnumerable<DynamicOrdering> ParseOrdering()
        {
            var orderings = new List<DynamicOrdering>();
            while (true)
            {
                Expression expr = ParseExpression();
                bool ascending = true;
                if (TokenIdentifierIs("asc") || TokenIdentifierIs("ascending"))
                {
                    NextToken();
                }
                else if (TokenIdentifierIs("desc") || TokenIdentifierIs("descending"))
                {
                    NextToken();
                    ascending = false;
                }
                orderings.Add(new DynamicOrdering { Selector = expr, Ascending = ascending });
                if (_token.ID != TokenId.Comma) break;
                NextToken();
            }
            ValidateToken(TokenId.End, Res.SyntaxError);
            return orderings;
        }

        // ?: operator
        private Expression ParseExpression()
        {
            int errorPos = _token.Position;
            Expression expr = ParseLogicalOr();
            if (_token.ID == TokenId.Question)
            {
                NextToken();
                Expression expr1 = ParseExpression();
                ValidateToken(TokenId.Colon, Res.ColonExpected);
                NextToken();
                Expression expr2 = ParseExpression();
                expr = GenerateConditional(expr, expr1, expr2, errorPos);
            }
            return expr;
        }

        // ||, or operator
        private Expression ParseLogicalOr()
        {
            var left = ParseLogicalAnd();
            while (_token.ID == TokenId.DoubleBar || TokenIdentifierIs("or"))
            {
                Token op = _token;
                NextToken();
                Expression right = ParseLogicalAnd();
                CheckAndPromoteOperands(typeof(ILogicalSignatures), op.Text, ref left, ref right, op.Position);
                left = Expression.OrElse(left, right);
            }
            return left;
        }

        // &&, and operator
        private Expression ParseLogicalAnd()
        {
            Expression left = ParseComparison();
            while (_token.ID == TokenId.DoubleAmphersand || TokenIdentifierIs("and"))
            {
                Token op = _token;
                NextToken();
                Expression right = ParseComparison();
                CheckAndPromoteOperands(typeof(ILogicalSignatures), op.Text, ref left, ref right, op.Position);
                left = Expression.AndAlso(left, right);
            }
            return left;
        }

        // =, ==, !=, <>, >, >=, <, <= operators
        private Expression ParseComparison()
        {
            Expression left = ParseAdditive();

            if (TokenIdentifierIs("in"))
            {
                NextToken();

                var args = ParseBracketExpression(false);
                return GenerateIn(left, args);
            }

            while (TokenIsOneOf(
                TokenId.Equal, TokenId.DoubleEqual,
                TokenId.ExclamationEqual, TokenId.LessGreater,
                TokenId.GreaterThan, TokenId.GreaterThanEqual,
                TokenId.LessThan, TokenId.LessThanEqual))
            {
                Token op = _token;
                NextToken();
                Expression right = ParseAdditive();
                bool isEquality = op.ID == TokenId.Equal || op.ID == TokenId.DoubleEqual ||
                    op.ID == TokenId.ExclamationEqual || op.ID == TokenId.LessGreater;
                if (isEquality && !left.Type.IsValueType && !right.Type.IsValueType)
                {
                    if (left.Type != right.Type)
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
                            throw IncompatibleOperandsError(op.Text, left, right, op.Position);
                        }
                    }
                }
                else if (IsEnumType(left.Type) || IsEnumType(right.Type))
                {
                    PromoteLeftOrRight(ref left, ref right, op);
                }
                else if (left.Type == typeof(char) || right.Type == typeof(char))
                {
                    PromoteLeftOrRight(ref left, ref right, op);
                }
                else
                {
                    CheckAndPromoteOperands(isEquality ? typeof(IEqualitySignatures) : typeof(IRelationalSignatures),
                        op.Text, ref left, ref right, op.Position);
                }
                switch (op.ID)
                {
                    case TokenId.Equal:
                    case TokenId.DoubleEqual:
                        left = GenerateEqual(left, right);
                        break;
                    case TokenId.ExclamationEqual:
                    case TokenId.LessGreater:
                        left = GenerateNotEqual(left, right);
                        break;
                    case TokenId.GreaterThan:
                        left = GenerateGreaterThan(op.Position, left, right);
                        break;
                    case TokenId.GreaterThanEqual:
                        left = GenerateGreaterThanEqual(op.Position, left, right);
                        break;
                    case TokenId.LessThan:
                        left = GenerateLessThan(op.Position, left, right);
                        break;
                    case TokenId.LessThanEqual:
                        left = GenerateLessThanEqual(op.Position, left, right);
                        break;
                }
            }
            return left;
        }

        private void PromoteLeftOrRight(ref Expression left, ref Expression right, Token op)
        {
            if (left.Type != right.Type)
            {
                Expression e;
                if ((e = PromoteExpression(right, left.Type, true)) != null)
                {
                    right = e;
                }
                else if ((e = PromoteExpression(left, right.Type, true)) != null)
                {
                    left = e;
                }
                else
                {
                    throw IncompatibleOperandsError(op.Text, left, right, op.Position);
                }
            }
        }

        private static Expression GenerateIn(Expression operand, IEnumerable<Expression> args)
        {
            return Expression.Call(
                ContainsMethod.MakeGenericMethod(operand.Type),
                Expression.NewArrayInit(
                    operand.Type,
                    args.Select(x => x.Type == operand.Type ? x : Expression.Convert(x, operand.Type))),
                operand);
        }

        // +, -, & operators
        private Expression ParseAdditive()
        {
            Expression left = ParseMultiplicative();
            while (TokenIsOneOf(TokenId.Plus, TokenId.Minus, TokenId.Amphersand))
            {
                Token op = _token;
                NextToken();
                Expression right = ParseMultiplicative();
                switch (op.ID)
                {
                    case TokenId.Plus:
                        if (left.Type == typeof(string) || right.Type == typeof(string))
                            goto case TokenId.Amphersand;
                        CheckAndPromoteOperands(typeof(IAddSignatures), op.Text, ref left, ref right, op.Position);
                        left = GenerateAdd(op.Position, left, right);
                        break;
                    case TokenId.Minus:
                        CheckAndPromoteOperands(typeof(ISubtractSignatures), op.Text, ref left, ref right, op.Position);
                        left = GenerateSubtract(left, right);
                        break;
                    case TokenId.Amphersand:
                        left = GenerateStringConcat(left, right);
                        break;
                }
            }
            return left;
        }

        // *, /, %, mod operators
        private Expression ParseMultiplicative()
        {
            var left = ParseUnary();

            while (TokenIsOneOf(TokenId.Asterisk, TokenId.Slash, TokenId.Percent) || TokenIdentifierIs("mod") || TokenIdentifierIs("div"))
            {
                var currentToken = _token;
                NextToken();
                var right = ParseUnary();

                CheckAndPromoteOperands(typeof(IArithmeticSignatures), currentToken.Text, ref left, ref right, currentToken.Position);

                switch (currentToken.ID)
                {
                    case TokenId.Asterisk:
                        left = Expression.Multiply(left, right);
                        break;
                    case TokenId.Slash:
                        left = GenerateDivide(left, right);
                        break;
                    case TokenId.Percent:
                        left = Expression.Modulo(left, right);
                        break;
                    case TokenId.Identifier:
                        if (TokenIdentifierIs("mod"))
                        {
                            left = Expression.Modulo(left, right);
                        }
                        else if (TokenIdentifierIs("div"))
                        {
                            left = Expression.Divide(left, right);
                        }
                        break;
                }
            }
            return left;
        }

        private static readonly Expression<Func<int?, int?, double?>> _nullableIntDiv = (x, y) => (1.0 * x) / y;
        private static readonly Expression<Func<int, int, double>> _intDiv = (x, y) => (1.0 * x) / y;

        private Expression GenerateDivide(Expression left, Expression right)
        {
            if (FixIntegerDivision)
            {
                if (left.Type == typeof(int?) && right.Type == typeof(int?))
                {
                    return _nullableIntDiv.ApplyLambdaParameterExpressions(left, right);
                }
                if (left.Type == typeof(int) && right.Type == typeof(int))
                {
                    return _intDiv.ApplyLambdaParameterExpressions(left, right);
                }
            }

            return Expression.Divide(left, right);
        }

        // -, !, not unary operators
        private Expression ParseUnary()
        {
            if (_token.ID == TokenId.Minus || _token.ID == TokenId.Exclamation || TokenIdentifierIs("not"))
            {
                Token op = _token;
                NextToken();
                if (op.ID != TokenId.Minus || (_token.ID != TokenId.IntegerLiteral && _token.ID != TokenId.RealLiteral))
                {
                    Expression expr = ParseUnary();
                    if (op.ID == TokenId.Minus)
                    {
                        CheckAndPromoteOperand(typeof(INegationSignatures), op.Text, ref expr, op.Position);
                        expr = Expression.Negate(expr);
                    }
                    else
                    {
                        CheckAndPromoteOperand(typeof(INotSignatures), op.Text, ref expr, op.Position);
                        expr = Expression.Not(expr);
                    }
                    return expr;
                }
                _token.Text = "-" + _token.Text;
                _token.Position = op.Position;
            }
            return ParsePrimary();
        }

        private Expression ParsePrimary()
        {
            Expression expr = ParsePrimaryStart();
            while (true)
            {
                if (_token.ID == TokenId.Dot)
                {
                    NextToken();
                    expr = ParseMemberAccess(null, expr);
                }
                else if (_token.ID == TokenId.OpenBracket)
                {
                    expr = ParseElementAccess(expr);
                }
                else
                {
                    break;
                }
            }
            return expr;
        }

        private Expression ParsePrimaryStart()
        {
            switch (_token.ID)
            {
                case TokenId.OpenCurly:
                    return ParseNewJson();
                case TokenId.Identifier:
                    return ParseIdentifier();
                case TokenId.StringLiteral:
                    return ParseStringLiteral();
                case TokenId.IntegerLiteral:
                    return ParseIntegerLiteral();
                case TokenId.RealLiteral:
                    return ParseRealLiteral();
                case TokenId.OpenParen:
                    return ParseParenExpression();
                default:
                    throw ParseError(Res.ExpressionExpected);
            }
        }

        private Expression ParseStringLiteral()
        {
            ValidateToken(TokenId.StringLiteral);
            char quote = _token.Text[0];
            string s;
            try
            {
                s = _token.Text.Substring(1, _token.Text.Length - 2).Unescape();
            }
            catch (Exception e)
            {
                throw ParseError(e);
            }
            if (quote == '\'')
            {
                NextToken();
                return s.Length != 1 ? CreateLiteral(s, s) : CreateLiteral(s[0], s);
            }
            NextToken();
            return CreateLiteral(s, s);
        }


        private Expression ParseIntegerLiteral()
        {
            ValidateToken(TokenId.IntegerLiteral);
            string text = _token.Text;
            if (text[0] != '-')
            {
                if (!ulong.TryParse(text, out ulong value) && ulong.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                {
                    throw ParseError(Res.InvalidIntegerLiteral(text));
                }

                NextToken();

                if (value <= int.MaxValue) { return CreateLiteral((int) value, text); }
                if (value <= uint.MaxValue) { return CreateLiteral((uint) value, text); }
                if (value <= long.MaxValue) { return CreateLiteral((long) value, text); }

                return CreateLiteral(value, text);
            }
            else
            {
                if (!long.TryParse(text, out long value) && !long.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                {
                    throw ParseError(Res.InvalidIntegerLiteral(text));
                }

                NextToken();
                if (value >= int.MinValue && value <= int.MaxValue)
                {
                    return CreateLiteral((int) value, text);
                }

                return CreateLiteral(value, text);
            }
        }

        private Expression ParseRealLiteral()
        {
            ValidateToken(TokenId.RealLiteral);
            string text = _token.Text;
            object value = null;
            char last = text[text.Length - 1];
            if (last == 'F' || last == 'f')
            {
                float f;
                string substring = text.Substring(0, text.Length - 1);
                if (float.TryParse(substring, out f) || float.TryParse(substring, NumberStyles.Any, CultureInfo.InvariantCulture, out f))
                {
                    value = f;
                }
            }
            else
            {
                double d;
                if (double.TryParse(text, out d) || double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out d))
                {
                    value = d;
                }
            }

            if (value == null)
            {
                throw ParseError(Res.InvalidRealLiteral(text));
            }

            NextToken();
            return CreateLiteral(value, text);
        }

        private Expression CreateLiteral(object value, string text)
        {
            ConstantExpression expr = Expression.Constant(value);
            _literals.Add(expr, text);
            return expr;
        }

        private Expression ParseParenExpression()
        {
            ValidateToken(TokenId.OpenParen, Res.OpenParenExpected);
            NextToken();
            Expression e = ParseExpression();
            ValidateToken(TokenId.CloseParen, Res.CloseParenOrOperatorExpected);
            NextToken();
            return e;
        }

        private Expression ParseIdentifier()
        {
            ValidateToken(TokenId.Identifier);
            object value;
            if (_keywords.TryGetValue(_token.Text, out value))
            {
                var type = value as Type;
                if (type != null) { return ParseTypeAccess(type); }
                if (Equals(value, KEYWORD_IT)) { return ParseIt(); }
                if (Equals(value, KEYWORD_IIF)) { return ParseIif(); }
                if (Equals(value, KEYWORD_NEW)) { return ParseNewObject(); }
                if (Equals(value, KEYWORD_IFNONE)) { return ParseIfNone(); }

                NextToken();
                return (Expression) value;
            }
            if (_symbols.TryGetValue(_token.Text, out value) ||
                _externals != null && _externals.TryGetValue(_token.Text, out value))
            {
                if (!(value is Expression expr))
                {
                    expr = Expression.Constant(value);
                }
                else
                {
                    if (expr is LambdaExpression lambda)
                    {
                        return ParseLambdaInvocation(lambda);
                    }
                }
                NextToken();
                return expr;
            }

            // Convert f(x) => x.f()
            if (_it != null)
            {
                return ParseMemberAccess(null, _it);
            }

            throw ParseError(Res.UnknownIdentifier(_token.Text));
        }

        private Expression ParseIt()
        {
            if (_it == null)
            {
                throw ParseError(Res.NoItInScope);
            }
            NextToken();
            return _it;
        }

        private Expression ParseIif()
        {
            int errorPos = _token.Position;
            NextToken();
            Expression[] args = ParseArgumentList();
            if (args.Length != 3)
            {
                throw ParseError(errorPos, Res.IifRequiresThreeArgs);
            }
            return GenerateConditional(args[0], args[1], args[2], errorPos);
        }

        private Expression ParseIfNone()
        {
            int errorPos = _token.Position;
            NextToken();
            Expression[] args = ParseArgumentList();
            if (args.Length != 2)
            {
                throw ParseError(errorPos, Res.IfNoneRequiresTwoArgs);
            }
            return GenerateCoalesce(args[0], args[1], errorPos);
        }

        private Expression GenerateConditional(Expression test, Expression expr1, Expression expr2, int errorPos)
        {
            test = ConvertNullableBoolToBoolExpression(test);
            if (test.Type != typeof(bool))
            {
                throw ParseError(errorPos, Res.FirstExprMustBeBool);
            }

            return EqualizeTypesAndCombine(expr1, expr2, errorPos, (e1, e2) => Expression.Condition(test, e1, e2));
        }

        private Expression GenerateCoalesce(Expression valueExpr, Expression ifNoneExpr, int errorPos)
        {
            if (!IsNullableType(valueExpr.Type) && valueExpr.Type != typeof(string))
            {
                throw ParseError(errorPos, Res.TypeHasNoNullableFormAndIsNotString(valueExpr.Type.Name, KEYWORD_IFNONE, 1));
            }

            bool ifNoneExprTypeIsSupported =
                IsNumericType(ifNoneExpr.Type) ||
                ifNoneExpr.Type == typeof(bool) ||
                ifNoneExpr.Type == typeof(string) ||
                ifNoneExpr.Type == typeof(DateTime) ||
                ifNoneExpr.Type == typeof(DateTime?);

            if (!ifNoneExprTypeIsSupported)
            {
                throw ParseError(errorPos, Res.SimpleTypeExpected(ifNoneExpr.Type.Name, KEYWORD_IFNONE, 2));
            }

            return EqualizeTypesAndCombine(valueExpr, ifNoneExpr, errorPos, Expression.Coalesce);
        }

        private Expression EqualizeTypesAndCombine(Expression expr1, Expression expr2, int errorPos,
            Func<Expression, Expression, Expression> combineResults)
        {
            if (expr1.Type != expr2.Type)
            {
                var expr1As2 = expr2 != NullLiteral ? PromoteExpression(expr1, expr2.Type, true) : null;
                var expr2As1 = expr1 != NullLiteral ? PromoteExpression(expr2, expr1.Type, true) : null;
                if (expr1As2 != null && expr2As1 == null)
                {
                    expr1 = expr1As2;
                }
                else if (expr2As1 != null && expr1As2 == null)
                {
                    expr2 = expr2As1;
                }
                else
                {
                    string type1 = expr1 != NullLiteral ? expr1.Type.Name : "null";
                    string type2 = expr2 != NullLiteral ? expr2.Type.Name : "null";
                    if (expr1As2 != null)
                    {
                        throw ParseError(errorPos, Res.BothTypesConvertToOther(type1, type2));
                    }

                    throw ParseError(errorPos, Res.NeitherTypeConvertsToOther(type1, type2));
                }
            }

            return combineResults(expr1, expr2);
        }

        protected static Expression ConvertNullableBoolToBoolExpression(Expression expr)
        {
            return expr.Type == typeof(bool?)
                ? Expression.Equal(expr, Expression.Constant(true, typeof(bool?)))
                : expr;
        }

        private Expression ParseNewJson()
        {
            var properties = new List<DynamicProperty>();
            var expressions = new List<Expression>();
            do
            {
                NextToken();

                ParseObjectPropertyDefinition(out Expression expr, out string propName);

                expressions.Add(expr);
                properties.Add(new DynamicProperty(propName, expr.Type));
            } while (_token.ID == TokenId.Comma);

            ValidateToken(TokenId.CloseCurly, Res.CloseParenOrCommaExpected);
            NextToken();
            return CreateMemberInit(properties, expressions);
        }

        private void ParseObjectPropertyDefinition(out Expression expr, out string propName)
        {
            propName = null;
            var exprPos = _token.Position;
            if (_token.ID == TokenId.Identifier)
            {
                propName = GetIdentifier();
                var token = _token;
                var pos = _textPos;
                var ch = _ch;
                NextToken();
                if (_token.ID != TokenId.Colon)
                {
                    _token = token;
                    _textPos = pos;
                    _ch = ch;
                }
                else
                {
                    NextToken();
                    expr = ParseExpression();
                    return;
                }
            }

            expr = ParseExpression();
            if (TokenIdentifierIs("as"))
            {
                NextToken();
                propName = GetIdentifier();
                NextToken();

                return;
            }

            var maybePropName = GetPropertyName(expr, exprPos);
            if (maybePropName.HasValue)
            {
                propName = maybePropName.Value;
            }
            else if (propName == null)
            {
                throw ParseError(exprPos, Res.MissingAsClause);
            }
        }

        private Maybe<string> GetPropertyName(Expression expr, int exprPos)
        {
            var maybePropName = expr
                .MaybeAs<MemberExpression>().Select(x => x.Member.Name)
                .OrElse(() => DynamicDictionary.GetAlias(expr, exprPos))
                .OrElse(() => GetConditionName(expr, exprPos))
                .OrElse(() => GetEnumerableRootName(expr, exprPos))
                .OrElse(() => GetMethodName(expr));

            return maybePropName;
        }

        // Please, order by usage frequency to microoptimize Enumerable.Contains().
        private static readonly string[] AggregationMethodNames = new []{
            nameof(Enumerable.Count), nameof(Enumerable.Min), nameof(Enumerable.Max),
            nameof(Enumerable.Sum), nameof(Enumerable.Average)}.Concat(nameof(Enumerable.Aggregate)).ToArray();

        // find the root of Enumerable chains and get it name
        // userstories.Where(x).select(y)=>'userstories'
        private Maybe<string> GetEnumerableRootName(Expression expr, int exprPos)
        {
            return expr.MaybeAs<MethodCallExpression>()
                .Where(x => x.Method.DeclaringType == typeof(Enumerable))
                .Where(x => !AggregationMethodNames.Contains(x.Method.Name))
                .Select(x => x.Arguments.First())
                .Bind(x => GetPropertyName(x, exprPos));
        }

        // special case for protected nullable properties
        // for x==null?null:(Nullable<T>)Expr(x) returns name of Expr(x)
        private Maybe<string> GetConditionName(Expression expr, int exprPos)
        {
            return expr.MaybeAs<ConditionalExpression>()
                .Bind(conditional => ExtensionsProvider.GetValue<Expression>(conditional)
                    .Bind(x => GetPropertyName(x, exprPos)));
        }

        private static Maybe<string> GetMethodName(Expression expr)
        {
            return
                from call in expr.MaybeAs<MethodCallExpression>()
                where call.Arguments.Count == 0 || (call.Method.IsExtensionMethod() && call.Arguments.Count == 1)
                select call.Method.Name;
        }

        private Expression ParseNewObject()
        {
            NextToken();
            ValidateToken(TokenId.OpenParen, Res.OpenParenExpected);
            NextToken();
            var properties = new List<DynamicProperty>();
            var expressions = new List<Expression>();
            while (true)
            {
                int exprPos = _token.Position;
                Expression expr = ParseExpression();
                string propName;
                if (TokenIdentifierIs("as"))
                {
                    NextToken();
                    propName = GetIdentifier();
                    NextToken();
                }
                else
                {
                    if (expr is MemberExpression me)
                    {
                        propName = me.Member.Name;
                    }
                    else
                    {
                        if (!DynamicDictionary.TryGetAlias(expr, exprPos, out propName))
                            throw ParseError(exprPos, Res.MissingAsClause);
                    }
                }
                expressions.Add(expr);
                properties.Add(new DynamicProperty(propName, expr.Type));
                if (_token.ID != TokenId.Comma) break;
                NextToken();
            }
            ValidateToken(TokenId.CloseParen, Res.CloseParenOrCommaExpected);
            NextToken();
            return CreateMemberInit(properties, expressions);
        }

        private Expression CreateMemberInit(
            IReadOnlyList<DynamicProperty> properties,
            IReadOnlyList<Expression> expressions)
        {
            var type = GenerateDynamicClassType(properties);

            var bindings = properties
                .Select((property, index) => Expression.Bind(type.GetProperty(property.Name), expressions[index]))
                .ToArray<MemberBinding>();

            return Expression.MemberInit(Expression.New(type), bindings);
        }

        [NotNull]
        protected virtual Type GenerateDynamicClassType(IReadOnlyList<DynamicProperty> properties)
        {
            return DynamicExpressionParser.CreateClass(properties);
        }

        [NotNull]
        private Expression ParseLambdaInvocation([NotNull] LambdaExpression lambda)
        {
            var errorPos = _token.Position;
            NextToken();
            var argumentExpressions = ParseArgumentList();
            if (!GetAppropriateMethod(lambda.Type, "Invoke", false, argumentExpressions).IsSingle)
            {
                throw ParseError(errorPos, Res.ArgsIncompatibleWithLambda);
            }

            return Expression.Invoke(lambda, argumentExpressions);
        }

        private Expression ParseTypeAccess([NotNull] Type type)
        {
            var errorPos = _token.Position;
            NextToken();

            if (_token.ID == TokenId.Question)
            {
                if (!type.IsValueType || IsNullableType(type))
                    throw ParseError(errorPos, Res.TypeHasNoNullableForm(GetTypeName(type)));
                type = typeof(Nullable<>).MakeGenericType(type);
                NextToken();
            }

            if (_token.ID == TokenId.OpenParen)
            {
                var argExpressions = ParseArgumentList();
                var bestMethod = FindBestMethod(type.GetConstructors(), Maybe.Nothing, argExpressions);
                if (bestMethod.HasNone)
                {
                    if (argExpressions.Length == 1)
                    {
                        return GenerateConversion(argExpressions[0], type, errorPos);
                    }

                    throw ParseError(errorPos, Res.NoMatchingConstructor(GetTypeName(type)));
                }

                if (bestMethod.HasSeveral)
                {
                    throw ParseError(errorPos, Res.AmbiguousConstructorInvocation(GetTypeName(type)));
                }

                return Expression.New(bestMethod.GetSingleOrThrow(), argExpressions);
            }

            ValidateToken(TokenId.Dot, Res.DotOrOpenParenExpected);
            NextToken();
            return ParseMemberAccess(type, null);
        }

        [Pure]
        [NotNull]
        private Expression GenerateConversion(
            [NotNull] Expression expr,
            [NotNull] Type type, int errorPos)
        {
            Type exprType = expr.Type;
            if (exprType == type) { return expr; }

            if (exprType.IsValueType && type.IsValueType)
            {
                if ((IsNullableType(exprType) || IsNullableType(type)) &&
                    GetNonNullableType(exprType) == GetNonNullableType(type))
                {
                    return Expression.Convert(expr, type);
                }

                if ((IsNumericType(exprType) || IsEnumType(exprType)) &&
                    IsNumericType(type) || IsEnumType(type))
                {
                    return Expression.ConvertChecked(expr, type);
                }
            }

            if (exprType.IsAssignableFrom(type) || type.IsAssignableFrom(exprType) ||
                exprType.IsInterface || type.IsInterface)
            {
                return Expression.Convert(expr, type);
            }

            throw ParseError(errorPos, Res.CannotConvertValue(GetTypeName(exprType), GetTypeName(type)));
        }

        [NotNull]
        private Expression ParseMemberAccess([CanBeNull] Type type, [CanBeNull] Expression instance)
        {
            if (instance != null)
            {
                type = instance.Type;
            }
            else if (type == null)
            {
                throw new ArgumentException($"Either {nameof(type)} or {nameof(instance)} should be specified");
            }

            var errorPos = _token.Position;
            var id = GetIdentifier();
            NextToken();
            var nextToken = _token.ID;
            return TryParseMemberAccess(type, instance, nextToken, errorPos, id).Value;
        }

        [Pure]
        [NotNull]
        protected virtual Try<Expression> TryParseMemberAccess(
            [NotNull] Type type, [CanBeNull] Expression instance,
            TokenId nextToken, int errorPos, [NotNull] string name)
        {
            if (nextToken == TokenId.OpenParen)
            {
                return GenerateMethodCall(type, instance, errorPos, name, Lazy.Create(ParseArgumentList))
                    .ToTry(() => new UnknownMethodParseException(errorPos, name, type));
            }

            return GenerateMemberAccess(type, instance, errorPos, name)
                .OrElse(() => GenerateMethodCall(type, instance, errorPos, name, Lazy.Create(() => new Expression[0])))
                .OrElse(() => GenerateNullableMethodCall(type, instance, errorPos, name, nextToken))
                .ToTry(() => new UnknownPropertyOrFieldParseException(errorPos, name, type));
        }

        [Pure]
        // returns x=>x==null?null:f(x.Value) with cast to Nullable if needed
        private Maybe<Expression> GenerateNullableMethodCall(
            [NotNull] Type type, [NotNull] Expression instance,
            int errorPos, [NotNull] string id, TokenId nextToken)
        {
            if (IsNullableType(type))
            {
                var expression = TryParseMemberAccess(type.GetGenericArguments()[0], Expression.Property(instance, "Value"), nextToken,
                    errorPos, id);
                return expression
                    .Select(e => new
                    {
                        expression = e,
                        protectedExpression = !IsNullableType(e.Type) && e.Type.IsValueType
                            ? Expression.Convert(e, typeof(Nullable<>).MakeGenericType(e.Type))
                            : e
                    })
                    .Select(notNull =>
                    {
                        var condition = (Expression) Expression.Condition(Expression.Equal(instance, Expression.Constant(null, type)),
                            Expression.Constant(null, notNull.protectedExpression.Type), notNull.protectedExpression);

                        // attach raw expression to condition to get it name in GetConditionalName()
                        ExtensionsProvider.SetValue(condition, notNull.expression);
                        return condition;
                    })
                    .ToMaybe();
            }
            return Maybe.Nothing;
        }

        [Pure]
        protected virtual Maybe<Expression> GenerateMethodCall(
            [NotNull] Type type, [CanBeNull] Expression instance,
            int errorPos, [NotNull] string id,
            [NotNull] Lazy<Expression[]> argumentList)
        {
            return EnumerableMethod(type, instance, id, argumentList)
                .OrElse(() => SelfMethod(type, instance, errorPos, id, argumentList));
        }

        [Pure]
        private Maybe<Expression> EnumerableMethod(
            [NotNull] Type type, [CanBeNull] Expression instance,
            [NotNull] string id, [NotNull] Lazy<Expression[]> argumentList)
        {
            if (instance != null && type != typeof(string))
            {
                Type enumerableType = FindGenericType(typeof(IEnumerable<>), type);
                if (enumerableType != null)
                {
                    Type elementType = enumerableType.GetGenericArguments()[0];
                    return ParseEnumerableMethods(instance, elementType, id, argumentList);
                }
            }
            return Maybe.Nothing;
        }

        [Pure]
        private Maybe<Expression> SelfMethod(
            [NotNull] Type type, [CanBeNull] Expression instance, int errorPos, string id, Lazy<Expression[]> args)
        {
            var methodResult = GetAppropriateMethod(type, id, instance == null, args.Value);
            if (!methodResult.IsSingle)
            {
                return Maybe<Expression>.Nothing;
            }

            var method = (MethodInfo) methodResult.GetSingleOrThrow();
            if (!IsPredefinedType(method.DeclaringType))
            {
                if (!method.DeclaringType.IsEnum)
                {
                    throw ParseError(errorPos, Res.MethodsAreInaccessible(GetTypeName(method.DeclaringType)));
                }
            }

            if (method.ReturnType == typeof(void))
            {
                throw ParseError(errorPos, Res.MethodIsVoid(id, GetTypeName(method.DeclaringType)));
            }

            return Expression.Call(instance, method, args.Value);
        }

        [Pure]
        private static Maybe<Expression> GenerateMemberAccess(
            [NotNull] Type type, [CanBeNull] Expression instance, int errorPos, [NotNull] string name)
        {
            MemberInfo member = FindPropertyOrField(type, name, instance == null);
            if (member != null)
            {
                return member is PropertyInfo
                    ? Expression.Property(instance, (PropertyInfo) member)
                    : Expression.Field(instance, (FieldInfo) member);
            }

            if (DynamicDictionary.TryGetExpression(type, instance, errorPos, name, out Expression expression))
            {
                return expression;
            }

            return Maybe.Nothing;
        }

        [Pure]
        [CanBeNull]
        private static Type FindGenericType([NotNull] Type generic, [NotNull] Type type)
        {
            while (type != null && type != typeof(object))
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == generic) return type;
                if (generic.IsInterface)
                {
                    foreach (Type intfType in type.GetInterfaces())
                    {
                        Type found = FindGenericType(generic, intfType);
                        if (found != null) return found;
                    }
                }
                type = type.BaseType;
            }
            return null;
        }

        [Pure]
        private Maybe<Expression> ParseEnumerableMethods(
            [NotNull] Expression instance,
            [NotNull] Type elementType, [NotNull] string methodName,
            [NotNull] Lazy<Expression[]> parseArgumentList)
        {
            var outerIt = _it;
            var innerIt = Expression.Parameter(elementType, "Param" + _paramCounter++);
            _it = innerIt;
            var args = parseArgumentList.Value;
            _it = outerIt;
            var typeArgs = new[] { elementType };

            if (methodName.EqualsIgnoreCase("Avg"))
            {
                methodName = nameof(Enumerable.Average);
            }

            var signatureInfo = GetAppropriateMethod(typeof(IEnumerableAggregateSignatures), methodName, false, args);
            if (signatureInfo.IsSingle)
            {
                var signature = signatureInfo.GetSingleOrThrow();
                if (signature.Name == nameof(IEnumerableAggregateSignatures.Min) || signature.Name == nameof(IEnumerableAggregateSignatures.Max))
                {
                    typeArgs = new[] { elementType, args[0].Type };
                }

                args = args.Length == 0 ? new[] { instance } : new[] { instance, Expression.Lambda(args[0], innerIt) };
                var methodSourceType = typeof(IQueryable).IsAssignableFrom(instance.Type) ? typeof(Queryable) : typeof(Enumerable);
                return Expression.Call(methodSourceType, signature.Name, typeArgs, args);
            }

            if (args.Length == 0)
            {
                return Maybe.Nothing;
            }

            var methodInfo = GetEnumerableMethod(methodName, new[] { elementType, args[0].Type });
            if (methodInfo == null)
            {
                return Maybe.Nothing;
            }

            var method = Expression.Call(methodInfo, instance, Expression.Lambda(args[0], innerIt));
            return method;
        }

        [Pure]
        [CanBeNull]
        private static MethodInfo GetEnumerableMethod(
            [NotNull] string methodName, [NotNull] [ItemNotNull] Type[] types)
        {
            return (from m in typeof(Enumerable).GetMethods()
                where m.Name.EqualsIgnoreCase(methodName) && m.IsGenericMethod && m.GetGenericArguments().Length == types.Length
                let parameters = m.GetParameters()
                where parameters.Length == 2 && parameters.All(x => x.ParameterType.IsGenericType)
                && parameters[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                && parameters[1].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>)
                select m.MakeGenericMethod(types)).FirstOrDefault();
        }

        [NotNull]
        [ItemNotNull]
        private Expression[] ParseArgumentList()
        {
            ValidateToken(TokenId.OpenParen, Res.OpenParenExpected);
            NextToken();
            var args = _token.ID != TokenId.CloseParen ? ParseArguments() : new Expression[0];
            ValidateToken(TokenId.CloseParen, Res.CloseParenOrCommaExpected);
            NextToken();
            return args;
        }

        [NotNull]
        [ItemNotNull]
        private Expression[] ParseArguments()
        {
            var argList = new List<Expression>();
            while (true)
            {
                argList.Add(ParseExpression());
                if (_token.ID != TokenId.Comma) break;
                NextToken();
            }
            return argList.ToArray();
        }

        [NotNull]
        private Expression ParseElementAccess([NotNull] Expression expr)
        {
            var errorPos = _token.Position;
            var args = ParseBracketExpression(true);
            if (expr.Type.IsArray)
            {
                if (expr.Type.GetArrayRank() != 1 || args.Length != 1)
                {
                    throw ParseError(errorPos, Res.CannotIndexMultiDimArray);
                }

                var index = PromoteExpression(args[0], typeof(int), true);
                if (index == null)
                {
                    throw ParseError(errorPos, Res.InvalidIndex);
                }

                return Expression.ArrayIndex(expr, index);
            }
            var indexer = FindIndexer(expr.Type, args);
            if (indexer.HasNone)
            {
                throw ParseError(errorPos, Res.NoApplicableIndexer(GetTypeName(expr.Type)));
            }

            if (indexer.HasSeveral)
            {
                throw ParseError(errorPos, Res.AmbiguousIndexerInvocation(GetTypeName(expr.Type)));
            }

            return Expression.Call(expr, (MethodInfo) indexer.GetSingleOrThrow(), args);
        }

        [NotNull]
        [ItemNotNull]
        private Expression[] ParseBracketExpression(bool throwOnEmptyArgs)
        {
            ValidateToken(TokenId.OpenBracket, Res.OpenParenExpected);
            NextToken();
            var args = new Expression[] { };

            if (_token.ID != TokenId.CloseBracket || throwOnEmptyArgs)
            {
                args = ParseArguments();
            }

            ValidateToken(TokenId.CloseBracket, Res.CloseBracketOrCommaExpected);
            NextToken();
            return args;
        }

        [Pure]
        private static bool IsPredefinedType([NotNull] Type type)
        {
            var nonNullableType = GetNonNullableType(type);

            return PredefinedTypes.Any(t => t == type || t == nonNullableType);
        }

        [Pure]
        private static bool IsNullableType([NotNull] Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        [Pure]
        [NotNull]
        private static Type GetNonNullableType(
            [NotNull] Type type)
        {
            return IsNullableType(type) ? type.GetGenericArguments()[0] : type;
        }

        [Pure]
        internal static string GetTypeName([NotNull] Type type)
        {
            var baseType = GetNonNullableType(type);

            var displayName = baseType.GetCustomAttribute<ComponentModel.DisplayNameAttribute>();
            var result = displayName.HasValue && !displayName.Value.DisplayName.IsNullOrEmpty()
                ? displayName.Value.DisplayName
                : baseType.Name;

            if (type != baseType)
            {
                result += '?';
            }

            return result;
        }

        [Pure]
        private static bool IsNumericType([NotNull] Type type)
        {
            return GetNumericTypeKind(type) != 0;
        }

        [Pure]
        private static bool IsSignedIntegralType([NotNull] Type type)
        {
            return GetNumericTypeKind(type) == 2;
        }

        [Pure]
        private static bool IsUnsignedIntegralType([NotNull] Type type)
        {
            return GetNumericTypeKind(type) == 3;
        }

        [Pure]
        private static int GetNumericTypeKind([NotNull] Type type)
        {
            type = GetNonNullableType(type);
            if (type.IsEnum) return 0;
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Char:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return 1;
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return 2;
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return 3;
                default:
                    return 0;
            }
        }

        [Pure]
        private static bool IsEnumType([NotNull] Type type)
        {
            return GetNonNullableType(type).IsEnum;
        }

        private void CheckAndPromoteOperand(
            // ReSharper disable once UnusedParameter.Local
            [NotNull] Type signatures, [NotNull] string opName,
            [NotNull] ref Expression expr, int errorPos)
        {
            var args = new[] { expr };
            if (!GetAppropriateMethod(signatures, "F", false, args).IsSingle)
            {
                throw ParseError(errorPos, Res.IncompatibleOperand(opName, GetTypeName(args[0].Type)));
            }

            expr = args[0];
        }

        private void CheckAndPromoteOperands(
            // ReSharper disable once UnusedParameter.Local
            [NotNull] Type signatures, [NotNull] string opName,
            [NotNull] ref Expression left, [NotNull] ref Expression right,
            int errorPos)
        {
            var args = new[] { left, right };
            if (!GetAppropriateMethod(signatures, "F", false, args).IsSingle)
            {
                throw IncompatibleOperandsError(opName, left, right, errorPos);
            }

            left = args[0];
            right = args[1];
        }

        [Pure]
        [NotNull]
        private Exception IncompatibleOperandsError(
            [NotNull] string opName, [NotNull] Expression left, [NotNull] Expression right, int pos)
        {
            return ParseError(pos, Res.IncompatibleOperands(opName, GetTypeName(left.Type), GetTypeName(right.Type)));
        }

        [Pure]
        private static MemberInfo FindPropertyOrField(
            [NotNull] Type type, [NotNull] string memberName, bool staticAccess)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.DeclaredOnly |
                (staticAccess ? BindingFlags.Static : BindingFlags.Instance);
            return (from t in SelfAndBaseTypes(type)
                select t.FindMembers(MemberTypes.Property | MemberTypes.Field, flags, Type.FilterNameIgnoreCase, memberName)
                into members
                where members.Length != 0
                select members[0]).FirstOrDefault();
        }

        [Pure]
        private SingleMember<MethodBase> GetAppropriateMethod(
            [NotNull] Type type,
            [NotNull] string methodName, bool staticAccess,
            [NotNull] [ItemNotNull] Expression[] args)
        {
            var flags = BindingFlags.Public | BindingFlags.DeclaredOnly | (staticAccess ? BindingFlags.Static : BindingFlags.Instance)
                | BindingFlags.IgnoreCase;

            foreach (MemberInfo[] members in SelfAndBaseTypes(type).Select(t => t.GetMethods(flags)))
            {
                var bestMethod = FindBestMethod(members.Cast<MethodBase>(), methodName, args);
                if (bestMethod.HasAny)
                {
                    return bestMethod;
                }
            }

            return SingleMember<MethodBase>.None;
        }

        [Pure]
        private SingleMember<MethodBase> FindIndexer(
            [NotNull] Type type,
            [NotNull] [ItemNotNull] Expression[] args)
        {
            foreach (var t in SelfAndBaseTypes(type))
            {
                var members = t.GetDefaultMembers();
                if (members.Length != 0)
                {
                    var methods = members
                        .OfType<PropertyInfo>()
                        .Select(p => (MethodBase) p.GetGetMethod())
                        .Where(m => m != null);

                    var bestMethod = FindBestMethod(methods, Maybe.Nothing, args);
                    if (bestMethod.HasAny)
                    {
                        return bestMethod;
                    }
                }
            }

            return SingleMember<MethodBase>.None;
        }

        [Pure]
        [NotNull]
        [ItemNotNull]
        private static IEnumerable<Type> SelfAndBaseTypes([NotNull] Type type)
        {
            if (type.IsInterface)
            {
                var types = new HashSet<Type>();
                AddInterface(types, type);
                return types;
            }
            return SelfAndBaseClasses(type);
        }

        [Pure]
        [NotNull]
        [ItemNotNull]
        private static IEnumerable<Type> SelfAndBaseClasses(
            [NotNull] Type type)
        {
            while (type != null)
            {
                yield return type;
                type = type.BaseType;
            }
        }

        private static void AddInterface(
            [NotNull] [ItemNotNull] ICollection<Type> types,
            [NotNull] Type type)
        {
            if (types.Contains(type))
            {
                return;
            }

            types.Add(type);
            foreach (var t in type.GetInterfaces())
            {
                AddInterface(types, t);
            }
        }

        private class MethodData<T>
            where T : MethodBase
        {
            public Expression[] Args;
            public T MethodBase;
            public ParameterInfo[] Parameters;
        }

        protected class SingleMember<T>
            where T : class
        {
            private readonly T _value;

            public bool HasAny { get; }
            public bool HasSeveral { get; }
            public bool HasNone => !HasAny;
            public bool IsSingle => HasAny && !HasSeveral;

            public SingleMember([NotNull] T value) : this(hasAny: true, hasSeveral: false)
            {
                _value = Argument.NotNull(nameof(value), value);
                HasAny = true;
                HasSeveral = false;
            }

            private SingleMember(bool hasAny, bool hasSeveral)
            {
                HasAny = hasAny;
                HasSeveral = hasSeveral;
            }

            [NotNull]
            public T GetSingleOrThrow()
            {
                if (HasSeveral)
                {
                    throw new InvalidOperationException($"Single value of type {typeof(T)} expected, but got several");
                }
                if (!HasAny)
                {
                    throw new InvalidOperationException($"Single value of type {typeof(T)} expected, but got none");
                }

                return _value;
            }

            public static SingleMember<T> None => new SingleMember<T>(false, false);
            public static SingleMember<T> Several => new SingleMember<T>(hasAny: true, hasSeveral: false);
        }

        [Pure]
        protected SingleMember<T> FindBestMethod<T>(
            [NotNull] [ItemNotNull] IEnumerable<T> candidates,
            Maybe<string> methodName,
            [NotNull] [ItemNotNull] Expression[] methodArgs)
            where T : MethodBase
        {
            var applicableMethods = candidates
                .Where(m => !methodName.HasValue || IsMethodSuitByName(methodName.Value, m))
                .Select(m => new MethodData<T> { MethodBase = m, Parameters = m.GetParameters() })
                .Where(m => IsApplicable(m, methodArgs))
                .ToArray();

            if (applicableMethods.Length > 1)
            {
                applicableMethods = applicableMethods
                    .Where(m => applicableMethods.All(n => m == n || IsBetterThan(methodArgs, m, n)))
                    .ToArray();
            }

            if (applicableMethods.Length == 1)
            {
                var md = applicableMethods[0];
                for (var i = 0; i < methodArgs.Length; i++)
                {
                    methodArgs[i] = md.Args[i];
                }

                return new SingleMember<T>(md.MethodBase);
            }

            if (applicableMethods.Length > 1)
            {
                return SingleMember<T>.Several;
            }

            return SingleMember<T>.None;
        }

        private static bool IsMethodSuitByName(
            [NotNull] string methodName, [NotNull] MemberInfo m)
        {
            var aliases = new[] { m.Name }.Concat(m.GetCustomAttributes<DynamicExpressionAliasAttribute>().Select(x => x.Name));

            return aliases.Contains(methodName, StringComparer.InvariantCultureIgnoreCase);
        }

        private bool IsApplicable<T>(
            [NotNull] MethodData<T> method, [NotNull] [ItemNotNull] Expression[] args)
            where T : MethodBase
        {
            if (method.Parameters.Length != args.Length) return false;
            var promotedArgs = new Expression[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                ParameterInfo pi = method.Parameters[i];
                if (pi.IsOut) return false;
                Expression promoted = PromoteExpression(args[i], pi.ParameterType, false);
                if (promoted == null) return false;
                promotedArgs[i] = promoted;
            }
            method.Args = promotedArgs;
            return true;
        }

        [CanBeNull]
        private Expression PromoteExpression(
            [NotNull] Expression expr, [NotNull] Type type, bool exact)
        {
            if (expr.Type == type) return expr;
            if (expr is ConstantExpression ce)
            {
                if (ce == NullLiteral)
                {
                    if (!type.IsValueType || IsNullableType(type))
                        return Expression.Constant(null, type);
                }
                else
                {
                    if (_literals.TryGetValue(ce, out string text))
                    {
                        Type target = GetNonNullableType(type);
                        object value = null;
                        switch (Type.GetTypeCode(ce.Type))
                        {
                            case TypeCode.Int32:
                            case TypeCode.UInt32:
                            case TypeCode.Int64:
                            case TypeCode.UInt64:
                                value = ParseNumber(text, target);
                                break;
                            case TypeCode.Double:
                                if (target == typeof(decimal)) value = ParseNumber(text, target);
                                break;
                            case TypeCode.String:
                                value = ParseEnum(text, target);
                                break;
                        }
                        if (value != null)
                            return Expression.Constant(value, type);
                    }
                }
            }
            if (IsCompatibleWith(expr.Type, type))
            {
                if (type.IsValueType || exact) return Expression.Convert(expr, type);
                return expr;
            }
            if (expr.Type == typeof(char) && type == typeof(string))
            {
                return Expression.Call(expr, nameof(string.ToString), Type.EmptyTypes);
            }
            return null;
        }

        [CanBeNull]
        private static object ParseNumber([NotNull] string text, [NotNull] Type type)
        {
            switch (Type.GetTypeCode(GetNonNullableType(type)))
            {
                case TypeCode.SByte:
                    sbyte sb;
                    if (sbyte.TryParse(text, out sb) || sbyte.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out sb))
                        return sb;
                    break;
                case TypeCode.Byte:
                    byte b;
                    if (byte.TryParse(text, out b) || byte.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out b)) return b;
                    break;
                case TypeCode.Int16:
                    short s;
                    if (short.TryParse(text, out s) || short.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out s))
                        return s;
                    break;
                case TypeCode.UInt16:
                    ushort us;
                    if (ushort.TryParse(text, out us) || ushort.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out us))
                        return us;
                    break;
                case TypeCode.Int32:
                    int i;
                    if (int.TryParse(text, out i) || int.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out i)) return i;
                    break;
                case TypeCode.UInt32:
                    uint ui;
                    if (uint.TryParse(text, out ui) || uint.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out ui))
                        return ui;
                    break;
                case TypeCode.Int64:
                    long l;
                    if (long.TryParse(text, out l) || long.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out l)) return l;
                    break;
                case TypeCode.UInt64:
                    ulong ul;
                    if (ulong.TryParse(text, out ul) || ulong.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out ul))
                        return ul;
                    break;
                case TypeCode.Single:
                    float f;
                    if (float.TryParse(text, out f) || float.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out f))
                        return f;
                    break;
                case TypeCode.Double:
                    double d;
                    if (double.TryParse(text, out d) || double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out d))
                        return d;
                    break;
                case TypeCode.Decimal:
                    decimal e;
                    if (decimal.TryParse(text, out e) || decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out e))
                        return e;
                    break;
            }
            return null;
        }

        [CanBeNull]
        private static object ParseEnum([NotNull] string name, [NotNull] Type type)
        {
            if (type.IsEnum)
            {
                MemberInfo[] memberInfos = type.FindMembers(MemberTypes.Field,
                    BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static,
                    Type.FilterNameIgnoreCase, name);
                if (memberInfos.Length != 0) return ((FieldInfo) memberInfos[0]).GetValue(null);
            }
            return null;
        }

        private static bool IsCompatibleWith([NotNull] Type source, [NotNull] Type target)
        {
            if (source == target) return true;
            if (!target.IsValueType) return target.IsAssignableFrom(source);
            Type st = GetNonNullableType(source);
            Type tt = GetNonNullableType(target);
            if (st != source && tt == target) return false;
            TypeCode sc = st.IsEnum ? TypeCode.Object : Type.GetTypeCode(st);
            TypeCode tc = tt.IsEnum ? TypeCode.Object : Type.GetTypeCode(tt);
            switch (sc)
            {
                case TypeCode.SByte:
                    switch (tc)
                    {
                        case TypeCode.SByte:
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Byte:
                    switch (tc)
                    {
                        case TypeCode.Byte:
                        case TypeCode.Int16:
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Int16:
                    switch (tc)
                    {
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.UInt16:
                    switch (tc)
                    {
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Int32:
                    switch (tc)
                    {
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.UInt32:
                    switch (tc)
                    {
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Int64:
                    switch (tc)
                    {
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.UInt64:
                    switch (tc)
                    {
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Single:
                    switch (tc)
                    {
                        case TypeCode.Single:
                        case TypeCode.Double:
                            return true;
                    }
                    break;
                case TypeCode.Decimal:
                    switch (tc)
                    {
                        case TypeCode.Decimal:
                        case TypeCode.Single:
                        case TypeCode.Double:
                            return true;
                    }
                    break;
                default:
                    if (st == tt) return true;
                    break;
            }
            return false;
        }

        private static bool IsBetterThan<T>(
            [NotNull] [ItemNotNull] Expression[] args,
            [NotNull] MethodData<T> m1, [NotNull] MethodData<T> m2)
            where T : MethodBase
        {
            bool better = false;
            for (int i = 0; i < args.Length; i++)
            {
                int c = CompareConversions(args[i].Type,
                    m1.Parameters[i].ParameterType,
                    m2.Parameters[i].ParameterType);
                if (c < 0) return false;
                if (c > 0) better = true;
            }
            return better;
        }

        // Return 1 if s -> t1 is a better conversion than s -> t2
        // Return -1 if s -> t2 is a better conversion than s -> t1
        // Return 0 if neither conversion is better
        private static int CompareConversions(
            [NotNull] Type s, [NotNull] Type t1, [NotNull] Type t2)
        {
            if (t1 == t2) return 0;
            if (s == t1) return 1;
            if (s == t2) return -1;
            bool t1T2 = IsCompatibleWith(t1, t2);
            bool t2T1 = IsCompatibleWith(t2, t1);
            if (t1T2 && !t2T1) return 1;
            if (t2T1 && !t1T2) return -1;
            if (IsSignedIntegralType(t1) && IsUnsignedIntegralType(t2)) return 1;
            if (IsSignedIntegralType(t2) && IsUnsignedIntegralType(t1)) return -1;
            return 0;
        }

        [NotNull]
        private static Expression GenerateEqual(
            [NotNull] Expression left, [NotNull] Expression right)
        {
            return Expression.Equal(left, right);
        }

        [NotNull]
        private static Expression GenerateNotEqual(
            [NotNull] Expression left, [NotNull] Expression right)
        {
            return Expression.NotEqual(left, right);
        }

        [NotNull]
        private static Expression GenerateGreaterThan(
            int errorPosition,
            [NotNull] Expression left, [NotNull] Expression right)
        {
            if (left.Type == typeof(string))
            {
                return Expression.GreaterThan(
                    GenerateStaticMethodCall(errorPosition, nameof(string.Compare), left, right),
                    Expression.Constant(0));
            }
            return Expression.GreaterThan(left, right);
        }

        [NotNull]
        private static Expression GenerateGreaterThanEqual(
            int errorPosition,
            [NotNull] Expression left, [NotNull] Expression right)
        {
            if (left.Type == typeof(string))
            {
                return Expression.GreaterThanOrEqual(
                    GenerateStaticMethodCall(errorPosition, nameof(string.Compare), left, right),
                    Expression.Constant(0));
            }
            return Expression.GreaterThanOrEqual(left, right);
        }

        [NotNull]
        private static Expression GenerateLessThan(
            int errorPosition,
            [NotNull] Expression left, [NotNull] Expression right)
        {
            if (left.Type == typeof(string))
            {
                return Expression.LessThan(
                    GenerateStaticMethodCall(errorPosition, nameof(string.Compare), left, right),
                    Expression.Constant(0));
            }
            return Expression.LessThan(left, right);
        }

        [NotNull]
        private static Expression GenerateLessThanEqual(
            int errorPosition,
            [NotNull] Expression left, [NotNull] Expression right)
        {
            if (left.Type == typeof(string))
            {
                return Expression.LessThanOrEqual(
                    GenerateStaticMethodCall(errorPosition, nameof(string.Compare), left, right),
                    Expression.Constant(0));
            }
            return Expression.LessThanOrEqual(left, right);
        }

        [NotNull]
        private static Expression GenerateAdd(
            int errorPosition,
            [NotNull] Expression left, [NotNull] Expression right)
        {
            if (left.Type == typeof(string) && right.Type == typeof(string))
            {
                return GenerateStaticMethodCall(errorPosition, nameof(string.Concat), left, right);
            }
            return Expression.Add(left, right);
        }

        [NotNull]
        private static Expression GenerateSubtract(
            [NotNull] Expression left, [NotNull] Expression right)
        {
            return Expression.Subtract(left, right);
        }

        [NotNull]
        private static Expression GenerateStringConcat(
            [NotNull] Expression left, [NotNull] Expression right)
        {
            return Expression.Call(
                null,
                // ReSharper disable once AssignNullToNotNullAttribute
                typeof(string).GetMethod(nameof(string.Concat), new[] { typeof(object), typeof(object) }),
                new[] { left, right });
        }

        [CanBeNull]
        private static MethodInfo GetStaticMethod(
            [NotNull] string methodName,
            [NotNull] Expression left, [NotNull] Expression right)
        {
            return left.Type.GetMethod(methodName, new[] { left.Type, right.Type });
        }

        [NotNull]
        private static Expression GenerateStaticMethodCall(
            int errorPosition,
            [NotNull] string methodName,
            [NotNull] Expression left, [NotNull] Expression right)
        {
            var method = GetStaticMethod(methodName, left, right);
            if (method == null)
            {
                throw ParseError(errorPosition, Res.NoApplicableMethod(methodName, left.Type.Name));
            }

            return Expression.Call(null, method, new[] { left, right });
        }

        private void SetTextPos(int pos)
        {
            _textPos = pos;
            _ch = _textPos < _textLen ? _text[_textPos] : '\0';
        }

        private void NextChar()
        {
            if (_textPos < _textLen) _textPos++;
            _ch = _textPos < _textLen ? _text[_textPos] : '\0';
        }

        protected void NextToken()
        {
            while (char.IsWhiteSpace(_ch)) { NextChar(); }
            TokenId t;
            int tokenPos = _textPos;
            switch (_ch)
            {
                case '!':
                    NextChar();
                    if (_ch == '=')
                    {
                        NextChar();
                        t = TokenId.ExclamationEqual;
                    }
                    else
                    {
                        t = TokenId.Exclamation;
                    }
                    break;
                case '%':
                    NextChar();
                    t = TokenId.Percent;
                    break;
                case '&':
                    NextChar();
                    if (_ch == '&')
                    {
                        NextChar();
                        t = TokenId.DoubleAmphersand;
                    }
                    else
                    {
                        t = TokenId.Amphersand;
                    }
                    break;
                case '(':
                    NextChar();
                    t = TokenId.OpenParen;
                    break;
                case ')':
                    NextChar();
                    t = TokenId.CloseParen;
                    break;
                case '*':
                    NextChar();
                    t = TokenId.Asterisk;
                    break;
                case '+':
                    NextChar();
                    t = TokenId.Plus;
                    break;
                case ',':
                    NextChar();
                    t = TokenId.Comma;
                    break;
                case '-':
                    NextChar();
                    t = TokenId.Minus;
                    break;
                case '.':
                    NextChar();
                    t = TokenId.Dot;
                    break;
                case '/':
                    NextChar();
                    t = TokenId.Slash;
                    break;
                case ':':
                    NextChar();
                    t = TokenId.Colon;
                    break;
                case '<':
                    NextChar();
                    if (_ch == '=')
                    {
                        NextChar();
                        t = TokenId.LessThanEqual;
                    }
                    else if (_ch == '>')
                    {
                        NextChar();
                        t = TokenId.LessGreater;
                    }
                    else
                    {
                        t = TokenId.LessThan;
                    }
                    break;
                case '=':
                    NextChar();
                    if (_ch == '=')
                    {
                        NextChar();
                        t = TokenId.DoubleEqual;
                    }
                    else
                    {
                        t = TokenId.Equal;
                    }
                    break;
                case '>':
                    NextChar();
                    if (_ch == '=')
                    {
                        NextChar();
                        t = TokenId.GreaterThanEqual;
                    }
                    else
                    {
                        t = TokenId.GreaterThan;
                    }
                    break;
                case '{':
                    NextChar();
                    t = TokenId.OpenCurly;
                    break;
                case '}':
                    NextChar();
                    t = TokenId.CloseCurly;
                    break;
                case '?':
                    NextChar();
                    t = TokenId.Question;
                    break;
                case '[':
                    NextChar();
                    t = TokenId.OpenBracket;
                    break;
                case ']':
                    NextChar();
                    t = TokenId.CloseBracket;
                    break;
                case '|':
                    NextChar();
                    if (_ch == '|')
                    {
                        NextChar();
                        t = TokenId.DoubleBar;
                    }
                    else
                    {
                        t = TokenId.Bar;
                    }
                    break;
                case '"':
                    const char slashSymbol = '\\';

                    Action first = () =>
                    {
                        if (_ch == slashSymbol)
                        {
                            NextChar();
                            NextChar();
                        }
                    };

                    Action second = () =>
                    {
                        if (_ch == slashSymbol)
                        {
                            NextChar();
                        }
                    };

                    t = GetLiteralToken(first, second);
                    break;
                case '\'':
                    t = GetLiteralToken(() => { }, () => { });
                    break;
                default:
                    if (char.IsLetter(_ch) || _ch == '@' || _ch == '_')
                    {
                        do
                        {
                            NextChar();
                        } while (char.IsLetterOrDigit(_ch) || _ch == '_');
                        t = TokenId.Identifier;
                        break;
                    }
                    if (char.IsDigit(_ch))
                    {
                        t = TokenId.IntegerLiteral;
                        do
                        {
                            NextChar();
                        } while (char.IsDigit(_ch));
                        if (_ch == '.')
                        {
                            t = TokenId.RealLiteral;
                            NextChar();
                            ValidateDigit();
                            do
                            {
                                NextChar();
                            } while (char.IsDigit(_ch));
                        }
                        if (_ch == 'E' || _ch == 'e')
                        {
                            t = TokenId.RealLiteral;
                            NextChar();
                            if (_ch == '+' || _ch == '-') NextChar();
                            ValidateDigit();
                            do
                            {
                                NextChar();
                            } while (char.IsDigit(_ch));
                        }
                        if (_ch == 'F' || _ch == 'f') NextChar();
                        break;
                    }
                    if (_textPos == _textLen)
                    {
                        t = TokenId.End;
                        break;
                    }
                    throw ParseError(_textPos, Res.InvalidCharacter(_ch));
            }
            _token.ID = t;
            _token.Text = _text.Substring(tokenPos, _textPos - tokenPos);
            _token.Position = tokenPos;
        }

        private TokenId GetLiteralToken(
            [NotNull] [InstantHandle] Action first,
            [NotNull] [InstantHandle] Action second)
        {
            char quote = _ch;
            do
            {
                NextChar();
                first();

                while (_textPos < _textLen && _ch != quote)
                {
                    second();
                    NextChar();
                }
                if (_textPos == _textLen)
                    throw ParseError(_textPos, Res.UnterminatedStringLiteral);
                NextChar();
            } while (_ch == quote);

            return TokenId.StringLiteral;
        }

        private bool TokenIdentifierIs([NotNull] string id)
        {
            return _token.ID == TokenId.Identifier && string.Equals(id, _token.Text, StringComparison.OrdinalIgnoreCase);
        }

        private bool TokenIsOneOf(TokenId first, params TokenId[] rest)
        {
            return _token.ID == first || rest.Any(x => x == _token.ID);
        }

        [NotNull]
        protected string GetIdentifier()
        {
            ValidateToken(TokenId.Identifier, Res.IdentifierExpected);
            var id = _token.Text;
            if (id.Length > 1 && id[0] == '@')
            {
                id = id.Substring(1);
            }

            return id;
        }

        [AssertionMethod]
        private void ValidateDigit()
        {
            if (!char.IsDigit(_ch)) throw ParseError(_textPos, Res.DigitExpected);
        }

        [AssertionMethod]
        protected void ValidateToken(TokenId t, IFormattedMessage errorMessage)
        {
            if (_token.ID != t) throw ParseError(errorMessage);
        }

        [AssertionMethod]
        private void ValidateToken(TokenId t)
        {
            if (_token.ID != t) throw ParseError(Res.SyntaxError);
        }

        [NotNull]
        protected ParseException ParseError([NotNull] IFormattedMessage message) =>
            ParseError(_token.Position, message);

        [NotNull]
        protected ParseException ParseError([NotNull] Exception innerException) =>
            ParseError(_token.Position, innerException);

        [NotNull]
        protected static ParseException ParseError(int pos, [NotNull] IFormattedMessage message) =>
            new ParseException(message, pos);

        [NotNull]
        protected ParseException ParseError(int pos, [NotNull] Exception innerException) =>
            new ParseException(innerException, pos);

        [NotNull]
        private static Dictionary<string, object> CreateKeywords()
        {
            var d = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                { "true", TrueLiteral },
                { "false", FalseLiteral },
                { "null", NullLiteral },
                { KEYWORD_IT, KEYWORD_IT },
                { KEYWORD_IIF, KEYWORD_IIF },
                { KEYWORD_NEW, KEYWORD_NEW },
                { KEYWORD_IFNONE, KEYWORD_IFNONE }
            };

            foreach (var type in PredefinedTypes)
            {
                d.Add(type.Name, type);
            }

            return d;
        }
    }
}
