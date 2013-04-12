namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "shelve" command (<see href="http://tortoisehg.bitbucket.org/manual/2.0/shelve.html#from-command-line"/>):
    /// Shelve/unshelve tool.
    /// </summary>
    public sealed class ShelveGuiCommand : GuiCommandBase<ShelveGuiCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShelveGuiCommand"/> class.
        /// </summary>
        public ShelveGuiCommand()
            : base("shelve")
        {
            // Do nothing here
        }
    }
}