namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "synchronize" command (<see href="http://tortoisehg.bitbucket.org/manual/2.0/sync.html#from-command-line"/>):
    /// Synchronize the repository with other repositories.
    /// </summary>
    public sealed class SynchronizeGuiCommand : GuiCommandBase<SynchronizeGuiCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizeGuiCommand"/> class.
        /// </summary>
        public SynchronizeGuiCommand()
            : base("synch")
        {
            // Do nothing here
        }
    }
}