// 
// Copyright (c) 2005-2018 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Mercurial;
using Mercurial.Attributes;
using Tp.Core;
using Tp.Integration.Plugin.Common.Domain;
using Tp.SourceControl.Settings;
using Tp.SourceControl.VersionControlSystem;
using Repository = Mercurial.Repository;

namespace Tp.Mercurial.VersionControlSystem
{
    public class MercurialClient
    {
        private readonly Repository _repository;
        private readonly ISourceControlConnectionSettingsSource _settings;
        private readonly IStorage<MercurialRepositoryFolder> _folder;

        public MercurialClient(ISourceControlConnectionSettingsSource settings, IStorage<MercurialRepositoryFolder> folder)
        {
            _settings = settings;
            _folder = folder;
            _repository = GetClient(settings);
        }

        public IEnumerable<RevisionRange> GetFromTillHead(DateTime from, int pageSize)
        {
            var command = new LogCommandNoVerbose().WithAdditionalArgument("-d >{0:yyyy-MM-dd}".Fmt(from));
            var pages = _repository.Execute(command)
                .Where(ch => ch.Timestamp >= from)
                .OrderBy(ch => ch.Timestamp)
                .ToArray()
                .Split(pageSize);

            var result = pages.Select(page => new RevisionRange(page.First().ToRevisionId(), page.Last().ToRevisionId()));

            return result;
        }

        public IEnumerable<RevisionRange> GetAfterTillHead(RevisionId revisionId, int pageSize)
        {
            var revSpec = new RevSpec(revisionId.Value);
            var command = new LogCommandNoVerbose().WithRevision(RevSpec.From(revSpec) && !new RevSpec(revisionId.Value));
            var pages = _repository.Execute(command)
                .OrderBy(ch => ch.Timestamp)
                .ToArray()
                .Split(pageSize);

            var result = pages.Select(page => new RevisionRange(page.First().ToRevisionId(), page.Last().ToRevisionId()));

            return result;
        }

        public IEnumerable<RevisionRange> GetFromAndBefore(RevisionId fromRevision, RevisionId toRevision, int pageSize)
        {
            var command = new LogCommandNoVerbose();
            if (string.IsNullOrEmpty(fromRevision.Value))
            {
                if (string.IsNullOrEmpty(toRevision.Value))
                {
                    command =
                        command.WithAdditionalArgument("-d {0:yyyy-MM-dd} to {1:yyyy-MM-dd}".Fmt(fromRevision.Time.Value,
                            toRevision.Time.Value));
                }
                else
                {
                    var to = new RevSpec(toRevision.Value);
                    command = command.WithRevision(RevSpec.To(to));
                    command = command.WithAdditionalArgument("-d >{0:yyyy-MM-dd}".Fmt(fromRevision.Time.Value));
                }
            }
            else
            {
                var from = new RevSpec(fromRevision.Value);
                if (string.IsNullOrEmpty(toRevision.Value))
                {
                    command = command.WithAdditionalArgument("-d <{0:yyyy-MM-dd}".Fmt(toRevision.Time.Value));
                    command = command.WithRevision(RevSpec.From(from));
                }
                else
                {
                    var to = new RevSpec(toRevision.Value);
                    command = command.WithRevision(RevSpec.Range(from, to));
                }
            }

            var pages = _repository.Execute(command)
                .OrderBy(ch => ch.Timestamp)
                .ToArray()
                .Split(pageSize);

            var result = pages.Select(page => new RevisionRange(page.First().ToRevisionId(), page.Last().ToRevisionId()));

            return result;
        }

        public Changeset GetParentCommit(Changeset commit)
        {
            var command = new LogCommand().WithRevision(new RevSpec("{0}^".Fmt(commit.Hash))).WithIncludePathActions();
            var changesets = _repository.Log(command);
            var changeset = changesets.FirstOrDefault();
            return changeset;
        }

