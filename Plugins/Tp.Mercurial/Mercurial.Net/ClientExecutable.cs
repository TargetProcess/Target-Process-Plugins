using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Mercurial.Configuration;
using Mercurial.Versions;

namespace Mercurial
{
    /// <summary>
    /// This class encapsulates the Mercurial client application.
    /// </summary>
    public static class ClientExecutable
    {
        /// <summary>
        /// If <c>true</c>, then the <see cref="LazyInitialize"/> method has executed, possibly unable
        /// to locate the client.
        /// </summary>
        private static bool _LazyInitializationExecuted;

        /// <summary>
        /// This is the backing field for the <see cref="ClientPath"/> property.
        /// </summary>
        private static string _ClientPath;

        /// <summary>
        /// This is the backing field for the <see cref="Configuration"/> property.
        /// </summary>
        private static ClientConfigurationCollection _Configuration;

        /// <summary>
        /// This is the backing field for the <see cref="CurrentVersion"/> property.
        /// </summary>
        private static Version _CurrentVersion;

        /// <summary>
        /// Gets a collection of supported versions of the Mercurial client.
        /// </summary>
        public static IEnumerable<Version> SupportedVersions
        {
            get
            {
                yield return new Version(1, 6, 0, 0);
                yield return new Version(1, 6, 1, 0);
                yield return new Version(1, 6, 2, 0);
                yield return new Version(1, 6, 3, 0);
                yield return new Version(1, 7, 0, 0);
                yield return new Version(1, 7, 1, 0);
                yield return new Version(1, 7, 2, 0);
                yield return new Version(1, 7, 3, 0);
                yield return new Version(1, 7, 4, 0);
                yield return new Version(1, 7, 5, 0);
                yield return new Version(1, 8, 0, 0);
                yield return new Version(1, 8, 1, 0);
                yield return new Version(1, 8, 2, 0);
                yield return new Version(1, 8, 3, 0);
                yield return new Version(1, 8, 4, 0);
                yield return new Version(1, 9, 0, 0);
            }
        }

