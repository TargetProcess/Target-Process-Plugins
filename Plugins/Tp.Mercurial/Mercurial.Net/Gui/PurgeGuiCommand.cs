namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "purge" command:
    /// Purge unknown and/or ignore files from repository.
    /// </summary>
    /// <remarks>
    /// This command is only available for the <see cref="GuiClientType.PyQT"/> client type.
    /// </remarks>
    [GuiClientRequirement(ClientType = GuiClientType.PyQT)]
    public sealed class PurgeGuiCommand : GuiCommandBase<PurgeGuiCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PurgeGuiCommand"/> class.
        /// </summary>
        public PurgeGuiCommand()
            : base("purge")
        {
            // Do nothing here
        }
    }
}