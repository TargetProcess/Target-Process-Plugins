namespace Mercurial
{
    /// <summary>
    /// This class implements the "hg verify" command (<see href="http://www.selenic.com/mercurial/hg.1.html#verify"/>):
    /// verify the integrity of the repository.
    /// </summary>
    public sealed class VerifyCommand : MercurialCommandBase<VerifyCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VerifyCommand"/> class.
        /// </summary>
        public VerifyCommand()
            : base("verify")
        {
            // Do nothing here
        }
    }
}