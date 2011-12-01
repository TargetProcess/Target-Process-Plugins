#region

using System;

#endregion

namespace Tp.Integration.Common
{
    /// <summary>
    /// It is marker for primary key attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    [Serializable]
    public class PrimaryKeyAttribute : Attribute
    {
    }
}