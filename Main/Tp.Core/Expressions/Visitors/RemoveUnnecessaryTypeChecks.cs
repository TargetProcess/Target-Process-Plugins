using System;
using System.Linq.Expressions;

namespace Tp.Core.Expressions.Visitors
{
    /// <summary>
    /// Removes `x is T` check from ternary operator of form `x is T ? : a ? b` when it's known that x inherits from T
    /// </summary>
    public class RemoveUnnecessaryTypeIsCheck : ExpressionVisitor
    {
        protected override Expression VisitConditional(ConditionalExpression node)
        {
            if (node.Test is TypeBinaryExpression binaryExpression && binaryExpression.NodeType == ExpressionType.TypeIs)
            {
                if (binaryExpression.Expression.Type.IsDescendantOrSame(binaryExpression.TypeOperand))
                {
                    return Visit(node.IfTrue);
                }
            }
            return base.VisitConditional(node);
        }
    }

    /// <summary>
    /// Removes `(T)x` when it's known that x is exactly of type T
    /// </summary>
    public class RemoveUnnecessaryDirectCast : ExpressionVisitor
    {
        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.NodeType == ExpressionType.Convert && node.Operand.Type == node.Type)
            {
                return Visit(node.Operand);
            }
            return base.VisitUnary(node);
        }        
    }

    /// <summary>
    /// Removes cast from `((T)x).y` when it's known that x inherits from T
    /// </summary>
    public class RemoveUnnecessaryCastFromPropertyAccess : ExpressionVisitor
    {
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression is UnaryExpression unary && unary.NodeType == ExpressionType.Convert && unary.Operand.Type.IsDescendantOrSame(unary.Type))
            {
                var operand = Visit(unary.Operand);
                return Expression.MakeMemberAccess(operand, node.Member);
            }
            return base.VisitMember(node);
        }
    }

    /// <summary>
    /// Removes cast from `(T)x == null` when x has a reference type
    /// </summary>
    public class RemoveUnnecessaryCastFromNullCheck : ExpressionVisitor
    {
        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node.NodeType == ExpressionType.Equal || node.NodeType == ExpressionType.NotEqual)
            {
                if (node.Right.IsConstantNull())
                {
                    if (node.Left is UnaryExpression unary && unary.NodeType == ExpressionType.Convert && !unary.Operand.Type.IsValueType)
                    {
                        return Expression.MakeBinary(node.NodeType, unary.Operand, Expression.Constant(null, unary.Operand.Type));
                    }
                }
                if (node.Left.IsConstantNull())
                {
                    if (node.Right is UnaryExpression unary && unary.NodeType == ExpressionType.Convert && !unary.Operand.Type.IsValueType)
                    {
                        return Expression.MakeBinary(node.NodeType, Expression.Constant(null, unary.Operand.Type), unary.Operand);
                    }
                }
            }
            return base.VisitBinary(node);
        }
    }    
}
