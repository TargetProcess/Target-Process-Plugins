using System;

namespace Mercurial.Extensions.CaseGuard
{
    /// <summary>
    /// This class adds extension methods to the <see cref="AddRemoveCommand"/> class, for
    /// the CaseGuard extension.
    /// </summary>
    public static class CaseGuardAddRemoveCommandExtensions
    {
        /// <summary>
        /// Add files regardless of possible case-collision problems.
        /// </summary>
        /// <param name="command">
        /// The <see cref="AddRemoveCommand"/> to modify.
        /// </param>
        /// <returns>
        /// The specified <see cref="AddRemoveCommand"/> <paramref name="command"/> object, for the
        /// fluent interface.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="command"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The CaseGuard extension is not installed and active.
        /// </exception>
        public static AddRemoveCommand WithOverrideCaseCollision(this AddRemoveCommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            if (!CaseGuardExtension.IsInstalled)
                throw new InvalidOperationException("The caseguard extension is not installed and active");

            command.AddArgument("--override");
            return command;
        }

        /// <summary>
        /// Do not check filenames for Windows incompatibilities.
        /// </summary>
        /// <param name="command">
        /// The <see cref="AddRemoveCommand"/> to modify.
        /// </param>
        /// <returns>
        /// The specified <see cref="AddRemoveCommand"/> <paramref name="command"/> object, for the
        /// fluent interface.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="command"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The CaseGuard extension is not installed and active.
        /// </exception>
        public static AddRemoveCommand WithoutWindowsFileNameChecks(this AddRemoveCommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            if (!CaseGuardExtension.IsInstalled)
                throw new InvalidOperationException("The caseguard extension is not installed and active");

            command.AddArgument("--nowincheck");
            return command;
        }

        /// <summary>
        /// Completely skip checks related to case-collision problems.
        /// </summary>
        /// <param name="command">
        /// The <see cref="AddRemoveCommand"/> to modify.
        /// </param>
        /// <returns>
        /// The specified <see cref="AddRemoveCommand"/> <paramref name="command"/> object, for the
        /// fluent interface.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="command"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The CaseGuard extension is not installed and active.
        /// </exception>
        public static AddRemoveCommand WithoutCaseGuarding(this AddRemoveCommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            if (!CaseGuardExtension.IsInstalled)
                throw new InvalidOperationException("The caseguard extension is not installed and active");

            command.AddArgument("--unguard");
            return command;
        }
    }
}