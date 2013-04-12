using System;

namespace Mercurial.Extensions.Queues
{
    /// <summary>
    /// This class adds extension methods to the <see cref="MercurialCommandBase{T}"/> class, for
    /// the Mercurial Queues extension.
    /// </summary>
    public static class QueueCommandBaseExtensions
    {
        /// <summary>
        /// Makes the command operate on the patch repository instead of the main repository.
        /// </summary>
        /// <typeparam name="T">
        /// The type of command that should operate on the patch repository instead.
        /// </typeparam>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <returns>
        /// The <paramref name="command"/>, a continuation of the fluent interface.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="command"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The Mercurial Queues extension is not installed and active.
        /// </exception>
        public static T OperateOnPatchRepository<T>(this T command) where T : CommandBase<T>
        {
            if (command == null)
                throw new ArgumentNullException("command");

            if (!QueueExtension.IsInstalled)
                throw new InvalidOperationException("The Mercurial Queues extension is not installed and active");

            return command.WithAdditionalArgument("--mq");
        }
    }
}