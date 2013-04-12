using System.ComponentModel;
using System.Text.RegularExpressions;
using Mercurial.Attributes;

namespace Mercurial
{
    /// <summary>
    /// This class implements the "hg identify" command (<see href="http://www.selenic.com/mercurial/hg.1.html#identify"/>):
    /// identify the working copy or specified revision.
    /// </summary>
    public sealed class IdentifyCommand : MercurialCommandBase<IdentifyCommand>, IMercurialCommand<RevSpec>
    {
        /// <summary>
        /// This is the backing field for the <see cref="Path"/> property.
        /// </summary>
        private string _Path = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentifyCommand"/> class.
        /// </summary>
        public IdentifyCommand()
            : base("identify")
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets or sets the path to the repository or working copy to identify.
        /// </summary>
        [NullableArgument]
        [DefaultValue("")]
        public string Path
        {
            get
            {
                return _Path;
            }

            set
            {
                _Path = (value ?? string.Empty).Trim();
            }
        }

        #region IMercurialCommand<RevSpec> Members

        /// <summary>
        /// Gets the result of executing the command as a <see cref="RevSpec"/> object.
        /// </summary>
        public RevSpec Result
        {
            get;
            private set;
        }

        #endregion

        /// <summary>
        /// Sets the <see cref="Path"/> property to the specified value and
        /// returns this <see cref="IdentifyCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Path"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="IdentifyCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public IdentifyCommand WithPath(string value)
        {
            Path = value;
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
        /// <remarks>
        /// Note that as long as you descend from <see cref="MercurialCommandBase{T}"/> you're not required to call
        /// the base method at all.
        /// </remarks>
        protected override void ParseStandardOutputForResults(int exitCode, string standardOutput)
        {
            if (exitCode != 0)
                return;

            Match ma = Regex.Match(standardOutput, @"^(?<hash>[a-f0-9]{12,40}).*$", RegexOptions.IgnoreCase);
            if (ma.Success)
                Result = RevSpec.Single(ma.Groups["hash"].Value);
        }
    }
}