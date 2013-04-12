namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "mq" command:
    /// Mercurial queue tool.
    /// </summary>
    /// <remarks>
    /// This command is only available for the <see cref="GuiClientType.PyQT"/> client type.
    /// </remarks>
    [GuiClientRequirement(ClientType = GuiClientType.PyQT)]
    public sealed class QueueGuiCommand : GuiCommandBase<QueueGuiCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueueGuiCommand"/> class.
        /// </summary>
        public QueueGuiCommand()
            : base("mq")
        {
            // Do nothing here
        }
    }
}