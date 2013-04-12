using System.Diagnostics;

namespace Mercurial
{
    /// <summary>
    /// This class implements <see cref="IMercurialCommandObserver"/>
    /// by simply writing everything to debug output through
    /// <see cref="Debug.WriteLine(string)"/>.
    /// </summary>
    public class DebugObserver : IMercurialCommandObserver
    {
        #region IMercurialCommandObserver Members

        /// <summary>
        /// This method will be called once for each line of normal output from the command. Note that this method will be called on
        /// a different thread than the thread that called the <see cref="NonPersistentClient.Execute(string,Mercurial.IMercurialCommand)"/> method.
        /// </summary>
        /// <param name="line">
        /// The line of text to output to the observer.
        /// </param>
        public void Output(string line)
        {
            Debug.WriteLine(line);
        }

        /// <summary>
        /// This method will be called once for each line of error output from the command. Note that this method will be called on
        /// a different thread than the thread that called the <see cref="NonPersistentClient.Execute(string,Mercurial.IMercurialCommand)"/> method.
        /// </summary>
        /// <param name="line">
        /// The line of error text to output to the observer.
        /// </param>
        public void ErrorOutput(string line)
        {
            Debug.WriteLine("! " + line);
        }

        /// <summary>
        /// This method will be called before the command starts executing.
        /// </summary>
        /// <param name="command">
        /// The command that will be executed.
        /// </param>
        /// <param name="arguments">
        /// The arguments to the <paramref name="command"/>.
        /// </param>
        public void Executing(string command, string arguments)
        {
            Debug.WriteLine("executing: " + command + " " + arguments);
        }

        /// <summary>
        /// This method will be called after the command has terminated (either timed out or completed by itself.)
        /// </summary>
        /// <param name="command">
        /// The command that was executed.
        /// </param>
        /// <param name="arguments">
        /// The arguments to the <paramref name="command"/>.
        /// </param>
        /// <param name="exitCode">
        /// The exit code from the process after it finished.
        /// </param>
        /// <param name="output">
        /// The standard output text from the process.
        /// </param>
        /// <param name="errorOutput">
        /// The error output text from the process.
        /// </param>
        public void Executed(string command, string arguments, int exitCode, string output, string errorOutput)
        {
            Debug.WriteLine("executed: " + command + " " + arguments + " --> " + exitCode);
        }

        #endregion
    }
}