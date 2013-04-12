using System.Text;

namespace Mercurial.Gui.Clients
{
    /// <summary>
    /// This <see cref="TortoiseHgClient"/> descendant implements specific methods for the PyQT (thg) version of TortoiseHg.
    /// </summary>
    internal class TortoiseHgPyQTClient : TortoiseHgClient
    {
        /// <summary>
        /// This field is used by the <see cref="FileListEncoding"/> property.
        /// </summary>
        private static readonly UTF8Encoding _Utf8EncodingWithoutBOM = new UTF8Encoding(false);

        /// <summary>
        /// Gets the option to pass to the TortoiseHg command line client to read in the file list.
        /// </summary>
        public override string FileListOption
        {
            get
            {
                return "--listfileutf8";
            }
        }

        /// <summary>
        /// Gets the <see cref="Encoding"/> to use when saving the file list to disk.
        /// </summary>
        public override Encoding FileListEncoding
        {
            get
            {
                return _Utf8EncodingWithoutBOM;
            }
        }
    }
}