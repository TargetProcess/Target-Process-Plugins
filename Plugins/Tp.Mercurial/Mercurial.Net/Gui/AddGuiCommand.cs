namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "add" command:
    /// Add files.
    /// </summary>
    public sealed class AddGuiCommand : FilesBasedGuiCommandBase<AddGuiCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddGuiCommand"/> class.
        /// </summary>
        public AddGuiCommand()
            : base("add")
        {
            // Do nothing here
        }
    }
}