using System.ComponentModel;
using Mercurial.Attributes;

namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "hgignore" command (<see href="http://tortoisehg.bitbucket.org/manual/2.0/ignore.html#from-command-line"/>):
    /// Show the TortoiseHg dialog for editing ignore filters.
    /// </summary>
    public sealed class EditIgnoreFiltersGuiCommand : GuiCommandBase<EditIgnoreFiltersGuiCommand>
    {
        /// <summary>
        /// This is the backing field for the <see cref="FileName"/> property.
        /// </summary>
        private string _FileName = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditIgnoreFiltersGuiCommand"/> class.
        /// </summary>
        public EditIgnoreFiltersGuiCommand()
            : base("hgignore")
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets or sets the filename to show in the dialog initially.
        /// </summary>
        /// <value>
        /// The name of the file to show in the dialog initially.
        /// </value>
        [NullableArgument]
        [DefaultValue("")]
        public string FileName
        {
            get
            {
                return _FileName;
            }

            set
            {
                _FileName = (value ?? string.Empty).Trim();
            }
        }

        /// <summary>
        /// Sets the <see cref="FileName"/> property to the specified value and
        /// returns this <see cref="EditIgnoreFiltersGuiCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="FileName"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="EditIgnoreFiltersGuiCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public EditIgnoreFiltersGuiCommand WithFileName(string value)
        {
            FileName = value;
            return this;
        }
    }
}