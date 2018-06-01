using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Tp.Core.Annotations;
using Tp.Core.Features;

namespace Tp.Core.Expressions.Visitors
{
    internal class ProtectFromNullReferenceVisitor : ExpressionVisitor
    {
        private readonly ISet<Expression> _notNullExpressions;
        private static readonly Type _nullableOpenType = typeof(Nullable<>);

        public ProtectFromNullReferenceVisitor(ISet<Expression> notNullExpressions = null)
        {
            _notNullExpressions = notNullExpressions;
        }

        protected override Expression VisitMember(MemberExpression memberExpression)
        {
            if (memberExpression.Expression != null && CanBeNull(memberExpression.Expression))
            {
                return ProtectFromNull(memberExpression, memberExpression.Expression);
            }
            return base.VisitMember(memberExpression);
        }

        protected override Expression VisitIndex(IndexExpression node)
        {
            return ProtectFromNull(node, node.Object);
        }

        private static readonly MethodInfo _toStringMethod = Reflect<object>.GetMethod(o => o.ToString());
        private bool CanBeNull(Expression expression)
        {
            if (expression == null)
            {
                return true;
            }

            if (_notNullExpressions?.Contains(expression) == true)
            {
                return false;
            }
            
            return expression
                .Match(true)
                .Case<MethodCallExpression>(c => !(c.Method.GetCustomAttribute<NotNullAttribute>().HasValue || c.Method.GetBaseDefinition() == _toStringMethod))
                .Case<MemberExpression>(a => !a.Member.GetCustomAttribute<NotNullAttribute>().HasValue)
                .End(true);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (!CanBeNull(node.Object) || !ShouldProtectMethodCall(node))
            {
                return base.VisitMethodCall(node);
            }

            var args = node.Arguments.Select(a => a.ProtectFromNullReference());
            var newNode = Expression.Call(node.Object, node.Method, args);
            return ProtectFromNull(newNode, newNode.Object);
        }

        private static bool ShouldProtectMethodCall(MethodCallExpression node)
        {
            var method = node.Method;

            if (method.IsStatic)
            {
                // This skips protection for extension methods as well, because they may be called on null values and can have logic for them. 
                // E.g. StringExtensions.Contains() protects from nulls inside itself.
                return false;
            }

            if (node.Object?.NodeType == ExpressionType.Constant)
            {
                // This removes redundant null checks in EnumerableProjector.Project etc
                return false;
            }

            return true;
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.NodeType == ExpressionType.Convert && node.Type.IsValueType && !node.Operand.Type.IsValueType)
            {
                return ProtectFromNull(node, node.Operand);
            }

            return base.VisitUnary(node);
        }

        private Expression ProtectFromNull(Expression memberExpression, Expression target)
        {
            var targetType = target.Type;

            Expression condition;

            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == _nullableOpenType)
            {
                // Nullable<> is a value type, but we still need to compare it with null using regular equality (not reference equality)
                condition = Expression.Equal(Visit(target), Expression.Constant(null, targetType));
            }
            else if (targetType.IsValueType)
            {
                // Non-nullable value types can't be null, so no protection is required
                return memberExpression;
            }
            else
            {
                // Reference types should should use reference equality comparison
                condition = Expression.ReferenceEqual(Visit(target), Expression.Constant(null, target.Type));
            }

            var @true = Expression.Constant(memberExpression.Type.DefaultValue(), memberExpression.Type);
            var @false = memberExpression;
            return Expression.Condition(condition, @true, @false);
        }
    }
}
