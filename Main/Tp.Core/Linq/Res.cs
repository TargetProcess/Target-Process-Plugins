using Tp.I18n;

namespace System.Linq.Dynamic
{
	internal static class Res
	{
		public static IFormattedMessage AngleBracketsExpected => "Angle brackets expected.".Localize();
		public static IFormattedMessage ExpressionExpected => "Expression expected.".Localize();
		public static IFormattedMessage InvalidCharacterLiteral => "Character literal must contain exactly one character.".Localize();
		public static IFormattedMessage NoItInScope => "No 'it' is in scope.".Localize();
		public static IFormattedMessage IifRequiresThreeArgs => "The 'iif' function requires three arguments.".Localize();
		public static IFormattedMessage IfNoneRequiresTwoArgs => "The 'ifnone' function requires two arguments.".Localize();
		public static IFormattedMessage FirstExprMustBeBool => "The first expression must be of type 'Boolean'.".Localize();
		public static IFormattedMessage MissingAsClause => "Expression is missing an 'as' clause.".Localize();
		public static IFormattedMessage ArgsIncompatibleWithLambda => "Argument list incompatible with lambda expression.".Localize();
		public static IFormattedMessage CannotIndexMultiDimArray => "Indexing of multi-dimensional arrays is not supported.".Localize();
		public static IFormattedMessage InvalidIndex => "Array index must be an integer expression.".Localize();
		public static IFormattedMessage DigitExpected => "Digit expected.".Localize();
		public static IFormattedMessage SyntaxError => "Syntax error.".Localize();
		public static IFormattedMessage ColonExpected => "':' expected.".Localize();
		public static IFormattedMessage OpenParenExpected => "'(' expected.".Localize();
		public static IFormattedMessage CloseParenOrOperatorExpected => "')' or operator expected.".Localize();
		public static IFormattedMessage CloseParenOrCommaExpected => "')' or ',' expected.".Localize();
		public static IFormattedMessage DotOrOpenParenExpected => "'.' or '(' expected.".Localize();
		public static IFormattedMessage CloseBracketOrCommaExpected => "']' or ',' expected.".Localize();
		public static IFormattedMessage UnterminatedStringLiteral => "Unterminated string literal.".Localize();
		public static IFormattedMessage IdentifierExpected => "Identifier expected.".Localize();

		public static IFormattedMessage UnknownType(string type)
		{
			return "Unknown type '{type}'.".Localize(new { type });
		}

		public static IFormattedMessage DuplicateIdentifier(string identifier)
		{
			return "The identifier '{identifier}' was defined more than once.".Localize(new { identifier });
		}

		public static IFormattedMessage ExpressionTypeMismatch(string type)
		{
			return "Expression of type '{type}' expected.".Localize(new { type });
		}


		public static IFormattedMessage InvalidIntegerLiteral(string literal)
		{
			return "Invalid integer literal '{literal}'.".Localize(new { literal });
		}

		public static IFormattedMessage InvalidRealLiteral(string literal)
		{
			return "Invalid real literal '{literal}'.".Localize(new { literal });
		}

		public static IFormattedMessage UnknownIdentifier(string identifier)
		{
			return "Unknown identifier '{identifier}'.".Localize(new { identifier });
		}

		public static IFormattedMessage BothTypesConvertToOther(string type1, string type2)
		{
			return "Both of the types '{type1}' and '{type2}' convert to the other.".Localize(new { type1, type2 });
		}

		public static IFormattedMessage NeitherTypeConvertsToOther(string type1, string type2)
		{
			return "Neither of the types '{type1}' and '{type2}' converts to the other.".Localize(new { type1, type2 });
		}

		public static IFormattedMessage TypeHasNoNullableForm(string type)
		{
			return "Type '{type}' has no nullable form.".Localize(new { type });
		}

		public static IFormattedMessage TypeHasNoNullableFormAndIsNotString(string type, string @operator, int argumentNumber)
		{
			return "{operator} operator works only with nullable numbers, bools and strings for #{argumentNumber} argument. Actual type:'{type}'.".Localize(new { type, @operator, argumentNumber });
		}

		public static IFormattedMessage NoMatchingConstructor(string type)
		{
			return "No matching constructor in type '{type}'.".Localize(new { type });
		}

		public static IFormattedMessage AmbiguousConstructorInvocation(string type)
		{
			return "Ambiguous invocation of '{type}' constructor.".Localize(new { type });
		}

		public static IFormattedMessage CannotConvertValue(string type1, string type2)
		{
			return "A value of type '{type1}' cannot be converted to type '{type2}'.".Localize(new { type1, type2 });
		}

		public static IFormattedMessage NoApplicableMethod(string method, string type)
		{
			return "Method '{method}' does not exist in '{type}'.".Localize(new { method, type });
		}

		public static IFormattedMessage MethodsAreInaccessible(string type)
		{
			return "Methods on type '{type}' are not accessible.".Localize(new { type });
		}

		public static IFormattedMessage MethodIsVoid(string method, string type)
		{
			return "Method '{method}' in type '{type}' does not return a value.".Localize(new { method, type });
		}

		public static IFormattedMessage UnknownPropertyOrField(string property, string type)
		{
			return "Property '{property}' does not exist in '{type}'.".Localize(new { property, type });
		}

		public static IFormattedMessage NoApplicableIndexer(string type)
		{
			return "No applicable indexer exists in type '{type}'.".Localize(new { type });
		}

		public static IFormattedMessage AmbiguousIndexerInvocation(string type)
		{
			return "Ambiguous invocation of indexer in type '{type}'.".Localize(new { type });
		}

		public static IFormattedMessage IncompatibleOperand(string @operator, string type)
		{
			return "Operator '{operator}' incompatible with operand type '{type}'.".Localize(new { @operator, type });
		}

		public static IFormattedMessage IncompatibleOperands(string @operator, string type1, string type2)
		{
			return "Operator '{operator}' incompatible with operand types '{type1}' and '{type2}'.".Localize(new { @operator, type1, type2 });
		}

		public static IFormattedMessage InvalidCharacter(char character)
		{
			return "Syntax error '{character}'.".Localize(new { character });
		}

		public static IFormattedMessage SimpleTypeExpected(string type, string @operator, int argumentNumber)
		{
			return "String, number or bool expected for {operator} operator in #{argumentNumber} argument instead of '{type}'.".Localize(new { type, @operator, argumentNumber });
		}
	}
}
