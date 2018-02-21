using System;

namespace Tp.Core.Expressions
{
    /// <summary>
    /// <remarks>
    /// When put on a method requires the presense of that's method overload with the same params wrapped into Expression&lt;Func&lt;,&gt;&gt;.
    /// For example:
    /// <code>
    /// [Inlineable]
    /// public static TOut Method1(this TIn item)
    /// {
    ///     return Method1().Apply(item);
    /// }
    ///
    /// public static Expression&lt;Func&lt;TIn, TOut&gt;&gt; Method1()
    /// {
    ///     //return expression here
    /// }
    /// </code>
    /// </remarks>
    /// </summary>
    public class InlineableAttribute : Attribute
    {
        public InlineableAttribute(string inlineMethodName = null)
        {
            InlineMethodName = inlineMethodName;
        }

        public string InlineMethodName { get; }
    }
}
