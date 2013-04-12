using System;

namespace Mercurial.Attributes
{
    /// <summary>
    /// This attribute can be applied to properties of command classes to provide
    /// the Mercurial executable with options, based on the properties of
    /// the command class.
    /// </summary>
    public abstract class ArgumentAttribute : Attribute
    {
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
        public abstract string[] GetOptions(object propertyValue);
    }
}