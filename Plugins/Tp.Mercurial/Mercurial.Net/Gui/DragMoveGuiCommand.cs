namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "drag_move" command:
    /// Move the selected files to the desired directory.
    /// </summary>
    /// <remarks>
    /// This command is only available for the <see cref="GuiClientType.PyQT"/> client type.
    /// </remarks>
    [GuiClientRequirement(ClientType = GuiClientType.PyQT)]
    public sealed class DragMoveGuiCommand : DragCopyMoveGuiCommandBase<DragMoveGuiCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DragMoveGuiCommand"/> class.
        /// </summary>
        public DragMoveGuiCommand()
            : base("drag_move")
        {
            // Do nothing here
        }
    }
}