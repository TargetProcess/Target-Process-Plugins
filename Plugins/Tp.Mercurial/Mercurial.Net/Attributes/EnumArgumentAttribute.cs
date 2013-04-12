using System;
using System.Globalization;

namespace Mercurial.Attributes
{
    /// <summary>
    /// This attribute can be applied to enum-properties in option classes,
    /// to specify the arguments to pass to the executable in case of
    /// each possible enum value for the property. Note that usage of any enum values that
    /// don't have a corresponding <see cref="EnumArgumentAttribute"/> will be ignored.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class EnumArgumentAttribute : ArgumentAttribute
    {
        /// <summary>
        /// This is the backing field for the <see cref="Argument1"/> and <see cref="Argument2"/> properties.
        /// </summary>
        private readonly string[] _Arguments = new string[0];

        /// <summary>
        /// This is the backing field for the <see cref="Value"/> property.
        /// </summary>
        private readonly object _Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumArgumentAttribute"/> class
        /// with one argument for the enum value.
        /// </summary>
        /// <param name="value">The enum value.</param>
        /// <param name="argument1">The argument.</param>
        public EnumArgumentAttribute(object value, string argument1)
        {
            _Value = value;
            _Arguments = new[]
            {
                argument1
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumArgumentAttribute"/> class
        /// with two arguments for the enum value.
        /// </summary>
        /// <param name="value">The enum value.</param>
        /// <param name="argument1">The first argument.</param>
        /// <param name="argument2">The second argument.</param>
        public EnumArgumentAttribute(object value, string argument1, string argument2)
        {
            _Value = value;
            _Arguments = new[]
            {
                argument1, argument2
            };
        }

        /// <summary>
        /// Gets the enum value to map the arguments to.
        /// </summary>
        public object Value
        {
            get
            {
                return _Value;
            }
        }

        /// <summary>
        /// Gets the first argument that the <see cref="Value"/> is mapped to,
        /// or <see cref="string.Empty"/> if there is no argument.
        /// </summary>
        public string Argument1
        {
            get
            {
                if (_Arguments != null && _Arguments.Length >= 1)
                    return _Arguments[0];

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the second argument that the <see cref="Value"/> is mapped to,
        /// or <see cref="string.Empty"/> if there is no second argument.
        /// </summary>
        public string Argument2
        {
            get
            {
                if (_Arguments != null && _Arguments.Length >= 2)
                    return _Arguments[1];

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the enum value is a bitmask value (see <see cref="FlagsAttribute"/>)
        /// or not.
        /// </summary>
        public bool IsBitmask
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a collection of options or arguments to pass to the Mercurial
        /// executable, based on the property value from the command class.
        /// </summary>
        /// <param name="propertyValue">
        /// The property value from the tagged property of the command class.
        /// </param>
        /// <returns>
        /// A collection of options or arguments, or an empty array or <c>null</c>
        /// for no options for the specified property value.
        /// </returns>
        public override string[] GetOptions(object propertyValue)
        {
            int propertyValueAsNumber = Convert.ToInt32(propertyValue, CultureInfo.InvariantCulture);
            int valueAsNumber = Convert.ToInt32(_Value, CultureInfo.InvariantCulture);

            if (IsBitmask)
            {
                if ((propertyValueAsNumber & valueAsNumber) == valueAsNumber)
                    return _Arguments;
            }
            else
            {
                if (propertyValueAsNumber == valueAsNumber)
                    return _Arguments;
            }

            return null;
        }
    }
}