using System;

namespace Tp.Utils.Documentation
{
    [AttributeUsage(AttributeTargets.All)]
    public class ApiDescriptionAttribute : Attribute
    {
        public ApiDescriptionAttribute(string description)
        {
            Description = description;
        }

        public string Description { get; }
    }
}
