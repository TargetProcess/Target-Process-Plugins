namespace Mercurial
{
    /// <summary>
    /// This <see cref="IMercurialCommand"/> descendant interface must be implemented by
    /// all classes that implement the command pattern for executing Mercurial commands,
    /// and that returns data back to the programmer.
    /// </summary>
    /// <typeparam name="TResult">
    /// The type of result from executing the command.
    /// </typeparam>
    public interface IMercurialCommand<out TResult> : IMercurialCommand
    {
        /// <summary>
        /// Gets the result from the command line execution, as an appropriately typed value.
        /// </summary>
        TResult Result
        {
            get;
        }
    }

    /// <summary>
    /// This interface must be implemented by all classes that implement the command
    /// pattern for executing Mercurial commands.
    /// </summary>
    public interface IMercurialCommand : ICommand
    {
    }
}