using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Mercurial.Versions
{
    /// <summary>
    /// This class, and its descendants implements version-specific features of Mercurial.
    /// </summary>
    public class MercurialVersionBase
    {
        /// <summary>
        /// This is the backing field for the <see cref="Current"/> property.
        /// </summary>
        /// <remarks>
        /// During startup, it will temporarily be assigned an instance of the <see cref="MercurialVersionBase"/>
        /// class, but this might change once the client detection logic has executed, figuring out the correct
        /// version to use.
        /// </remarks>
        private static MercurialVersionBase _Current = new MercurialVersionBase();

        /// <summary>
        /// Gets the <see cref="MercurialVersionBase"/> that implements support for the current Mercurial version,
        /// as identified by the <see cref="ClientExecutable"/> class.
        /// </summary>
        public static MercurialVersionBase Current
        {
            get
            {
                return _Current;
            }

            private set
            {
                _Current = value;
            }
        }

        /// <summary>
        /// This method finds the correct <see cref="MercurialVersionBase"/> implementation,
        /// based on the version.
        /// </summary>
        /// <param name="version">
        /// The version to find the correct <see cref="MercurialVersionBase"/> implementation for.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Internal error, unable to find correct implementation for Mercurial version <paramref name="version"/>.
        /// </exception>
        public static void AssignCurrent(Version version)
        {
            Current = GetImplementationFor(version);
            if (Current == null)
                throw new InvalidOperationException("Internal error, unable to find correct implementation for Mercurial " + version);
        }

        /// <summary>
        /// This method finds the correct <see cref="MercurialVersionBase"/> implementation,
        /// based on the version.
        /// </summary>
        /// <returns>
        /// A <see cref="MercurialVersionBase"/> (or descendant) instance appropriate for the
        /// specified <paramref name="version"/>.
        /// </returns>
        /// <param name="version">
        /// The version to find the correct <see cref="MercurialVersionBase"/> implementation for.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="version"/> is <c>null</c>.</para>
        /// </exception>
        public static MercurialVersionBase GetImplementationFor(Version version)
        {
            if (version == null)
                throw new ArgumentNullException("version");

            var implementations =
                from type in typeof(MercurialVersionBase).Assembly.GetTypes()
                where type.IsDefined(typeof(MercurialVersionAttribute), false)
                from MercurialVersionAttribute attr in type.GetCustomAttributes(typeof(MercurialVersionAttribute), false)
                orderby attr
                select new { type, attr };

            Type bestImplementation =
                (from implementation in implementations
                 where implementation.attr.IsMatch(version)
                 select implementation.type).FirstOrDefault();

            if (bestImplementation != null)
                return (MercurialVersionBase)Activator.CreateInstance(bestImplementation);

            return new MercurialVersionBase();
        }

        /// <summary>
        /// This method produces a collection of options and arguments to pass on the command line
        /// to specify the merge tool.
        /// </summary>
        /// <param name="tool">
        /// The merge tool to generate options and arguments for.
        /// </param>
        /// <returns>
        /// A collection of options and arguments to pass on the command line.
        /// </returns>
        public virtual IEnumerable<string> MergeToolOption(string tool)
        {
            if (StringEx.IsNullOrWhiteSpace(tool))
                yield break;

            yield return "--tool";
            yield return StringEx.EncapsulateInQuotesIfWhitespace(tool);
        }

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
        public virtual void WaitForLocksToDissipate(string repositoryPath)
        {
            if (StringEx.IsNullOrWhiteSpace(repositoryPath))
                throw new ArgumentNullException("repositoryPath");

            // Do nothing by default
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

            //
        }
    }
}