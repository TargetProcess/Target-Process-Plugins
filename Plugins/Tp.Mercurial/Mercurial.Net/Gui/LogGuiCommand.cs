using System.ComponentModel;
using Mercurial.Attributes;

namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "log" command (<see href="http://tortoisehg.bitbucket.org/manual/2.0/workbench.html#from-command-line"/>):
    /// Repository explorer (changelog viewer.)
    /// </summary>
    public sealed class LogGuiCommand : GuiCommandBase<LogGuiCommand>
    {
        /// <summary>
        /// This is the backing field for the <see cref="File"/> property.
        /// </summary>
        private string _File = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogGuiCommand"/> class.
        /// </summary>
        public LogGuiCommand()
            : base("log")
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets or sets which file to show the log for.
        /// </summary>
        [NullableArgument]
        [DefaultValue("")]
        public string File
        {
            get
            {
                return _File;
            }

            set
            {
                _File = (value ?? string.Empty).Trim();
            }
        }

        /// <summary>
        /// Sets the <see cref="File"/> property to the specified value and
        /// returns this <see cref="LogGuiCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="File"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="LogGuiCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public LogGuiCommand WithFile(string value)
        {
            File = value;
            return this;
        }
    }
}