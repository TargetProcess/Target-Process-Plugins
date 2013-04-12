namespace Mercurial
{
    /// <summary>
    /// This enum is used to specify the type of compression to apply to files, typically
    /// bundle files produced with the <see cref="BundleCommand"/>.
    /// </summary>
    public enum MercurialCompressionType
    {
        /// <summary>
        /// Use the "bzip2" compression type (<see href="http://bzip.org/"/>).
        /// </summary>
        BZip2,

        /// <summary>
        /// Use the "gzip" compression type (<see href="http://www.gzip.org/"/>).
        /// </summary>
        GZip,

        /// <summary>
        /// Don't compress.
        /// </summary>
        None
    }
}