using System.Linq.Expressions;
using Tp.Core.Linq;
using Tp.I18n;

namespace Tp.Core.Expressions.Parsing
{
    public class Iif : FunctionInfo
    {
        public Iif()
        : base (
            "IIF",
            "Checks the value of the first argument and returns the second argument if it's TRUE and third one if it's FALSE",
            new[]
            {
                new FunctionParameterInfo("condition", "Boolean value which should be tested"),
                new FunctionParameterInfo("trueValue", "Returned when condition is true"),
                new FunctionParameterInfo("falseValue", "Returned when condition is false")
            },
            BuildExpression)
        {
        }

        private static Either<IFormattedMessage, Expression> BuildExpression(FunctionExpressionContext context)
        {
            var args = context.Arguments;

            if (args.Length != 3)
            {
                return Either.CreateLeft<IFormattedMessage, Expression>(Res.IifRequiresThreeArgs);
            }

            var test = args[0];
            var expr1 = args[1];
            var expr2 = args[2];

            test = SharedParserUtils.ConvertNullableBoolToBoolExpression(test);
            if (test.Type != typeof(bool))
            {
                return Either.CreateLeft<IFormattedMessage, Expression>(Res.FirstExprMustBeBool);
            }

            var resultExpr = SharedParserUtils.EqualizeTypesAndCombine(
                context.Literals, expr1, expr2, context.ErrorPosition, (e1, e2) => Expression.Condition(test, e1, e2));
            return Either.CreateRight<IFormattedMessage, Expression>(resultExpr);
        }
    }
}