        public Changeset GetCommit(RevisionId id)
        {
            var command = new LogCommand().WithRevision(RevSpec.ById(id.Value)).WithIncludePathActions();
            var changeset = _repository.Log(command).FirstOrDefault(ch => ch.Hash == id.Value);
            return changeset;
        }

        public string[] RetrieveAuthors(DateRange dateRange)
        {
            var command = new LogCommandNoVerbose();
            var authors = _repository.Execute(command)
                .Where(ch => ch.Timestamp >= dateRange.StartDate && ch.Timestamp <= dateRange.EndDate)
                .Select(ch => ch.AuthorName)
                .Distinct()
                .ToArray();

            return authors;
        }

        public RevisionInfo[] GetRevisions(RevisionId fromChangeset, RevisionId toChangeset)
        {
            var from = new RevSpec(fromChangeset.Value);
            var to = new RevSpec(toChangeset.Value);
            var command = new LogCommand().WithRevision(RevSpec.Range(from, to)).WithIncludePathActions();
            var revisionInfos = _repository.Log(command)
                .Where(ch => ch.Timestamp >= fromChangeset.Time.Value && ch.Timestamp <= toChangeset.Time.Value)
                .Select(ch => ch.ToRevisionInfo())
                .ToArray();

            return revisionInfos;
        }

        public string GetFileContent(Changeset commit, string path)
        {
            var command = new CatCommand().WithFile(path);
            if (commit != null)
            {
                command = command.WithAdditionalArgument($"-r {commit.RevisionNumber}");
            }
            _repository.Execute(command);
            return command.RawStandardOutput;
        }

        private Repository GetClient(ISourceControlConnectionSettingsSource settings)
        {
            var repositoryFolder = GetLocalRepository(settings);
            if (IsRepositoryUriChanged(repositoryFolder, settings))
            {
                repositoryFolder.Delete();
                repositoryFolder = MercurialRepositoryFolder.Create(settings.Uri);
                _folder.ReplaceWith(repositoryFolder);
            }

            Repository repository;

            try
            {
                if (repositoryFolder.Exists())
                {
                    string path = repositoryFolder.GetAbsolutePath();
                    repository = new Repository(path, new NonPersistentClientFactory());
                    repository.Pull(settings.Uri);
                }
                else
                {
                    string path = repositoryFolder.GetAbsolutePath();
                    Directory.CreateDirectory(path);
                    CloneCommand cloneCommand = new CloneCommand().WithUpdate(false);
                    repository = new Repository(path, new NonPersistentClientFactory());
                    repository.Clone(settings.Uri, cloneCommand);
                }
            }
            catch (ArgumentNullException e)
            {
                throw new ArgumentException(
                    MercurialCheckConnectionErrorResolver.INVALID_URI_OR_INSUFFICIENT_ACCESS_RIGHTS_ERROR_MESSAGE, e);
            }
            catch (FileNotFoundException e)
            {
                throw new ArgumentException(
                    MercurialCheckConnectionErrorResolver.INVALID_URI_OR_INSUFFICIENT_ACCESS_RIGHTS_ERROR_MESSAGE, e);
            }
            catch (MercurialMissingException e)
            {
                throw new ArgumentException(
                    MercurialCheckConnectionErrorResolver.MERCURIAL_IS_NOT_INSTALLED_ERROR_MESSAGE, e);
            }

            return repository;
        }

        private MercurialRepositoryFolder GetLocalRepository(ISourceControlConnectionSettingsSource settings)
        {
            if (_folder.Empty())
            {
                var repositoryFolder = MercurialRepositoryFolder.Create(settings.Uri);
                _folder.ReplaceWith(repositoryFolder);
                return repositoryFolder;
            }

            MercurialRepositoryFolder folder = _folder.Single();
            if (!folder.CheckFolder(_folder))
            {
                var repositoryFolder = MercurialRepositoryFolder.Create(settings.Uri);
                _folder.ReplaceWith(repositoryFolder);
                return repositoryFolder;
            }

            return folder;
        }

