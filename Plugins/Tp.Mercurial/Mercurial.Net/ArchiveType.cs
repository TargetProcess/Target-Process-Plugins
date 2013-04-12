namespace Mercurial
{
    /// <summary>
    /// The type of archive to produce using the <see cref="ArchiveCommand"/>.
    /// </summary>
    public enum ArchiveType
    {
        /// <summary>
        /// Detect the type of archive automatically by the extension of the archive file.
        /// </summary>
        Automatic,

        /// <summary>
        /// A directory full of files.
        /// </summary>
        DirectoryWithFiles,

        /// <summary>
        /// Tar archive, uncompressed.
        /// </summary>
        TarUncompressed,

        /// <summary>
        /// Tar archive, compressed using bzip2.
        /// </summary>
        TarBZip2Compressed,

        /// <summary>
        /// Tar archive, compressed using gzip.
        /// </summary>
        TarGZipCompressed,

        /// <summary>
        /// Zip archive, uncompressed.
        /// </summary>
        ZipUncompressed,

        /// <summary>
        /// Zip archive, compressed using deflate.
        /// </summary>
        ZipDeflateCompressed,
    }
}