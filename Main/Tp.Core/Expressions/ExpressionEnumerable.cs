using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Tp.Core.Expressions
{
    internal sealed class ExpressionEnumerable : ExpressionVisitor
    {
        private ExpressionEnumerable()
        {
        }

        private readonly List<Expression> _children = new List<Expression>();

        public override Expression Visit(Expression node)
        {
            _children.Add(node);
            return node;
        }

        public static IEnumerable<Expression> All(Expression node)
        {
            var visitor = new ExpressionEnumerable();
            return visitor.AllInternal(node);
        }

        internal IEnumerable<Expression> AllInternal(Expression node)
        {
            yield return node;
            base.Visit(node);

            var nestedVisitor = new ExpressionEnumerable();
            foreach (var descendant in from child in _children
                from descendants in nestedVisitor.AllInternal(child)
                select descendants)
            {
                yield return descendant;
            }
        }
    }
}
