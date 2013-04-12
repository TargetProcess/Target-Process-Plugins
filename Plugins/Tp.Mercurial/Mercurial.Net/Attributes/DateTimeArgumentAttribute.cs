using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Mercurial.Attributes
{
    /// <summary>
    /// This attribute can be applied to nullable properties in option classes,
    /// to specify the option to pass to the executable in case
    /// of a <c>null</c> value, or the option to pass before the property
    /// value itself.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class DateTimeArgumentAttribute : ArgumentAttribute
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
        /// Gets or sets the format to use for the <see cref="DateTime"/> property value.
        /// If <c>null</c> or empty string, use the default format. Default is <c>null</c>.
        /// </summary>
        public string Format
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
        /// <exception cref="InvalidOperationException">
        /// <see cref="DateTimeArgumentAttribute"/> applied to non-<see cref="DateTime"/> property.
        /// </exception>
        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "DateTime? is a value-type, can't do as the rule suggests")]
        public override string[] GetOptions(object propertyValue)
        {
            if (propertyValue == null)
            {
                if (StringEx.IsNullOrWhiteSpace(NullOption))
                    return null;
                return new[]
                {
                    NullOption
                };
            }

            string format = Format;
            if (StringEx.IsNullOrWhiteSpace(format))
                format = "yyyy-MM-dd HH:mm:ss";
            if (!(propertyValue is DateTime) && !(propertyValue is DateTime?))
                throw new InvalidOperationException("DateTimeArgumentAttribute applied to non-DateTime property");

            if (propertyValue is DateTime?)
                propertyValue = ((DateTime?)propertyValue).Value;
            string result = string.Format(
                CultureInfo.InvariantCulture, "\"{0}\"", ((DateTime)propertyValue).ToString(format, CultureInfo.InvariantCulture));

            if (StringEx.IsNullOrWhiteSpace(NonNullOption))
                return new[]
                {
                    result
                };

            return new[]
            {
                NonNullOption, result
            };
        }
    }
}