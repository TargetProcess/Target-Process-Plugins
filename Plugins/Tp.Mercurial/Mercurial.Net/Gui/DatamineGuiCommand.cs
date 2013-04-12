namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "datamine" command:
    /// Search the repository.
    /// </summary>
    /// <remarks>
    /// This command is only available for the <see cref="GuiClientType.PyGTK"/> client type.
    /// </remarks>
    [GuiClientRequirement(ClientType = GuiClientType.PyGTK)]
    public sealed class DatamineGuiCommand : GuiCommandBase<DatamineGuiCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatamineGuiCommand"/> class.
        /// </summary>
        public DatamineGuiCommand()
            : base("datamine")
        {
            // Do nothing here
        }
    }
}