using System.Linq.Expressions;

namespace Tp.Core.Expressions.Visitors
{
    class BooleanEvaluator : ExpressionVisitor
    {
        protected override Expression VisitConditional(ConditionalExpression node)
        {
            var newExpr = base.VisitConditional(node);
            if (newExpr.NodeType == ExpressionType.Conditional)
            {
                var cond = (ConditionalExpression) newExpr;
                if (cond.Test.NodeType == ExpressionType.Constant)
                {
                    var testResult = (bool) ((ConstantExpression) cond.Test).Value;
                    return testResult ? cond.IfTrue : cond.IfFalse;
                }
            }
            return newExpr;
        }

        protected override Expression VisitExtension(Expression node)
        {
            return node;
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            var visited = base.VisitBinary(node);
            switch (visited.NodeType)
            {
                case ExpressionType.OrElse:
                    return VisitBoolBinary(true, (BinaryExpression) visited);
                case ExpressionType.AndAlso:
                    return VisitBoolBinary(false, (BinaryExpression) visited);
                default:
                    return visited;
            }
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            var visited = base.VisitUnary(node);
            if (visited.NodeType == ExpressionType.Not)
            {
                var unary = (UnaryExpression) (visited);
                if (unary.Operand.NodeType == ExpressionType.Constant)
                {
                    var boolValue = (bool) ((ConstantExpression) unary.Operand).Value;
                    return boolValue ? Expression.Constant(false) : Expression.Constant(true);
                }
            }
            return visited;
        }

        private static Expression VisitBoolBinary(bool zero, BinaryExpression binary)
        {
            var booleanConstant = binary.Left as ConstantExpression ?? binary.Right as ConstantExpression;
            if (booleanConstant != null)
            {
                var boolValue = (bool) booleanConstant.Value;
                if (boolValue == zero)
                {
                    return Expression.Constant(zero);
                }
                if (binary.Left.NodeType == ExpressionType.Constant)
                {
                    return binary.Right;
                }
                return binary.Left;
            }
            return binary;
        }
    }
}