        /// <summary>
        /// Gets the path to the Mercurial client executable.
        /// </summary>
        public static string ClientPath
        {
            get
            {
                LazyInitialize();
                return _ClientPath;
            }

            private set
            {
                _ClientPath = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Mercurial client executable could be located or not.
        /// </summary>
        public static bool CouldLocateClient
        {
            get
            {
                LazyInitialize();
                return !StringEx.IsNullOrWhiteSpace(ClientPath);
            }
        }

        /// <summary>
        /// Perform lazy on-demand initialization of locating the client and extracting its version.
        /// </summary>
        internal static void LazyInitialize()
        {
            if (!_LazyInitializationExecuted)
            {
                _LazyInitializationExecuted = true;
                ClientPath = LocateClient();
                Initialize();
            }
        }

        /// <summary>
        /// Gets the current client configuration.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The Mercurial client configuration is not available because the client executable could not be located.
        /// </exception>
        public static ClientConfigurationCollection Configuration
        {
            get
            {
                LazyInitialize();
                if (!CouldLocateClient)
                    throw new InvalidOperationException(
                        "The Mercurial client configuration is not available because the client executable could not be located");
                return _Configuration;
            }
        }

        /// <summary>
        /// Gets the version of the Mercurial client installed and in use, that was detected during
        /// startup of the Mercurial.Net library, or overriden through the use of <see cref="SetClientPath"/>.
        /// </summary>
        /// <remarks>
        /// Note that this value is cached from startup/override time, and does not execute the executable when
        /// you read it. If you want a fresh, up-to-date value, use the <see cref="GetVersion"/> method instead.
        /// </remarks>
        public static Version CurrentVersion
        {
            get
            {
                LazyInitialize();
                return _CurrentVersion;
            }

            private set
            {
                _CurrentVersion = value;
            }
        }

        /// <summary>
        /// Initializes local data structures from the Mercurial client.
        /// </summary>
        private static void Initialize()
        {
            if (!StringEx.IsNullOrWhiteSpace(ClientPath))
            {
                _Configuration = new ClientConfigurationCollection();
                CurrentVersion = GetVersion();
                MercurialVersionBase.AssignCurrent(CurrentVersion);

                _Configuration.Refresh();
            }
            else
            {
                _Configuration = null;
                CurrentVersion = null;
            }
        }

        /// <summary>
        /// Assigns a specific client path to the Mercurial.Net library, allowing
        /// the program that uses the library to select the applicable
        /// Mercurial version to use, or even to come bundled with its own Mercurial
        /// client files.
        /// </summary>
        /// <param name="clientPath">
        /// The full path to the folder and file that is the
        /// Mercurial command line executable that should be used.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="clientPath"/> is <c>null</c> or empty.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><paramref name="clientPath"/> does not point to the Mercurial executable.</para>
        /// </exception>
        public static void SetClientPath(string clientPath)
        {
            if (StringEx.IsNullOrWhiteSpace(clientPath))
                throw new ArgumentNullException("clientPath");

            if (Directory.Exists(clientPath) && File.Exists(Path.Combine(clientPath, "hg.exe")))
                clientPath = Path.Combine(clientPath, "hg.exe");
            if (!File.Exists(clientPath))
                throw new ArgumentException("The specified path does not contain the Mercurial executable");
            ClientPath = clientPath;
            Initialize();
        }

        /// <summary>
        /// Gets the version of the Mercurial client installed and in use. Note that <see cref="System.Version.Revision"/>
        /// and <see cref="System.Version.Build"/> can be 0 for major and minor versions of Mercurial.
        /// </summary>
        /// <returns>
        /// The <see cref="System.Version"/> of the Mercurial client installed and in use.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// <para>Unable to find or interpret version number from the Mercurial client.</para>
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Since reading the version means executing an external program, a property is not the way to go.")]
        public static Version GetVersion()
        {
            LazyInitialize();
            var command = new VersionCommand();
            NonPersistentClient.Execute(command);
            string firstLine = command.Result.Split(
                new[]
                {
                    '\n', '\r'
                }, StringSplitOptions.RemoveEmptyEntries).First();
            var re = new Regex(@"\(version\s+(?<version>[0-9.]+)(\+\d+-[a-f0-9]+)?\)", RegexOptions.IgnoreCase);
            Match ma = re.Match(firstLine);
            if (!ma.Success)
                throw new InvalidOperationException(
                    string.Format(CultureInfo.InvariantCulture, "Unable to locate Mercurial version number in '{0}'", firstLine));

            string versionString = ma.Groups["version"].Value;
            switch (versionString.Split('.').Length)
            {
                case 1:
                    return new Version(string.Format(CultureInfo.InvariantCulture, "{0}.0.0.0", versionString));

                case 2:
                    return new Version(string.Format(CultureInfo.InvariantCulture, "{0}.0.0", versionString));

                case 3:
                    return new Version(string.Format(CultureInfo.InvariantCulture, "{0}.0", versionString));

                case 4:
                    return new Version(versionString);

                default:
                    throw new InvalidOperationException(
                        string.Format(CultureInfo.InvariantCulture, "Incorrect version number length, too many or too few parts: {0}", versionString));
            }
        }

        /// <summary>
        /// Initializes a repository remotely, through a server which supports such functionality.
        /// </summary>
        /// <param name="location">
        /// The url of the repository to initialize remotely.
        /// </param>
        /// <param name="command">
        /// Any extra options to the init method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="location"/> is <c>null</c> or empty.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><see cref="RemoteInitCommand.Location"/> cannot be set before calling this method.</para>
        /// </exception>
        public static void RemoteInit(string location, RemoteInitCommand command = null)
        {
            if (StringEx.IsNullOrWhiteSpace(location))
                throw new ArgumentNullException("location");
            if (command != null && !StringEx.IsNullOrWhiteSpace(command.Location))
                throw new ArgumentException("RemoteInitCommand.Location cannot be set before calling this method", "command");

            command = (command ?? new RemoteInitCommand())
                .WithLocation(location);

            NonPersistentClient.Execute(command);
        }

        /// <summary>
        /// Initializes a repository remotely, through a server which supports such functionality.
        /// </summary>
        /// <param name="command">
        /// The options to the init method.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="command"/> is <c>null</c>.</para>
        /// </exception>
        public static void RemoteInit(RemoteInitCommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            NonPersistentClient.Execute(command);
        }

        /// <summary>
        /// Attempts to locate the command line client executable.
        /// </summary>
        /// <returns>
        /// The full path to the client executable, or <c>null</c> if it could not be located.
        /// </returns>
        private static string LocateClient()
        {
            string environmentPath = Environment.GetEnvironmentVariable("PATH");
            if (string.IsNullOrEmpty(environmentPath))
                return null;

            return (from path in environmentPath.Split(';')
                    where !StringEx.IsNullOrWhiteSpace(path)
                    let executablePath = Path.Combine(path.Trim(), "hg.exe")
                    where File.Exists(executablePath)
                    select executablePath).FirstOrDefault();
        }
    }
}