namespace Mercurial
{
    /// <summary>
    /// This class implements the "hg version" command (<see href="http://www.selenic.com/mercurial/hg.1.html#version"/>):
    /// output version and copyright information.
    /// </summary>
    public sealed class VersionCommand : MercurialCommandBase<VersionCommand>, IMercurialCommand<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VersionCommand"/> class.
        /// </summary>
        public VersionCommand()
            : base("version")
        {
        }

        #region IMercurialCommand<string> Members

        /// <summary>
        /// Gets the result of executing the command as a <see cref="string"/> containing the output
        /// of executing "hg version".
        /// </summary>
        public string Result
        {
            get;
            private set;
        }

        #endregion

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