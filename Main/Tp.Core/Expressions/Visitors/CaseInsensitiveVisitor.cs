using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Tp.Core.Expressions.Visitors
{
    public class CaseInsensitiveVisitor : ExpressionVisitor
    {
        private static readonly MethodInfo _equalsIgnoreCase =
            Reflect.GetMethod(() => string.Equals(default, default, default));

        private static readonly MethodInfo _contains = Reflect<string>.GetMethod(x => x.Contains(default));

        private static readonly MethodInfo _contains2 =
            Reflect<string>.GetMethod(x => x.Contains(default, default));

        private static readonly MethodInfo _startsWith = Reflect<string>.GetMethod(x => x.StartsWith(default));

        private static readonly MethodInfo _startsWith2 =
            Reflect<string>.GetMethod(x => x.StartsWith(default, default));

        private static readonly MethodInfo _endsWith = Reflect<string>.GetMethod(x => x.EndsWith(default));

        private static readonly MethodInfo _endsWith2 =
            Reflect<string>.GetMethod(x => x.EndsWith(default, default));

        private static readonly Expression _stringComparison = Expression.Constant(StringComparison.InvariantCultureIgnoreCase);


        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node.NodeType == ExpressionType.Equal || node.NodeType == ExpressionType.NotEqual)
            {
                if (node.Left.Type == typeof(string) && node.Right.Type == typeof(string))
                {
                    Expression call = Expression.Call(_equalsIgnoreCase, node.Left, node.Right, _stringComparison);
                    if (node.NodeType == ExpressionType.NotEqual)
                    {
                        call = Expression.Not(call);
                    }

                    return call;
                }
            }
            return base.VisitBinary(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            Maybe<Expression> f = Maybe.Nothing;
            if (node.Arguments.Count == 1)
            {
                f = Maybe.Any(
                    () => ReplaceMethod(node, _contains, _contains2),
                    () => ReplaceMethod(node, _startsWith, _startsWith2),
                    () => ReplaceMethod(node, _endsWith, _endsWith2));
            }

            return f.GetOrElse(() => base.VisitMethodCall(node));
        }

        private static Maybe<Expression> ReplaceMethod(MethodCallExpression node, MethodInfo source, MethodInfo target)
        {
            if (node.Method == source)
            {
                if (target.IsStatic)
                {
                    return Expression.Call(target, node.Object, node.Arguments[0], _stringComparison);
                }
                return Expression.Call(node.Object, target, node.Arguments[0], _stringComparison);
            }
            return Maybe.Nothing;
        }
    }
}
