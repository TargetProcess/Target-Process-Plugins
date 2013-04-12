using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Remoting.Messaging;

namespace Mercurial
{
    /// <summary>
    /// This class encapsulates a repository on disk.
    /// </summary>
    public sealed class Repository : IDisposable
    {
        /// <summary>
        /// This field holds the <see cref="IClientFactory"/> this <see cref="Repository"/>
        /// will use to manage client instances.
        /// </summary>
        private readonly IClientFactory _ClientFactory;

        /// <summary>
        /// This is the backing field for the <see cref="Client"/> property.
        /// </summary>
        private IClient _Client;

        /// <summary>
        /// Initializes a new instance of the <see cref = "Repository" /> class.
        /// </summary>
        /// <param name="rootPath">
        /// The path where the repository is stored locally, or the
        /// path to a directory that will be initialized with a new
        /// repository.
        /// </param>
        /// <exception cref="MercurialMissingException">
        /// The Mercurial command line client could not be located.
        /// </exception>
        /// <exception cref = "ArgumentNullException">
        /// <para><paramref name = "rootPath" /> is <c>null</c> or empty.</para>
        /// </exception>
        /// <exception cref = "DirectoryNotFoundException">
        /// <para><paramref name = "rootPath" /> refers to a directory that does not exist.</para>
        /// </exception>
        /// <exception cref = "InvalidOperationException">
        /// <para><paramref name = "rootPath" /> refers to a directory that doesn't appear to contain
        /// a Mercurial repository (no .hg directory.)</para>
        /// </exception>
        public Repository(string rootPath)
            : this(rootPath, new AutoSwitchingClientFactory())
        {
            // Do nothing here
        }

        /// <summary>
        /// Initializes a new instance of the <see cref = "Repository" /> class.
        /// </summary>
        /// <param name="rootPath">
        /// The path where the repository is stored locally, or the
        /// path to a directory that will be initialized with a new
        /// repository.
        /// </param>
        /// <param name="clientFactory">
        /// The <see cref="IClientFactory"/> to use for this <see cref="Repository"/>.
        /// </param>
        /// <exception cref="MercurialMissingException">
        /// The Mercurial command line client could not be located.
        /// </exception>
        /// <exception cref = "ArgumentNullException">
        /// <para><paramref name = "rootPath" /> is <c>null</c> or empty.</para>
        /// <para>- or -</para>
        /// <para><paramref name = "clientFactory" /> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref = "DirectoryNotFoundException">
        /// <para><paramref name = "rootPath" /> refers to a directory that does not exist.</para>
        /// </exception>
        /// <exception cref = "InvalidOperationException">
        /// <para><paramref name = "rootPath" /> refers to a directory that doesn't appear to contain
        /// a Mercurial repository (no .hg directory.)</para>
        /// </exception>
        public Repository(string rootPath, IClientFactory clientFactory)
        {
            if (StringEx.IsNullOrWhiteSpace(rootPath))
                throw new ArgumentNullException("rootPath");
            if (!Directory.Exists(rootPath))
                throw new DirectoryNotFoundException("The specified directory for the Mercurial repository root does not exist");
            if (!ClientExecutable.CouldLocateClient)
                throw new MercurialMissingException("The Mercurial command line client could not be located");
            if (clientFactory == null)
                throw new ArgumentNullException("clientFactory");

            _ClientFactory = clientFactory;
            _Client = clientFactory.CreateClient(System.IO.Path.GetFullPath(rootPath));
        }

        /// <summary>
        /// Gets the path of the repository root.
        /// </summary>
        /// <value>
        /// The path of the repository root.
        /// </value>
        public string Path
        {
            get
            {
                return _Client.RepositoryPath;
            }
        }

        /// <summary>
        /// Gets the <see cref="IClient"/> object used by this <see cref="Repository"/>.
        /// </summary>
        public IClient Client
        {
            get
            {
                return _Client;
            }
        }

        /// <summary>
        /// Executes the given <see cref="IMercurialCommand"/> command asynchronously against
        /// the Mercurial repository, returning a <see cref="IAsyncResult"/> object
        /// keeping track of the execution.
        /// </summary>
        /// <param name="command">
        /// The <see cref="IMercurialCommand"/> command to execute.
        /// </param>
        /// <param name="callback">
        /// A callback to call when the execution has completed. The <see cref="IAsyncResult.AsyncState"/> value of the
        /// <see cref="IAsyncResult"/> object passed to the <paramref name="callback"/> will be the
        /// <paramref name="command"/> object.
        /// </param>
        /// <returns>
        /// A <see cref="IAsyncResult"/> object keeping track of the asynchronous execution.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="command"/> is <c>null</c>.</para>
        /// </exception>
        public IAsyncResult BeginExecute(IMercurialCommand command, AsyncCallback callback)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            var executeDelegate = new ExecuteDelegate(Execute);
            return executeDelegate.BeginInvoke(command, callback, command);
        }

