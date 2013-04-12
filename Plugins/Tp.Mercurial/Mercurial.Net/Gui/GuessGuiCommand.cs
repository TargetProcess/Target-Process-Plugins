namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "guess" command (<see href="http://tortoisehg.bitbucket.org/manual/2.0/guess.html#from-command-line"/>):
    /// Guess previous renames or copies.
    /// </summary>
    public sealed class GuessGuiCommand : GuiCommandBase<GuessGuiCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GuessGuiCommand"/> class.
        /// </summary>
        public GuessGuiCommand()
            : base("guess")
        {
            // Do nothing here
        }
    }
}