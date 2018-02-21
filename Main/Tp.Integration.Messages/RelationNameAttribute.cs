using System;

namespace Tp.Integration.Common
{
    /// <summary>
    /// It is a marker for DTO relation name properties. Only for system usage.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    [Serializable]
    public class RelationNameAttribute : Attribute
    {
    }
}
