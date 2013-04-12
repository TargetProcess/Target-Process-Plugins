namespace Mercurial
{
    /// <summary>
    /// This class implements the "hg move" command (<see href="http://www.selenic.com/mercurial/hg.1.html#move"/>):
    /// move files; equivalent of copy + remove.
    /// </summary>
    public class MoveCommand : MoveRenameCommandBase<MoveCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MoveCommand"/> class.
        /// </summary>
        public MoveCommand()
            : base("mv")
        {
            // Do nothing here
        }
    }
}