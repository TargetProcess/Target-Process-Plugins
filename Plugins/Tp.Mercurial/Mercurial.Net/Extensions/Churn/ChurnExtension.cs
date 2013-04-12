using System;
using System.Collections.Generic;

namespace Mercurial.Extensions.Churn
{
    /// <summary>
    /// This class contains logic for the caseguard Mercurial extension.
    /// </summary>
    public static class ChurnExtension
    {
        /// <summary>
        /// Gets a value indicating whether the caseguard extension is installed and active.
        /// </summary>
        public static bool IsInstalled
        {
            get
            {
                return ClientExecutable.Configuration.ValueExists("extensions", "churn");
            }
        }

        /// <summary>
        /// Calculates the churn report; a histogram of changes to the repository.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to calculate the churn report for.
        /// </param>
        /// <param name="command">
        /// Any extra options to the churn method, or <c>null</c> for default options.
        /// </param>
        /// <returns>
        /// A collection of <see cref="ChurnGroup" /> instances.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The Churn extension is not installed and active.
        /// </exception>
        public static IEnumerable<ChurnGroup> Churn(this Repository repository, ChurnCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            if (!IsInstalled)
                throw new InvalidOperationException("The churn extension is not installed and active");

            command = command ?? new ChurnCommand();
            repository.Execute(command);
            return command.Result;
        }
    }
}