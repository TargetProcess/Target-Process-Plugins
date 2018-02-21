using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Tp.Testing.Common.NUnit
{
    /// <summary>
    /// The Have class is a synonym for Has intended for use with the Should extension methods for more DSL-like syntax
    /// </summary>
    public class Have : Has
    {
        public static ResolvableConstraintExpression Value => new ConstraintExpression().Value();
    }
}
