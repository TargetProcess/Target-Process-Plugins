namespace Mercurial
{
    /// <summary>
    /// This class implements the "hg rollback" command (<see href="http://www.selenic.com/mercurial/hg.1.html#rollback"/>):
    /// Roll back the last transaction (dangerous.)
    /// </summary>
    public sealed class RollbackCommand : MercurialCommandBase<RollbackCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RollbackCommand"/> class.
        /// </summary>
        public RollbackCommand()
            : base("rollback")
        {
            // Do nothing here
        }
    }
}