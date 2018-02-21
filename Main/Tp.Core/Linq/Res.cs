using Tp.Core.Annotations;
using Tp.I18n;

// ReSharper disable once CheckNamespace

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

        [NotNull]
        public static IFormattedMessage UnknownType(string type) =>
            "Unknown type '{type}'.".Localize(new { type });

        [NotNull]
        public static IFormattedMessage DuplicateIdentifier(string identifier) =>
            "The identifier '{identifier}' was defined more than once.".Localize(new { identifier });

        [NotNull]
        public static IFormattedMessage ExpressionTypeMismatch(string type) =>
            "Expression of type '{type}' expected.".Localize(new { type });

        [NotNull]
        public static IFormattedMessage InvalidIntegerLiteral(string literal) =>
            "Invalid integer literal '{literal}'.".Localize(new { literal });

        [NotNull]
        public static IFormattedMessage InvalidRealLiteral(string literal) =>
            "Invalid real literal '{literal}'.".Localize(new { literal });

        [NotNull]
        public static IFormattedMessage UnknownIdentifier(string identifier) =>
            "Unknown identifier '{identifier}'.".Localize(new { identifier });

        [NotNull]
        public static IFormattedMessage BothTypesConvertToOther(string type1, string type2) =>
            "Both of the types '{type1}' and '{type2}' convert to the other.".Localize(new { type1, type2 });

        [NotNull]
        public static IFormattedMessage NeitherTypeConvertsToOther(string type1, string type2) =>
            "Neither of the types '{type1}' and '{type2}' converts to the other.".Localize(new { type1, type2 });

        [NotNull]
        public static IFormattedMessage TypeHasNoNullableForm(string type) =>
            "Type '{type}' has no nullable form.".Localize(new { type });

        [NotNull]
        public static IFormattedMessage TypeHasNoNullableFormAndIsNotString(string type, string @operator, int argumentNumber) =>
            "{operator} operator works only with nullable numbers, bools, dates and strings for #{argumentNumber} argument. Actual type:'{type}'."
                .Localize(new { type, @operator, argumentNumber });

        [NotNull]
        public static IFormattedMessage NoMatchingConstructor(string type) =>
            "No matching constructor in type '{type}'.".Localize(new { type });

        [NotNull]
        public static IFormattedMessage AmbiguousConstructorInvocation(string type) =>
            "Ambiguous invocation of '{type}' constructor.".Localize(new { type });

        [NotNull]
        public static IFormattedMessage CannotConvertValue(string type1, string type2) =>
            "A value of type '{type1}' cannot be converted to type '{type2}'.".Localize(new { type1, type2 });

        [NotNull]
        public static IFormattedMessage NoApplicableMethod(string method, string type) =>
            "Method '{method}' does not exist in '{type}'.".Localize(new { method, type });

        [NotNull]
        public static IFormattedMessage MethodsAreInaccessible(string type) =>
            "Methods on type '{type}' are not accessible.".Localize(new { type });

        [NotNull]
        public static IFormattedMessage MethodIsVoid(string method, string type) =>
            "Method '{method}' in type '{type}' does not return a value.".Localize(new { method, type });

        [NotNull]
        public static IFormattedMessage UnknownPropertyOrField(string property, string type) =>
            "Property '{property}' does not exist in '{type}'.".Localize(new { property, type });

        [NotNull]
        public static IFormattedMessage NoApplicableIndexer(string type) =>
            "No applicable indexer exists in type '{type}'.".Localize(new { type });

        [NotNull]
        public static IFormattedMessage AmbiguousIndexerInvocation(string type) =>
            "Ambiguous invocation of indexer in type '{type}'.".Localize(new { type });

        [NotNull]
        public static IFormattedMessage IncompatibleOperand(string @operator, string type) =>
            "Operator '{operator}' incompatible with operand type '{type}'.".Localize(new { @operator, type });

        [NotNull]
        public static IFormattedMessage IncompatibleOperands(string @operator, string type1, string type2) =>
            "Operator '{operator}' incompatible with operand types '{type1}' and '{type2}'.".Localize(new { @operator, type1, type2 });

        [NotNull]
        public static IFormattedMessage InvalidCharacter(char character) =>
            "Syntax error '{character}'.".Localize(new { character });

        [NotNull]
        public static IFormattedMessage SimpleTypeExpected(string type, string @operator, int argumentNumber) =>
            "String, number, bool or date expected for {operator} operator in #{argumentNumber} argument instead of '{type}'.".Localize(
                new { type, @operator, argumentNumber });
    }
}
