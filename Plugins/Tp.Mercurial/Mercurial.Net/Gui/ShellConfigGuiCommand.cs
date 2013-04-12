namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "shellconfig" command:
    /// Open the Windows Explorer extension configuration editor.
    /// </summary>
    public sealed class ShellConfigGuiCommand : GuiCommandBase<ShellConfigGuiCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShellConfigGuiCommand"/> class.
        /// </summary>
        public ShellConfigGuiCommand()
            : base("shellconfig")
        {
            // Do nothing here
        }
    }
}