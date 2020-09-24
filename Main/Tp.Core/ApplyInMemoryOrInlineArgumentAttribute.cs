// ReSharper disable once CheckNamespace

namespace System
{
    /// <summary>
    /// This attribute allows to mark method as behaving differently when used for report or .net expression tree generation.
    /// In case method is used in .net expression tree, we use method's body (apply in memory part), and inline single method
    /// argument in report expression tree otherwise. Also inlining works ONLY in free form filters, as
    /// <see cref="SqlFunctionAttribute"/>, <see cref="SqlDateFunctionAttribute"/>, etc.
    ///
    /// Restrictions on marked method signature are:
    /// 1. Method should have single parameter. Otherwise we can't find which one to inline for report.
    /// 2. Method parameter and return value should have same types. Otherwise report and .net expression trees can be splitted
    /// by return type and do not match each other.
    ///
    /// Both restrictions are checked in runtime.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class ApplyInMemoryOrInlineArgumentAttribute : Attribute
    {
    }
}
