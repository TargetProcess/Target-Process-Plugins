namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "userconfig" command:
    /// Open the Windows Explorer extension configuration editor.
    /// </summary>
    public sealed class UserConfigGuiCommand : GuiCommandBase<UserConfigGuiCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserConfigGuiCommand"/> class.
        /// </summary>
        public UserConfigGuiCommand()
            : base("userconfig")
        {
            // Do nothing here
        }
    }
}