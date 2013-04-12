namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "mv" command:
    /// Move dialog.
    /// </summary>
    public sealed class MoveGuiCommand : MoveCopyRenameGuiCommandBase<MoveGuiCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MoveGuiCommand"/> class.
        /// </summary>
        public MoveGuiCommand()
            : base("mv")
        {
            // Do nothing here
        }
    }
}