namespace Mercurial
{
    /// <summary>
    /// This class implements the "hg recover" command (<see href="http://www.selenic.com/mercurial/hg.1.html#recover"/>):
    /// Recover from an interrupted commit or pull.
    /// </summary>
    public sealed class RecoverCommand : MercurialCommandBase<RecoverCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecoverCommand"/> class.
        /// </summary>
        public RecoverCommand()
            : base("recover")
        {
            // Do nothing here
        }
    }
}