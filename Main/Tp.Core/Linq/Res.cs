using Tp.Core.Annotations;
using Tp.I18n;

// ReSharper disable once CheckNamespace

namespace System.Linq.Dynamic
{
    internal static class Res
    {
        public static IFormattedMessage InvalidCharacterLiteral => "Character literal must contain exactly one character.".Localize();
        public static IFormattedMessage FirstExprMustBeBool => "The first expression must be of type 'Boolean'.".Localize();

        [NotNull]
        public static IFormattedMessage UnknownType(string type) =>
            "Unknown type '{type}'.".Localize(new { type });

        [NotNull]
        public static IFormattedMessage ExpressionTypeMismatch(string type) =>
            "Expression of type '{type}' expected.".Localize(new { type });

        [NotNull]
        public static IFormattedMessage BothTypesConvertToOther(string type1, string type2) =>
            "Both of the types '{type1}' and '{type2}' convert to the other.".Localize(new { type1, type2 });

        [NotNull]
        public static IFormattedMessage NeitherTypeConvertsToOther(string type1, string type2) =>
            "Neither of the types '{type1}' and '{type2}' converts to the other.".Localize(new { type1, type2 });

        [NotNull]
        public static IFormattedMessage NoApplicableMethod(string method, string type) =>
            "Method '{method}' does not exist in '{type}'.".Localize(new { method, type });

        [NotNull]
        public static IFormattedMessage UnknownPropertyOrField(string property, string type) =>
            "Property '{property}' does not exist in '{type}'.".Localize(new { property, type });

        [NotNull]
        public static IFormattedMessage IncompatibleOperand(string @operator, string type) =>
            "Operator '{operator}' incompatible with operand type '{type}'.".Localize(new { @operator, type });

        [NotNull]
        public static IFormattedMessage IncompatibleOperands(string @operator, string type1, string type2) =>
            "Operator '{operator}' incompatible with operand types '{type1}' and '{type2}'.".Localize(new { @operator, type1, type2 });

        [NotNull]
        public static IFormattedMessage InvalidCharacter(char character) =>
            "Syntax error '{character}'.".Localize(new { character });
    }
}
