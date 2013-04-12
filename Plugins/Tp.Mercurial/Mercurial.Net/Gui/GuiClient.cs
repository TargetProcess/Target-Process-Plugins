using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Mercurial.Gui.Clients;

namespace Mercurial.Gui
{
    /// <summary>
    /// This class encapsulates the Mercurial TortoiseHg client application.
    /// </summary>
    public static class GuiClient
    {
        /// <summary>
        /// This is the backing field for the <see cref="ClientType"/> property.
        /// </summary>
        private static GuiClientType _ClientType;

        /// <summary>
        /// Initializes static members of the <see cref="GuiClient"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "To avoid calling the same methods multiple times, this is done through a static constructor.")]
        static GuiClient()
        {
            KeyValuePair<GuiClientType, string> result = LocateClient(null);
            ClientPath = result.Value;
            _ClientType = result.Key;

            Initialize();
        }

        /// <summary>
        /// Initializes local data structures from the TortoiseHg client.
        /// </summary>
        private static void Initialize()
        {
            if (!StringEx.IsNullOrWhiteSpace(ClientPath))
            {
                CurrentVersion = GetVersion();
                TortoiseHgClient.AssignCurrent(_ClientType);
            }
            else
                CurrentVersion = null;
        }

        /// <summary>
        /// Attempts to locate the command line client executable.
        /// </summary>
        /// <param name="type">
        /// Which client type to attempt to locate; or <c>null</c> to locate any.
        /// </param>
        /// <returns>
        /// A <see cref="KeyValuePair{TKey,TValue}"/> containing the type of client
        /// located and the full path to the client executable, or an empty value if no client could be located.
        /// </returns>
        private static KeyValuePair<GuiClientType, string> LocateClient(GuiClientType? type)
        {
            IEnumerable<string> paths =
                from path in Environment.GetEnvironmentVariable("PATH").Split(';')
                where !StringEx.IsNullOrWhiteSpace(path)
                select path.Trim();

            if (type == null || type == GuiClientType.PyQT)
            {
                string client =
                    (from path in paths
                     let executablePath = Path.Combine(path, "thg.exe")
                     where File.Exists(executablePath)
                     select executablePath).FirstOrDefault();
                if (client != null)
                    return new KeyValuePair<GuiClientType, string>(GuiClientType.PyQT, client);
            }

            if (type == null || type == GuiClientType.PyGTK)
            {
                string client =
                    (from path in paths
                     let executablePath = Path.Combine(path, "hgtk.exe")
                     where File.Exists(executablePath)
                     select executablePath).FirstOrDefault();
                if (client != null)
                    return new KeyValuePair<GuiClientType, string>(GuiClientType.PyGTK, client);
            }

            return new KeyValuePair<GuiClientType, string>(GuiClientType.None, string.Empty);
        }

        /// <summary>
        /// Determines if the specified client type is available on this system.
        /// </summary>
        /// <param name="clientType">
        /// The type of client to look for.
        /// </param>
        /// <returns>
        /// <c>true</c> if the client executable was located; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <para><paramref name="clientType"/> can not be <see cref="GuiClientType.None"/> or an undefined value.</para>
        /// </exception>
        public static bool GetClientAvailability(GuiClientType clientType)
        {
            if (clientType == GuiClientType.None)
                throw new ArgumentException("clientType cannot be GuiClientType.None");
            if (!Enum.IsDefined(typeof(GuiClientType), clientType))
                throw new ArgumentException("clientType must be a predefined value");

            return LocateClient(clientType).Key == clientType;
        }

        /// <summary>
        /// Gets the version of the TortoiseHg client installed and in use, that was detected during
        /// startup of the Mercurial.Net library.
        /// </summary>
        /// <remarks>
        /// Note that this value is cached from startup/override time, and does not execute the executable when
        /// you read it. If you want a fresh, up-to-date value, use the <see cref="GetVersion"/> method instead.
        /// </remarks>
        public static Version CurrentVersion
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the path to the TortoiseHg client executable.
        /// </summary>
        public static string ClientPath
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the version of the TortoiseHg client installed and in use. Note that <see cref="System.Version.Revision"/>
        /// and <see cref="System.Version.Build"/> can be 0 for major and minor versions of TortoiseHg.
        /// </summary>
        /// <returns>
        /// The <see cref="System.Version"/> of the TortoiseHg client installed and in use.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// <para>Unable to find or interpret version number from the TortoiseHg client.</para>
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Since reading the version means executing an external program, a property is not the way to go.")]
        public static Version GetVersion()
        {
            var command = new GuiVersionCommand();
            using (var repository = new Repository(Path.GetTempPath(), new NonPersistentClientFactory()))
            {
                Execute(repository, command);
            }
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
        /// Gets or sets which <see cref="GuiClientType">type of client</see> to use. Note that
        /// setting this property will attempt to locate that client, and if that can't be done,
        /// the property won't change and an <see cref="InvalidOperationException"/> will be thrown.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// <para>The chosen client is not available, cannot change the property.</para>
        /// </exception>
        public static GuiClientType ClientType
        {
            get
            {
                return _ClientType;
            }

            set
            {
                if (value == _ClientType || value == GuiClientType.None)
                    return;
                
                KeyValuePair<GuiClientType, string> client = LocateClient(value);
                if (client.Key != value)
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "The client type {0} is not available on this system", value));
                
                ClientPath = client.Value;
                _ClientType = client.Key;
            }
        }

