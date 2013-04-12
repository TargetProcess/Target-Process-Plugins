using System;
using System.Globalization;

namespace Mercurial
{
    /// <summary>
    /// This is the base class for option classes for various commands for
    /// the Mercurial client.
    /// </summary>
    /// <typeparam name="T">
    /// The actual type descending from <see cref="MercurialCommandBase{T}"/>, used to generate type-correct
    /// methods in this base class.
    /// </typeparam>
    public abstract class MercurialCommandBase<T> : CommandBase<T>, IMercurialCommand
        where T : MercurialCommandBase<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MercurialCommandBase{T}"/> class.
        /// </summary>
        /// <param name="command">
        /// The name of the command that will be passed to the Mercurial command line client.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="command"/> is <c>null</c> or empty.</para>
        /// </exception>
        protected MercurialCommandBase(string command)
            : base(command)
        {
            // Do nothing here
        }

        /// <summary>
        /// Adds a configuration override specification to the <see cref="CommandBase{T}.AdditionalArguments"/>
        /// collection in the form of <c>section.name=value</c>.
        /// </summary>
        /// <param name="sectionName">
        /// The name of the section.
        /// </param>
        /// <param name="name">
        /// The name of the value.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="sectionName"/> is <c>null</c> or empty.</para>
        /// <para>- or -</para>
        /// <para><paramref name="name"/> is <c>null</c> or empty.</para>
        /// <para>- or -</para>
        /// <para><paramref name="value"/> is <c>null</c>.</para>
        /// </exception>
        public void WithConfigurationOverride(string sectionName, string name, string value)
        {
            if (StringEx.IsNullOrWhiteSpace(sectionName))
                throw new ArgumentNullException("sectionName");
            if (StringEx.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");
            if (value == null)
                throw new ArgumentNullException("value");

            AdditionalArguments.Add("--config");
            AdditionalArguments.Add(string.Format(CultureInfo.InvariantCulture, "{0}.{1}=\"{2}\"", sectionName, name, value));
        }

        /// <summary>
        /// This method will check the current client version and throw a <see cref="NotSupportedException"/>
        /// if the current client version is older than the required one.
        /// </summary>
        /// <param name="requiredVersion">
        /// The version of the client required for the command, property, method, etc.
        /// </param>
        /// <param name="whatRequiresTheSpecifiedVersion">
        /// A text message that identifies the item that requires the version. Will be part of the exception
        /// message as "The XYZ requires version A.B.C...".
        /// </param>
        /// <exception cref="NotSupportedException">
        /// <para>The current version is older than <paramref name="requiredVersion"/>.</para>
        /// </exception>
        public void RequiresVersion(Version requiredVersion, string whatRequiresTheSpecifiedVersion)
        {
            if (ClientExecutable.CurrentVersion < requiredVersion)
            {
                throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "The {0} requires version {1} in order to be used; current version is {2}", whatRequiresTheSpecifiedVersion, requiredVersion, ClientExecutable.CurrentVersion));
            }
        }
    }
}