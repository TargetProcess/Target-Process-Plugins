// 
// Copyright (c) 2005-2018 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using SharpSvn;
using SharpSvn.Security;
using Tp.Core;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Validation;
using Tp.SourceControl.Commands;
using Tp.SourceControl.Diff;
using Tp.SourceControl.Settings;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.Subversion.Subversion
{
    public class Subversion : VersionControlSystem<ISourceControlConnectionSettingsSource>
    {
        private readonly IDiffProcessor _diffProcessor;
        private SvnClient _client;

        private string _root;
        private string _projectPath;

        public Subversion(ISourceControlConnectionSettingsSource settings, ICheckConnectionErrorResolver errorResolver,
            IActivityLogger logger, IDiffProcessor diffProcessor)
            : base(settings, errorResolver, logger)
        {
            _diffProcessor = diffProcessor;
            Connect();
        }

        private void Connect()
        {
            var client = Client; //trigger connection validation
        }

        private SvnClient Client
        {
            get
            {
                if (_client != null)
                {
                    return _client;
                }

                _client = CreateSvnClient();

                try
                {
                    var info = GetRepositoryInfo();

                    _root = info.RepositoryRoot.AbsoluteUri;
                    _projectPath = HttpUtility.UrlDecode(info.Uri.AbsoluteUri.Substring(_root.Length));
                    _root.Remove(_root.Length - 1, 1);
                }
                catch (SvnException ex)
                {
                    _logger.Error("Connection failed.", ex);
                    throw new VersionControlException($"Subversion exception: {ex.Message.Trim()}", ex);
                }
                return _client;
            }
        }

        #region ISourceControlSession Members

        private SvnRevisionId GetLastRevisionId()
        {
            try
            {
                var repos = _settings.Uri;
                Client.GetInfo(repos, out var info);

                return info.Revision;
            }
            catch (SvnException ex)
            {
                _logger.Error("Svn plugin Error when gettin revision id.", ex);
                throw;
            }
        }

        private SvnInfoEventArgs GetRepositoryInfo(string root)
        {
            _logger.Debug($"Getting repository info '{root}'");

            var repos = new Uri(root);
            Client.GetInfo(repos, out var info);
            return info;
        }

        public override RevisionInfo[] GetRevisions(RevisionRange revisionRange)
        {
            try
            {
                SvnRevisionId fromChangeset = revisionRange.FromChangeset;
                SvnRevisionId toChangeset = revisionRange.ToChangeset;

                _logger.Debug($"Getting revision infos [{fromChangeset}:{toChangeset}]");
                var arg = new SvnLogArgs(new SvnRevisionRange(fromChangeset.Value, toChangeset.Value)) { ThrowOnError = true };
                return SubversionUtils.ArrayOfSvnRevisionToArrayOfRevisionInfo(GetSvnRevisions(arg), this).ToArray();
            }
            catch (SvnException e)
            {
                throw new VersionControlException($"Subversion exception: {e.Message}", e);
            }
        }

        private Collection<SvnLogEventArgs> GetSvnRevisions(SvnLogArgs arg)
        {
            return GetSvnRevisions(arg, _projectPath);
        }

        private Collection<SvnLogEventArgs> GetSvnRevisions(SvnLogArgs arg, string path)
        {
            var targetPath = GetPath(path);
            if (Client.GetLog(targetPath, arg, out var svnRevisions))
            {
                RemoveChangedItemFromOtherProject(svnRevisions);
                return svnRevisions;
            }

            return null;
        }

        private Uri GetPath(string path)
        {
            return new Uri(new Uri(_root), path);
        }

        private void RemoveChangedItemFromOtherProject(IEnumerable<SvnLogEventArgs> svnRevisions)
        {
            var removedChangedPaths = new List<SvnChangeItem>();
            foreach (var svnRevision in svnRevisions)
            {
                _logger.Debug($"Processing SVN Revision #{svnRevision.Revision} for removing");

                if (svnRevision.ChangedPaths == null)
                {
                    continue;
                }

                removedChangedPaths.AddRange(svnRevision.ChangedPaths.Where(x => !x.RepositoryPath.ToString().StartsWith(_projectPath)));

                foreach (var changeItem in removedChangedPaths)
                {
                    svnRevision.ChangedPaths.Remove(changeItem);
                }

                _logger.Debug($"SVN Revision #{svnRevision.Revision} is processed for removing");
            }
        }

        public RevisionInfo[] GetRevisions(RevisionRange revisionsRange, string path)
        {
            _logger.Debug($"GET SVN Revisions for {path} [{revisionsRange.FromChangeset}, {revisionsRange.ToChangeset}]");
            var arg = CreateSvnLogArgument(path, new RevisionRange(revisionsRange.FromChangeset, revisionsRange.ToChangeset));
            var tpRevisionInfo = SubversionUtils.ArrayOfSvnRevisionToArrayOfRevisionInfo(GetSvnRevisions(arg, path), this);
            _logger.Debug($"SVN Revisions for {path} [{revisionsRange.FromChangeset}, {revisionsRange.ToChangeset}] is retrieved");

            return tpRevisionInfo.ToArray();
        }

        private SvnLogArgs CreateSvnLogArgument(string path, RevisionRange revisionRange)
        {
            return new SvnLogArgs(CreateSvnRevisionRangeBy(revisionRange)) { BaseUri = GetPath(path), ThrowOnError = true };
        }

        private static SvnRevisionRange CreateSvnRevisionRangeBy(RevisionRange revisionRange)
        {
            SvnRevisionId fromChangeset = revisionRange.FromChangeset;
            SvnRevisionId toChangeSet = revisionRange.ToChangeset;
            return new SvnRevisionRange(fromChangeset.Value, toChangeSet.Value);
        }

        public override string GetTextFileContent(RevisionId changeset, string path)
        {
            try
            {
                using (var stream = GetSvnFileStream(changeset, path))
                {
                    stream.Position = 0;
                    return new StreamReader(stream).ReadToEnd();
                }
            }
            catch (SvnFileSystemException ex)
            {
                throw new VersionControlException($"Subversion exception: {ex.Message}");
            }
        }

        private MemoryStream GetSvnFileStream(SvnRevisionId changeset, string path)
        {
            var memoryStream = new MemoryStream();
            //If you use Uri you should encode '#' as %23, as Uri's define the # as Fragment separator.
            //And in this case the fragment is not send to the server.
            path = path.Replace("#", "%23");
            if (SvnTarget.TryParse(GetPath(path).AbsolutePath, out _))
            {
                if (FileWasDeleted(path, changeset))
                {
                    return new MemoryStream();
                }

                var uriTarget = new SvnUriTarget(_root + path, changeset.Value);
                var svnWriteArgs = new SvnWriteArgs { Revision = changeset.Value };

                Client.Write(uriTarget, memoryStream, svnWriteArgs);
                return memoryStream;
            }
            return new MemoryStream();
        }

        private bool FileWasDeleted(string path, SvnRevisionId changeset)
        {
            var revisionId = new RevisionId { Value = changeset.Value.ToString() };
            var arg = new SvnLogArgs(CreateSvnRevisionRangeBy(new RevisionRange(revisionId, revisionId)))
                { BaseUri = new Uri(_root), ThrowOnError = true };

            var revisions = GetSvnRevisions(arg);

            var item = revisions[0].ChangedPaths.FirstOrDefault(itemCollection => itemCollection.Path == path);

            return item != null && item.Action == SvnChangeAction.Delete;
        }

        private SvnClient CreateSvnClient()
        {
            var client = new SvnClient();
            client.Authentication.DefaultCredentials = new NetworkCredential(_settings.Login, _settings.Password);
            client.Authentication.SslServerTrustHandlers +=
                delegate(object sender, SvnSslServerTrustEventArgs args)
                {
                    // If needed we can look at the rest of the arguments of 'args' whether 
                    // we wish to accept. If accept:
                    args.AcceptedFailures = args.Failures;
                    args.Save = true; // Save acceptance to authentication store
                };
            return client;
        }

        public override byte[] GetBinaryFileContent(RevisionId changeset, string path)
        {
            try
            {
                using (var stream = GetSvnFileStream(new SvnRevisionId(changeset), path))
                {
                    return stream.ToArray();
                }
            }
            catch (SvnFileSystemException ex)
            {
                throw new VersionControlException($"Subversion exception: {ex.Message}");
            }
        }

        private const int Timeout = 20;

        private SvnInfoEventArgs GetRepositoryInfo()
        {
            SvnInfoEventArgs info = null;
            Exception exception = null;
            var repositoryUri = new Uri(_settings.Uri).AbsoluteUri;

            var thread = new Thread(() =>
            {
                try
                {
                    info = GetRepositoryInfo(repositoryUri);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            });
            thread.Start();
            var timeoutAcquired = !thread.Join(TimeSpan.FromSeconds(Timeout));

            if (timeoutAcquired)
            {
                thread.Abort();
            }

            if (!timeoutAcquired && exception != null)
            {
                throw exception;
            }

            if (timeoutAcquired)
            {
                throw new SvnException($"Timeout while connecting to svn repository {_settings.Uri}");
            }

            return info;
        }

        public override void CheckRevision(RevisionId revision, PluginProfileErrorCollection errors)
        {
            try
            {
                GetRevisions(new RevisionRange(revision, revision));
            }
            catch (VersionControlException e)
            {
                _errorResolver.HandleConnectionError(e, errors);
            }
        }

        public override string[] RetrieveAuthors(DateRange dateRange)
        {
            var startRevision = new SvnRevision(dateRange.StartDate.GetValueOrDefault());
            var endRevision = new SvnRevision(dateRange.EndDate.GetValueOrDefault());

            var range = new SvnRevisionRange(startRevision, endRevision);
            var result = GetSvnRevisions(new SvnLogArgs(range));
            return result.Select(x => x.Author).Where(y => !string.IsNullOrEmpty(y)).Distinct().ToArray();
        }

        #endregion

        public override void Dispose()
        {
            if (_client == null)
            {
                return;
            }
            _client.Dispose();
            _client = null;
            GC.SuppressFinalize(this);
        }

        public override RevisionRange[] GetFromTillHead(RevisionId from, int pageSize)
        {
            var lastRevision = GetLastRevisionId();

            return GetFromTo(@from, lastRevision, pageSize);
        }

        private static RevisionRange[] GetFromTo(SvnRevisionId @from, SvnRevisionId lastRevision, int pageSize)
        {
            var result = new List<RevisionRange>();

            while (from.Value <= lastRevision.Value)
            {
                var fromRevisionId = from;

                RevisionRange revisionRange;

                var fromRevision = fromRevisionId;
                if ((fromRevisionId.Value + pageSize) < lastRevision.Value)
                {
                    revisionRange = new RevisionRange(fromRevision,
                        new RevisionId { Value = (fromRevisionId.Value + pageSize - 1).ToString() });
                }
                else
                {
                    revisionRange = new RevisionRange(fromRevision, lastRevision);
                }

                result.Add(revisionRange);

                from = new SvnRevisionId(fromRevisionId.Value + pageSize);
            }
            return result.ToArray();
        }

        public override RevisionRange[] GetAfterTillHead(RevisionId @from, int pageSize)
        {
            SvnRevisionId svnRevisionId = from;
            svnRevisionId = svnRevisionId.Value + 1;
            return GetFromTillHead(svnRevisionId, pageSize);
        }

        public override RevisionRange[] GetFromAndBefore(RevisionId @from, RevisionId to, int pageSize)
        {
            SvnRevisionId lastRevision = ((SvnRevisionId) to).Value - 1;
            return GetFromTo(@from, lastRevision, pageSize);
        }

        public override DiffResult GetDiff(RevisionId changeset, string path)
        {
            var revisionId = new SvnRevisionId(changeset);
            var previousRevisionId = ((revisionId).Value - 1).ToString();
            try
            {
                return GetDiff(changeset, path, previousRevisionId);
            }
            catch (SvnFileSystemException ex)
            {
                throw new VersionControlException($"Subversion exception: {ex.Message}");
            }
        }

        private DiffResult GetDiff(RevisionId changeset, string path, string previousRevisionId)
        {
            var fileContent = GetTextFileContentSafe(changeset.Value, path);
            var previousRevisionFileContent = GetTextFileContentSafe(previousRevisionId, path);
            var diff = _diffProcessor.GetDiff(previousRevisionFileContent, fileContent);

            diff.LeftPanRevisionId = previousRevisionId;
            diff.RightPanRevisionId = changeset.Value;

            return diff;
        }

        private string GetTextFileContentSafe(string revision, string path)
        {
            try
            {
                return GetTextFileContent(revision, path);
            }
            catch
            {
                return string.Empty;
            }
        }

//		private Stream GetDiffFileStream(SvnRevisionId changeset, string path)
//		{
//			var memoryStream = new MemoryStream();
//			SvnTarget target;
//			//If you use Uri you should encode '#' as %23, as Uri's define the # as Fragment separator.
//			//And in this case the fragment is not send to the server.
//			path = path.Replace("#", "%23");
//			if (SvnTarget.TryParse(GetPath(path).AbsolutePath, out target))
//			{
//				if (FileWasDeleted(path, changeset))
//				{
//					return new MemoryStream();
//				}
//
//				var from = new SvnUriTarget(_root + path, changeset.Value);
//				var to = new SvnUriTarget(_root + path, changeset.Value - 1);
////				var svnWriteArgs = new SvnWriteArgs { Revision = changeset.Value };
//
//				Client.Diff(from, to, memoryStream);
//				return memoryStream;
//			}
//			return new MemoryStream();
//		}
    }
}
