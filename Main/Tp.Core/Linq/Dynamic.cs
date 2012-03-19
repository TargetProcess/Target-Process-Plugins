// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

// ReSharper disable CheckNamespace

namespace System.Linq.Dynamic
// ReSharper restore CheckNamespace
{
	public sealed class ParseException : Exception
	{
		private readonly int position;

		public ParseException(string message, int position)
			: base(message)
		{
			this.position = position;
		}

		public int Position
		{
			get { return position; }
		}

		public override string ToString()
		{
			return string.Format(Res.ParseExceptionFormat, Message, position);
		}
	}

	internal class ExpressionParser
	{
		private struct Token
		{
			public TokenId _id;
			public int _pos;
			public string text;
		}

		private enum TokenId
		{
			Unknown,
			End,
			Identifier,
			StringLiteral,
			IntegerLiteral,
			RealLiteral,
			Exclamation,
			Percent,
			Amphersand,
			OpenParen,
			CloseParen,
			Asterisk,
			Plus,
			Comma,
			Minus,
			Dot,
			Slash,
			Colon,
			LessThan,
			Equal,
			GreaterThan,
			Question,
			OpenBracket,
			CloseBracket,
			Bar,
			ExclamationEqual,
			DoubleAmphersand,
			LessThanEqual,
			LessGreater,
			DoubleEqual,
			GreaterThanEqual,
			DoubleBar,
			In
		}

		private interface ILogicalSignatures
		{
			void F(bool x, bool y);
			void F(bool? x, bool? y);
		}

		private interface IArithmeticSignatures
		{
			void F(int x, int y);
			void F(uint x, uint y);
			void F(long x, long y);
			void F(ulong x, ulong y);
			void F(float x, float y);
			void F(double x, double y);
			void F(decimal x, decimal y);
			void F(int? x, int? y);
			void F(uint? x, uint? y);
			void F(long? x, long? y);
			void F(ulong? x, ulong? y);
			void F(float? x, float? y);
			void F(double? x, double? y);
			void F(decimal? x, decimal? y);
		}

		private interface IRelationalSignatures : IArithmeticSignatures
		{
			void F(string x, string y);
			void F(char x, char y);
			void F(DateTime x, DateTime y);
			void F(TimeSpan x, TimeSpan y);
			void F(char? x, char? y);
			void F(DateTime? x, DateTime? y);
			void F(TimeSpan? x, TimeSpan? y);
		}

		private interface IEqualitySignatures : IRelationalSignatures
		{
			void F(bool x, bool y);
			void F(bool? x, bool? y);
			void F(Guid x, Guid y);
			void F(Guid? x, Guid? y);
		}

		private interface IAddSignatures : IArithmeticSignatures
		{
			void F(DateTime x, TimeSpan y);
			void F(TimeSpan x, TimeSpan y);
			void F(DateTime? x, TimeSpan? y);
			void F(TimeSpan? x, TimeSpan? y);
		}

		private interface ISubtractSignatures : IAddSignatures
		{
			void F(DateTime x, DateTime y);
			void F(DateTime? x, DateTime? y);
		}

		private interface INegationSignatures
		{
			void F(int x);
			void F(long x);
			void F(float x);
			void F(double x);
			void F(decimal x);
			void F(int? x);
			void F(long? x);
			void F(float? x);
			void F(double? x);
			void F(decimal? x);
		}

		private interface INotSignatures
		{
			void F(bool x);
			void F(bool? x);
		}

		private interface IEnumerableSignatures
		{
			void Where(bool predicate);
			void Any();
			void Any(bool predicate);
			void All(bool predicate);
			void Count();
			void Count(bool predicate);
			void Min(object selector);
			void Max(object selector);
			void Sum(int selector);
			void Sum(int? selector);
			void Sum(long selector);
			void Sum(long? selector);
			void Sum(float selector);
			void Sum(float? selector);
			void Sum(double selector);
			void Sum(double? selector);
			void Sum(decimal selector);
			void Sum(decimal? selector);
			void Average(int selector);
			void Average(int? selector);
			void Average(long selector);
			void Average(long? selector);
			void Average(float selector);
			void Average(float? selector);
			void Average(double selector);
			void Average(double? selector);
			void Average(decimal selector);
			void Average(decimal? selector);
		}

		private static readonly Type[] PredefinedTypes = {
		                                                 	typeof (Object),
		                                                 	typeof (Boolean),
		                                                 	typeof (Char),
		                                                 	typeof (String),
		                                                 	typeof (SByte),
		                                                 	typeof (Byte),
		                                                 	typeof (Int16),
		                                                 	typeof (UInt16),
		                                                 	typeof (Int32),
		                                                 	typeof (UInt32),
		                                                 	typeof (Int64),
		                                                 	typeof (UInt64),
		                                                 	typeof (Single),
		                                                 	typeof (Double),
		                                                 	typeof (Decimal),
		                                                 	typeof (DateTime),
		                                                 	typeof (TimeSpan),
		                                                 	typeof (Guid),
		                                                 	typeof (Math),
		                                                 	typeof (Convert)
		                                                 };

		private static readonly Expression TrueLiteral = Expression.Constant(true);
		private static readonly Expression FalseLiteral = Expression.Constant(false);
		private static readonly Expression NullLiteral = Expression.Constant(null);

		private const string KEYWORD_IT = "it";
		private const string KEYWORD_IIF = "iif";
		private const string KEYWORD_NEW = "new";

		private static Dictionary<string, object> _keywords;

		private readonly Dictionary<string, object> _symbols;
		private IDictionary<string, object> _externals;
		private readonly Dictionary<Expression, string> _literals;
		private ParameterExpression _it;
		private readonly string _text;
		private int _textPos;
		private readonly int _textLen;
		private char _ch;
		private Token _token;

		public ExpressionParser(ParameterExpression[] parameters, string expression, object[] values)
		{
			if (expression == null) throw new ArgumentNullException("expression");
			if (_keywords == null) _keywords = CreateKeywords();
			_symbols = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
			_literals = new Dictionary<Expression, string>();
			if (parameters != null) ProcessParameters(parameters);
			if (values != null) ProcessValues(values);
			_text = expression;
			_textLen = _text.Length;
			SetTextPos(0);
			NextToken();
		}

		private void ProcessParameters(ParameterExpression[] parameters)
		{
			foreach (ParameterExpression pe in parameters)
				if (!String.IsNullOrEmpty(pe.Name))
					AddSymbol(pe.Name, pe);
			if (parameters.Length == 1 && String.IsNullOrEmpty(parameters[0].Name))
				_it = parameters[0];
		}

		private void ProcessValues(object[] values)
		{
			for (int i = 0; i < values.Length; i++)
			{
				object value = values[i];
				if (i == values.Length - 1 && value is IDictionary<string, object>)
				{
					_externals = (IDictionary<string, object>) value;
				}
				else
				{
					AddSymbol("@" + i.ToString(CultureInfo.InvariantCulture), value);
				}
			}
		}

		private void AddSymbol(string name, object value)
		{
			if (_symbols.ContainsKey(name))
				throw ParseError(Res.DuplicateIdentifier, name);
			_symbols.Add(name, value);
		}

		public Expression Parse(Type resultType)
		{
			int exprPos = _token._pos;
			Expression expr = ParseExpression();
			if (resultType != null)
				if ((expr = PromoteExpression(expr, resultType, true)) == null)
					throw ParseError(exprPos, Res.ExpressionTypeMismatch, GetTypeName(resultType));
			ValidateToken(TokenId.End, Res.SyntaxError);
			return expr;
		}

#pragma warning disable 0219
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
				orderings.Add(new DynamicOrdering {Selector = expr, Ascending = ascending});
				if (_token._id != TokenId.Comma) break;
				NextToken();
			}
			ValidateToken(TokenId.End, Res.SyntaxError);
			return orderings;
		}
