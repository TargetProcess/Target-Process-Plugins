namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "rename" command:
    /// Rename dialog (also copies or moves.)
    /// </summary>
    public sealed class RenameGuiCommand : MoveCopyRenameGuiCommandBase<RenameGuiCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenameGuiCommand"/> class.
        /// </summary>
        public RenameGuiCommand()
            : base("rename")
        {
            // Do nothing here
        }
    }
}