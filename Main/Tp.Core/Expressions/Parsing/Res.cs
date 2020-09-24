using System;
using Tp.Core.Annotations;
using Tp.I18n;

namespace Tp.Core.Expressions.Parsing
{
    internal static class Res
    {
        public static IFormattedMessage IifRequiresThreeArgs => "The 'iif' function requires three arguments.".Localize();
        public static IFormattedMessage IfNoneRequiresTwoArgs => "The 'ifnone' function requires two arguments.".Localize();
        public static IFormattedMessage FirstExprMustBeBool => "The first expression must be of type 'Boolean'.".Localize();
        public static IFormattedMessage MissingAsClause => "Expression is missing an 'as' clause.".Localize();
        public static IFormattedMessage CannotIndexMultiDimArray => "Indexing of multi-dimensional arrays is not supported.".Localize();
        public static IFormattedMessage InvalidIndex => "Array index must be an integer expression.".Localize();

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
        public static IFormattedMessage TypeHasNoNullableFormAndIsNotString(string type, string @operator, int argumentNumber) =>
            "{operator} operator works only with nullable numbers, bools, dates and strings for #{argumentNumber} argument. Actual type:'{type}'."
                .Localize(new { type, @operator, argumentNumber });

        [NotNull]
        public static IFormattedMessage MethodIsVoid(string method, string type) =>
            "Method '{method}' in type '{type}' does not return a value.".Localize(new { method, type });

        [NotNull]
        public static IFormattedMessage NoApplicableIndexer(string type) =>
            "No applicable indexer exists in type '{type}'.".Localize(new { type });

        [NotNull]
        public static IFormattedMessage SimpleTypeExpected(string type, string @operator, int argumentNumber) =>
            "String, number, bool or date expected for {operator} operator in #{argumentNumber} argument instead of '{type}'.".Localize(
                new { type, @operator, argumentNumber });
    }
}
