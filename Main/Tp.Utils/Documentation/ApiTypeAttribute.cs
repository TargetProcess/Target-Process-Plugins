using System;
using Tp.Core.Annotations;

namespace Tp.Utils.Documentation
{
    /// <summary>
    /// Declares that an element should have a custom type in API documention.
    /// </summary>
    [AttributeUsage(
         AttributeTargets.ReturnValue |
         AttributeTargets.Parameter |
         AttributeTargets.Class |
         AttributeTargets.Interface)]
    public class ApiTypeAttribute : Attribute
    {
        public ApiTypeAttribute([NotNull] Type valueType)
        {
            if (valueType == null)
            {
                throw new ArgumentNullException(nameof(valueType));
            }

            Type = valueType;
        }

        [NotNull]
        public Type Type { get; }
    }
}
