namespace Mercurial.Gui
{
    /// <summary>
    /// This enum is used by <see cref="GuiClient"/> to specify which client to use.
    /// </summary>
    public enum GuiClientType
    {
        /// <summary>
        /// No Gui client was detected on the system.
        /// </summary>
        None,

        /// <summary>
        /// This is the "old style" TortoiseHg client, pre-2.0.
        /// </summary>
        PyGTK,

        /// <summary>
        /// This is the "new style" TortoiseHg client, from 2.0 and onwards.
        /// </summary>
        PyQT,
    }
}