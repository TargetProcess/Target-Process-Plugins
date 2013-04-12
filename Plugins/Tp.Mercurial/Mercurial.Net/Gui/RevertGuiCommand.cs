namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "revert" command:
    /// Revert selected files.
    /// </summary>
    public sealed class RevertGuiCommand : FilesBasedGuiCommandBase<RevertGuiCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RevertGuiCommand"/> class.
        /// </summary>
        public RevertGuiCommand()
            : base("revert")
        {
            // Do nothing here
        }
    }
}