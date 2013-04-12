using System;
using System.ComponentModel;
using Mercurial.Attributes;

namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "commit" command (<see href="http://tortoisehg.bitbucket.org/manual/2.0/commit.html#from-command-line"/>):
    /// Commit changes in the working folder to the repository.
    /// </summary>
    public sealed class CommitGuiCommand : FilesBasedGuiCommandBase<CommitGuiCommand>
    {
        /// <summary>
        /// This is the backing field for the <see cref="OverrideAuthor"/> property.
        /// </summary>
        private string _OverrideAuthor = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommitGuiCommand"/> class.
        /// </summary>
        public CommitGuiCommand()
            : base("commit")
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets or sets the username to use when committing;
        /// or <see cref="string.Empty"/> to use the username configured in the repository or by
        /// the current user. Default is <see cref="string.Empty"/>.
        /// </summary>
        [NullableArgument(NonNullOption = "--user")]
        [DefaultValue("")]
        public string OverrideAuthor
        {
            get
            {
                return _OverrideAuthor;
            }

            set
            {
                _OverrideAuthor = (value ?? string.Empty).Trim();
            }
        }

        /// <summary>
        /// Gets or sets the timestamp <see cref="DateTime"/> to use when committing;
        /// or <c>null</c> which means use the current date and time. Default is <c>null</c>.
        /// </summary>
        [DateTimeArgument(NonNullOption = "--date")]
        [DefaultValue(null)]
        public DateTime? OverrideTimestamp
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the <see cref="OverrideAuthor"/> property to the specified value and
        /// returns this <see cref="CommitGuiCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="OverrideAuthor"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="CommitGuiCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public CommitGuiCommand WithOverrideAuthor(string value)
        {
            OverrideAuthor = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="OverrideTimestamp"/> property to the specified value and
        /// returns this <see cref="CommitGuiCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="OverrideTimestamp"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="CommitGuiCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public CommitGuiCommand WithOverrideTimestamp(DateTime value)
        {
            OverrideTimestamp = value;
            return this;
        }
    }
}