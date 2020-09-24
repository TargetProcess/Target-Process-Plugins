using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;

namespace Tp.Core.Expressions.Parsing
{
    public class BuildOrderingVisitor : Antlr.ExpressionParserBaseVisitor<IReadOnlyList<DynamicOrdering>>
    {
        private readonly Antlr.ExpressionParserBaseVisitor<Expression> _queryVisitor;

        public BuildOrderingVisitor(
            Antlr.ExpressionParserBaseVisitor<Expression> queryVisitor)
        {
            _queryVisitor = queryVisitor;
        }

        public override IReadOnlyList<DynamicOrdering> VisitOrderingProgram(Antlr.ExpressionParser.OrderingProgramContext context)
        {
            return context.ordering()
                .Select(orderingContext =>
                {
                    var ascending = orderingContext.DESC() == null;
                    var selector = _queryVisitor.VisitExpressionContainer(orderingContext.selector);
                    return new DynamicOrdering(ascending, selector);
                }).ToReadOnlyCollection();
        }
    }
}
