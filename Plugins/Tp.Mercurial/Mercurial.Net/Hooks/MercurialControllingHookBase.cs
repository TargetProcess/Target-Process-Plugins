using System;

namespace Mercurial.Hooks
{
    /// <summary>
    /// This is the base class for Mercurial hook type implementations that are of type "controlling", that is
    /// that they can determine whether the Mercurial command being executed is allowed to proceed or not.
    /// </summary>
    public class MercurialControllingHookBase : MercurialHookBase, IMercurialControllingHook
    {
        /// <summary>
        /// Terminates the hook program and allows the Mercurial command being hooked to proceed as normal.
        /// </summary>
        public void TerminateHookAndProceed()
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// Terminates the hook and cancels the Mercurial command being executed.
        /// </summary>
        /// <param name="exitCode">
        /// The exit code to pass back to Mercurial, must be or higher to signal failure. Default is 1.
        /// </param>
        /// <param name="message">
        /// A message to output to the Mercurial console log; or an empty string if no such message is needed.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <para><paramref name="exitCode"/> must be 1 or higher.</para>
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="message"/> is <c>null</c>.</para>
        /// </exception>
        public void TerminateHookAndCancelCommand(int exitCode, string message)
        {
            if (exitCode < 1)
                throw new ArgumentOutOfRangeException("exitCode", exitCode, "exitCode must be 1 or higher");
            if (message == null)
                throw new ArgumentNullException("message");

            if (!StringEx.IsNullOrWhiteSpace(message))
                Console.Error.WriteLine(message);
            Environment.Exit(exitCode);
        }

        /// <summary>
        /// Terminates the hook and cancels the Mercurial command being executed, with
        /// an exit code of 1 and no message.
        /// </summary>
        public void TerminateHookAndCancelCommand()
        {
            TerminateHookAndCancelCommand(1, string.Empty);
        }

        /// <summary>
        /// Terminates the hook and cancels the Mercurial command being executed, with
        /// the specified exit code and no message.
        /// </summary>
        /// <param name="exitCode">
        /// The exit code to pass back to Mercurial, must be or higher to signal failure. Default is 1.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <para><paramref name="exitCode"/> must be 1 or higher.</para>
        /// </exception>
        public void TerminateHookAndCancelCommand(int exitCode)
        {
            TerminateHookAndCancelCommand(1, string.Empty);
        }
    }
}
