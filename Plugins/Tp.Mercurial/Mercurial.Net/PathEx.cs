using System;
using System.Collections.Generic;
using System.IO;

namespace Mercurial
{
    /// <summary>
    /// Adds utility methods for paths.
    /// </summary>
    internal static class PathEx
    {
        /// <summary>
        /// Joins all the specified parts using <see cref="Path.Combine"/>.
        /// </summary>
        /// <param name="parts">
        /// The parts of the path to build.
        /// </param>
        /// <returns>
        /// The combined path.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="parts"/> is <c>null</c>.</para>
        /// </exception>
        internal static string Combine(params string[] parts)
        {
            if (parts == null)
                throw new ArgumentNullException("parts");

            using (IEnumerator<string> enumerator = ((IList<string>)parts).GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    return string.Empty;

                string result = enumerator.Current ?? string.Empty;
                while (enumerator.MoveNext())
                    result = Path.Combine(result, enumerator.Current ?? string.Empty);

                return result;
            }
        }
    }
}