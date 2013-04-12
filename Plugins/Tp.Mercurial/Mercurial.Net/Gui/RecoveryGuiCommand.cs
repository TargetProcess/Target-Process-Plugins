namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "recovery" command:
    /// Show the dialog for attempting to recover or rollback.
    /// </summary>
    /// <remarks>
    /// This command is only available for the <see cref="GuiClientType.PyGTK"/> client type.
    /// </remarks>
    [GuiClientRequirement(ClientType = GuiClientType.PyGTK)]
    public sealed class RecoveryGuiCommand : GuiCommandBase<RecoveryGuiCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecoveryGuiCommand"/> class.
        /// </summary>
        public RecoveryGuiCommand()
            : base("recovery")
        {
            // Do nothing here
        }
    }
}