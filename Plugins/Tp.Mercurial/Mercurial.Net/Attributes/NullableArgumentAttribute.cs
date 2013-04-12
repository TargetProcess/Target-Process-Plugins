using System;

namespace Mercurial.Attributes
{
    /// <summary>
    /// This attribute can be applied to nullable properties in option classes,
    /// to specify the option to pass to the executable in case
    /// of a <c>null</c> value, or the option to pass before the property
    /// value itself.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class NullableArgumentAttribute : ArgumentAttribute
    {
        /// <summary>
        /// Gets or sets the option to pass to the Mercurial executable if the
        /// property value is <c>null</c>. If <see cref="string.Empty"/>,
        /// no option will be passed in this case.
        /// </summary>
        public string NullOption
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the option to specify before the property value when
        /// passing it to the Mercurial executable. If <see cref="string.Empty"/>,
        /// only the property value itself will be passed.
        /// </summary>
        public string NonNullOption
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a collection of options or arguments to pass to the Mercurial
        /// executable, based on the property value from the options class.
        /// </summary>
        /// <param name="propertyValue">
        /// The property value from the tagged property of the options class.
        /// </param>
        /// <returns>
        /// A collection of options or arguments, or an empty array or <c>null</c>
        /// for no options for the specified property value.
        /// </returns>
        public override string[] GetOptions(object propertyValue)
        {
            string result;
            if (propertyValue == null)
                result = null;
            else
                result = propertyValue.ToString().Trim();

            if (StringEx.IsNullOrWhiteSpace(result))
            {
                if (StringEx.IsNullOrWhiteSpace(NullOption))
                    return null;
                return new[]
                {
                    NullOption
                };
            }

            if (!StringEx.IsNullOrWhiteSpace(NonNullOption))
                return new[]
                {
                    NonNullOption, "\"" + result + "\""
                };

            return new[]
            {
                "\"" + result + "\""
            };
        }
    }
}