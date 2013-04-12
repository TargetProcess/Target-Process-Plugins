using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Mercurial
{
    /// <summary>
    /// This class can be used internally by the other command classes, to provide support for a list of files
    /// written out to a temporary file, instead of passing them all on the command line.
    /// </summary>
    /// <remarks>
    /// Using the list file syntax is only supported on Mercurial 1.8 and above. This class will handle this
    /// automatically.
    /// </remarks>
    public sealed class ListFile
    {
        /// <summary>
        /// This field is used by the <see cref="GetArguments"/> method.
        /// </summary>
        private static readonly Encoding _ListFileEncoding = Encoding.GetEncoding("Windows-1252");

        /// <summary>
        /// This is the backing field for the <see cref="Collection"/> property.
        /// </summary>
        private readonly Collection<string> _Collection = new Collection<string>(new List<string>());

        /// <summary>
        /// This field holds the full path to and name of the temporary file containing all the file names.
        /// </summary>
        private string _ListFileName;

        /// <summary>
        /// Gets the collection of filenames to write out to the file.
        /// </summary>
        public Collection<string> Collection
        {
            get
            {
                return _Collection;
            }
        }

        /// <summary>
        /// Gets the sequence of arguments to pass to the command line client. This might write out a temporary file on disk,
        /// so be sure to call <see cref="Cleanup"/> when the command has completed execution.
        /// </summary>
        /// <returns>
        /// A collection of arguments to pass to the command line client.
        /// </returns>
        public string[] GetArguments()
        {
            var arguments =
                (from argument in _Collection
                 where !StringEx.IsNullOrWhiteSpace(argument)
                 select argument.Trim()).ToArray();

            if (arguments.Length == 0)
                return arguments;

            if (ClientExecutable.CurrentVersion < new Version(1, 8))
                return arguments;

            _ListFileName = Path.GetTempFileName();

            File.WriteAllText(_ListFileName, string.Join(Environment.NewLine, arguments), _ListFileEncoding);

            return new[] { string.Format(CultureInfo.InvariantCulture, "\"listfile:{0}\"", _ListFileName) };
        }

        /// <summary>
        /// Cleans up by removing the temporary file left from <see cref="GetArguments"/>, provided
        /// one was created and it is still present.
        /// </summary>
        public void Cleanup()
        {
            try
            {
                if (StringEx.IsNullOrWhiteSpace(_ListFileName))
                    return;
                if (!File.Exists(_ListFileName))
                    return;

                File.Delete(_ListFileName);
            }
            catch (IOException)
            {
                // We'll swallow this one, the file is left in the temp-directory of the user, and will
                // be cleaned along with other temporary files by the operating system.
            }
            finally
            {
                _ListFileName = null;
            }
        }
    }
}
