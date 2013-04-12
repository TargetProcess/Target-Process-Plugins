namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "about" command:
    /// Show the "About TortoiseHg" dialog.
    /// </summary>
    public sealed class AboutGuiCommand : GuiCommandBase<AboutGuiCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AboutGuiCommand"/> class.
        /// </summary>
        public AboutGuiCommand()
            : base("about")
        {
            // Do nothing here
        }
    }
}