        private static bool IsRepositoryUriChanged(
            MercurialRepositoryFolder repositoryFolder,
            ISourceControlConnectionSettingsSource settings)
        {
            return (settings.Uri.ToLower() != repositoryFolder.RepoUri.ToLower()) && repositoryFolder.Exists();
        }
    }

    /// <summary>
    /// This class implements the "hg log" command (<see href="http://www.selenic.com/mercurial/hg.1.html#log"/>):
    /// show revision history of entire repository or files.
    /// </summary>
    internal sealed class LogCommandNoVerbose : IncludeExcludeCommandBase<LogCommandNoVerbose>, IMercurialCommand<IEnumerable<Changeset>>
    {
        /// <summary>
        /// This is the backing field for the <see cref="Users"/> property.
        /// </summary>
        private readonly List<string> _Users = new List<string>();

        /// <summary>
        /// This is the backing field for the <see cref="Keywords"/> property.
        /// </summary>
        private readonly List<string> _Keywords = new List<string>();

        /// <summary>
        /// This is the backing field for the <see cref="Revisions"/> property.
        /// </summary>
        private readonly List<RevSpec> _Revisions = new List<RevSpec>();

        /// <summary>
        /// This is the backing field for the <see cref="Path"/> property.
        /// </summary>
        private string _Path = string.Empty;

        /// <summary>
        /// This is the backing field for the <see cref="IncludeHiddenChangesets"/> property.
        /// </summary>
        private bool _IncludeHiddenChangesets;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogCommandNoVerbose"/> class.
        /// </summary>
        public LogCommandNoVerbose()
            : base("log")
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets or sets the date to show the log for, or <c>null</c> if no filtering on date should be done.
        /// Default is <c>null</c>.
        /// </summary>
        [DateTimeArgument(NonNullOption = "--date", Format = "yyyy-MM-dd")]
        [DefaultValue(null)]
        public DateTime? Date { get; set; }

