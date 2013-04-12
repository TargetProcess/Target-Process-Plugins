using System;
using System.ComponentModel;
using Mercurial.Attributes;

namespace Mercurial
{
    /// <summary>
    /// This class implements the "hg help" command (<see href="http://www.selenic.com/mercurial/hg.1.html#help"/>):
    /// show help for a given topic or a help overview.
    /// </summary>
    public sealed class HelpCommand : MercurialCommandBase<HelpCommand>, IMercurialCommand<string>
    {
        /// <summary>
        /// This is the backing field for the <see cref="Topic"/> property.
        /// </summary>
        private string _Topic = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpCommand"/> class.
        /// </summary>
        public HelpCommand()
            : base("help")
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets or sets which topic (command name, or other topics) to request help on.
        /// If left empty, will request the main help text.
        /// Default is <see cref="string.Empty"/>.
        /// </summary>
        [NullableArgument]
        [DefaultValue("")]
        public string Topic
        {
            get
            {
                return _Topic;
            }

            set
            {
                _Topic = (value ?? string.Empty).Trim();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to include global help information.
        /// Default is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "-v")]
        [DefaultValue(false)]
        public bool IncludeGlobalHelp
        {
            get;
            set;
        }

        #region IMercurialCommand<string> Members

        /// <summary>
        /// Gets the result of executing the command as a string containing the help text for the <see cref="Topic"/>.
        /// </summary>
        public string Result
        {
            get;
            private set;
        }

        #endregion

        /// <summary>
        /// Sets the <see cref="Topic"/> property to the specified value and
        /// returns this <see cref="HelpCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Topic"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="HelpCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public HelpCommand WithTopic(string value)
        {
            Topic = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="IncludeGlobalHelp"/> property to the specified value and
        /// returns this <see cref="HelpCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="IncludeGlobalHelp"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="HelpCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public HelpCommand WithIncludeGlobalHelp(bool value = true)
        {
            IncludeGlobalHelp = value;
            return this;
        }

        /// <summary>
        /// This method should parse and store the appropriate execution result output
        /// according to the type of data the command line client would return for
        /// the command.
        /// </summary>
        /// <param name="exitCode">
        /// The exit code from executing the command line client.
        /// </param>
        /// <param name="standardOutput">
        /// The standard output from executing the command line client.
        /// </param>
        protected override void ParseStandardOutputForResults(int exitCode, string standardOutput)
        {
            base.ParseStandardOutputForResults(exitCode, standardOutput);

            Result = standardOutput.Trim();
        }
    }
}