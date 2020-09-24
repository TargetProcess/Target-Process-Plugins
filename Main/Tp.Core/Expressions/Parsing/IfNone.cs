using System;
using System.Linq.Expressions;
using Tp.Core.Linq;
using Tp.I18n;

namespace Tp.Core.Expressions.Parsing
{
    public class IfNone : FunctionInfo
    {
        public IfNone() : base(
            "IFNONE",
            "Returns the first argument if it is not NULL, and the second one otherwise",
            new[]
            {
                new FunctionParameterInfo("testValue", "Value which should be tested for NULL"),
                new FunctionParameterInfo("fallbackValue", "Value returned when the first argument is NULL")
            },
            BuildExpression)
        {
        }

        private static Either<IFormattedMessage, Expression> BuildExpression(FunctionExpressionContext context)
        {
            var args = context.Arguments;

            if (args.Length != 2)
            {
                return Either.CreateLeft<IFormattedMessage, Expression>(Res.IfNoneRequiresTwoArgs);
            }

            var valueExpr = args[0];
            var ifNoneExpr = args[1];

            if (!SharedParserUtils.IsNullableType(valueExpr.Type) && valueExpr.Type != typeof(string))
            {
                return Either.CreateLeft<IFormattedMessage, Expression>(Res.TypeHasNoNullableFormAndIsNotString(valueExpr.Type.Name, "ifnone", 1));
            }

            bool ifNoneExprTypeIsSupported =
                SharedParserUtils.IsNumericType(ifNoneExpr.Type) ||
                ifNoneExpr.Type == typeof(bool) ||
                ifNoneExpr.Type == typeof(string) ||
                ifNoneExpr.Type == typeof(DateTime) ||
                ifNoneExpr.Type == typeof(DateTime?);

            if (!ifNoneExprTypeIsSupported)
            {
                return Either.CreateLeft<IFormattedMessage, Expression>(Res.SimpleTypeExpected(ifNoneExpr.Type.Name, "ifnone", 2));
            }

            var result = SharedParserUtils.EqualizeTypesAndCombine(context.Literals, valueExpr, ifNoneExpr, context.ErrorPosition, Expression.Coalesce);
            return Either.CreateRight<IFormattedMessage, Expression>(result);
        }
    }
}
