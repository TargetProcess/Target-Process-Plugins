namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "forget" command:
    /// Show file status viewer in "forget mode".
    /// </summary>
    public sealed class ForgetGuiCommand : FilesBasedGuiCommandBase<ForgetGuiCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForgetGuiCommand"/> class.
        /// </summary>
        public ForgetGuiCommand()
            : base("forget")
        {
        }
    }
}