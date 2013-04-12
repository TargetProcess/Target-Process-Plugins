namespace Mercurial
{
    /// <summary>
    /// This class implements the "hg init" command (<see href="http://www.selenic.com/mercurial/hg.1.html#init"/>):
    /// create a new repository in the given directory.
    /// </summary>
    public sealed class InitCommand : MercurialCommandBase<InitCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InitCommand"/> class.
        /// </summary>
        public InitCommand()
            : base("init")
        {
            // Do nothing here
        }
    }
}