namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "drag_copy" command:
    /// Copy the selected files to the desired directory.
    /// </summary>
    /// <remarks>
    /// This command is only available for the <see cref="GuiClientType.PyQT"/> client type.
    /// </remarks>
    [GuiClientRequirement(ClientType = GuiClientType.PyQT)]
    public sealed class DragCopyGuiCommand : DragCopyMoveGuiCommandBase<DragCopyGuiCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DragCopyGuiCommand"/> class.
        /// </summary>
        public DragCopyGuiCommand()
            : base("drag_copy")
        {
            // Do nothing here
        }
    }
}