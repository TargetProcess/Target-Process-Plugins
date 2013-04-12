namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "qreorder" command:
    /// Reorder unapplied MQ patches.
    /// </summary>
    /// <remarks>
    /// This command is only available for the <see cref="GuiClientType.PyQT"/> client type.
    /// </remarks>
    [GuiClientRequirement(ClientType = GuiClientType.PyQT)]
    public sealed class QueueReorderGuiCommand : GuiCommandBase<QueueReorderGuiCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueueReorderGuiCommand"/> class.
        /// </summary>
        public QueueReorderGuiCommand()
            : base("qreorder")
        {
            // Do nothing here
        }
    }
}