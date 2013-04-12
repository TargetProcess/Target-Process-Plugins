using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mercurial.Configuration
{
    /// <summary>
    /// This class encapsulates the Mercurial command line client configuration information.
    /// </summary>
    public sealed class ClientConfigurationCollection : IEnumerable<ConfigurationEntry>
    {
        /// <summary>
        /// This is the backing field for this <see cref="ClientConfigurationCollection"/>.
        /// </summary>
        private readonly Dictionary<string, Dictionary<string, string>> _Configuration = new Dictionary<string, Dictionary<string, string>>();

        /// <summary>
        /// Gets a collection of section names from the configuration.
        /// </summary>
        public IEnumerable<string> Sections
        {
            get
            {
                lock (_Configuration)
                    return _Configuration.Keys.ToArray();
            }
        }

        #region IEnumerable<ConfigurationEntry> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<ConfigurationEntry> GetEnumerator()
        {
            ConfigurationEntry[] result;
            lock (_Configuration)
            {
                result = (from kvp1 in _Configuration
                          from kvp2 in kvp1.Value
                          select new ConfigurationEntry(kvp1.Key, kvp2.Key, kvp2.Value)).ToArray();
            }
            return ((IEnumerable<ConfigurationEntry>)result).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Gets the value of the specified configuration entry from
        /// the specified section.
        /// </summary>
        /// <param name="sectionName">
        /// The name of the section to get the value of a configuration entry from.
        /// </param>
        /// <param name="name">
        /// The name of the configuration entry to retrieve the value for.
        /// </param>
        /// <returns>
        /// The value of the configuration entry, or <see cref="string.Empty"/>
        /// if no such value exists.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="sectionName"/> is <c>null</c> or empty.</para>
        /// <para>- or -</para>
        /// <para><paramref name="name"/> is <c>null</c> or empty.</para>
        /// </exception>
        public string GetValue(string sectionName, string name)
        {
            if (StringEx.IsNullOrWhiteSpace(sectionName))
                throw new ArgumentNullException("sectionName");
            if (StringEx.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            Dictionary<string, string> section;
            lock (_Configuration)
            {
                if (_Configuration.TryGetValue(sectionName, out section))
                {
                    string value;
                    if (section.TryGetValue(name, out value))
                        return value;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Refreshes the client configuration information by calling the Mercurial command line
        /// client and asking it to report the current configuration.
        /// </summary>
        public void Refresh()
        {
            lock (_Configuration)
            {
                _Configuration.Clear();
                var command = new ShowConfigCommand();
                NonPersistentClient.Execute(command);

                foreach (ConfigurationEntry entry in command.Result.ToArray())
                {
                    Dictionary<string, string> section;
                    if (!_Configuration.TryGetValue(entry.Section, out section))
                    {
                        section = new Dictionary<string, string>();
                        _Configuration[entry.Section] = section;
                    }
                    section[entry.Name] = entry.Value;
                }
            }
        }

        /// <summary>
        /// Gets a collection of configuration entry names for the given
        /// section, or an empty collection if no such section exists.
        /// </summary>
        /// <param name="sectionName">
        /// The name of the section to get configuration entry names for.
        /// </param>
        /// <returns>
        /// A collection of configuration entry names for the given section.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="sectionName"/> is <c>null</c> or empty.</para>
        /// </exception>
        public IEnumerable<string> GetNamesForSection(string sectionName)
        {
            if (StringEx.IsNullOrWhiteSpace(sectionName))
                throw new ArgumentNullException("sectionName");

            Dictionary<string, string> section;
            lock (_Configuration)
            {
                if (_Configuration.TryGetValue(sectionName, out section))
                    return section.Keys.ToArray();
            }

            return new string[0];
        }

        /// <summary>
        /// Gets whether a specific value exists in the configuration file. Note that the value
        /// of the configuration entry can be empty (ie. <see cref="string.Empty"/>), all this method
        /// checks is whether there was a line with "section.name=" present.
        /// </summary>
        /// <param name="sectionName">
        /// The name of the section to get the value of a configuration entry from.
        /// </param>
        /// <param name="name">
        /// The name of the configuration entry to retrieve the value for.
        /// </param>
        /// <returns>
        /// <c>true</c> if the value was specified;
        /// otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="sectionName"/> is <c>null</c> or empty.</para>
        /// <para>- or -</para>
        /// <para><paramref name="name"/> is <c>null</c> or empty.</para>
        /// </exception>
        public bool ValueExists(string sectionName, string name)
        {
            if (StringEx.IsNullOrWhiteSpace(sectionName))
                throw new ArgumentNullException("sectionName");
            if (StringEx.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            Dictionary<string, string> section;
            lock (_Configuration)
            {
                if (_Configuration.TryGetValue(sectionName, out section))
                {
                    string value;
                    if (section.TryGetValue(name, out value))
                        return true;
                }
            }

            return false;
        }
    }
}