#pragma warning restore 0219

		// ?: operator
		private Expression ParseExpression()
		{
			int errorPos = _token._pos;
			Expression expr = ParseLogicalOr();
			if (_token._id == TokenId.Question)
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
			Expression left = ParseLogicalAnd();
			while (_token._id == TokenId.DoubleBar || TokenIdentifierIs("or"))
			{
				Token op = _token;
				NextToken();
				Expression right = ParseLogicalAnd();
				CheckAndPromoteOperands(typeof (ILogicalSignatures), op.text, ref left, ref right, op._pos);
				left = Expression.OrElse(left, right);
			}
			return left;
		}

		// &&, and operator
		private Expression ParseLogicalAnd()
		{
			Expression left = ParseComparison();
			while (_token._id == TokenId.DoubleAmphersand || TokenIdentifierIs("and"))
			{
				Token op = _token;
				NextToken();
				Expression right = ParseComparison();
				CheckAndPromoteOperands(typeof (ILogicalSignatures), op.text, ref left, ref right, op._pos);
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

				var args = ParseBracketExpression();
				return GenerateIn(left, args);
			}


			while (_token._id == TokenId.Equal || _token._id == TokenId.DoubleEqual ||
			       _token._id == TokenId.ExclamationEqual || _token._id == TokenId.LessGreater ||
			       _token._id == TokenId.GreaterThan || _token._id == TokenId.GreaterThanEqual ||
			       _token._id == TokenId.LessThan || _token._id == TokenId.LessThanEqual)
			{
				Token op = _token;
				NextToken();
				Expression right = ParseAdditive();
				bool isEquality = op._id == TokenId.Equal || op._id == TokenId.DoubleEqual ||
				                  op._id == TokenId.ExclamationEqual || op._id == TokenId.LessGreater;
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
							throw IncompatibleOperandsError(op.text, left, right, op._pos);
						}
					}
				}
				else if (IsEnumType(left.Type) || IsEnumType(right.Type))
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
							throw IncompatibleOperandsError(op.text, left, right, op._pos);
						}
					}
				}
				else
				{
					CheckAndPromoteOperands(isEquality ? typeof (IEqualitySignatures) : typeof (IRelationalSignatures),
					                        op.text, ref left, ref right, op._pos);
				}
				switch (op._id)
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
						left = GenerateGreaterThan(left, right);
						break;
					case TokenId.GreaterThanEqual:
						left = GenerateGreaterThanEqual(left, right);
						break;
					case TokenId.LessThan:
						left = GenerateLessThan(left, right);
						break;
					case TokenId.LessThanEqual:
						left = GenerateLessThanEqual(left, right);
						break;
				}
			}
			return left;
		}

		private Expression GenerateIn(Expression operand, Expression[] args)
		{
			Expression<Func<int[], bool>> contains = x => x.Contains(1);

			var containsMethod =
				(contains.Body as MethodCallExpression).Method.GetGenericMethodDefinition().MakeGenericMethod(operand.Type);
			return Expression.Call(containsMethod, Expression.NewArrayInit(operand.Type, args), operand);
		}

		// +, -, & operators
		private Expression ParseAdditive()
		{
			Expression left = ParseMultiplicative();
			while (_token._id == TokenId.Plus || _token._id == TokenId.Minus ||
			       _token._id == TokenId.Amphersand)
			{
				Token op = _token;
				NextToken();
				Expression right = ParseMultiplicative();
				switch (op._id)
				{
					case TokenId.Plus:
						if (left.Type == typeof (string) || right.Type == typeof (string))
							goto case TokenId.Amphersand;
						CheckAndPromoteOperands(typeof (IAddSignatures), op.text, ref left, ref right, op._pos);
						left = GenerateAdd(left, right);
						break;
					case TokenId.Minus:
						CheckAndPromoteOperands(typeof (ISubtractSignatures), op.text, ref left, ref right, op._pos);
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
			Expression left = ParseUnary();
			while (_token._id == TokenId.Asterisk || _token._id == TokenId.Slash ||
			       _token._id == TokenId.Percent || TokenIdentifierIs("mod"))
			{
				Token op = _token;
				NextToken();
				Expression right = ParseUnary();
				CheckAndPromoteOperands(typeof (IArithmeticSignatures), op.text, ref left, ref right, op._pos);
				switch (op._id)
				{
					case TokenId.Asterisk:
						left = Expression.Multiply(left, right);
						break;
					case TokenId.Slash:
						left = Expression.Divide(left, right);
						break;
					case TokenId.Percent:
					case TokenId.Identifier:
						left = Expression.Modulo(left, right);
						break;
				}
			}
			return left;
		}

		// -, !, not unary operators
		private Expression ParseUnary()
		{
			if (_token._id == TokenId.Minus || _token._id == TokenId.Exclamation ||
			    TokenIdentifierIs("not"))
			{
				Token op = _token;
				NextToken();
				if (op._id == TokenId.Minus && (_token._id == TokenId.IntegerLiteral ||
				                               _token._id == TokenId.RealLiteral))
				{
					_token.text = "-" + _token.text;
					_token._pos = op._pos;
					return ParsePrimary();
				}
				Expression expr = ParseUnary();
				if (op._id == TokenId.Minus)
				{
					CheckAndPromoteOperand(typeof (INegationSignatures), op.text, ref expr, op._pos);
					expr = Expression.Negate(expr);
				}
				else
				{
					CheckAndPromoteOperand(typeof (INotSignatures), op.text, ref expr, op._pos);
					expr = Expression.Not(expr);
				}
				return expr;
			}
			return ParsePrimary();
		}

		private Expression ParsePrimary()
		{
			Expression expr = ParsePrimaryStart();
			while (true)
			{
				if (_token._id == TokenId.Dot)
				{
					NextToken();
					expr = ParseMemberAccess(null, expr);
				}
				else if (_token._id == TokenId.OpenBracket)
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
			switch (_token._id)
			{
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
			char quote = _token.text[0];
			string s = _token.text.Substring(1, _token.text.Length - 2);
			int start = 0;
			while (true)
			{
				int i = s.IndexOf(quote, start);
				if (i < 0) break;
				s = s.Remove(i, 1);
				start = i + 1;
			}
			if (quote == '\'')
			{
				if (s.Length != 1)
					throw ParseError(Res.InvalidCharacterLiteral);
				NextToken();
				return CreateLiteral(s[0], s);
			}
			NextToken();
			return CreateLiteral(s, s);
		}

		private Expression ParseIntegerLiteral()
		{
			ValidateToken(TokenId.IntegerLiteral);
			string text = _token.text;
			if (text[0] != '-')
			{
				ulong value;
				if (!UInt64.TryParse(text, out value))
					throw ParseError(Res.InvalidIntegerLiteral, text);
				NextToken();
				if (value <= Int32.MaxValue) return CreateLiteral((int) value, text);
				if (value <= UInt32.MaxValue) return CreateLiteral((uint) value, text);
				if (value <= Int64.MaxValue) return CreateLiteral((long) value, text);
				return CreateLiteral(value, text);
			}
			else
			{
				long value;
				if (!Int64.TryParse(text, out value))
					throw ParseError(Res.InvalidIntegerLiteral, text);
				NextToken();
				if (value >= Int32.MinValue && value <= Int32.MaxValue)
					return CreateLiteral((int) value, text);
				return CreateLiteral(value, text);
			}
		}

		private Expression ParseRealLiteral()
		{
			ValidateToken(TokenId.RealLiteral);
			string text = _token.text;
			object value = null;
			char last = text[text.Length - 1];
			if (last == 'F' || last == 'f')
			{
				float f;
				if (Single.TryParse(text.Substring(0, text.Length - 1), out f)) value = f;
			}
			else
			{
				double d;
				if (Double.TryParse(text, out d)) value = d;
			}
			if (value == null) throw ParseError(Res.InvalidRealLiteral, text);
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
			if (_keywords.TryGetValue(_token.text, out value))
			{
				if (value is Type) return ParseTypeAccess((Type) value);
				if (value == (object)KEYWORD_IT) return ParseIt();
				if (value == (object)KEYWORD_IIF) return ParseIif();
				if (value == (object)KEYWORD_NEW) return ParseNew();
				NextToken();
				return (Expression) value;
			}
			if (_symbols.TryGetValue(_token.text, out value) ||
			    _externals != null && _externals.TryGetValue(_token.text, out value))
			{
				var expr = value as Expression;
				if (expr == null)
				{
					expr = Expression.Constant(value);
				}
				else
				{
					var lambda = expr as LambdaExpression;
					if (lambda != null) return ParseLambdaInvocation(lambda);
				}
				NextToken();
				return expr;
			}
			if (_it != null) return ParseMemberAccess(null, _it);
			throw ParseError(Res.UnknownIdentifier, _token.text);
		}

		private Expression ParseIt()
		{
			if (_it == null)
				throw ParseError(Res.NoItInScope);
			NextToken();
			return _it;
		}

		private Expression ParseIif()
		{
			int errorPos = _token._pos;
			NextToken();
			Expression[] args = ParseArgumentList();
			if (args.Length != 3)
				throw ParseError(errorPos, Res.IifRequiresThreeArgs);
			return GenerateConditional(args[0], args[1], args[2], errorPos);
		}

		private Expression GenerateConditional(Expression test, Expression expr1, Expression expr2, int errorPos)
		{
			if (test.Type != typeof (bool))
				throw ParseError(errorPos, Res.FirstExprMustBeBool);
			if (expr1.Type != expr2.Type)
			{
				Expression expr1as2 = expr2 != NullLiteral ? PromoteExpression(expr1, expr2.Type, true) : null;
				Expression expr2as1 = expr1 != NullLiteral ? PromoteExpression(expr2, expr1.Type, true) : null;
				if (expr1as2 != null && expr2as1 == null)
				{
					expr1 = expr1as2;
				}
				else if (expr2as1 != null && expr1as2 == null)
				{
					expr2 = expr2as1;
				}
				else
				{
					string type1 = expr1 != NullLiteral ? expr1.Type.Name : "null";
					string type2 = expr2 != NullLiteral ? expr2.Type.Name : "null";
					if (expr1as2 != null && expr2as1 != null)
						throw ParseError(errorPos, Res.BothTypesConvertToOther, type1, type2);
					throw ParseError(errorPos, Res.NeitherTypeConvertsToOther, type1, type2);
				}
			}
			return Expression.Condition(test, expr1, expr2);
		}

		private Expression ParseNew()
		{
			NextToken();
			ValidateToken(TokenId.OpenParen, Res.OpenParenExpected);
			NextToken();
			var properties = new List<DynamicProperty>();
			var expressions = new List<Expression>();
			while (true)
			{
				int exprPos = _token._pos;
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
					MemberExpression me = expr as MemberExpression;
					if (me == null) throw ParseError(exprPos, Res.MissingAsClause);
					propName = me.Member.Name;
				}
				expressions.Add(expr);
				properties.Add(new DynamicProperty(propName, expr.Type));
				if (_token._id != TokenId.Comma) break;
				NextToken();
			}
			ValidateToken(TokenId.CloseParen, Res.CloseParenOrCommaExpected);
			NextToken();
			Type type = DynamicExpression.CreateClass(properties);
			var bindings = new MemberBinding[properties.Count];
			for (int i = 0; i < bindings.Length; i++)
				bindings[i] = Expression.Bind(type.GetProperty(properties[i].Name), expressions[i]);
			return Expression.MemberInit(Expression.New(type), bindings);
		}

		private Expression ParseLambdaInvocation(LambdaExpression lambda)
		{
			int errorPos = _token._pos;
			NextToken();
			Expression[] args = ParseArgumentList();
			MethodBase method;
			if (FindMethod(lambda.Type, "Invoke", false, args, out method) != 1)
				throw ParseError(errorPos, Res.ArgsIncompatibleWithLambda);
			return Expression.Invoke(lambda, args);
		}

		private Expression ParseTypeAccess(Type type)
		{
			int errorPos = _token._pos;
			NextToken();
			if (_token._id == TokenId.Question)
			{
				if (!type.IsValueType || IsNullableType(type))
					throw ParseError(errorPos, Res.TypeHasNoNullableForm, GetTypeName(type));
				type = typeof (Nullable<>).MakeGenericType(type);
				NextToken();
			}
			if (_token._id == TokenId.OpenParen)
			{
				Expression[] args = ParseArgumentList();
				MethodBase method;
				switch (FindBestMethod(type.GetConstructors(), args, out method))
				{
					case 0:
						if (args.Length == 1)
							return GenerateConversion(args[0], type, errorPos);
						throw ParseError(errorPos, Res.NoMatchingConstructor, GetTypeName(type));
					case 1:
						return Expression.New((ConstructorInfo) method, args);
					default:
						throw ParseError(errorPos, Res.AmbiguousConstructorInvocation, GetTypeName(type));
				}
			}
			ValidateToken(TokenId.Dot, Res.DotOrOpenParenExpected);
			NextToken();
			return ParseMemberAccess(type, null);
		}

		private Expression GenerateConversion(Expression expr, Type type, int errorPos)
		{
			Type exprType = expr.Type;
			if (exprType == type) return expr;
			if (exprType.IsValueType && type.IsValueType)
			{
				if ((IsNullableType(exprType) || IsNullableType(type)) &&
				    GetNonNullableType(exprType) == GetNonNullableType(type))
					return Expression.Convert(expr, type);
				if ((IsNumericType(exprType) || IsEnumType(exprType)) &&
				    (IsNumericType(type)) || IsEnumType(type))
					return Expression.ConvertChecked(expr, type);
			}
			if (exprType.IsAssignableFrom(type) || type.IsAssignableFrom(exprType) ||
			    exprType.IsInterface || type.IsInterface)
				return Expression.Convert(expr, type);
			throw ParseError(errorPos, Res.CannotConvertValue,
			                 GetTypeName(exprType), GetTypeName(type));
		}

		private Expression ParseMemberAccess(Type type, Expression instance)
		{
			if (instance != null) type = instance.Type;
			int errorPos = _token._pos;
			string id = GetIdentifier();
			NextToken();
			if (_token._id == TokenId.OpenParen)
			{
				if (instance != null && type != typeof (string))
				{
					Type enumerableType = FindGenericType(typeof (IEnumerable<>), type);
					if (enumerableType != null)
					{
						Type elementType = enumerableType.GetGenericArguments()[0];
						return ParseAggregate(instance, elementType, id, errorPos);
					}
				}
				Expression[] args = ParseArgumentList();
				MethodBase mb;
				switch (FindMethod(type, id, instance == null, args, out mb))
				{
					case 0:
						throw ParseError(errorPos, Res.NoApplicableMethod,
						                 id, GetTypeName(type));
					case 1:
						var method = (MethodInfo) mb;
						if (!IsPredefinedType(method.DeclaringType))
							throw ParseError(errorPos, Res.MethodsAreInaccessible, GetTypeName(method.DeclaringType));
						if (method.ReturnType == typeof (void))
							throw ParseError(errorPos, Res.MethodIsVoid,
							                 id, GetTypeName(method.DeclaringType));
						return Expression.Call(instance, method, args);
					default:
						throw ParseError(errorPos, Res.AmbiguousMethodInvocation,
						                 id, GetTypeName(type));
				}
			}
			
			MemberInfo member = FindPropertyOrField(type, id, instance == null);
			if (member == null)
				throw ParseError(errorPos, Res.UnknownPropertyOrField,
				                 id, GetTypeName(type));
			return member is PropertyInfo
			       	? Expression.Property(instance, (PropertyInfo) member)
			       	: Expression.Field(instance, (FieldInfo) member);
		}

		private static Type FindGenericType(Type generic, Type type)
		{
			while (type != null && type != typeof (object))
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

		private Expression ParseAggregate(Expression instance, Type elementType, string methodName, int errorPos)
		{
			ParameterExpression outerIt = _it;
			ParameterExpression innerIt = Expression.Parameter(elementType, "");
			_it = innerIt;
			Expression[] args = ParseArgumentList();
			_it = outerIt;
			MethodBase signature;
			if (FindMethod(typeof (IEnumerableSignatures), methodName, false, args, out signature) != 1)
				throw ParseError(errorPos, Res.NoApplicableAggregate, methodName);
			Type[] typeArgs;
			if (signature.Name == "Min" || signature.Name == "Max")
			{
				typeArgs = new[] {elementType, args[0].Type};
			}
			else
			{
				typeArgs = new[] {elementType};
			}
			args = args.Length == 0 ? new[] {instance} : new[] {instance, Expression.Lambda(args[0], innerIt)};
			return Expression.Call(typeof (Enumerable), signature.Name, typeArgs, args);
		}

		private Expression[] ParseArgumentList()
		{
			ValidateToken(TokenId.OpenParen, Res.OpenParenExpected);
			NextToken();
			Expression[] args = _token._id != TokenId.CloseParen ? ParseArguments() : new Expression[0];
			ValidateToken(TokenId.CloseParen, Res.CloseParenOrCommaExpected);
			NextToken();
			return args;
		}

		private Expression[] ParseArguments()
		{
			List<Expression> argList = new List<Expression>();
			while (true)
			{
				argList.Add(ParseExpression());
				if (_token._id != TokenId.Comma) break;
				NextToken();
			}
			return argList.ToArray();
		}

		private Expression ParseElementAccess(Expression expr)
		{
			int errorPos = _token._pos;
			var args = ParseBracketExpression();
			if (expr.Type.IsArray)
			{
				if (expr.Type.GetArrayRank() != 1 || args.Length != 1)
					throw ParseError(errorPos, Res.CannotIndexMultiDimArray);
				Expression index = PromoteExpression(args[0], typeof (int), true);
				if (index == null)
					throw ParseError(errorPos, Res.InvalidIndex);
				return Expression.ArrayIndex(expr, index);
			}
			else
			{
				MethodBase mb;
				switch (FindIndexer(expr.Type, args, out mb))
				{
					case 0:
						throw ParseError(errorPos, Res.NoApplicableIndexer,
						                 GetTypeName(expr.Type));
					case 1:
						return Expression.Call(expr, (MethodInfo) mb, args);
					default:
						throw ParseError(errorPos, Res.AmbiguousIndexerInvocation,
						                 GetTypeName(expr.Type));
				}
			}
		}

		private Expression[] ParseBracketExpression()
		{
			ValidateToken(TokenId.OpenBracket, Res.OpenParenExpected);
			NextToken();
			Expression[] args = ParseArguments();
			ValidateToken(TokenId.CloseBracket, Res.CloseBracketOrCommaExpected);
			NextToken();
			return args;
		}

		private static bool IsPredefinedType(Type type)
		{
			return PredefinedTypes.Any(t => t == type);
		}

		private static bool IsNullableType(Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>);
		}

		private static Type GetNonNullableType(Type type)
		{
			return IsNullableType(type) ? type.GetGenericArguments()[0] : type;
		}

		private static string GetTypeName(Type type)
		{
			Type baseType = GetNonNullableType(type);
			string s = baseType.Name;
			if (type != baseType) s += '?';
			return s;
		}

		private static bool IsNumericType(Type type)
		{
			return GetNumericTypeKind(type) != 0;
		}

		private static bool IsSignedIntegralType(Type type)
		{
			return GetNumericTypeKind(type) == 2;
		}

		private static bool IsUnsignedIntegralType(Type type)
		{
			return GetNumericTypeKind(type) == 3;
		}

		private static int GetNumericTypeKind(Type type)
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

		private static bool IsEnumType(Type type)
		{
			return GetNonNullableType(type).IsEnum;
		}

		private void CheckAndPromoteOperand(Type signatures, string opName, ref Expression expr, int errorPos)
		{
			var args = new[] {expr};
			MethodBase method;
			if (FindMethod(signatures, "F", false, args, out method) != 1)
				throw ParseError(errorPos, Res.IncompatibleOperand,
				                 opName, GetTypeName(args[0].Type));
			expr = args[0];
		}

		private void CheckAndPromoteOperands(Type signatures, string opName, ref Expression left, ref Expression right,
		                                     int errorPos)
		{
			var args = new[] {left, right};
			MethodBase method;
			if (FindMethod(signatures, "F", false, args, out method) != 1)
				throw IncompatibleOperandsError(opName, left, right, errorPos);
			left = args[0];
			right = args[1];
		}

		private Exception IncompatibleOperandsError(string opName, Expression left, Expression right, int pos)
		{
			return ParseError(pos, Res.IncompatibleOperands,
			                  opName, GetTypeName(left.Type), GetTypeName(right.Type));
		}

		private MemberInfo FindPropertyOrField(Type type, string memberName, bool staticAccess)
		{
			BindingFlags flags = BindingFlags.Public | BindingFlags.DeclaredOnly |
			                     (staticAccess ? BindingFlags.Static : BindingFlags.Instance);
			return (from t in SelfAndBaseTypes(type)
			        select t.FindMembers(MemberTypes.Property | MemberTypes.Field, flags, Type.FilterNameIgnoreCase, memberName)
			        into members
			        where members.Length != 0
			        select members[0]).FirstOrDefault();
		}

		private int FindMethod(Type type, string methodName, bool staticAccess, Expression[] args, out MethodBase method)
		{
			BindingFlags flags = BindingFlags.Public | BindingFlags.DeclaredOnly |
			                     (staticAccess ? BindingFlags.Static : BindingFlags.Instance);
			foreach (Type t in SelfAndBaseTypes(type))
			{
				MemberInfo[] members = t.FindMembers(MemberTypes.Method,
				                                     flags, Type.FilterNameIgnoreCase, methodName);
				int count = FindBestMethod(members.Cast<MethodBase>(), args, out method);
				if (count != 0) return count;
			}
			method = null;
			return 0;
		}

		private int FindIndexer(Type type, Expression[] args, out MethodBase method)
		{
			foreach (Type t in SelfAndBaseTypes(type))
			{
				MemberInfo[] members = t.GetDefaultMembers();
				if (members.Length != 0)
				{
					IEnumerable<MethodBase> methods = members.
						OfType<PropertyInfo>().
						Select(p => (MethodBase) p.GetGetMethod()).
						Where(m => m != null);
					int count = FindBestMethod(methods, args, out method);
					if (count != 0) return count;
				}
			}
			method = null;
			return 0;
		}

		private static IEnumerable<Type> SelfAndBaseTypes(Type type)
		{
			if (type.IsInterface)
			{
				List<Type> types = new List<Type>();
				AddInterface(types, type);
				return types;
			}
			return SelfAndBaseClasses(type);
		}

		private static IEnumerable<Type> SelfAndBaseClasses(Type type)
		{
			while (type != null)
			{
				yield return type;
				type = type.BaseType;
			}
		}

		private static void AddInterface(List<Type> types, Type type)
		{
			if (!types.Contains(type))
			{
				types.Add(type);
				foreach (Type t in type.GetInterfaces()) AddInterface(types, t);
			}
		}

		private class MethodData
		{
			public Expression[] Args;
			public MethodBase MethodBase;
			public ParameterInfo[] Parameters;
		}

		private int FindBestMethod(IEnumerable<MethodBase> methods, Expression[] args, out MethodBase method)
		{
			MethodData[] applicable = methods.
				Select(m => new MethodData {MethodBase = m, Parameters = m.GetParameters()}).
				Where(m => IsApplicable(m, args)).
				ToArray();
			if (applicable.Length > 1)
			{
				applicable = applicable.
					Where(m => applicable.All(n => m == n || IsBetterThan(args, m, n))).
					ToArray();
			}
			if (applicable.Length == 1)
			{
				MethodData md = applicable[0];
				for (int i = 0; i < args.Length; i++) args[i] = md.Args[i];
				method = md.MethodBase;
			}
			else
			{
				method = null;
			}
			return applicable.Length;
		}

		private bool IsApplicable(MethodData method, Expression[] args)
		{
			if (method.Parameters.Length != args.Length) return false;
			Expression[] promotedArgs = new Expression[args.Length];
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

		private Expression PromoteExpression(Expression expr, Type type, bool exact)
		{
			if (expr.Type == type) return expr;
			if (expr is ConstantExpression)
			{
				ConstantExpression ce = (ConstantExpression) expr;
				if (ce == NullLiteral)
				{
					if (!type.IsValueType || IsNullableType(type))
						return Expression.Constant(null, type);
				}
				else
				{
					string text;
					if (_literals.TryGetValue(ce, out text))
					{
						Type target = GetNonNullableType(type);
						Object value = null;
						switch (Type.GetTypeCode(ce.Type))
						{
							case TypeCode.Int32:
							case TypeCode.UInt32:
							case TypeCode.Int64:
							case TypeCode.UInt64:
								value = ParseNumber(text, target);
								break;
							case TypeCode.Double:
								if (target == typeof (decimal)) value = ParseNumber(text, target);
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
			return null;
		}

		private static object ParseNumber(string text, Type type)
		{
			switch (Type.GetTypeCode(GetNonNullableType(type)))
			{
				case TypeCode.SByte:
					sbyte sb;
					if (sbyte.TryParse(text, out sb)) return sb;
					break;
				case TypeCode.Byte:
					byte b;
					if (byte.TryParse(text, out b)) return b;
					break;
				case TypeCode.Int16:
					short s;
					if (short.TryParse(text, out s)) return s;
					break;
				case TypeCode.UInt16:
					ushort us;
					if (ushort.TryParse(text, out us)) return us;
					break;
				case TypeCode.Int32:
					int i;
					if (int.TryParse(text, out i)) return i;
					break;
				case TypeCode.UInt32:
					uint ui;
					if (uint.TryParse(text, out ui)) return ui;
					break;
				case TypeCode.Int64:
					long l;
					if (long.TryParse(text, out l)) return l;
					break;
				case TypeCode.UInt64:
					ulong ul;
					if (ulong.TryParse(text, out ul)) return ul;
					break;
				case TypeCode.Single:
					float f;
					if (float.TryParse(text, out f)) return f;
					break;
				case TypeCode.Double:
					double d;
					if (double.TryParse(text, out d)) return d;
					break;
				case TypeCode.Decimal:
					decimal e;
					if (decimal.TryParse(text, out e)) return e;
					break;
			}
			return null;
		}

		private static object ParseEnum(string name, Type type)
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

		private static bool IsCompatibleWith(Type source, Type target)
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
				default:
					if (st == tt) return true;
					break;
			}
			return false;
		}

		private static bool IsBetterThan(Expression[] args, MethodData m1, MethodData m2)
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
		private static int CompareConversions(Type s, Type t1, Type t2)
		{
			if (t1 == t2) return 0;
			if (s == t1) return 1;
			if (s == t2) return -1;
			bool t1t2 = IsCompatibleWith(t1, t2);
			bool t2t1 = IsCompatibleWith(t2, t1);
			if (t1t2 && !t2t1) return 1;
			if (t2t1 && !t1t2) return -1;
			if (IsSignedIntegralType(t1) && IsUnsignedIntegralType(t2)) return 1;
			if (IsSignedIntegralType(t2) && IsUnsignedIntegralType(t1)) return -1;
			return 0;
		}

		private Expression GenerateEqual(Expression left, Expression right)
		{
			return Expression.Equal(left, right);
		}

		private Expression GenerateNotEqual(Expression left, Expression right)
		{
			return Expression.NotEqual(left, right);
		}

		private Expression GenerateGreaterThan(Expression left, Expression right)
		{
			if (left.Type == typeof (string))
			{
				return Expression.GreaterThan(
					GenerateStaticMethodCall("Compare", left, right),
					Expression.Constant(0)
					);
			}
			return Expression.GreaterThan(left, right);
		}

		private Expression GenerateGreaterThanEqual(Expression left, Expression right)
		{
			if (left.Type == typeof (string))
			{
				return Expression.GreaterThanOrEqual(
					GenerateStaticMethodCall("Compare", left, right),
					Expression.Constant(0)
					);
			}
			return Expression.GreaterThanOrEqual(left, right);
		}

		private Expression GenerateLessThan(Expression left, Expression right)
		{
			if (left.Type == typeof (string))
			{
				return Expression.LessThan(
					GenerateStaticMethodCall("Compare", left, right),
					Expression.Constant(0)
					);
			}
			return Expression.LessThan(left, right);
		}

		private Expression GenerateLessThanEqual(Expression left, Expression right)
		{
			if (left.Type == typeof (string))
			{
				return Expression.LessThanOrEqual(
					GenerateStaticMethodCall("Compare", left, right),
					Expression.Constant(0)
					);
			}
			return Expression.LessThanOrEqual(left, right);
		}

		private Expression GenerateAdd(Expression left, Expression right)
		{
			if (left.Type == typeof (string) && right.Type == typeof (string))
			{
				return GenerateStaticMethodCall("Concat", left, right);
			}
			return Expression.Add(left, right);
		}

		private Expression GenerateSubtract(Expression left, Expression right)
		{
			return Expression.Subtract(left, right);
		}

		private Expression GenerateStringConcat(Expression left, Expression right)
		{
			return Expression.Call(
				null,
				typeof (string).GetMethod("Concat", new[] {typeof (object), typeof (object)}),
				new[] {left, right});
		}

		private MethodInfo GetStaticMethod(string methodName, Expression left, Expression right)
		{
			return left.Type.GetMethod(methodName, new[] {left.Type, right.Type});
		}

		private Expression GenerateStaticMethodCall(string methodName, Expression left, Expression right)
		{
			return Expression.Call(null, GetStaticMethod(methodName, left, right), new[] {left, right});
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

		private void NextToken()
		{
			while (Char.IsWhiteSpace(_ch)) NextChar();
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
				case '\'':
					char quote = _ch;
					do
					{
						NextChar();
						while (_textPos < _textLen && _ch != quote) NextChar();
						if (_textPos == _textLen)
							throw ParseError(_textPos, Res.UnterminatedStringLiteral);
						NextChar();
					} while (_ch == quote);
					t = TokenId.StringLiteral;
					break;
				default:
					if (Char.IsLetter(_ch) || _ch == '@' || _ch == '_')
					{
						do
						{
							NextChar();
						} while (Char.IsLetterOrDigit(_ch) || _ch == '_');
						t = TokenId.Identifier;
						break;
					}
					if (Char.IsDigit(_ch))
					{
						t = TokenId.IntegerLiteral;
						do
						{
							NextChar();
						} while (Char.IsDigit(_ch));
						if (_ch == '.')
						{
							t = TokenId.RealLiteral;
							NextChar();
							ValidateDigit();
							do
							{
								NextChar();
							} while (Char.IsDigit(_ch));
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
							} while (Char.IsDigit(_ch));
						}
						if (_ch == 'F' || _ch == 'f') NextChar();
						break;
					}
					if (_textPos == _textLen)
					{
						t = TokenId.End;
						break;
					}
					throw ParseError(_textPos, Res.InvalidCharacter, _ch);
			}
			_token._id = t;
			_token.text = _text.Substring(tokenPos, _textPos - tokenPos);
			_token._pos = tokenPos;
		}

		private bool TokenIdentifierIs(string id)
		{
			return _token._id == TokenId.Identifier && String.Equals(id, _token.text, StringComparison.OrdinalIgnoreCase);
		}

		private string GetIdentifier()
		{
			ValidateToken(TokenId.Identifier, Res.IdentifierExpected);
			string id = _token.text;
			if (id.Length > 1 && id[0] == '@') id = id.Substring(1);
			return id;
		}

		private void ValidateDigit()
		{
			if (!Char.IsDigit(_ch)) throw ParseError(_textPos, Res.DigitExpected);
		}

		private void ValidateToken(TokenId t, string errorMessage)
		{
			if (_token._id != t) throw ParseError(errorMessage);
		}

		private void ValidateToken(TokenId t)
		{
			if (_token._id != t) throw ParseError(Res.SyntaxError);
		}

		private Exception ParseError(string format, params object[] args)
		{
			return ParseError(_token._pos, format, args);
		}

		private Exception ParseError(int pos, string format, params object[] args)
		{
			return new ParseException(string.Format(CultureInfo.CurrentCulture, format, args), pos);
		}

		private static Dictionary<string, object> CreateKeywords()
		{
			Dictionary<string, object> d = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
			d.Add("true", TrueLiteral);
			d.Add("false", FalseLiteral);
			d.Add("null", NullLiteral);
			d.Add(KEYWORD_IT, KEYWORD_IT);
			d.Add(KEYWORD_IIF, KEYWORD_IIF);
			d.Add(KEYWORD_NEW, KEYWORD_NEW);
			foreach (Type type in PredefinedTypes) d.Add(type.Name, type);
			return d;
		}
	}

	internal static class Res
	{
		public const string DuplicateIdentifier = "The identifier '{0}' was defined more than once";
		public const string ExpressionTypeMismatch = "Expression of type '{0}' expected";
		public const string ExpressionExpected = "Expression expected";
		public const string InvalidCharacterLiteral = "Character literal must contain exactly one character";
		public const string InvalidIntegerLiteral = "Invalid integer literal '{0}'";
		public const string InvalidRealLiteral = "Invalid real literal '{0}'";
		public const string UnknownIdentifier = "Unknown identifier '{0}'";
		public const string NoItInScope = "No 'it' is in scope";
		public const string IifRequiresThreeArgs = "The 'iif' function requires three arguments";
		public const string FirstExprMustBeBool = "The first expression must be of type 'Boolean'";
		public const string BothTypesConvertToOther = "Both of the types '{0}' and '{1}' convert to the other";
		public const string NeitherTypeConvertsToOther = "Neither of the types '{0}' and '{1}' converts to the other";
		public const string MissingAsClause = "Expression is missing an 'as' clause";
		public const string ArgsIncompatibleWithLambda = "Argument list incompatible with lambda expression";
		public const string TypeHasNoNullableForm = "Type '{0}' has no nullable form";
		public const string NoMatchingConstructor = "No matching constructor in type '{0}'";
		public const string AmbiguousConstructorInvocation = "Ambiguous invocation of '{0}' constructor";
		public const string CannotConvertValue = "A value of type '{0}' cannot be converted to type '{1}'";
		public const string NoApplicableMethod = "No applicable method '{0}' exists in type '{1}'";
		public const string MethodsAreInaccessible = "Methods on type '{0}' are not accessible";
		public const string MethodIsVoid = "Method '{0}' in type '{1}' does not return a value";
		public const string AmbiguousMethodInvocation = "Ambiguous invocation of method '{0}' in type '{1}'";
		public const string UnknownPropertyOrField = "No property or field '{0}' exists in type '{1}'";
		public const string NoApplicableAggregate = "No applicable aggregate method '{0}' exists";
		public const string CannotIndexMultiDimArray = "Indexing of multi-dimensional arrays is not supported";
		public const string InvalidIndex = "Array index must be an integer expression";
		public const string NoApplicableIndexer = "No applicable indexer exists in type '{0}'";
		public const string AmbiguousIndexerInvocation = "Ambiguous invocation of indexer in type '{0}'";
		public const string IncompatibleOperand = "Operator '{0}' incompatible with operand type '{1}'";
		public const string IncompatibleOperands = "Operator '{0}' incompatible with operand types '{1}' and '{2}'";
		public const string UnterminatedStringLiteral = "Unterminated string literal";
		public const string InvalidCharacter = "Syntax error '{0}'";
		public const string DigitExpected = "Digit expected";
		public const string SyntaxError = "Syntax error";
		public const string TokenExpected = "{0} expected";
		public const string ParseExceptionFormat = "{0} (at index {1})";
		public const string ColonExpected = "':' expected";
		public const string OpenParenExpected = "'(' expected";
		public const string CloseParenOrOperatorExpected = "')' or operator expected";
		public const string CloseParenOrCommaExpected = "')' or ',' expected";
		public const string DotOrOpenParenExpected = "'.' or '(' expected";
		public const string OpenBracketExpected = "'[' expected";
		public const string CloseBracketOrCommaExpected = "']' or ',' expected";
		public const string IdentifierExpected = "Identifier expected";
	}
}