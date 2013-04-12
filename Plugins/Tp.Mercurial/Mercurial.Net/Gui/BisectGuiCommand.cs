namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "bisect" command:
    /// Show the bisect dialog.
    /// </summary>
    /// <remarks>
    /// This command is only available for the <see cref="GuiClientType.PyQT"/> client type.
    /// </remarks>
    [GuiClientRequirement(ClientType = GuiClientType.PyQT)]
    public sealed class BisectGuiCommand : GuiCommandBase<BisectGuiCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BisectGuiCommand"/> class.
        /// </summary>
        public BisectGuiCommand()
            : base("bisect")
        {
            // Do nothing here
        }
    }
}