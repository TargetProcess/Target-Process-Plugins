using System;
using System.Linq;
using System.Linq.Expressions;

namespace Tp.Core.Expressions.Visitors
{
    /// <summary>
    /// Removes all type checks of targetExpression to types other than specified in possibleTypes, or their base and derived types
    /// </summary>
    public class RemoveImpossibleTypeChecks : ExpressionVisitor
    {
        private readonly Expression _targetExpression;
        private readonly Type[] _possibleTypes;

        public RemoveImpossibleTypeChecks(Expression targetExpression, params Type[] possibleTypes)
        {
            if (possibleTypes.Empty())
            {
                throw new ArgumentException("Empty collection", nameof(possibleTypes));
            }
            _targetExpression = targetExpression;
            _possibleTypes = possibleTypes;
        }

        protected override Expression VisitConditional(ConditionalExpression node)
        {
            if (node.Test is TypeBinaryExpression binaryExpression && binaryExpression.NodeType == ExpressionType.TypeIs)
            {
                if (binaryExpression.Expression == _targetExpression &&
                    !_possibleTypes.Any(t => TypeHelper.IsDescendantOrSame(binaryExpression.TypeOperand, t)) &&
                    !_possibleTypes.Any(t => TypeHelper.IsDescendantOrSame(t, binaryExpression.TypeOperand)))
                {
                    return Visit(node.IfFalse);
                }
            }
            return base.VisitConditional(node);
        }
    }
}
