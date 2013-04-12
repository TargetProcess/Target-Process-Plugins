namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "remove" command:
    /// Open the file status viewer in revert mode.
    /// </summary>
    public sealed class RemoveGuiCommand : FilesBasedGuiCommandBase<RemoveGuiCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveGuiCommand"/> class.
        /// </summary>
        public RemoveGuiCommand()
            : base("remove")
        {
            // Do nothing here
        }
    }
}