using System;
using System.ComponentModel;
using Mercurial.Attributes;

namespace Mercurial.Gui
{
    /// <summary>
    /// This is the base class for <see cref="AnnotateGuiCommand"/> and <see cref="ManifestGuiCommand"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The actual type descending from <see cref="BrowserGuiCommandBase{T}"/>, used to generate type-correct
    /// methods in this base class.
    /// </typeparam>
    public abstract class BrowserGuiCommandBase<T> : GuiCommandBase<T>
        where T : BrowserGuiCommandBase<T>
    {
        /// <summary>
        /// This is the backing field for the <see cref="SearchPattern"/> property.
        /// </summary>
        private string _SearchPattern = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserGuiCommandBase{T}"/> class.
        /// </summary>
        /// <param name="command">
        /// The name of the command that will be passed to the TortoiseHg command line client.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="command"/> is <c>null</c> or empty.</para>
        /// </exception>
        protected BrowserGuiCommandBase(string command)
            : base(command)
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets or sets the <see cref="RevSpec"/> to annotate or show the manifest for.
        /// Default value is <c>null</c>.
        /// </summary>
        [NullableArgument(NonNullOption = "--rev")]
        [DefaultValue(null)]
        public RevSpec Revision
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the initial search pattern.
        /// Default value is <see cref="string.Empty"/>.
        /// </summary>
        [NullableArgument(NonNullOption = "--pattern")]
        [DefaultValue("")]
        public string SearchPattern
        {
            get
            {
                return _SearchPattern;
            }

            set
            {
                _SearchPattern = (value ?? string.Empty).Trim();
            }
        }

        /// <summary>
        /// Sets the <see cref="Revision"/> property to the specified value and
        /// returns this command instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Revision"/> property.
        /// </param>
        /// <returns>
        /// This command instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public T WithRevision(RevSpec value)
        {
            Revision = value;
            return (T)this;
        }

        /// <summary>
        /// Sets the <see cref="SearchPattern"/> property to the specified value and
        /// returns this command instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="SearchPattern"/> property.
        /// </param>
        /// <returns>
        /// This command instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public T WithSearchPattern(string value)
        {
            SearchPattern = value;
            return (T)this;
        }
    }
}