        /// <summary>
        /// Executes the given <see cref="IMercurialCommand{TResult}"/> command asynchronously against
        /// the Mercurial repository, returning a <see cref="IAsyncResult{T}"/> object
        /// keeping track of the execution.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type of result that will be returned from executing the command.
        /// </typeparam>
        /// <param name="command">
        /// The <see cref="IMercurialCommand{T}"/> command to execute.
        /// </param>
        /// <param name="callback">
        /// A callback to call when the execution has completed. The <see cref="IAsyncResult.AsyncState"/> value of the
        /// <see cref="IAsyncResult"/> object passed to the <paramref name="callback"/> will be the
        /// <paramref name="command"/> object.
        /// </param>
        /// <returns>
        /// A <see cref="IAsyncResult"/> object keeping track of the asynchronous execution.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="command"/> is <c>null</c>.</para>
        /// </exception>
        public IAsyncResult<TResult> BeginExecute<TResult>(IMercurialCommand<TResult> command, AsyncCallback callback)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            var executeDelegate = new ExecuteDelegate<TResult>(Execute);
            IAsyncResult ar = executeDelegate.BeginInvoke(command, callback, command);
            return new AsyncResult<TResult>(ar);
        }

        /// <summary>
        /// Finalizes the asynchronous execution started with <see cref="BeginExecute"/>.
        /// </summary>
        /// <param name="result">
        /// The <see cref="IAsyncResult"/> object returned from <see cref="BeginExecute"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="result"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><paramref name="result"/> is not a <see cref="IAsyncResult"/> that was returned from <see cref="BeginExecute"/>.</para>
        /// </exception>
        public void EndExecute(IAsyncResult result)
        {
            if (result == null)
                throw new ArgumentNullException("result");
            var asyncResult = result as AsyncResult;
            if (asyncResult == null)
                throw new ArgumentException("invalid IAsyncResult object passed to CommandProcessor.EndExecute", "result");
            var executeDelegate = asyncResult.AsyncDelegate as ExecuteDelegate;
            if (executeDelegate == null)
                throw new ArgumentException("invalid IAsyncResult object passed to CommandProcessor.EndExecute", "result");
            executeDelegate.EndInvoke(result);
        }

        /// <summary>
        /// Finalizes the asynchronous execution started with <see cref="BeginExecute{T}"/>.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type of result of executing the command.
        /// </typeparam>
        /// <param name="result">
        /// The <see cref="IAsyncResult{T}"/> object returned from <see cref="BeginExecute{T}"/>.
        /// </param>
        /// <returns>
        /// The result of executing the command.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="result"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><paramref name="result"/> is not a <see cref="IAsyncResult"/> that was returned from <see cref="BeginExecute"/>.</para>
        /// </exception>
        public TResult EndExecute<TResult>(IAsyncResult<TResult> result)
        {
            if (result == null)
                throw new ArgumentNullException("result");

            var typedAsyncResult = result as AsyncResult<TResult>;
            if (typedAsyncResult == null)
                throw new ArgumentException("invalid IAsyncResult object passed to CommandProcessor.EndExecute", "result");
            var asyncResult = result.InnerResult as AsyncResult;
            if (asyncResult == null)
                throw new ArgumentException("invalid IAsyncResult object passed to CommandProcessor.EndExecute", "result");
            var executeDelegate = asyncResult.AsyncDelegate as ExecuteDelegate;
            if (executeDelegate == null)
                throw new ArgumentException("invalid IAsyncResult object passed to CommandProcessor.EndExecute", "result");
            executeDelegate.EndInvoke(result);

            return ((IMercurialCommand<TResult>)result.AsyncState).Result;
        }

        /// <summary>
        /// Executes the given <see cref="IMercurialCommand{TResult}"/> command against
        /// the Mercurial repository, returning the result as a typed value.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type of result that is returned from executing the command.
        /// </typeparam>
        /// <param name="command">
        /// The <see cref="IMercurialCommand{T}"/> command to execute.
        /// </param>
        /// <returns>
        /// The result of executing the command.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="command"/> is <c>null</c>.</para>
        /// </exception>
        public TResult Execute<TResult>(IMercurialCommand<TResult> command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            var switcher = _ClientFactory as IClientSwitcher;
            if (switcher != null)
                _Client = switcher.SwitchBeforeCommand(command, _Client);

            Client.Execute(command);

            if (switcher != null)
                _Client = switcher.SwitchAfterCommand(command, _Client);

            return command.Result;
        }

        /// <summary>
        /// Executes the given <see cref="IMercurialCommand"/> command against
        /// the Mercurial repository.
        /// </summary>
        /// <param name="command">
        /// The <see cref="IMercurialCommand"/> command to execute.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="command"/> is <c>null</c>.</para>
        /// </exception>
        public void Execute(IMercurialCommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            var switcher = _ClientFactory as IClientSwitcher;
            if (switcher != null)
                _Client = switcher.SwitchBeforeCommand(command, _Client);

            Client.Execute(command);

            if (switcher != null)
                _Client = switcher.SwitchAfterCommand(command, _Client);
        }

        /// <summary>
        /// Stops command executing
        /// </summary>
        public void CancelExecuting()
        {
            Client.CancelExecuting();
        }

        /// <summary>
        /// Gets all the changesets in the log.
        /// </summary>
        /// <param name="command">
        /// Any extra options to the log method, or <c>null</c> for default options.
        /// </param>
        /// <returns>
        /// A collection of <see cref = "Changeset" /> instances.
        /// </returns>
        public IEnumerable<Changeset> Log(LogCommand command = null)
        {
            command = command ?? new LogCommand();
            return Execute(command);
        }

        /// <summary>
        /// Gets all the changesets in the log.
        /// </summary>
        /// <param name="set">
        /// The <see cref="RevSpec"/> that specifies the set of revisions
        /// to include in the log. If <c>null</c>, return the whole log.
        /// Default is <c>null</c>.
        /// </param>
        /// <param name="command">
        /// Any extra options to the log method, or <c>null</c> for default options.
        /// </param>
        /// <returns>
        /// A collection of <see cref="Changeset" /> instances.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="set"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><paramref name="command"/>.<see cref="LogCommand.Revisions"/> is non-<c>null</c>, this is not valid for this method.</para>
        /// </exception>
        public IEnumerable<Changeset> Log(RevSpec set, LogCommand command = null)
        {
            if (set == null)
                throw new ArgumentNullException("set");
            if (command != null && command.Revisions != null)
                throw new ArgumentException("LogOptions.Revision cannot be set before calling this method");

            command = command ?? new LogCommand();
            command.Revisions.Add(set);
            return Execute(command);
        }

        /// <summary>
        /// Initializes a new repository in the directory this <see cref="Repository"/> refers to.
        /// </summary>
        /// <param name="command">
        /// Any extra options to the init method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="MercurialExecutionException">
        /// There was a problem executing the command, check the message for details.
        /// </exception>
        /// <exception cref="MercurialException">
        /// There was a problem with the command, check the message for details.
        /// </exception>
        public void Init(InitCommand command = null)
        {
            command = command ?? new InitCommand();
            Execute(command);
        }

        /// <summary>
        /// Commits the specified files or all outstanding changes to the repository.
        /// </summary>
        /// <param name="command">
        /// Any extra options to the commit method.
        /// </param>
        /// <returns>
        /// The <see cref="RevSpec"/> with the hash of the new changeset.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="command"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="command"/>.<see cref="CommitCommand.Message">Message</see> is empty.</para>
        /// </exception>
        public RevSpec Commit(CommitCommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            if (StringEx.IsNullOrWhiteSpace(command.Message))
                throw new ArgumentNullException("command", "The Message property of the commit options cannot be an empty string");

            Execute(command);
            return command.Result;
        }

        /// <summary>
        /// Commits the specified files or all outstanding changes to the repository.
        /// </summary>
        /// <param name="message">
        /// The commit message to use.
        /// </param>
        /// <param name="command">
        /// Any extra options to the commit method, or <c>null</c> for default options.
        /// </param>
        /// <returns>
        /// The <see cref="RevSpec"/> with the hash of the new changeset.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="message"/> is <c>null</c> or empty.</para>
        /// </exception>
        public RevSpec Commit(string message, CommitCommand command = null)
        {
            if (StringEx.IsNullOrWhiteSpace(message))
                throw new ArgumentNullException("message");

            command = command ?? new CommitCommand();
            command.Message = message;
            Execute(command);
            return command.Result;
        }

        /// <summary>
        /// Updates the working copy to a new revision.
        /// </summary>
        /// <param name="command">
        /// Any extra options to the update method, or <c>null</c> for default options.
        /// </param>
        public void Update(UpdateCommand command = null)
        {
            Execute(command ?? new UpdateCommand());
        }

        /// <summary>
        /// Updates the working copy to a new revision.
        /// </summary>
        /// <param name="revision">
        /// The revision to update the working copy to.
        /// </param>
        /// <param name="command">
        /// Any extra options to the update method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="revision"/> is <c>null</c>.</para>
        /// </exception>
        public void Update(RevSpec revision, UpdateCommand command = null)
        {
            if (revision == null)
                throw new ArgumentNullException("revision");

            command = command ?? new UpdateCommand();
            command.Revision = revision;
            Execute(command);
        }

        /// <summary>
        /// Retrieves the status of changed files in the working directory.
        /// </summary>
        /// <param name="command">
        /// Any extra options to the status method, or <c>null</c> for default options.
        /// </param>
        /// <returns>
        /// A collection of <see cref="FileStatus"/> objects, one for each modified file.
        /// </returns>
        public IEnumerable<FileStatus> Status(StatusCommand command = null)
        {
            command = command ?? new StatusCommand();
            return Execute(command);
        }

        /// <summary>
        /// Clones a repository into this <see cref="Repository"/>.
        /// </summary>
        /// <param name="command">
        /// Any extra options to the clone method.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="command" /> is <c>null</c>.</para>
        /// </exception>
        public void Clone(CloneCommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            Execute(command);
        }

        /// <summary>
        /// Clones a repository into this <see cref="Repository"/>, from the specified <paramref name="source"/>.
        /// </summary>
        /// <param name="source">
        /// The path or Uri to the source to clone from.
        /// </param>
        /// <param name="command">
        /// Any extra options to the clone method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="source"/> is <c>null</c> or empty.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><see cref="CloneCommand.Source"/> cannot be set before calling this method.</para>
        /// </exception>
        public void Clone(string source, CloneCommand command = null)
        {
            if (StringEx.IsNullOrWhiteSpace(source))
                throw new ArgumentNullException("source");
            if (command != null && !StringEx.IsNullOrWhiteSpace(command.Source))
                throw new ArgumentException("CloneCommand.Source cannot be set before calling this method");

            command = command ?? new CloneCommand();
            command.Source = source;
            Execute(command);
        }

        /// <summary>
        /// Add all new files, delete all missing files.
        /// </summary>
        /// <param name="command">
        /// Any extra options to the addremove method, or <c>null</c> for default options.
        /// </param>
        public void AddRemove(AddRemoveCommand command = null)
        {
            command = command ?? new AddRemoveCommand();
            Execute(command);
        }

        /// <summary>
        /// Pull changes from the specified source.
        /// </summary>
        /// <param name="command">
        /// Any extra options to the pull method, or <c>null</c> for default options.
        /// </param>
        public void Pull(PullCommand command = null)
        {
            command = command ?? new PullCommand();
            Execute(command);
        }

        /// <summary>
        /// Pull changes from the specified source.
        /// </summary>
        /// <param name="source">
        /// The name of the source or the URL to the source, to pull from.
        /// </param>
        /// <param name="command">
        /// Any extra options to the pull method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="source"/> is <c>null</c> or empty.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><see cref="PullCommand.Source"/> cannot be set before calling this method.</para>
        /// </exception>
        public void Pull(string source, PullCommand command = null)
        {
            if (StringEx.IsNullOrWhiteSpace(source))
                throw new ArgumentNullException("source");
            if (command != null && !StringEx.IsNullOrWhiteSpace(command.Source))
                throw new ArgumentException("PullCommand.Source cannot be set before calling this method");

            command = command ?? new PullCommand();
            command.Source = source;
            Execute(command);
        }

        /// <summary>
        /// Push changes to the specified destination.
        /// </summary>
        /// <param name="command">
        /// Any extra options to the push method, or <c>null</c> for default options.
        /// </param>
        public void Push(PushCommand command = null)
        {
            command = command ?? new PushCommand();
            Execute(command);
        }

        /// <summary>
        /// Push changes to the specified destination.
        /// </summary>
        /// <param name="destination">
        /// The name of the destination or the URL to the destination, to push to.
        /// </param>
        /// <param name="command">
        /// Any extra options to the push method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="destination"/> is <c>null</c> or empty.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><see cref="PushCommand.Destination"/> cannot be set before calling this method.</para>
        /// </exception>
        public void Push(string destination, PushCommand command = null)
        {
            if (StringEx.IsNullOrWhiteSpace(destination))
                throw new ArgumentNullException("destination");
            if (command != null && !StringEx.IsNullOrWhiteSpace(command.Destination))
                throw new ArgumentException("PushCommand.Destination cannot be set before calling this method", "command");

            command = command ?? new PushCommand();
            command.Destination = destination;
            Execute(command);
        }

        /// <summary>
        /// Get current repository heads or get branch heads.
        /// </summary>
        /// <param name="command">
        /// Any extra options to the heads method, or <c>null</c> for default options.
        /// </param>
        /// <returns>
        /// A collection of <see cref="Changeset" /> instances.
        /// </returns>
        public IEnumerable<Changeset> Heads(HeadsCommand command = null)
        {
            command = command ?? new HeadsCommand();
            return Execute(command);
        }

        /// <summary>
        /// Annotates the specified item, returning annotation objects for the lines of
        /// the file.
        /// </summary>
        /// <param name="command">
        /// Any extra options to the annotate method, including the
        /// path to the item to annotate.
        /// </param>
        /// <returns>
        /// A collection of <see cref="Annotation"/> objects, one for each line.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="command"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><see cref="AnnotateCommand.Path"/> cannot be set before calling this method.</para>
        /// </exception>
        public IEnumerable<Annotation> Annotate(AnnotateCommand command = null)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            if (StringEx.IsNullOrWhiteSpace(command.Path))
                throw new ArgumentException("AnnotateCommand.Path must be set before calling this method", "command");

            return Execute(command);
        }

        /// <summary>
        /// Annotates the specified item, returning annotation objects for the lines of
        /// the file.
        /// </summary>
        /// <param name="path">
        /// The path to the item to annotate.
        /// </param>
        /// <param name="command">
        /// Any extra options to the annotate method, or <c>null</c> for default options.
        /// </param>
        /// <returns>
        /// A collection of <see cref="Annotation"/> objects, one for each line.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="path"/> is <c>null</c> or empty.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><see cref="AnnotateCommand.Path"/> cannot be set before calling this method.</para>
        /// </exception>
        public IEnumerable<Annotation> Annotate(string path, AnnotateCommand command = null)
        {
            if (StringEx.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException("path");
            if (command != null && !StringEx.IsNullOrWhiteSpace(command.Path))
                throw new ArgumentException("AnnotateCommand.Path cannot be set before calling this method", "command");

            command = command ?? new AnnotateCommand();
            command.Path = path;
            return Execute(command);
        }

        /// <summary>
        /// Gets or sets the current branch name.
        /// </summary>
        /// <param name="command">
        /// Any extra options to the branch method, or <c>null</c> for default options.
        /// </param>
        /// <returns>
        /// The current or new branch name.
        /// </returns>
        public string Branch(BranchCommand command = null)
        {
            command = command ?? new BranchCommand();
            return Execute(command);
        }

        /// <summary>
        /// Gets or sets the current branch name.
        /// </summary>
        /// <param name="name">
        /// The name to use for the new branch.
        /// </param>
        /// <param name="command">
        /// Any extra options to the branch method, or <c>null</c> for default options.
        /// </param>
        /// <returns>
        /// The current or new branch name.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="name"/> is <c>null</c> or empty.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><see cref="BranchCommand.Name"/> cannot be set before calling this method.</para>
        /// </exception>
        public string Branch(string name, BranchCommand command = null)
        {
            if (StringEx.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");
            if (command != null && !StringEx.IsNullOrWhiteSpace(command.Name))
                throw new ArgumentException("BranchCommand.Name cannot be set before calling this method", "command");

            command = command ?? new BranchCommand();
            command.Name = name;
            return Execute(command);
        }

        /// <summary>
        /// List repository named branches.
        /// </summary>
        /// <param name="command">
        /// Any extra options to the branches method, or <c>null</c> for default options.
        /// </param>
        /// <returns>
        /// A collection of <see cref="BranchHead"/> objects, one for each named branch included in the results.
        /// </returns>
        public IEnumerable<BranchHead> Branches(BranchesCommand command = null)
        {
            command = command ?? new BranchesCommand();

            return Execute(command);
        }

        /// <summary>
        /// Retrieve aliases for remote repositories.
        /// </summary>
        /// <param name="command">
        /// Any extra options to the paths method, or <c>null</c> for default options.
        /// </param>
        /// <returns>
        /// A collection of <see cref="RemoteRepositoryPath"/> objects, one for each
        /// remote repository.
        /// </returns>
        public IEnumerable<RemoteRepositoryPath> Paths(PathsCommand command = null)
        {
            command = command ?? new PathsCommand();
            return Execute(command);
        }

        /// <summary>
        /// Adds one or more files to the repository.
        /// </summary>
        /// <param name="path">
        /// The path to a file, or a path containing wildcards to files to add
        /// to the repository.
        /// </param>
        /// <param name="command">
        /// The information object for the add command, containing the paths of the files
        /// to add.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="path"/> is <c>null</c> or empty.</para>
        /// </exception>
        public void Add(string path, AddCommand command = null)
        {
            if (StringEx.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException("path");

            command = command ?? new AddCommand();
            command.Paths.Add(path);
            Execute(command);
        }

        /// <summary>
        /// Adds one or more files to the repository.
        /// </summary>
        /// <param name="command">
        /// The information object for the add command, containing the paths of the files
        /// to add.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="command"/> is <c>null</c>.</para>
        /// </exception>
        public void Add(AddCommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            Execute(command);
        }

        /// <summary>
        /// Removes one or more files from the repository.
        /// </summary>
        /// <param name="path">
        /// The path to a file, or a path containing wildcards to files to remove
        /// from the repository.
        /// </param>
        /// <param name="command">
        /// The information object for the remove command, containing the paths of the files
        /// to remove.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="path"/> is <c>null</c> or empty.</para>
        /// </exception>
        public void Remove(string path, RemoveCommand command = null)
        {
            if (StringEx.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException("path");

            command = command ?? new RemoveCommand();
            command.Paths.Add(path);
            Execute(command);
        }

        /// <summary>
        /// Removes one or more files from the repository.
        /// </summary>
        /// <param name="command">
        /// The information object for the remove command, containing the paths of the files
        /// to remove.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="command"/> is <c>null</c>.</para>
        /// </exception>
        public void Remove(RemoveCommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            Execute(command);
        }

        /// <summary>
        /// Forgets one or more tracked files in the repository, making Mercurial
        /// stop tracking them.
        /// </summary>
        /// <param name="path">
        /// The path to a file, or a path containing wildcards to files to forget
        /// in the repository.
        /// </param>
        /// <param name="command">
        /// The information object for the forget command, containing the paths of the files
        /// to forget.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="path"/> is <c>null</c> or empty.</para>
        /// </exception>
        public void Forget(string path, ForgetCommand command = null)
        {
            if (StringEx.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException("path");

            command = command ?? new ForgetCommand();
            command.Paths.Add(path);
            Execute(command);
        }

        /// <summary>
        /// Forgets one or more tracked files in the repository, making Mercurial
        /// stop tracking them.
        /// </summary>
        /// <param name="command">
        /// The information object for the forget command, containing the paths of the files
        /// to forget.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="command"/> is <c>null</c>.</para>
        /// </exception>
        public void Forget(ForgetCommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            Execute(command);
        }

        /// <summary>
        /// Retrieves the tip revision.
        /// </summary>
        /// <param name="command">
        /// The information object for the tip command.
        /// </param>
        /// <returns>
        /// The <see cref="Changeset"/> of the tip revision.
        /// </returns>
        public Changeset Tip(TipCommand command = null)
        {
            command = command ?? new TipCommand();
            return Execute(command);
        }

        /// <summary>
        /// Add or remove a tag for a changeset.
        /// </summary>
        /// <param name="command">
        /// The options to the tag command.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="command"/> is <c>null</c>.</para>
        /// </exception>
        public void Tag(TagCommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            Execute(command);
        }

        /// <summary>
        /// Add or remove a tag for a changeset.
        /// </summary>
        /// <param name="name">
        /// The name of the tag.
        /// </param>
        /// <param name="command">
        /// The information object for the tag command, or <c>null</c> if no extra information
        /// is necessary.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="name"/> is <c>null</c> or empty.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <para><paramref name="command"/>.<see cref="TagCommand.Name">Name</see> and <paramref name="name"/> was both set.</para>
        /// </exception>
        public void Tag(string name, TagCommand command = null)
        {
            if (StringEx.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");
            if (command != null && !StringEx.IsNullOrWhiteSpace(command.Name))
                throw new InvalidOperationException("Both name and command.Name cannot be set before calling Tag");

            command = command ?? new TagCommand();
            command.Name = name;
            Execute(command);
        }

        /// <summary>
        /// Retrieve changesets not found in the default destination.
        /// </summary>
        /// <param name="command">
        /// Any extra options to the outgoing method, or <c>null</c> for default options.
        /// </param>
        /// <returns>
        /// The result of executing the command as a collection of <see cref="Changeset"/> objects.
        /// </returns>
        public IEnumerable<Changeset> Outgoing(OutgoingCommand command = null)
        {
            command = command ?? new OutgoingCommand();
            Execute(command);
            return command.Result;
        }

        /// <summary>
        /// Retrieve changesets not found in the destination.
        /// </summary>
        /// <param name="destination">
        /// The name of the destination or the URL to the destination, to check outgoing to.
        /// </param>
        /// <param name="command">
        /// Any extra options to the outgoing method, or <c>null</c> for default options.
        /// </param>
        /// <returns>
        /// The result of executing the command as a collection of <see cref="Changeset"/> objects.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="destination"/> is <c>null</c> or empty.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><see cref="OutgoingCommand.Destination"/> cannot be set before calling this method.</para>
        /// </exception>
        public IEnumerable<Changeset> Outgoing(string destination, OutgoingCommand command = null)
        {
            if (StringEx.IsNullOrWhiteSpace(destination))
                throw new ArgumentNullException("destination");
            if (command != null && !StringEx.IsNullOrWhiteSpace(command.Destination))
                throw new ArgumentException("OutgoingCommand.Destination cannot be set before calling this method", "command");

            command = command ?? new OutgoingCommand();
            command.Destination = destination;
            Execute(command);
            return command.Result;
        }

        /// <summary>
        /// Retrieve new changesets found in the default source.
        /// </summary>
        /// <param name="command">
        /// Any extra options to the incoming method, or <c>null</c> for default options.
        /// </param>
        /// <returns>
        /// The result of executing the command as a collection of <see cref="Changeset"/> objects.
        /// </returns>
        public IEnumerable<Changeset> Incoming(IncomingCommand command = null)
        {
            command = command ?? new IncomingCommand();
            Execute(command);
            return command.Result;
        }

        /// <summary>
        /// Retrieve new changesets found in the source.
        /// </summary>
        /// <param name="source">
        /// The name of the source or the URL to the source, to check incoming from.
        /// </param>
        /// <param name="command">
        /// Any extra options to the incoming method, or <c>null</c> for default options.
        /// </param>
        /// <returns>
        /// The result of executing the command as a collection of <see cref="Changeset"/> objects.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="source"/> is <c>null</c> or empty.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><see cref="IncomingCommand.Source"/> cannot be set before calling this method.</para>
        /// </exception>
        public IEnumerable<Changeset> Incoming(string source, IncomingCommand command = null)
        {
            if (StringEx.IsNullOrWhiteSpace(source))
                throw new ArgumentNullException("source");
            if (command != null && !StringEx.IsNullOrWhiteSpace(command.Source))
                throw new ArgumentException("IncomingCommand.Source cannot be set before calling this method", "command");

            command = command ?? new IncomingCommand();
            command.Source = source;
            Execute(command);
            return command.Result;
        }

        /// <summary>
        /// Bundles changesets into a changegroup file, a bundle.
        /// </summary>
        /// <param name="fileName">
        /// The full path to and name of the file to save the bundle to.
        /// </param>
        /// <param name="destination">
        /// The destination repository to compare to the local one, to determine which
        /// changesets to bundle.
        /// </param>
        /// <param name="command">
        /// Any extra options to the bundle method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="fileName"/> is <c>null</c> or empty.</para>
        /// <para>- or -</para>
        /// <para><paramref name="destination"/> is <c>null</c> or empty.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><see cref="BundleCommand.FileName"/> cannot be set before calling this method.</para>
        /// <para>- or -</para>
        /// <para><see cref="BundleCommand.Destination"/> cannot be set before calling this method.</para>
        /// </exception>
        /// <exception cref="NoChangesFoundMercurialExecutionException">
        /// <para>No changes found to include in the bundle.</para>
        /// </exception>
        public void Bundle(string fileName, string destination, BundleCommand command = null)
        {
            if (StringEx.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException("fileName");
            if (StringEx.IsNullOrWhiteSpace(destination))
                throw new ArgumentNullException("destination");
            if (command != null && !StringEx.IsNullOrWhiteSpace(command.FileName))
                throw new ArgumentException("BundleCommand.FileName cannot be set before calling this method", "command");
            if (command != null && !StringEx.IsNullOrWhiteSpace(command.Destination))
                throw new ArgumentException("BundleCommand.Destination cannot be set before calling this method", "command");

            command = command ?? new BundleCommand();
            command.FileName = fileName;
            command.Destination = destination;
            Execute(command);
        }

        /// <summary>
        /// Bundles changesets into a changegroup file, a bundle.
        /// </summary>
        /// <param name="fileName">
        /// The full path to and name of the file to save the bundle to.
        /// </param>
        /// <param name="command">
        /// Any extra options to the bundle method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="fileName"/> is <c>null</c> or empty.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><see cref="BundleCommand.FileName"/> cannot be set before calling this method.</para>
        /// </exception>
        /// <exception cref="NoChangesFoundMercurialExecutionException">
        /// <para>No changes found to include in the bundle.</para>
        /// </exception>
        public void Bundle(string fileName, BundleCommand command = null)
        {
            if (StringEx.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException("fileName");
            if (command != null && !StringEx.IsNullOrWhiteSpace(command.FileName))
                throw new ArgumentException("BundleCommand.FileName cannot be set before calling this method", "command");

            command = command ?? new BundleCommand();
            command.FileName = fileName;
            Execute(command);
        }

        /// <summary>
        /// Bundles changesets into a changegroup file, a bundle.
        /// </summary>
        /// <param name="command">
        /// The options to the bundle command.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="command"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="NoChangesFoundMercurialExecutionException">
        /// <para>No changes found to include in the bundle.</para>
        /// </exception>
        public void Bundle(BundleCommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            Execute(command);
        }

        /// <summary>
        /// Apply one or more changegroup files generated by the bundle command.
        /// </summary>
        /// <param name="fileName">
        /// The full path to and name of the file to load the bundle from.
        /// </param>
        /// <param name="command">
        /// Any extra options to the unbundle method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="fileName"/> is <c>null</c> or empty.</para>
        /// </exception>
        public void Unbundle(string fileName, UnbundleCommand command = null)
        {
            if (StringEx.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException("fileName");

            command = command ?? new UnbundleCommand();
            command.FileNames.Add(fileName);
            Execute(command);
        }

        /// <summary>
        /// Apply one or more changegroup files generated by the bundle command.
        /// </summary>
        /// <param name="command">
        /// The options to the unbundle method.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="command"/> is <c>null</c>.</para>
        /// </exception>
        public void Unbundle(UnbundleCommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            Execute(command);
        }

        /// <summary>
        /// Copies one or more files from one place to another, and records the actions
        /// as copy-commands in the next commit.
        /// </summary>
        /// <param name="source">
        /// The source file specification.
        /// </param>
        /// <param name="destination">
        /// The destination to copy the file(s) that match the <paramref name="source"/> to.
        /// </param>
        /// <param name="command">
        /// Any extra options to the copy method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="source"/> is <c>null</c> or empty.</para>
        /// <para>- or -</para>
        /// <para><paramref name="destination"/> is <c>null</c> or empty.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><see cref="CopyCommand.Source"/> cannot be set before calling this method.</para>
        /// <para>- or -</para>
        /// <para><see cref="CopyCommand.Destination"/> cannot be set before calling this method.</para>
        /// </exception>
        public void Copy(string source, string destination, CopyCommand command = null)
        {
            if (StringEx.IsNullOrWhiteSpace(source))
                throw new ArgumentNullException("source");
            if (StringEx.IsNullOrWhiteSpace(destination))
                throw new ArgumentNullException("destination");
            if (command != null && !StringEx.IsNullOrWhiteSpace(command.Source))
                throw new ArgumentException("CopyCommand.Source cannot be set before calling this method", "command");
            if (command != null && !StringEx.IsNullOrWhiteSpace(command.Destination))
                throw new ArgumentException("CopyCommand.Destination cannot be set before calling this method", "command");

            command = command ?? new CopyCommand();
            command.Source = source;
            command.Destination = destination;

            Execute(command);
        }

        /// <summary>
        /// Copies one or more files from one place to another, and records the actions
        /// as copy-commands in the next commit.
        /// </summary>
        /// <param name="command">
        /// The options to the copy command.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="command"/> is <c>null</c>.</para>
        /// </exception>
        public void Copy(CopyCommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            Execute(command);
        }

        /// <summary>
        /// Identifies the working copy.
        /// </summary>
        /// <param name="command">
        /// Any extra options to the identify method, or <c>null</c> for default options.
        /// </param>
        /// <returns>
        /// The <see cref="RevSpec"/> object containing the hash of the working copy.
        /// </returns>
        public RevSpec Identify(IdentifyCommand command = null)
        {
            command = command ?? new IdentifyCommand();
            Execute(command);
            return command.Result;
        }

        /// <summary>
        /// Recover from an interrupted commit or pull.
        /// </summary>
        /// <param name="command">
        /// Any extra options to the recover method, or <c>null</c> for default options.
        /// </param>
        public void Recover(RecoverCommand command = null)
        {
            command = command ?? new RecoverCommand();
            Execute(command);
        }

        /// <summary>
        /// Verifies the integrity of the repository.
        /// </summary>
        /// <param name="command">
        /// Any extra options to the verify method, or <c>null</c> for default options.
        /// </param>
        public void Verify(VerifyCommand command = null)
        {
            command = command ?? new VerifyCommand();
            Execute(command);
        }

        /// <summary>
        /// Verifies the integrity of the repository.
        /// </summary>
        /// <param name="destination">
        /// The destination folder or file to archive to.
        /// </param>
        /// <param name="command">
        /// Any extra options to the verify method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="destination"/> is <c>null</c> or empty.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><paramref name="command"/>.<see cref="ArchiveCommand.Destination"/> cannot be set before calling this method.</para>
        /// </exception>
        public void Archive(string destination, ArchiveCommand command = null)
        {
            if (StringEx.IsNullOrWhiteSpace(destination))
                throw new ArgumentNullException("destination");
            if (command != null && !StringEx.IsNullOrWhiteSpace(command.Destination))
                throw new ArgumentException("ArchiveCommand.Destination cannot be set before calling this method", "command");

            command = command ?? new ArchiveCommand();
            command.Destination = destination;

            Execute(command);
        }

        /// <summary>
        /// Verifies the integrity of the repository.
        /// </summary>
        /// <param name="command">
        /// Any extra options to the verify method, or <c>null</c> for default options.
        /// </param>
        public void Archive(ArchiveCommand command = null)
        {
            command = command ?? new ArchiveCommand();

            Execute(command);
        }

        /// <summary>
        /// Bisects the repository, doing a subdivision search for a good changeset.
        /// </summary>
        /// <param name="state">
        /// How to mark the current changeset.
        /// </param>
        /// <param name="command">
        /// Any extra options to the bisect method, or <c>null</c> for default options.
        /// </param>
        /// <returns>
        /// The <see cref="BisectResult"/> results from executing the command.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <para><paramref name="command"/>.<see cref="BisectCommand.State">State</see> cannot be set before calling this method.</para>
        /// </exception>
        public BisectResult Bisect(BisectState state, BisectCommand command = null)
        {
            if (command != null && command.State != BisectState.None)
                throw new ArgumentException("BisectCommand.State cannot be set before calling this method", "command");

            command = (command ?? new BisectCommand())
                .WithState(state);

            return Execute(command);
        }

        /// <summary>
        /// Bisects the repository, doing a subdivision search for a good changeset.
        /// </summary>
        /// <param name="command">
        /// The options to the bisect command.
        /// </param>
        /// <returns>
        /// The <see cref="BisectResult"/> results from executing the command.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="command"/> is <c>null</c>.</para>
        /// </exception>
        public BisectResult Bisect(BisectCommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            return Execute(command);
        }

        /// <summary>
        /// Creates, deletes, moves, or renames a bookmark.
        /// </summary>
        /// <param name="command">
        /// The options to the bookmark command.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="command"/> is <c>null</c>.</para>
        /// </exception>
        public void Bookmark(BookmarkCommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            Execute(command);
        }

        /// <summary>
        /// Bookmarks the working folder parent revision.
        /// </summary>
        /// <param name="name">
        /// The name of the bookmark to create.
        /// </param>
        /// <param name="command">
        /// Any extra options to the bookmark method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="name"/> is <c>null</c> or empty.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><paramref name="command"/>.<see cref="BookmarkCommand.Name">Name</see> cannot be set before calling this method.</para>
        /// </exception>
        public void Bookmark(string name, BookmarkCommand command = null)
        {
            if (StringEx.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            command = (command ?? new BookmarkCommand())
                .WithName(name);

            Execute(command);
        }

        /// <summary>
        /// Bookmarks the specified revision.
        /// </summary>
        /// <param name="name">
        /// The name of the bookmark to create.
        /// </param>
        /// <param name="revision">
        /// The revision to bookmark.
        /// </param>
        /// <param name="command">
        /// Any extra options to the bookmark method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="name"/> is <c>null</c> or empty.</para>
        /// <para>- or -</para>
        /// <para><paramref name="revision"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><paramref name="command"/>.<see cref="BookmarkCommand.Name">Name</see> cannot be set before calling this method.</para>
        /// <para>- or -</para>
        /// <para><paramref name="command"/>.<see cref="BookmarkCommand.Revision">Revision</see> cannot be set before calling this method.</para>
        /// </exception>
        public void Bookmark(string name, RevSpec revision, BookmarkCommand command = null)
        {
            if (StringEx.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");
            if (revision == null)
                throw new ArgumentNullException("revision");

            if (command != null && command.Revision != null)
                throw new ArgumentException("BookmarkCommand.Revision cannot be set before calling this method", "command");
            if (command != null && !StringEx.IsNullOrWhiteSpace(command.Name))
                throw new ArgumentException("BookmarkCommand.Name cannot be set before calling this method", "command");

            command = (command ?? new BookmarkCommand())
                .WithName(name)
                .WithRevision(revision);

            Execute(command);
        }

        /// <summary>
        /// Get the collection of existing bookmarks in this <see cref="Repository"/>.
        /// </summary>
        /// <param name="command">
        /// Any extra options to the bookmarks method, or <c>null</c> for default options.
        /// </param>
        /// <returns>
        /// A collection of <see cref="T:Mercurial.Bookmark"/> objects, one for each bookmark in this <see cref="Repository"/>.
        /// </returns>
        public IEnumerable<Bookmark> Bookmarks(BookmarksCommand command = null)
        {
            command = command ?? new BookmarksCommand();

            return Execute(command);
        }

        /// <summary>
        /// Rename files; equivalent of copy + remove.
        /// </summary>
        /// <param name="command">
        /// The options to the rename command.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="command"/> is <c>null</c>.</para>
        /// </exception>
        public void Rename(RenameCommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            Execute(command);
        }

        /// <summary>
        /// Rename files; equivalent of copy + remove.
        /// </summary>
        /// <param name="source">
        /// The source of the move command; what to move.
        /// </param>
        /// <param name="destination">
        /// The destination of the move command; where to move it.
        /// </param>
        /// <param name="command">
        /// Any extra options to the move method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="source"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="destination"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><paramref name="command"/>.<see cref="MoveRenameCommandBase{T}.Source">Source</see> cannot be set before calling this method.</para>
        /// <para>- or -</para>
        /// <para><paramref name="command"/>.<see cref="MoveRenameCommandBase{T}.Destination">Destination</see> cannot be set before calling this method.</para>
        /// </exception>
        public void Rename(string source, string destination, RenameCommand command = null)
        {
            if (StringEx.IsNullOrWhiteSpace(source))
                throw new ArgumentNullException("source");
            if (StringEx.IsNullOrWhiteSpace(destination))
                throw new ArgumentNullException("destination");
            if (command != null && command.Source != null)
                throw new ArgumentException("MoveCommand.Source cannot be set before calling this method", "command");
            if (command != null && command.Destination != null)
                throw new ArgumentException("MoveCommand.Destination cannot be set before calling this method", "command");

            command = (command ?? new RenameCommand())
                .WithSource(source)
                .WithDestination(destination);

            Execute(command);
        }

        /// <summary>
        /// Move files; equivalent of copy + remove.
        /// </summary>
        /// <param name="command">
        /// The options to the copy command.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="command"/> is <c>null</c>.</para>
        /// </exception>
        public void Move(MoveCommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            Execute(command);
        }

        /// <summary>
        /// Move files; equivalent of copy + remove.
        /// </summary>
        /// <param name="source">
        /// The source of the move command; what to move.
        /// </param>
        /// <param name="destination">
        /// The destination of the move command; where to move it.
        /// </param>
        /// <param name="command">
        /// Any extra options to the move method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="source"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="destination"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><paramref name="command"/>.<see cref="MoveRenameCommandBase{T}.Source">Source</see> cannot be set before calling this method.</para>
        /// <para>- or -</para>
        /// <para><paramref name="command"/>.<see cref="MoveRenameCommandBase{T}.Destination">Destination</see> cannot be set before calling this method.</para>
        /// </exception>
        public void Move(string source, string destination, MoveCommand command = null)
        {
            if (StringEx.IsNullOrWhiteSpace(source))
                throw new ArgumentNullException("source");
            if (StringEx.IsNullOrWhiteSpace(destination))
                throw new ArgumentNullException("destination");
            if (command != null && command.Source != null)
                throw new ArgumentException("MoveCommand.Source cannot be set before calling this method", "command");
            if (command != null && command.Destination != null)
                throw new ArgumentException("MoveCommand.Destination cannot be set before calling this method", "command");

            command = (command ?? new MoveCommand())
                .WithSource(source)
                .WithDestination(destination);

            Execute(command);
        }

        /// <summary>
        /// Restore individual files or directories to an earlier state.
        /// </summary>
        /// <param name="command">
        /// The options to the revert command.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="command"/> is <c>null</c>.</para>
        /// </exception>
        public void Revert(RevertCommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            Execute(command);
        }

        /// <summary>
        /// Restore individual files or directories to an earlier state.
        /// </summary>
        /// <param name="name">
        /// The name of the file or directory to revert.
        /// </param>
        /// <param name="command">
        /// Any extra options to the revert method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="name"/> is <c>null</c> or empty.</para>
        /// </exception>
        public void Revert(string name, RevertCommand command = null)
        {
            if (StringEx.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            command = (command ?? new RevertCommand())
                .WithName(name);

            Execute(command);
        }

        /// <summary>
        /// List repository tags.
        /// </summary>
        /// <param name="command">
        /// Any extra options to the tags method, or <c>null</c> for default options.
        /// </param>
        /// <returns>
        /// A collection of <see cref="T:Mercurial.Tag"/> objects for the existing tags in the repository.
        /// </returns>
        public IEnumerable<Tag> Tags(TagsCommand command = null)
        {
            command = command ?? new TagsCommand();

            return Execute(command);
        }

        /// <summary>
        /// Output the given revision of the project manifest.
        /// </summary>
        /// <param name="revision">
        /// The revision to get the manifest of.
        /// </param>
        /// <param name="command">
        /// Any extra options to the tags method, or <c>null</c> for default options.
        /// </param>
        /// <returns>
        /// A collection of <see cref="string"/>s, each specifying the relative path to and name of a file
        /// in the specified <paramref name="revision"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="revision"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><paramref name="command"/>.<see cref="ManifestCommand.Revision">Revision</see> cannot be set before calling this method.</para>
        /// </exception>
        public IEnumerable<string> Manifest(RevSpec revision, ManifestCommand command = null)
        {
            if (revision == null)
                throw new ArgumentNullException("revision");
            if (command != null && command.Revision != null)
                throw new ArgumentException("ManifestCommand.Revision cannot be set before calling this method", "command");

            command = (command ?? new ManifestCommand())
                .WithRevision(revision);

            return Execute(command);
        }

        /// <summary>
        /// Output the current or given revision of the project manifest.
        /// </summary>
        /// <param name="command">
        /// Any extra options to the tags method, or <c>null</c> for default options.
        /// </param>
        /// <returns>
        /// A collection of <see cref="string"/>s, each specifying the relative path to and name of a file
        /// in the current or specified revision.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <para><paramref name="command"/>.<see cref="ManifestCommand.Revision">Revision</see> cannot be set before calling this method.</para>
        /// </exception>
        public IEnumerable<string> Manifest(ManifestCommand command = null)
        {
            command = command ?? new ManifestCommand();

            return Execute(command);
        }

        /// <summary>
        /// Performs a merge between the current working folder and the specified revision.
        /// </summary>
        /// <param name="revision">
        /// The revision to merge with.
        /// </param>
        /// <param name="command">
        /// Any extra options to the merge method, or <c>null</c> for default options.
        /// </param>
        /// <returns>
        /// A <see cref="MergeResult"/> value indicitating the result of the merge.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="revision"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><paramref name="command"/>.<see cref="MergeCommand.Revision">Revision</see> cannot be set before calling this method.</para>
        /// </exception>
        public MergeResult Merge(RevSpec revision, MergeCommand command = null)
        {
            if (revision == null)
                throw new ArgumentNullException("revision");
            if (command != null && command.Revision != null)
                throw new ArgumentException("MergeCommand.Revision cannot be set before calling this method", "command");

            command = (command ?? new MergeCommand())
                .WithRevision(revision);

            return Execute(command);
        }

        /// <summary>
        /// Performs a merge between the current working folder and another revision.
        /// </summary>
        /// <param name="command">
        /// Any extra options to the merge method, or <c>null</c> for default options.
        /// </param>
        /// <returns>
        /// A <see cref="MergeResult"/> value indicitating the result of the merge.
        /// </returns>
        public MergeResult Merge(MergeCommand command = null)
        {
            command = command ?? new MergeCommand();

            return Execute(command);
        }

        /// <summary>
        /// Summarize working directory state.
        /// </summary>
        /// <param name="command">
        /// Any extra options to the summary method, or <c>null</c> for default options.
        /// </param>
        /// <returns>
        /// A <see cref="RepositorySummary"/> object with the summarized working directory state.
        /// </returns>
        public RepositorySummary Summary(SummaryCommand command = null)
        {
            command = command ?? new SummaryCommand();

            return Execute(command);
        }

        /// <summary>
        /// Mark the file as resolved.
        /// </summary>
        /// <param name="file">
        /// The file to mark as resolved.
        /// </param>
        /// <param name="command">
        /// Any extra options to the resolve method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="file"/> is <c>null</c> or empty.</para>
        /// </exception>
        public void Resolve(string file, ResolveCommand command = null)
        {
            if (StringEx.IsNullOrWhiteSpace(file))
                throw new ArgumentNullException("file");

            command = (command ?? new ResolveCommand())
                .WithFile(file);

            Execute(command);
        }

        /// <summary>
        /// Mark the file as resolved or unresolved, or attempt to redo the merge.
        /// </summary>
        /// <param name="file">
        /// The file to process.
        /// </param>
        /// <param name="action">
        /// Which action to take for the file.
        /// </param>
        /// <param name="command">
        /// Any extra options to the resolve method, or <c>null</c> for default options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="file"/> is <c>null</c> or empty.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// This <see cref="Resolve(string,ResolveAction,ResolveCommand)"/> method does not accept the <see cref="ResolveAction.List"/> action.
        /// </exception>
        public void Resolve(string file, ResolveAction action, ResolveCommand command = null)
        {
            if (StringEx.IsNullOrWhiteSpace(file))
                throw new ArgumentNullException("file");
            if (action == ResolveAction.List)
                throw new ArgumentException("This Repository.Resolve method does not accept the ResolveAction.List action");

            command = (command ?? new ResolveCommand())
                .WithFile(file)
                .WithAction(action);

            Execute(command);
        }

        /// <summary>
        /// Redo merges or set/view the merge status of files.
        /// </summary>
        /// <param name="command">
        /// The options to the revert command.
        /// </param>
        /// <returns>
        /// A collection of <see cref="MergeConflict"/> objects in the case of a <see cref="ResolveAction.List"/> action;
        /// or <c>null</c> if any other action was performed.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="command"/> is <c>null</c>.</para>
        /// </exception>
        public IEnumerable<MergeConflict> Resolve(ResolveCommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            return Execute(command);
        }

        /// <summary>
        /// Starts a controlled merge, using the <see cref="MergeJob"/> helper class to control the process.
        /// </summary>
        /// <param name="command">
        /// Any extra options to the initial merge method, or <c>null</c> for default options.
        /// </param>
        /// <returns>
        /// A new <see cref="MergeJob"/> instance used to control the merge.
        /// </returns>
        /// <remarks>
        /// This method will override the <see cref="MergeCommand.MergeTool"/> property to avoid any
        /// merge tools from firing at this point.
        /// </remarks>
        public MergeJob StartMerge(MergeCommand command = null)
        {
            command = (command ?? new MergeCommand())
                .WithMergeTool(MergeTools.InternalDump);

            Execute(command);

            return new MergeJob(this, command);
        }

        /// <summary>
        /// Retrieve the parents of the working directory or revision.
        /// </summary>
        /// <param name="command">
        /// Any extra options to the parents method, or <c>null</c> for default options.
        /// </param>
        /// <returns>
        /// A collection of <see cref="Changeset"/> objects, the parents of the
        /// working directory or the specified revision.
        /// </returns>
        public IEnumerable<Changeset> Parents(ParentsCommand command = null)
        {
            command = command ?? new ParentsCommand();

            return Execute(command);
        }

        /// <summary>
        /// Roll back the last transaction (dangerous.)
        /// </summary>
        /// <param name="command">
        /// Any extra options to the rollback method, or <c>null</c> for default options.
        /// </param>
        public void Rollback(RollbackCommand command = null)
        {
            command = command ?? new RollbackCommand();

            Execute(command);
        }

        /// <summary>
        /// Retrieve the current or given revisions of file(s).
        /// </summary>
        /// <param name="filename">
        /// The name of the file to retrieve.
        /// </param>
        /// <param name="command">
        /// Any extra options to the cat method, or <c>null</c> for default options.
        /// </param>
        /// <returns>
        /// Returns the output from the cat command, which if only one file is retrieved, will be the
        /// contents of that file. Note that binary files might be mangled, so retrieve these to disk.
        /// </returns>
        public string Cat(string filename, CatCommand command = null)
        {
            command = (command ?? new CatCommand())
                .WithFile(filename);

            Execute(command);
            return command.RawStandardOutput;
        }

        /// <summary>
        /// Retrieve the current or given revisions of file(s).
        /// </summary>
        /// <param name="command">
        /// The options to the cat command.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="command"/> is <c>null</c>.</para>
        /// </exception>
        /// <returns>
        /// Returns the output from the cat command, which if only one file is retrieved, will be the
        /// contents of that file. Note that binary files might be mangled, so retrieve these to disk.
        /// </returns>
        public string Cat(CatCommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            Execute(command);
            return command.RawStandardOutput;
        }

        /// <summary>
        /// Diff repository (or selected files).
        /// </summary>
        /// <param name="revisions">
        /// The <see cref="RevSpec"/> that identifies the revision or the revision range to view a diff of.
        /// </param>
        /// <param name="command">
        /// Any extra options to the diff method, or <c>null</c> for default options.
        /// </param>
        /// <returns>
        /// The diff command output.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="revisions"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><paramref name="command"/>.<see cref="DiffCommand.Revisions">Revisions</see> cannot be set before calling this method.</para>
        /// </exception>
        public string Diff(RevSpec revisions, DiffCommand command = null)
        {
            if (revisions == null)
                throw new ArgumentNullException("revisions");
            if (command != null && command.Revisions != null)
                throw new ArgumentException("DiffCommand.Revisions cannot be set before calling this method", "command");

            command = (command ?? new DiffCommand())
                .WithRevisions(revisions);

            return Execute(command);
        }

        /// <summary>
        /// Diff repository (or selected files).
        /// </summary>
        /// <param name="command">
        /// Any extra options to the diff method, or <c>null</c> for default options.
        /// </param>
        /// <returns>
        /// The diff command output.
        /// </returns>
        public string Diff(DiffCommand command = null)
        {
            command = command ?? new DiffCommand();

            return Execute(command);
        }

        #region Nested type: ExecuteDelegate

        /// <summary>
        /// A delegate for <see cref="Execute"/>.
        /// </summary>
        /// <param name="command">
        /// The command to execute.
        /// </param>
        private delegate void ExecuteDelegate(IMercurialCommand command);

        /// <summary>
        /// A delegate for <see cref="Execute{TResult}"/>.
        /// </summary>
        /// <typeparam name="TResult">
        /// The result of executing the <paramref name="command"/>.
        /// </typeparam>
        /// <param name="command">
        /// The command to execute.
        /// </param>
        /// <returns>
        /// The result of executing the command.
        /// </returns>
        private delegate TResult ExecuteDelegate<TResult>(IMercurialCommand<TResult> command);

        #endregion

        /// <summary>
        /// Returns a <see cref="String"/> that represents the current <see cref="Repository"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="String"/> that represents the current <see cref="Repository"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Repository (Path={0})", Client.RepositoryPath);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            var disposable = _Client as IDisposable;
            _Client = null;
            if (disposable != null)
                disposable.Dispose();
        }
    }
}