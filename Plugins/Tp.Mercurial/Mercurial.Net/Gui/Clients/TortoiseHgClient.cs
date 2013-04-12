using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Mercurial.Gui.Clients
{
    /// <summary>
    /// This class, and its descendants implement specific handling for the specific type of clients.
    /// </summary>
    internal abstract class TortoiseHgClient
    {
        /// <summary>
        /// Gets the current <see cref="TortoiseHgClient"/> implementation in use.
        /// </summary>
        public static TortoiseHgClient Current
        {
            get;
            private set;
        }

        /// <summary>
        /// Assigns the <see cref="Current"/> <see cref="TortoiseHgClient"/> implementation based on the
        /// <see cref="GuiClientType"/> specified.
        /// </summary>
        /// <param name="clientType">
        /// The <see cref="GuiClientType"/> to assign the client for.
        /// </param>
        /// <exception cref="InvalidOperationException">Internal error, unknown client type passed to TortoiseHgClient.AssignCurrent</exception>
        internal static void AssignCurrent(GuiClientType clientType)
        {
            switch (clientType)
            {
                case GuiClientType.PyGTK:
                    Current = new TortoiseHgPyGTKClient();
                    break;

                case GuiClientType.PyQT:
                    Current = new TortoiseHgPyQTClient();
                    break;

                default:
                    throw new InvalidOperationException("Internal error, unknown client type passed to TortoiseHgClient.AssignCurrent");
            }
        }

        /// <summary>
        /// Saves the file list to a file with the specified filename.
        /// </summary>
        /// <param name="files">
        /// The names of all the files to save to the file.
        /// </param>
        /// <param name="filename">
        /// The full path to and name of the file to write the <paramref name="files"/> to.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="files"/> is <c>null</c> or empty.</para>
        /// </exception>
        public void SaveFileList(IEnumerable<string> files, string filename)
        {
            if (files == null || !files.Any())
                throw new ArgumentNullException("files");

            File.WriteAllLines(filename, files.ToArray(), FileListEncoding);
        }

        /// <summary>
        /// Saves the file list to disk in a temporary file, and returns the options necessary to pass to the
        /// TortoiseHg command line client to read back that list.
        /// </summary>
        /// <param name="files">
        /// The names of all the files to save to the file.
        /// </param>
        /// <param name="filename">
        /// Upon return from this method, will either be the full path to and name of the file
        /// the files was written to, or <c>null</c> if the file was not saved.
        /// </param>
        /// <returns>
        /// The options to pass to the TortoiseHg command line client.
        /// </returns>
        public string[] GetFileListArguments(IEnumerable<string> files, out string filename)
        {
            filename = null;

            if (files != null && files.Any())
            {
                filename = Path.GetTempFileName();
                SaveFileList(files, filename);

                return new[]
                {
                    FileListOption,
                    string.Format(CultureInfo.InvariantCulture, "\"{0}\"", filename),
                };
            }

            return new string[0];
        }

        /// <summary>
        /// Gets the option to pass to the TortoiseHg command line client to read in the file list.
        /// </summary>
        public abstract string FileListOption
        {
            get;
        }

        /// <summary>
        /// Gets the <see cref="Encoding"/> to use when saving the file list to disk.
        /// </summary>
        public abstract Encoding FileListEncoding
        {
            get;
        }
    }
}