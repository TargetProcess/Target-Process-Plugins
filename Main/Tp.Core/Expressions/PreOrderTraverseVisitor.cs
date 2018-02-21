using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Tp.Core.Expressions
{
    /// <summary>
    /// Provides an <see cref="IEnumerable{T}"/> created by walking through an expression
    /// tree.
    /// </summary>
    internal sealed class PreOrderTraverseVisitor : ExpressionVisitor
    {
        private List<Expression> _children;

        public override Expression Visit(Expression node)
        {
            if (node != null)
            {
                _children.Add(node);
            }
            return node;
        }

        internal IEnumerable<Expression> Traverse(Expression node)
        {
            _children = new List<Expression>();

            yield return node;
            base.Visit(node);

            var nestedVisitor = new PreOrderTraverseVisitor();
            foreach (var descendant in _children.SelectMany(nestedVisitor.Traverse))
            {
                yield return descendant;
            }
        }

        protected override Expression VisitExtension(Expression node)
        {
            return node;
        }
    }
}
