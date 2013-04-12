using System.ComponentModel;
using Mercurial.Attributes;

namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "status" command:
    /// Show working copy status.
    /// </summary>
    public sealed class StatusGuiCommand : FilesBasedGuiCommandBase<StatusGuiCommand>
    {
        /// <summary>
        /// This is the backing field for the <see cref="ShowClean"/> property.
        /// </summary>
        private bool _ShowClean;

        /// <summary>
        /// This is the backing field for the <see cref="ShowIgnored"/> property.
        /// </summary>
        private bool _ShowIgnored;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusGuiCommand"/> class.
        /// </summary>
        public StatusGuiCommand()
            : base("status")
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show files without changes.
        /// Default is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--clean")]
        [DefaultValue(false)]
        public bool ShowClean
        {
            get
            {
                return _ShowClean;
            }

            set
            {
                if (_ShowClean == value)
                    return;
                
                EnsurePropertyAvailability("ShowClean", GuiClientType.PyQT);
                _ShowClean = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show ignored files.
        /// Default is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--ignored")]
        [DefaultValue(false)]
        public bool ShowIgnored
        {
            get
            {
                return _ShowIgnored;
            }

            set
            {
                if (_ShowIgnored == value)
                    return;
                
                EnsurePropertyAvailability("ShowIgnored", GuiClientType.PyQT);
                _ShowIgnored = value;
            }
        }

        /// <summary>
        /// Sets the <see cref="ShowClean"/> property to the specified value and
        /// returns this <see cref="StatusGuiCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="ShowClean"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="StatusGuiCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public StatusGuiCommand WithShowClean(bool value = true)
        {
            ShowClean = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="ShowIgnored"/> property to the specified value and
        /// returns this <see cref="StatusGuiCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="ShowIgnored"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="StatusGuiCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public StatusGuiCommand WithShowIgnored(bool value = true)
        {
            ShowIgnored = value;
            return this;
        }
    }
}