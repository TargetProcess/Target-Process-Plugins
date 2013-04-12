namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "version" command:
    /// Retrieve the version text from the TortoiseHg client.
    /// </summary>
    public sealed class GuiVersionCommand : GuiCommandBase<GuiVersionCommand>, IMercurialCommand<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GuiVersionCommand"/> class.
        /// </summary>
        public GuiVersionCommand()
            : base("version")
        {
            // Do nothing here
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
            if (exitCode == 0)
                Result = standardOutput;
        }

        /// <summary>
        /// Gets the result from the command line execution, as an appropriately typed value.
        /// </summary>
        public string Result
        {
            get;
            private set;
        }
    }
}