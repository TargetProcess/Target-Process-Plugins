namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "copy" command:
    /// Copy dialog.
    /// </summary>
    /// <remarks>
    /// This command is only available for the <see cref="GuiClientType.PyQT"/> client type.
    /// </remarks>
    [GuiClientRequirement(ClientType = GuiClientType.PyQT)]
    public sealed class CopyGuiCommand : MoveCopyRenameGuiCommandBase<CopyGuiCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CopyGuiCommand"/> class.
        /// </summary>
        public CopyGuiCommand()
            : base("copy")
        {
            // Do nothing here
        }
    }
}