        /// <summary>
        /// Gets the collection of case-insensitive keywords to search the log for.
        /// Default is empty which indicates no searching will be done.
        /// </summary>
        [RepeatableArgument(Option = "--keyword")]
        public Collection<string> Keywords
        {
            get { return new Collection<string>(_Keywords); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to follow renames and copies when limiting the log.
        /// Default is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--follow")]
        [DefaultValue(false)]
        public bool FollowRenamesAndMoves { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to only follow the first parent of merge changesets.
        /// Default value is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--follow-first")]
        [DefaultValue(false)]
        public bool OnlyFollowFirstParent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include path actions (which files were modified, and the
        /// type of modification) or not. Default is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--verbose")]
        [DefaultValue(false)]
        public bool IncludePathActions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include hidden changesets.
        /// Default is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--hidden")]
        [DefaultValue(false)]
        public bool IncludeHiddenChangesets
        {
            get { return _IncludeHiddenChangesets; }

            set
            {
                RequiresVersion(new Version(1, 9), "The IncludeHiddenChangesets property of the LogCommand");
                _IncludeHiddenChangesets = value;
            }
        }

        /// <summary>
        /// Gets or sets the path to produce the log for, or <see cref="string.Empty"/> to produce
        /// the log for the repository. Default is <see cref="string.Empty"/>.
        /// </summary>
        [NullableArgument]
        [DefaultValue("")]
        public string Path
        {
            get { return _Path; }

            set { _Path = (value ?? string.Empty).Trim(); }
        }

        /// <summary>
        /// Gets the collection of users to produce the log for, or an empty collection to produce
        /// the log for the repository. Default is an empty collection.
        /// </summary>
        [RepeatableArgument(Option = "--user")]
        public Collection<string> Users
        {
            get { return new Collection<string>(_Users); }
        }

        /// <summary>
        /// Gets the collection of <see cref="Revisions"/> to process/include.
        /// </summary>
        [RepeatableArgument(Option = "--rev")]
        public Collection<RevSpec> Revisions
        {
            get { return new Collection<RevSpec>(_Revisions); }
        }

        #region IMercurialCommand<IEnumerable<Changeset>> Members

        /// <summary>
        /// Gets all the arguments to the <see cref="CommandBase{T}.Command"/>, or an
        /// empty array if there are none.
        /// </summary>
        /// <value></value>
        public override IEnumerable<string> Arguments
        {
            get
            {
                var arguments = new[]
                {
                    "--style=XML"
                };
                return arguments.Concat(base.Arguments).ToArray();
            }
        }

        /// <summary>
        /// Gets the result of executing the command as a collection of <see cref="Changeset"/> objects.
        /// </summary>
        public IEnumerable<Changeset> Result { get; private set; }

        #endregion

        /// <summary>
        /// Sets the <see cref="Date"/> property to the specified value and
        /// returns this <see cref="LogCommandNoVerbose"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Date"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="LogCommandNoVerbose"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public LogCommandNoVerbose WithDate(DateTime value)
        {
            Date = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="FollowRenamesAndMoves"/> property to the specified value and
        /// returns this <see cref="LogCommandNoVerbose"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="FollowRenamesAndMoves"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="LogCommandNoVerbose"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public LogCommandNoVerbose WithFollowRenamesAndMoves(bool value = true)
        {
            FollowRenamesAndMoves = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="OnlyFollowFirstParent"/> property to the specified value and
        /// returns this <see cref="LogCommandNoVerbose"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="OnlyFollowFirstParent"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="LogCommandNoVerbose"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public LogCommandNoVerbose WithOnlyFollowFirstParent(bool value = true)
        {
            OnlyFollowFirstParent = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="IncludePathActions"/> property to the specified value and
        /// returns this <see cref="LogCommandNoVerbose"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="IncludePathActions"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="LogCommandNoVerbose"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public LogCommandNoVerbose WithIncludePathActions(bool value = true)
        {
            IncludePathActions = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="IncludeHiddenChangesets"/> property to the specified value and
        /// returns this <see cref="LogCommandNoVerbose"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="IncludeHiddenChangesets"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="LogCommandNoVerbose"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public LogCommandNoVerbose WithIncludeHiddenChangesets(bool value = true)
        {
            IncludeHiddenChangesets = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="Path"/> property to the specified value and
        /// returns this <see cref="LogCommandNoVerbose"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Path"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="LogCommandNoVerbose"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public LogCommandNoVerbose WithPath(string value)
        {
            Path = value;
            return this;
        }

        /// <summary>
        /// Adds the value to the <see cref="Revisions"/> collection property and
        /// returns this <see cref="LogCommandNoVerbose"/> instance.
        /// </summary>
        /// <param name="value">
        /// The value to add to the <see cref="Revisions"/> collection property.
        /// </param>
        /// <returns>
        /// This <see cref="LogCommandNoVerbose"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public LogCommandNoVerbose WithRevision(RevSpec value)
        {
            Revisions.Add(value);
            return this;
        }

        /// <summary>
        /// Adds the value to the <see cref="Keywords"/> collection property and
        /// returns this <see cref="LogCommandNoVerbose"/> instance.
        /// </summary>
        /// <param name="value">
        /// The value to add to the <see cref="Keywords"/> collection property.
        /// </param>
        /// <returns>
        /// This <see cref="LogCommandNoVerbose"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public LogCommandNoVerbose WithKeyword(string value)
        {
            Keywords.Add(value);
            return this;
        }

        /// <summary>
        /// Sets the <see cref="Users"/> property to the specified value and
        /// returns this <see cref="LogCommandNoVerbose"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Users"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="LogCommandNoVerbose"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public LogCommandNoVerbose WithUser(string value)
        {
            Users.Add(value);
            return this;
        }

        /// <summary>
        /// Parses the standard output for results.
        /// </summary>
        /// <param name="exitCode">The exit code.</param>
        /// <param name="standardOutput">The standard output.</param>
        protected override void ParseStandardOutputForResults(int exitCode, string standardOutput)
        {
            base.ParseStandardOutputForResults(exitCode, standardOutput);
            Result = ChangesetXmlParser.LazyParse(standardOutput);
        }
    }
}
