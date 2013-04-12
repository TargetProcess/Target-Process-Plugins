namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "init" command (<see href="http://tortoisehg.bitbucket.org/manual/2.0/init.html#from-command-line"/>):
    /// Initialize a new repository.
    /// </summary>
    public sealed class InitGuiCommand : GuiCommandBase<InitGuiCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InitGuiCommand"/> class.
        /// </summary>
        public InitGuiCommand()
            : base("init")
        {
            // Do nothing here
        }
    }
}