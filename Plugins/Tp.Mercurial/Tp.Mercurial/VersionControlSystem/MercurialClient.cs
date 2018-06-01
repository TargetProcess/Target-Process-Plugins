// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mercurial;
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
            var command = new LogCommand().WithAdditionalArgument("-d >{0:yyyy-MM-dd}".Fmt(from));
            var pages = _repository.Log(command)
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
            var command = new LogCommand().WithRevision(RevSpec.From(revSpec) && !new RevSpec(revisionId.Value));
            var pages = _repository.Log(command)
                .OrderBy(ch => ch.Timestamp)
                .ToArray()
                .Split(pageSize);

            var result = pages.Select(page => new RevisionRange(page.First().ToRevisionId(), page.Last().ToRevisionId()));

            return result;
        }

        public IEnumerable<RevisionRange> GetFromAndBefore(RevisionId fromRevision, RevisionId toRevision, int pageSize)
        {
            var command = new LogCommand();
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

            var pages = _repository.Log(command)
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
            var command = new LogCommand();
            var authors = _repository.Log(command)
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
}
