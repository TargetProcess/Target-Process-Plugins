using System.Linq.Expressions;
using Tp.Core;
using Tp.Core.Annotations;

namespace System.Linq.Dynamic
{
    public class DynamicOrdering
    {
        public DynamicOrdering(bool ascending, [NotNull] Expression selector)
        {
            Ascending = ascending;
            Selector = Argument.NotNull(nameof(selector), selector);
        }

        public bool Ascending { get; }
        public Expression Selector { get; }
    }
}
