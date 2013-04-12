namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "qqueue" command:
    /// Manage multiple MQ patch queues.
    /// </summary>
    /// <remarks>
    /// This command is only available for the <see cref="GuiClientType.PyQT"/> client type.
    /// </remarks>
    [GuiClientRequirement(ClientType = GuiClientType.PyQT)]
    public sealed class QueueManagerGuiCommand : GuiCommandBase<QueueManagerGuiCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueueManagerGuiCommand"/> class.
        /// </summary>
        public QueueManagerGuiCommand()
            : base("qqueue")
        {
            // Do nothing here
        }
    }
}