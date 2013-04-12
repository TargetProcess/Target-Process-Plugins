using System.Text;

namespace Mercurial.Gui.Clients
{
    /// <summary>
    /// This <see cref="TortoiseHgClient"/> descendant implements specific methods for the PyGTK (hgtk) version of TortoiseHg.
    /// </summary>
    internal class TortoiseHgPyGTKClient : TortoiseHgClient
    {
        /// <summary>
        /// This field is used to specify the encoding of the listfile.
        /// </summary>
        private static readonly Encoding _FileListEncoding = Encoding.GetEncoding("Windows-1252");

        /// <summary>
        /// Gets the option to pass to the TortoiseHg command line client to read in the file list.
        /// </summary>
        public override string FileListOption
        {
            get
            {
                return "--listfile";
            }
        }

        /// <summary>
        /// Gets the <see cref="Encoding"/> to use when saving the file list to disk.
        /// </summary>
        public override Encoding FileListEncoding
        {
            get
            {
                return _FileListEncoding;
            }
        }
    }
}