        /// <summary>
        /// Open the repository explorer for the repository.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to open the explorer for.
        /// </param>
        /// <param name="command">
        /// Any extra options to the log method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// </exception>
        public static void LogGui(this Repository repository, LogGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            command = command ?? new LogGuiCommand();
            repository.Execute(command);
        }

        /// <summary>
        /// Executes the given <see cref="IGuiCommand"/> command against
        /// the Mercurial repository, asynchronously.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to execute the command in.
        /// </param>
        /// <param name="command">
        /// The <see cref="IGuiCommand"/> command to execute.
        /// </param>
        /// <param name="callback">
        /// A callback to call when the execution has completed. The <see cref="IAsyncResult.AsyncState"/> value of the
        /// <see cref="IAsyncResult"/> object passed to the <paramref name="callback"/> will be the
        /// <paramref name="command"/> object.
        /// </param>
        /// <returns>
        /// A <see cref="IAsyncResult"/> object to hold on to until the asynchronous execution has
        /// completed, and then pass to <see cref="EndExecute"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="command"/> is <c>null</c>.</para>
        /// </exception>
        public static IAsyncResult BeginExecute(this Repository repository, IGuiCommand command, AsyncCallback callback)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            if (command == null)
                throw new ArgumentNullException("command");

            var environmentVariables = new[]
            {
                new KeyValuePair<string, string>("LANGUAGE", "EN"),
                new KeyValuePair<string, string>("HGENCODING", "cp1252"),
            };

