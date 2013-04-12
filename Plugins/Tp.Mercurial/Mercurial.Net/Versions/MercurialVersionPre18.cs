using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Mercurial.Versions
{
    /// <summary>
    /// This base class handles Mercurial &lt; 1.8.
    /// </summary>
    [SuppressMessage("Microsoft.Usage", "CA2243:AttributeStringLiteralsShouldParseCorrectly", Justification = "There is more advanced parsing here to handle version ranges, so this is not necessary.")]
    [MercurialVersion("", "1.7")]
    public class MercurialVersionPre18 : MercurialVersionBase
    {
        /// <summary>
        /// This method will wait for lingering POSIX-style file locks to dissipate before
        /// continuing, to get around problems with such locks in pre-1.8 Mercurial.
        /// </summary>
        /// <param name="repositoryPath">
        /// The path to the repository that the locks exists in.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repositoryPath"/> is <c>null</c> or empty.</para>
        /// </exception>
        public override void WaitForLocksToDissipate(string repositoryPath)
        {
            string repositoryFolderPath = Path.Combine(repositoryPath, ".hg");
            if (!Directory.Exists(repositoryFolderPath))
                return;

            string lockPath = Path.Combine(repositoryFolderPath, "lock");
            string wlockPath = Path.Combine(repositoryFolderPath, "wlock");

            foreach (string path in new[] { lockPath, wlockPath })
            {
                Stopwatch sw = Stopwatch.StartNew();
                try
                {
                    while (sw.ElapsedMilliseconds < 1000)
                    {
                        try
                        {
                            File.Create(path).Dispose();
                            break;
                        }
                        catch (IOException)
                        {
                            // swallow this
                        }
                    }
                }
                finally
                {
                    sw = Stopwatch.StartNew();
                    while (sw.ElapsedMilliseconds < 1000)
                    {
                        try
                        {
                            File.Delete(path);
                            break;
                        }
                        catch (IOException)
                        {
                            // swallow this
                        }
                    }
                }
            }
        }
    }
}