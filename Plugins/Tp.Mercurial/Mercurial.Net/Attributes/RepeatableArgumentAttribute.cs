using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mercurial.Attributes
{
    /// <summary>
    /// This attribute can be applied to collection-properties in option classes,
    /// to specify the option to prefix each element in the collection with,
    /// when building the command line arguments for the Mercurial executable.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class RepeatableArgumentAttribute : ArgumentAttribute
    {
        /// <summary>
        /// This is the backing field for the <see cref="Option"/> property.
        /// </summary>
        private string _Option;

        /// <summary>
        /// Gets or sets the option prefix to use for each element in the collection.
        /// </summary>
        public string Option
        {
            get
            {
                return _Option;
            }

            set
            {
                _Option = (value ?? string.Empty).Trim();
            }
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
            var list = propertyValue as IList;
            if (list != null)
            {
                var result = new List<string>();
                foreach (object element in list.Cast<object>().Where(element => element != null))
                {
                    result.Add(Option);
                    result.Add("\"" + element + "\"");
                }
                return result.ToArray();
            }
            return null;
        }
    }
}