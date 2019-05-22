// 
// Copyright (c) 2005-2018 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Perforce.P4;
using Tp.Core;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Validation;
using Tp.SourceControl.Commands;
using Tp.SourceControl.Diff;
using Tp.SourceControl.Settings;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.Perforce.VersionControlSystem
{
    public class P4VersionControlSystem : VersionControlSystem<ISourceControlConnectionSettingsSource>
    {
        private readonly IDiffProcessor _diffProcessor;
        private Repository _p4Repository;

        public P4VersionControlSystem(ISourceControlConnectionSettingsSource settings, ICheckConnectionErrorResolver errorResolver, IActivityLogger logger, IDiffProcessor diffProcessor) : base(settings, errorResolver, logger)
        {
            _diffProcessor = diffProcessor;
            var repository = P4Repository;
        }

        public override void Dispose()
        {
            if (_p4Repository?.Connection != null)
            {
                _p4Repository.Connection.Logout(null);
                _p4Repository.Connection.Disconnect();
            }

            base.Dispose();
        }

        private Repository P4Repository
        {
            get
            {
                if (_p4Repository != null)
                {
                    return _p4Repository;
                }

                try
                {
                    _p4Repository = CreatePerforceRepository();
                }
                catch (P4Exception ex)
                {
                    var err = new StringBuilder(ex.Message);
                    if (ex.InnerException != null)
                    {
                        var iex = ex.InnerException;
                        while (iex != null)
                        {
                            err.AppendLine("");
                            err.Append(iex.Message);
                            if (iex.InnerException != null)
                            {
                                iex = iex.InnerException;
                            }
                        }
                    }
                    _logger.Error($"Error Connecting to Perforce\r\n {err}\r\n{ex.StackTrace}");
                    throw new VersionControlException($"Perforce exception: {ex.Message.Trim()}", ex);
                }
                return _p4Repository;
            }
        }

        private Repository CreatePerforceRepository()
        {
            // define the server, repository and connection
            Server server = new Server(new ServerAddress(_settings.Uri));
            Repository repository = new Repository(server);
            // Connect to Perforce server
            repository.Connection.UserName = _settings.Login;
            repository.Connection.Client = new Client();
            repository.Connection.Connect(null);
            Options opts = new Options(LoginCmdFlags.DisplayTicket, null);
            // login
            Credential cred = repository.Connection.Login(_settings.Password, opts);
            // log any errors and log ticket
            P4CommandResult loginCommandResult = repository.Connection.LastResults;
            if (loginCommandResult.ErrorList != null) // No errors returned
            {
                foreach (var p4ClientError in loginCommandResult.ErrorList)
                {
                    _logger.Debug(p4ClientError.ErrorMessage);
                }
            }
            if (loginCommandResult.InfoOutput != null) // InfoOutput=.....Success:  Password verified./r/n3EAFE4C286C01E4C10AFC44269059DF4/r/n
            {
                _logger.Debug("Success: Password verified." + loginCommandResult.InfoOutput);
            }
            if (cred == null)
            {
                _logger.Debug("cred = null");
            }
            else
            {
                _logger.Debug("cred.Ticket=" + cred.Ticket); // cred.Ticket=Success:  Password verified.
                repository.Connection.Credential = cred;
                //repository.Connection.CharacterSetName = "uft8";
                //repository.Connection.SetP4EnvironmentVar("P4CHARSET", "uft8");

                //If the server is set to Unicode - mode, the client sets P4CHARSET to auto and examines the client's environment to determine the character set to use in
                //converting files of type unicode. Thus, the only time you need to set P4CHARSET to a specific type is if the client's choice of charset results in a faulty
                //conversion or if you have other special needs. For example, the application that uses the checked out files expects a specific character set.

                //P4CHARSET only affects files of type unicode and utf16; non - unicode files are never translated.

                if (_settings is P4PluginProfile settings && !string.IsNullOrEmpty(settings.Workspace))
                {
                    repository.Connection.SetClient(settings.Workspace);
                }
            }

            var p4Info = repository.GetServerMetaData(null);
            _logger.Info($"Perforce Server info: Product - {p4Info.Version.Product}, Version - {p4Info.Version}, Platform - {p4Info.Version.Platform}");

            return repository;
        }

        public override RevisionInfo[] GetRevisions(RevisionRange revisionRange)
        {
            try
            {
                P4RevisionId fromChangeset = revisionRange.FromChangeset;
                P4RevisionId toChangeset = revisionRange.ToChangeset;

                _logger.Debug($"Getting revision infos [{fromChangeset}:{toChangeset}]");

                var changesCmdOptions = new ChangesCmdOptions(ChangesCmdFlags.None, null, toChangeset.Value - fromChangeset.Value + 1,
                    ChangeListStatus.Submitted, null);

                var toFileSpec = new FileSpec(new DepotPath(null), null, null, new ChangelistIdVersion(toChangeset.Value));
                var changelists = _p4Repository.GetChangelists(changesCmdOptions, toFileSpec) ?? new List<Changelist>();

                var changes = changelists.Where(c => c.Id >= fromChangeset.Value && c.Id <= toChangeset.Value).OrderBy(x => x.Id)
                    .Select(c => _p4Repository.GetChangelist(c.Id, new ChangeCmdOptions(ChangeCmdFlags.IncludeJobs))).ToList();

                return P4Utils.ArrayOfSvnRevisionToArrayOfRevisionInfo(changes, this).ToArray();
            }
            catch (P4Exception ex)
            {
                throw new VersionControlException($"Perforce exception: {ex.Message}", ex);
            }
        }

        public override string GetTextFileContent(RevisionId changeset, string path)
        {
            P4RevisionId p4RevisionId = changeset;

            var fileMetaDataCmdOptions =
                new GetFileMetaDataCmdOptions(GetFileMetadataCmdFlags.AllRevisions, null, null, 1, null, null, null);
            var toFileSpec = new FileSpec(new DepotPath(path), null, null, new ChangelistIdVersion(p4RevisionId.Value));
            var fileMetaDatas = _p4Repository.GetFileMetaData(fileMetaDataCmdOptions, toFileSpec);

            var commit = fileMetaDatas.FirstOrDefault(x => x.HeadChange == p4RevisionId.Value);

            if (commit == null)
            {
                throw new VersionControlException($"No revision #{changeset.Value} found in Perforce",
                    new P4RevisionNotFoundException(p4RevisionId.Value));
            }

            return GetFileContent(commit, path);
        }

        public override byte[] GetBinaryFileContent(RevisionId changeset, string path)
        {
            throw new NotImplementedException();
        }

        public override void CheckRevision(RevisionId revision, PluginProfileErrorCollection errors)
        {
            try
            {
                var revisions = GetRevisions(new RevisionRange(revision, revision));
                if (!revisions.Any())
                {
                    throw new VersionControlException($"No revision #{revision.Value} found in Perforce",
                        new P4RevisionNotFoundException(((P4RevisionId)revision).Value));
                }
            }
            catch (VersionControlException ex)
            {
                _errorResolver.HandleConnectionError(ex, errors);
            }
        }

        public override string[] RetrieveAuthors(DateRange dateRange)
        {
            var opts = new UsersCmdOptions(UsersCmdFlags.IncludeAll, -1);
            var users = _p4Repository.GetUsers(opts, null);
            return users.Select(user => user.FullName).ToArray();
        }

        public override RevisionRange[] GetFromTillHead(RevisionId from, int pageSize)
        {
            return GetFromTo(from, new P4RevisionId(int.MaxValue), pageSize);
        }

        private RevisionRange[] GetFromTo(P4RevisionId from, P4RevisionId to, int pageSize)
        {
            var result = new List<RevisionRange>();

            if (to.Value == int.MaxValue)
            {
                var changesCmdOptions = new ChangesCmdOptions(ChangesCmdFlags.None, null, 1, ChangeListStatus.Submitted, null);
                var lastChangelist = _p4Repository.GetChangelists(changesCmdOptions);
                to = new P4RevisionId(lastChangelist.First().Id);
            }

            while (from.Value <= to.Value)
            {
                var fromRevisionId = from;

                RevisionRange revisionRange;

                var fromRevision = fromRevisionId;
                if (fromRevisionId.Value + pageSize < to.Value)
                {
                    revisionRange = new RevisionRange(fromRevision,
                        new RevisionId { Value = (fromRevisionId.Value + pageSize - 1).ToString() });
                }
                else
                {
                    revisionRange = new RevisionRange(fromRevision, to);
                }

                result.Add(revisionRange);

                from = new P4RevisionId(fromRevisionId.Value + pageSize);
            }
            return result.ToArray();
        }

        public override RevisionRange[] GetAfterTillHead(RevisionId @from, int pageSize)
        {
            P4RevisionId p4RevisionId = from;
            p4RevisionId = p4RevisionId.Value + 1;
            return GetFromTillHead(p4RevisionId, pageSize);
        }

        public override RevisionRange[] GetFromAndBefore(RevisionId from, RevisionId to, int pageSize)
        {
            P4RevisionId p4RevisionId = to;
            p4RevisionId = p4RevisionId.Value - 1;
            return GetFromTo(from, p4RevisionId, pageSize);
        }

        public override DiffResult GetDiff(RevisionId changeset, string path)
        {
            try
            {
                P4RevisionId p4RevisionId = changeset;

                var fileMetaDataCmdOptions =
                    new GetFileMetaDataCmdOptions(GetFileMetadataCmdFlags.AllRevisions, null, null, 2, null, null, null);
                var toFileSpec = new FileSpec(new DepotPath(path), null, null, new ChangelistIdVersion(p4RevisionId.Value));

                var fileMetaDatas = _p4Repository.GetFileMetaData(fileMetaDataCmdOptions, toFileSpec);

                var commit = fileMetaDatas.FirstOrDefault(m => m.HeadChange == p4RevisionId.Value);
                var prevCommmit = fileMetaDatas.FirstOrDefault(m => m.HeadChange != p4RevisionId.Value);

                if (commit == null || commit.HeadChange != p4RevisionId.Value)
                {
                    throw new VersionControlException($"No revision #{changeset.Value} found in Perforce");
                }

                if (prevCommmit == null || prevCommmit.HeadChange >= p4RevisionId.Value)
                {
                    throw new VersionControlException($"No parent revision of revision #{changeset.Value} found in Perforce");
                }

                return GetDiff(path, prevCommmit, commit);
            }
            catch (P4Exception ex)
            {
                throw new VersionControlException($"Perforce exception: {ex.Message}");
            }
        }

        private DiffResult GetDiff(string path, FileMetaData prevCommmit, FileMetaData commit)
        {
            var fileContent = GetTextFileContentSafe(commit, path);
            var previousRevisionFileContent = GetTextFileContentSafe(prevCommmit, path);
            var diff = _diffProcessor.GetDiff(previousRevisionFileContent, fileContent);

            diff.LeftPanRevisionId = prevCommmit.HeadRev.ToString();
            diff.RightPanRevisionId = commit.HeadRev.ToString();

            return diff;
        }

        private string GetTextFileContentSafe(FileMetaData commit, string path)
        {
            try
            {
                return GetFileContent(commit, path);
            }
            catch
            {
                return string.Empty;
            }
        }

        private string GetFileContent(FileMetaData commit, string path)
        {
            if (commit.HeadType.BaseType == BaseFileType.Binary)
            {
                return $"Binary file: {path}";
            }

            if (commit.HeadType.BaseType == BaseFileType.Resource)
            {
                return $"Binary file: {path}";
            }

            var contentsCmdOptions = new GetFileContentsCmdOptions(GetFileContentsCmdFlags.Suppress, null);
            var content = _p4Repository.GetFileContents(contentsCmdOptions,
                new FileSpec(commit.DepotPath, new Revision(commit.HeadRev)));

            var fileContent = content[0];

            if (commit.HeadType.BaseType == BaseFileType.UTF8)
            {
                return Encoding.UTF8.GetString(Encoding.Default.GetBytes(content[0]));
            }

            return fileContent;
        }
    }

    internal class P4RevisionNotFoundException : P4Exception
    {
        public int Revision { get; }

        public P4RevisionNotFoundException(int revision) : base(ErrorSeverity.E_WARN, $"No revision #{revision} found in Perforce")
        {
            Revision = revision;
        }
    }
}
