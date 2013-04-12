namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "resolve" command:
    /// Resolve dialog.
    /// </summary>
    /// <remarks>
    /// This command is only available for the <see cref="GuiClientType.PyQT"/> client type.
    /// </remarks>
    [GuiClientRequirement(ClientType = GuiClientType.PyQT)]
    public sealed class ResolveGuiCommand : GuiCommandBase<ResolveGuiCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResolveGuiCommand"/> class.
        /// </summary>
        public ResolveGuiCommand()
            : base("resolve")
        {
            // Do nothing here
        }
    }
}