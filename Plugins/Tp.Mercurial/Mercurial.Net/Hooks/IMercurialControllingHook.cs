using System;

namespace Mercurial.Hooks
{
    /// <summary>
    /// This interface must be implementing by all controlling hooks.
    /// </summary>
    public interface IMercurialControllingHook
    {
        /// <summary>
        /// Terminates the hook program and allows the Mercurial command being hooked to proceed as normal.
        /// </summary>
        void TerminateHookAndProceed();

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
        void TerminateHookAndCancelCommand(int exitCode, string message);

        /// <summary>
        /// Terminates the hook and cancels the Mercurial command being executed, with
        /// an exit code of 1 and no message.
        /// </summary>
        void TerminateHookAndCancelCommand();

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
        void TerminateHookAndCancelCommand(int exitCode);
    }
}
