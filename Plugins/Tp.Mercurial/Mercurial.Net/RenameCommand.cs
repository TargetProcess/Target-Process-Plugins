namespace Mercurial
{
    /// <summary>
    /// This class implements the "hg rename" command (<see href="http://www.selenic.com/mercurial/hg.1.html#rename"/>):
    /// rename files; equivalent of copy + remove.
    /// </summary>
    public class RenameCommand : MoveRenameCommandBase<RenameCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenameCommand"/> class.
        /// </summary>
        public RenameCommand()
            : base("rename")
        {
            // Do nothing here
        }
    }
}