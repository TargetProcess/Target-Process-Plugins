using System;

namespace Mercurial
{
    /// <summary>
    /// This class encapsulates a single line of configuration information from the
    /// <see cref="ShowConfigCommand"/>.
    /// </summary>
    public class ConfigurationEntry
    {
        /// <summary>
        /// This is the backing field for the <see cref="Name"/> property.
        /// </summary>
        private readonly string _Name;

        /// <summary>
        /// This is the backing field for the <see cref="Section"/> property.
        /// </summary>
        private readonly string _Section;

        /// <summary>
        /// This is the backing field for the <see cref="Value"/> property.
        /// </summary>
        private readonly string _Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationEntry"/> class.
        /// </summary>
        /// <param name="section">
        /// The section of the configuration entry, used to group configuration entries together.
        /// </param>
        /// <param name="name">
        /// The name of the configuration entry.
        /// </param>
        /// <param name="value">
        /// The value of the configuration entry.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="section"/> is <c>null</c> or empty.</para>
        /// <para>- or -</para>
        /// <para><paramref name="name"/> is <c>null</c> or empty.</para>
        /// <para>- or -</para>
        /// <para><paramref name="value"/> is <c>null</c>.</para>
        /// </exception>
        public ConfigurationEntry(string section, string name, string value)
        {
            if (StringEx.IsNullOrWhiteSpace(section))
                throw new ArgumentNullException("section");
            if (StringEx.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");
            if (value == null)
                throw new ArgumentNullException("value");

            _Section = section;
            _Name = name;
            _Value = value;
        }

        /// <summary>
        /// Gets the section of the configuration entry, used to group configuration entries together.
        /// </summary>
        public string Section
        {
            get
            {
                return _Section;
            }
        }

        /// <summary>
        /// Gets the name of the configuration entry.
        /// </summary>
        public string Name
        {
            get
            {
                return _Name;
            }
        }

        /// <summary>
        /// Gets the value of the configuration entry.
        /// </summary>
        public string Value
        {
            get
            {
                return _Value;
            }
        }
    }
}