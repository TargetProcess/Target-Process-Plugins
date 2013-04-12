using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Mercurial
{
    /// <summary>
    /// This interface must be implemented by all classes that implement the command
    /// pattern for executing Mercurial and TortoiseHg commands.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Gets the command to execute with the Mercurial command line client.
        /// </summary>
        string Command
        {
            get;
        }

        /// <summary>
        /// Gets all the arguments to the <see cref="Command"/>, or an
        /// empty array if there are none.
        /// </summary>
        IEnumerable<string> Arguments
        {
            get;
        }

        /// <summary>
        /// Gets any additional arguments to the <see cref="Command"/>, or an
        /// empty collection if there are none.
        /// </summary>
        Collection<string> AdditionalArguments
        {
            get;
        }

        /// <summary>
        /// Gets the <see cref="IMercurialCommandObserver"/> that will be informed of
        /// execution progress. Can be <c>null</c> in case there is no observer.
        /// </summary>
        IMercurialCommandObserver Observer
        {
            get;
        }

        /// <summary>
        /// Gets the timeout in seconds for how long to wait for the command to
        /// complete successfully before terminating it and throwing an exception. A
        /// typical default value is 60.
        /// </summary>
        int Timeout
        {
            get;
        }

        /// <summary>
        /// Validates the command configuration. This method should throw the necessary
        /// exceptions to signal missing or incorrect configuration (like attempting to
        /// add files to the repository without specifying which files to add.)
        /// </summary>
        void Validate();

        /// <summary>
        /// This method is called before the command is executed. You can use this to
        /// store temporary files (like a commit message or similar) that the
        /// <see cref="Arguments"/> refer to, before the command is executed.
        /// </summary>
        void Before();

        /// <summary>
        /// This method is called after the command has been executed. You can use this to
        /// clean up after the command execution (like removing temporary files), and to
        /// react to the exit code from the command line client. If the exit code is
        /// considered a failure, this method should throw the correct exception.
        /// </summary>
        /// <param name="exitCode">
        /// The exit code from the command line client. Typically 0 means success, but this
        /// can vary from command to command.
        /// </param>
        /// <param name="standardOutput">
        /// The standard output of the execution, or <see cref="string.Empty"/> if there
        /// was none.
        /// </param>
        /// <param name="standardErrorOutput">
        /// The standard error output of the execution, or <see cref="string.Empty"/> if
        /// there was none.
        /// </param>
        /// <remarks>
        /// Also note that if the command exit code is considered a success, this method
        /// should parse the output and prepare any results based on it.
        /// </remarks>
        void After(int exitCode, string standardOutput, string standardErrorOutput);
    }
}