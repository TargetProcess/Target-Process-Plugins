using System;
using System.Collections.Generic;
using System.IO;

namespace Mercurial
{
    /// <summary>
    /// This class holds utility methods for parsing the output from the Mercurial commands.
    /// </summary>
    internal static class OutputParsingUtilities
    {
        /// <summary>
        /// Split the multi-line output into separate line strings.
        /// </summary>
        /// <param name="output">
        /// The output from the Mercurial command string to split into separate line strings.
        /// </param>
        /// <returns>
        /// An array of strings, one for each line in the <paramref name="output"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="output"/> is <c>null</c>.</para>
        /// </exception>
        public static string[] SplitIntoLines(string output)
        {
            if (output == null)
                throw new ArgumentNullException("output");

            var result = new List<string>();
            using (var reader = new StringReader(output))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                    result.Add(line);
            }
            return result.ToArray();
        }
    }
}