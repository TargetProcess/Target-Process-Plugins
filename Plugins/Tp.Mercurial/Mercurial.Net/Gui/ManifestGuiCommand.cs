namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "manifest" command:
    /// Display the current or given revision of the project manifest.
    /// </summary>
    /// <remarks>
    /// This command is only available for the <see cref="GuiClientType.PyQT"/> client type.
    /// </remarks>
    [GuiClientRequirement(ClientType = GuiClientType.PyQT)]
    public sealed class ManifestGuiCommand : BrowserGuiCommandBase<ManifestGuiCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManifestGuiCommand"/> class.
        /// </summary>
        public ManifestGuiCommand()
            : base("manifest")
        {
            // Do nothing here
        }
    }
}