            return CommandProcessor.BeginExecute(
                repository.Path, ClientPath, command, environmentVariables, new string[0], callback);
        }

        /// <summary>
        /// Finalizes the asynchronous execution started with <see cref="BeginExecute"/>.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to finalize the command executionfor.
        /// </param>
        /// <param name="result">
        /// The <see cref="IAsyncResult"/> object returned from <see cref="BeginExecute"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="result"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><paramref name="result"/> is not a <see cref="IAsyncResult"/> that was returned from <see cref="BeginExecute"/>.</para>
        /// </exception>
        public static void EndExecute(this Repository repository, IAsyncResult result)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            if (result == null)
                throw new ArgumentNullException("result");

            CommandProcessor.EndExecute(result);
        }

        /// <summary>
        /// Executes the given <see cref="IGuiCommand"/> command against
        /// the Mercurial repository.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to execute the command in.
        /// </param>
        /// <param name="command">
        /// The <see cref="IGuiCommand"/> command to execute.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// <para><paramref name="command"/> is <c>null</c>.</para>
        /// </exception>
        public static void Execute(this Repository repository, IGuiCommand command)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            if (command == null)
                throw new ArgumentNullException("command");

            var environmentVariables = new[]
            {
                new KeyValuePair<string, string>("LANGUAGE", "EN"),
                new KeyValuePair<string, string>("HGENCODING", "cp1252"),
            };

            CommandProcessor.Execute(repository.Path, ClientPath, command, environmentVariables, new string[0]);
        }

        /// <summary>
        /// Add files to the repository.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to add files to.
        /// </param>
        /// <param name="command">
        /// Any extra options to the add method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// </exception>
        public static void AddGui(this Repository repository, AddGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            command = command ?? new AddGuiCommand();
            repository.Execute(command);
        }

        /// <summary>
        /// Show the about dialog for TortoiseHg.
        /// </summary>
        /// <param name="command">
        /// Any extra options to the about method, or <c>null</c> for default options.
        /// </param>
        public static void AboutGui(AboutGuiCommand command = null)
        {
            command = command ?? new AboutGuiCommand();
            using (var repository = new Repository(Path.GetTempPath(), new NonPersistentClientFactory()))
            {
                repository.Execute(command);
            }
        }

        /// <summary>
        /// Show the TortoiseHg dialog for cloning a repository.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to clone into.
        /// </param>
        /// <param name="command">
        /// Any extra options to the clone method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// </exception>
        public static void CloneGui(this Repository repository, CloneGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            command = command ?? new CloneGuiCommand();
            repository.Execute(command);
        }

        /// <summary>
        /// Show the TortoiseHg dialog for creating an unversioned archive from
        /// a working folder.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to archive from.
        /// </param>
        /// <param name="command">
        /// Any extra options to the archive method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// </exception>
        public static void ArchiveGui(this Repository repository, ArchiveGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            command = command ?? new ArchiveGuiCommand();
            repository.Execute(command);
        }

        /// <summary>
        /// Show the TortoiseHg dialog for commiting changes to the repository.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to commit to.
        /// </param>
        /// <param name="command">
        /// Any extra options to the commit method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// </exception>
        public static void CommitGui(this Repository repository, CommitGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            command = command ?? new CommitGuiCommand();
            repository.Execute(command);
        }

        /// <summary>
        /// Show the TortoiseHg dialog for datamining, searching the repository.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to datamine to.
        /// </param>
        /// <param name="command">
        /// Any extra options to the datamine method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// </exception>
        /// <remarks>
        /// This command is only available for the <see cref="GuiClientType.PyGTK"/> client type.
        /// </remarks>
        public static void DatamineGui(this Repository repository, DatamineGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            command = command ?? new DatamineGuiCommand();
            repository.Execute(command);
        }

        /// <summary>
        /// Show the TortoiseHg dialog for guessing previous renames or copies.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to guess renames and copies in.
        /// </param>
        /// <param name="command">
        /// Any extra options to the guess method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// </exception>
        public static void GuessGui(this Repository repository, GuessGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            command = command ?? new GuessGuiCommand();
            repository.Execute(command);
        }

        /// <summary>
        /// Show the TortoiseHg dialog for editing ignore filters for the repository.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to edit the ignore filters for.
        /// </param>
        /// <param name="command">
        /// Any extra options to the hgignore method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// </exception>
        public static void EditIgnoreFiltersGui(this Repository repository, EditIgnoreFiltersGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            command = command ?? new EditIgnoreFiltersGuiCommand();
            repository.Execute(command);
        }

        /// <summary>
        /// Show the TortoiseHg dialog for importing patches into the repository or the patch queue.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to import patches into.
        /// </param>
        /// <param name="command">
        /// Any extra options to the import method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// </exception>
        public static void ImportGui(this Repository repository, ImportGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            command = command ?? new ImportGuiCommand();
            repository.Execute(command);
        }

        /// <summary>
        /// Show the TortoiseHg dialog for initializing a new repository.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> folder to initialize.
        /// </param>
        /// <param name="command">
        /// Any extra options to the init method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// </exception>
        public static void InitGui(this Repository repository, InitGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            command = command ?? new InitGuiCommand();
            repository.Execute(command);
        }

        /// <summary>
        /// Show the TortoiseHg dialog for forgetting tracked files.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> folder to forget tracked files in.
        /// </param>
        /// <param name="command">
        /// Any extra options to the forget method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// </exception>
        public static void ForgetGui(this Repository repository, ForgetGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            command = command ?? new ForgetGuiCommand();
            repository.Execute(command);
        }

        /// <summary>
        /// Show the TortoiseHg dialog for merging another revision with the local one.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to merge in.
        /// </param>
        /// <param name="command">
        /// Any extra options to the merge method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// </exception>
        public static void MergeGui(this Repository repository, MergeGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            command = command ?? new MergeGuiCommand();
            repository.Execute(command);
        }

        /// <summary>
        /// Show the TortoiseHg dialog for recovery for the repository.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to execute the recovery command in.
        /// </param>
        /// <param name="command">
        /// Any extra options to the recovery method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// </exception>
        /// <remarks>
        /// This command is only available for the <see cref="GuiClientType.PyGTK"/> client type.
        /// </remarks>
        public static void RecoveryGui(this Repository repository, RecoveryGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            command = command ?? new RecoveryGuiCommand();
            repository.Execute(command);
        }

        /// <summary>
        /// Show the TortoiseHg file status dialog in revert mode, for removing or reverting files.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to remove files in.
        /// </param>
        /// <param name="command">
        /// Any extra options to the remove method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// </exception>
        public static void RemoveGui(this Repository repository, RemoveGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            command = command ?? new RemoveGuiCommand();
            repository.Execute(command);
        }

        /// <summary>
        /// Show the TortoiseHg annotate dialog.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to annotate in.
        /// </param>
        /// <param name="command">
        /// Any extra options to the annotate method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// <para> - or -</para>
        /// <para><paramref name="command"/> is <c>null</c>.</para>
        /// </exception>
        public static void AnnotateGui(this Repository repository, AnnotateGuiCommand command)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            if (command == null)
                throw new ArgumentNullException("command");

            repository.Execute(command);
        }

        /// <summary>
        /// Show the TortoiseHg annotate dialog.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to annotate in.
        /// </param>
        /// <param name="file">
        /// The file to annotate.
        /// </param>
        /// <param name="command">
        /// Any extra options to the annotate method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="file"/> is <c>null</c> or empty.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><see cref="AnnotateGuiCommand.File"/> cannot be set before calling this method.</para>
        /// </exception>
        /// <remarks>
        /// This command is only available for the <see cref="GuiClientType.PyQT"/> client type.
        /// </remarks>
        public static void AnnotateGui(this Repository repository, string file, AnnotateGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            if (StringEx.IsNullOrWhiteSpace(file))
                throw new ArgumentNullException("file");
            if (command != null && !StringEx.IsNullOrWhiteSpace(command.File))
                throw new ArgumentException("AnnotateGuiCommand.File cannot be set before calling this method", "command");

            command = (command ?? new AnnotateGuiCommand())
                .WithFile(file);

            repository.Execute(command);
        }

        /// <summary>
        /// Copy the selected files to the desired directory.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to copy the files in.
        /// </param>
        /// <param name="file">
        /// The file to copy.
        /// </param>
        /// <param name="destination">
        /// The destination directory.
        /// </param>
        /// <param name="command">
        /// Any extra options to the drag_copy method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="file"/> is <c>null</c> or empty.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><see cref="DragCopyMoveGuiCommandBase{T}.Destination"/> cannot be set before calling this method.</para>
        /// </exception>
        /// <remarks>
        /// This command is only available for the <see cref="GuiClientType.PyQT"/> client type.
        /// </remarks>
        public static void DragCopyGui(this Repository repository, string file, string destination, DragCopyGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            if (StringEx.IsNullOrWhiteSpace(file))
                throw new ArgumentNullException("file");
            if (StringEx.IsNullOrWhiteSpace(destination))
                throw new ArgumentNullException("destination");
            if (command != null && !StringEx.IsNullOrWhiteSpace(command.Destination))
                throw new ArgumentException("DragCopyGuiCommand.Destination cannot be set before calling this method", "command");

            command = (command ?? new DragCopyGuiCommand())
                .WithSourceFiles(file)
                .WithDestination(destination);

            repository.Execute(command);
        }

        /// <summary>
        /// Copy the selected files to the desired directory.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to copy the files in.
        /// </param>
        /// <param name="command">
        /// The options to the drag_copy method.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="command"/> is <c>null</c>.</para>
        /// </exception>
        /// <remarks>
        /// This command is only available for the <see cref="GuiClientType.PyQT"/> client type.
        /// </remarks>
        public static void DragCopyGui(this Repository repository, DragCopyGuiCommand command)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            if (command == null)
                throw new ArgumentNullException("command");

            repository.Execute(command);
        }

        /// <summary>
        /// Move the selected files to the desired directory.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to move the files in.
        /// </param>
        /// <param name="file">
        /// The file to move.
        /// </param>
        /// <param name="destination">
        /// The destination directory.
        /// </param>
        /// <param name="command">
        /// Any extra options to the drag_move method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="file"/> is <c>null</c> or empty.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><see cref="DragCopyMoveGuiCommandBase{T}.Destination"/> cannot be set before calling this method.</para>
        /// </exception>
        /// <remarks>
        /// This command is only available for the <see cref="GuiClientType.PyQT"/> client type.
        /// </remarks>
        public static void DragMoveGui(this Repository repository, string file, string destination, DragMoveGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            if (StringEx.IsNullOrWhiteSpace(file))
                throw new ArgumentNullException("file");
            if (StringEx.IsNullOrWhiteSpace(destination))
                throw new ArgumentNullException("destination");
            if (command != null && !StringEx.IsNullOrWhiteSpace(command.Destination))
                throw new ArgumentException("DragMoveGuiCommand.Destination cannot be set before calling this method", "command");

            command = (command ?? new DragMoveGuiCommand())
                .WithSourceFiles(file)
                .WithDestination(destination);

            repository.Execute(command);
        }

        /// <summary>
        /// Move the selected files to the desired directory.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to move the files in.
        /// </param>
        /// <param name="command">
        /// The options to the drag_move method.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="command"/> is <c>null</c>.</para>
        /// </exception>
        /// <remarks>
        /// This command is only available for the <see cref="GuiClientType.PyQT"/> client type.
        /// </remarks>
        public static void DragMoveGui(this Repository repository, DragMoveGuiCommand command)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            if (command == null)
                throw new ArgumentNullException("command");

            repository.Execute(command);
        }

        /// <summary>
        /// Send changesets by email.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to send email from.
        /// </param>
        /// <param name="command">
        /// Any extra options to the email method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// </exception>
        /// <remarks>
        /// This command is only available for the <see cref="GuiClientType.PyQT"/> client type.
        /// </remarks>
        public static void EmailGui(this Repository repository, EmailGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            command = command ?? new EmailGuiCommand();

            repository.Execute(command);
        }

        /// <summary>
        /// Grep/search the repository.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to search in.
        /// </param>
        /// <param name="command">
        /// Any extra options to the search method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// </exception>
        /// <remarks>
        /// This command is only available for the <see cref="GuiClientType.PyQT"/> client type.
        /// </remarks>
        public static void SearchGui(this Repository repository, SearchGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            command = command ?? new SearchGuiCommand();

            repository.Execute(command);
        }

        /// <summary>
        /// Display the current or given revision of the project manifest.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to open the manifest for.
        /// </param>
        /// <param name="command">
        /// Any extra options to the manifest method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// </exception>
        /// <remarks>
        /// This command is only available for the <see cref="GuiClientType.PyQT"/> client type.
        /// </remarks>
        public static void ManifestGui(this Repository repository, ManifestGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            command = command ?? new ManifestGuiCommand();

            repository.Execute(command);
        }

        /// <summary>
        /// Attempt to resolve conflicts in a .rej file.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to resolve conflicts in.
        /// </param>
        /// <param name="file">
        /// The file to resolve conflicts in.
        /// </param>
        /// <param name="command">
        /// Any extra options to the mpatch method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="file"/> is <c>null</c> or empty.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><see cref="MPatchGuiCommand.File"/> cannot be set before calling this method.</para>
        /// </exception>
        public static void MPatchGui(this Repository repository, string file, MPatchGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            if (StringEx.IsNullOrWhiteSpace(file))
                throw new ArgumentNullException("file");
            if (command != null && !StringEx.IsNullOrWhiteSpace(command.File))
                throw new ArgumentException("MPatchGuiCommand.File cannot be set before calling this method", "command");

            command = (command ?? new MPatchGuiCommand())
                .WithFile(file);

            repository.Execute(command);
        }

        /// <summary>
        /// Attempt to resolve conflicts in a .rej file.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to resolve conflicts in.
        /// </param>
        /// <param name="command">
        /// The options to the mpatch method.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// </exception>
        /// <remarks>
        /// This command is only available for the <see cref="GuiClientType.PyGTK"/> client type.
        /// </remarks>
        public static void MPatchGui(this Repository repository, MPatchGuiCommand command)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            command = command ?? new MPatchGuiCommand();

            repository.Execute(command);
        }

        /// <summary>
        /// Show the Mercurial queue tool.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to open the queue tool for.
        /// </param>
        /// <param name="command">
        /// Any extra options to the mq method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// </exception>
        /// <remarks>
        /// This command is only available for the <see cref="GuiClientType.PyQT"/> client type.
        /// </remarks>
        public static void QueueGui(this Repository repository, QueueGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            command = command ?? new QueueGuiCommand();

            repository.Execute(command);
        }

        /// <summary>
        /// Purge unknown and/or ignore files from repository.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to purge files in.
        /// </param>
        /// <param name="command">
        /// Any extra options to the purge method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// </exception>
        /// <remarks>
        /// This command is only available for the <see cref="GuiClientType.PyQT"/> client type.
        /// </remarks>
        public static void PurgeGui(this Repository repository, PurgeGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            command = command ?? new PurgeGuiCommand();

            repository.Execute(command);
        }

        /// <summary>
        /// Manage multiple MQ patch queues.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to manage patch queues for.
        /// </param>
        /// <param name="command">
        /// Any extra options to the qqueue method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// </exception>
        /// <remarks>
        /// This command is only available for the <see cref="GuiClientType.PyQT"/> client type.
        /// </remarks>
        public static void QueueManagerGui(this Repository repository, QueueManagerGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            command = command ?? new QueueManagerGuiCommand();

            repository.Execute(command);
        }

        /// <summary>
        /// Reorder unapplied MQ patches.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to reorder patches in.
        /// </param>
        /// <param name="command">
        /// Any extra options to the qreorder method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// </exception>
        /// <remarks>
        /// This command is only available for the <see cref="GuiClientType.PyQT"/> client type.
        /// </remarks>
        public static void QueueReorderGui(this Repository repository, QueueReorderGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            command = command ?? new QueueReorderGuiCommand();

            repository.Execute(command);
        }

        /// <summary>
        /// Rebase changesets.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to rebase changesets in.
        /// </param>
        /// <param name="sourceRevision">
        /// The source revision to rebase. This changeset, and all its descendants, will be rebased.
        /// </param>
        /// <param name="destinationRevision">
        /// The Destination revision to rebase. This changeset will be the parent of the changesets after the rebase.
        /// </param>
        /// <param name="command">
        /// Any extra options to the rebase method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="sourceRevision"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="destinationRevision"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><see cref="RebaseGuiCommand.SourceRevision"/> cannot be set before calling this method.</para>
        /// <para>- or -</para>
        /// <para><see cref="RebaseGuiCommand.DestinationRevision"/> cannot be set before calling this method.</para>
        /// </exception>
        /// <remarks>
        /// This command is only available for the <see cref="GuiClientType.PyQT"/> client type.
        /// </remarks>
        public static void RebaseGui(this Repository repository, RevSpec sourceRevision, RevSpec destinationRevision, RebaseGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            if (sourceRevision == null)
                throw new ArgumentNullException("sourceRevision");
            if (destinationRevision == null)
                throw new ArgumentNullException("destinationRevision");
            if (command != null && command.SourceRevision != null)
                throw new ArgumentException("RebaseGuiCommand.SourceRevision cannot be set before calling this method", "command");
            if (command != null && command.DestinationRevision != null)
                throw new ArgumentException("RebaseGuiCommand.DestinationRevision cannot be set before calling this method", "command");

            command = (command ?? new RebaseGuiCommand())
                .WithSourceRevision(sourceRevision)
                .WithDestinationRevision(destinationRevision);

            repository.Execute(command);
        }

        /// <summary>
        /// Rebase changesets.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to rebase changesets in.
        /// </param>
        /// <param name="command">
        /// The options to the rebase method.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="command"/> is <c>null</c>.</para>
        /// </exception>
        /// <remarks>
        /// This command is only available for the <see cref="GuiClientType.PyQT"/> client type.
        /// </remarks>
        public static void RebaseGui(this Repository repository, RebaseGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            if (command == null)
                throw new ArgumentNullException("command");

            repository.Execute(command);
        }

        /// <summary>
        /// Manually resolve rejected patch chunks.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to resolve rejected patch chunks in.
        /// </param>
        /// <param name="file">
        /// The file to resolve rejected patch chunks in.
        /// </param>
        /// <param name="command">
        /// Any extra options to the rejects method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="file"/> is <c>null</c> or empty.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><see cref="RejectsGuiCommand.File"/> cannot be set before calling this method.</para>
        /// </exception>
        /// <remarks>
        /// This command is only available for the <see cref="GuiClientType.PyQT"/> client type.
        /// </remarks>
        public static void RejectsGui(this Repository repository, string file, RejectsGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            if (StringEx.IsNullOrWhiteSpace(file))
                throw new ArgumentNullException("file");
            if (command != null && !StringEx.IsNullOrWhiteSpace(command.File))
                throw new ArgumentException("RejectsGuiCommand.File cannot be set before calling this method", "command");

            command = (command ?? new RejectsGuiCommand())
                .WithFile(file);

            repository.Execute(command);
        }

        /// <summary>
        /// Manually resolve rejected patch chunks.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to resolve rejected patch chunks in.
        /// </param>
        /// <param name="command">
        /// Any extra options to the rejects method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="command"/> is <c>null</c>.</para>
        /// </exception>
        public static void RejectsGui(this Repository repository, RejectsGuiCommand command)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            if (command == null)
                throw new ArgumentNullException("command");

            repository.Execute(command);
        }

        /// <summary>
        /// Backout tool.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to backout a changeset in.
        /// </param>
        /// <param name="command">
        /// Any extra options to the backout method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// </exception>
        /// <remarks>
        /// This command is only available for the <see cref="GuiClientType.PyQT"/> client type.
        /// </remarks>
        public static void BackoutGui(this Repository repository, BackoutGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            command = command ?? new BackoutGuiCommand();

            repository.Execute(command);
        }

        /// <summary>
        /// Bisect tool.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to bisect in.
        /// </param>
        /// <param name="command">
        /// Any extra options to the bisect method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// </exception>
        /// <remarks>
        /// This command is only available for the <see cref="GuiClientType.PyQT"/> client type.
        /// </remarks>
        public static void BisectGui(this Repository repository, BisectGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            command = command ?? new BisectGuiCommand();

            repository.Execute(command);
        }

        /// <summary>
        /// Shows the resolve dialog.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to resolve merge conflicts in.
        /// </param>
        /// <param name="command">
        /// Any extra options to the resolve method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// </exception>
        /// <remarks>
        /// This command is only available for the <see cref="GuiClientType.PyQT"/> client type.
        /// </remarks>
        public static void ResolveGui(this Repository repository, ResolveGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            command = command ?? new ResolveGuiCommand();

            repository.Execute(command);
        }

        /// <summary>
        /// Show the copy file dialog.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to copy files in.
        /// </param>
        /// <param name="source">
        /// The source file to copy.
        /// </param>
        /// <param name="destination">
        /// The destination to copy it to.
        /// </param>
        /// <param name="command">
        /// Any extra options to the copy method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="source"/> is <c>null</c> or empty.</para>
        /// <para>- or -</para>
        /// <para><paramref name="destination"/> is <c>null</c> or empty.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><see cref="MoveCopyRenameGuiCommandBase{T}.Source"/> cannot be set before calling this method.</para>
        /// <para>- or -</para>
        /// <para><see cref="MoveCopyRenameGuiCommandBase{T}.Destination"/> cannot be set before calling this method.</para>
        /// </exception>
        /// <remarks>
        /// This command is only available for the <see cref="GuiClientType.PyQT"/> client type.
        /// </remarks>
        public static void CopyGui(this Repository repository, string source, string destination, CopyGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            if (StringEx.IsNullOrWhiteSpace(source))
                throw new ArgumentNullException("source");
            if (StringEx.IsNullOrWhiteSpace(destination))
                throw new ArgumentNullException("destination");
            if (command != null && !StringEx.IsNullOrWhiteSpace(command.Source))
                throw new ArgumentException("CopyGuiCommand.Source cannot be set before calling this method", "command");
            if (command != null && !StringEx.IsNullOrWhiteSpace(command.Destination))
                throw new ArgumentException("CopyGuiCommand.Destination cannot be set before calling this method", "command");

            command = (command ?? new CopyGuiCommand())
                .WithSource(source)
                .WithDestination(destination);

            repository.Execute(command);
        }

        /// <summary>
        /// Show the copy file dialog.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to copy files in.
        /// </param>
        /// <param name="command">
        /// Any extra options to the copy method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// </exception>
        /// <remarks>
        /// This command is only available for the <see cref="GuiClientType.PyQT"/> client type.
        /// </remarks>
        public static void CopyGui(this Repository repository, CopyGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            command = command ?? new CopyGuiCommand();

            repository.Execute(command);
        }

        /// <summary>
        /// Show the move file dialog.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to move files in.
        /// </param>
        /// <param name="source">
        /// The source file to move.
        /// </param>
        /// <param name="destination">
        /// The destination to move it to.
        /// </param>
        /// <param name="command">
        /// Any extra options to the mv method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="source"/> is <c>null</c> or empty.</para>
        /// <para>- or -</para>
        /// <para><paramref name="destination"/> is <c>null</c> or empty.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><see cref="MoveCopyRenameGuiCommandBase{T}.Source"/> cannot be set before calling this method.</para>
        /// <para>- or -</para>
        /// <para><see cref="MoveCopyRenameGuiCommandBase{T}.Destination"/> cannot be set before calling this method.</para>
        /// </exception>
        public static void MoveGui(this Repository repository, string source, string destination, MoveGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            if (StringEx.IsNullOrWhiteSpace(source))
                throw new ArgumentNullException("source");
            if (StringEx.IsNullOrWhiteSpace(destination))
                throw new ArgumentNullException("destination");

            command = (command ?? new MoveGuiCommand())
                .WithSource(source)
                .WithDestination(destination);

            repository.Execute(command);
        }

        /// <summary>
        /// Show the move file dialog.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to move files in.
        /// </param>
        /// <param name="command">
        /// Any extra options to the mv method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// </exception>
        public static void MoveGui(this Repository repository, MoveGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            command = command ?? new MoveGuiCommand();

            repository.Execute(command);
        }

        /// <summary>
        /// Show the rename file dialog.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to rename files in.
        /// </param>
        /// <param name="source">
        /// The source file to rename.
        /// </param>
        /// <param name="destination">
        /// The destination to rename it to.
        /// </param>
        /// <param name="command">
        /// Any extra options to the rename method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="source"/> is <c>null</c> or empty.</para>
        /// <para>- or -</para>
        /// <para><paramref name="destination"/> is <c>null</c> or empty.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><see cref="MoveCopyRenameGuiCommandBase{T}.Source"/> cannot be set before calling this method.</para>
        /// <para>- or -</para>
        /// <para><see cref="MoveCopyRenameGuiCommandBase{T}.Destination"/> cannot be set before calling this method.</para>
        /// </exception>
        public static void RenameGui(this Repository repository, string source, string destination, RenameGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            if (StringEx.IsNullOrWhiteSpace(source))
                throw new ArgumentNullException("source");
            if (StringEx.IsNullOrWhiteSpace(destination))
                throw new ArgumentNullException("destination");

            command = (command ?? new RenameGuiCommand())
                .WithSource(source)
                .WithDestination(destination);

            repository.Execute(command);
        }

        /// <summary>
        /// Show the rename file dialog.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to rename files in.
        /// </param>
        /// <param name="command">
        /// Any extra options to the rename method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// </exception>
        public static void RenameGui(this Repository repository, RenameGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            command = command ?? new RenameGuiCommand();

            repository.Execute(command);
        }

        /// <summary>
        /// Revert selected files.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to revert files in.
        /// </param>
        /// <param name="command">
        /// Any extra options to the revert method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// </exception>
        public static void RevertGui(this Repository repository, RevertGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            command = command ?? new RevertGuiCommand();

            repository.Execute(command);
        }

        /// <summary>
        /// Open the Windows Explorer extension configuration editor.
        /// </summary>
        /// <param name="command">
        /// Any extra options to the shellconfig method, or <c>null</c> for default options.
        /// </param>
        public static void ShellConfigGui(ShellConfigGuiCommand command = null)
        {
            command = command ?? new ShellConfigGuiCommand();

            using (var repository = new Repository(Path.GetTempPath(), new NonPersistentClientFactory()))
            {
                repository.Execute(command);
            }
        }

        /// <summary>
        /// Open the user configuration editor.
        /// </summary>
        /// <param name="command">
        /// Any extra options to the userconfig method, or <c>null</c> for default options.
        /// </param>
        public static void UserConfigGui(UserConfigGuiCommand command = null)
        {
            command = command ?? new UserConfigGuiCommand();

            using (var repository = new Repository(Path.GetTempPath(), new NonPersistentClientFactory()))
            {
                repository.Execute(command);
            }
        }

        /// <summary>
        /// Show the repository configuration editor.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to edit the configuration for.
        /// </param>
        /// <param name="command">
        /// Any extra options to the repoconfig method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// </exception>
        public static void RepoConfigGui(this Repository repository, RepoConfigGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            command = command ?? new RepoConfigGuiCommand();

            repository.Execute(command);
        }

        /// <summary>
        /// Open the shelve tool for the repository.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to open the shelve tool for.
        /// </param>
        /// <param name="command">
        /// Any extra options to the shelve method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// </exception>
        public static void ShelveGui(this Repository repository, ShelveGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            command = command ?? new ShelveGuiCommand();

            repository.Execute(command);
        }

        /// <summary>
        /// Browse working directory status.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to browse the status of.
        /// </param>
        /// <param name="command">
        /// Any extra options to the status method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// </exception>
        public static void StatusGui(this Repository repository, StatusGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            command = command ?? new StatusGuiCommand();

            repository.Execute(command);
        }

        /// <summary>
        /// Strip changesets from the repository.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to strip changesets from.
        /// </param>
        /// <param name="revision">
        /// The <see cref="RevSpec"/> of the revision to strip.
        /// </param>
        /// <param name="command">
        /// Any extra options to the strip method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="revision"/> is <c>null</c>.</para>
        /// </exception>
        public static void StripGui(this Repository repository, RevSpec revision, StripGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            if (revision == null)
                throw new ArgumentNullException("revision");

            command = (command ?? new StripGuiCommand())
                .WithRevision(revision);

            repository.Execute(command);
        }

        /// <summary>
        /// Strip changesets from the repository.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to strip changesets from.
        /// </param>
        /// <param name="command">
        /// Any extra options to the strip method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// </exception>
        public static void StripGui(this Repository repository, StripGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            command = command ?? new StripGuiCommand();

            repository.Execute(command);
        }

        /// <summary>
        /// Synchronize the repository with other repositories.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to synchronize.
        /// </param>
        /// <param name="command">
        /// Any extra options to the synch method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// </exception>
        public static void SynchronizeGui(this Repository repository, SynchronizeGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            command = command ?? new SynchronizeGuiCommand();

            repository.Execute(command);
        }

        /// <summary>
        /// Open the Tag gui.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to synchronize.
        /// </param>
        /// <param name="revision">
        /// The <see cref="RevSpec"/> of the revision to tag.
        /// </param>
        /// <param name="name">
        /// The name of the tag.
        /// </param>
        /// <param name="command">
        /// Any extra options to the tag method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="revision"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="name"/> is <c>null</c> or empty.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><see cref="TagGuiCommand.Revision"/> cannot be set before calling this method.</para>
        /// <para>- or -</para>
        /// <para><see cref="TagGuiCommand.Name"/> cannot be set before calling this method.</para>
        /// </exception>
        /// <remarks>
        /// This command is only available for the <see cref="GuiClientType.PyQT"/> client type.
        /// </remarks>
        public static void TagGui(this Repository repository, RevSpec revision, string name, TagGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            if (revision == null)
                throw new ArgumentNullException("revision");
            if (StringEx.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");
            if (command != null && command.Revision != null)
                throw new ArgumentException("TagGuiCommand.Revision cannot be set before calling this method", "command");
            if (command != null && !StringEx.IsNullOrWhiteSpace(command.Name))
                throw new ArgumentException("TagGuiCommand.Name cannot be set before calling this method", "command");

            command = (command ?? new TagGuiCommand())
                .WithRevision(revision)
                .WithName(name);

            repository.Execute(command);
        }

        /// <summary>
        /// Open the Tag gui.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to synchronize.
        /// </param>
        /// <param name="command">
        /// Any extra options to the tag method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// </exception>
        /// <remarks>
        /// This command is only available for the <see cref="GuiClientType.PyQT"/> client type.
        /// </remarks>
        public static void TagGui(this Repository repository, TagGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            command = command ?? new TagGuiCommand();

            repository.Execute(command);
        }

        /// <summary>
        /// Update/checkout changeset to working directory.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to update.
        /// </param>
        /// <param name="revision">
        /// The <see cref="RevSpec"/> of the revision to update to.
        /// </param>
        /// <param name="command">
        /// Any extra options to the tag method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><see cref="UpdateGuiCommand.Revision"/> cannot be set before calling this method.</para>
        /// </exception>
        public static void UpdateGui(this Repository repository, RevSpec revision, UpdateGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            if (revision == null)
                throw new ArgumentNullException("revision");
            if (command != null && !StringEx.IsNullOrWhiteSpace(command.Revision))
                throw new ArgumentException("UpdateGuiCommand.Revision cannot be set before calling this method", "command");

            command = (command ?? new UpdateGuiCommand())
                .WithRevision(revision);

            repository.Execute(command);
        }

        /// <summary>
        /// Update/checkout changeset to working directory.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to update.
        /// </param>
        /// <param name="command">
        /// Any extra options to the tag method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// </exception>
        public static void UpdateGui(this Repository repository, UpdateGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            command = command ?? new UpdateGuiCommand();

            repository.Execute(command);
        }

        /// <summary>
        /// Launch the visual diff tool.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to view diffs from.
        /// </param>
        /// <param name="revisions">
        /// The revision <see cref="RevSpec"/> that identifies the revision or the
        /// revision range to view a diff of.
        /// </param>
        /// <param name="command">
        /// Any extra options to the vdiff method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><see cref="DiffGuiCommand.Revisions"/> cannot be set before calling this method.</para>
        /// </exception>
        public static void DiffGui(this Repository repository, RevSpec revisions, DiffGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            if (revisions == null)
                throw new ArgumentNullException("revisions");
            if (command != null && !StringEx.IsNullOrWhiteSpace(command.Revisions))
                throw new ArgumentException("DiffGuiCommand.Revisions cannot be set before calling this method", "command");

            command = (command ?? new DiffGuiCommand())
                .WithRevisions(revisions);

            repository.Execute(command);
        }

        /// <summary>
        /// Launch the visual diff tool.
        /// </summary>
        /// <param name="repository">
        /// The <see cref="Repository"/> to view diffs from.
        /// </param>
        /// <param name="command">
        /// Any extra options to the vdiff method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repository"/> is <c>null</c>.</para>
        /// </exception>
        public static void DiffGui(this Repository repository, DiffGuiCommand command = null)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            command = command ?? new DiffGuiCommand();

            repository.Execute(command);
        }
    }
}