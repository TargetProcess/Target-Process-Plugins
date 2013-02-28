// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using Mercurial;
using Tp.Core;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Validation;
using Tp.SourceControl.Commands;
using Tp.SourceControl.Diff;
using Tp.SourceControl.Settings;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.Mercurial.VersionControlSystem
{
	public class MercurialVersionControlSystem : SourceControl.VersionControlSystem.VersionControlSystem
	{
		private readonly IDiffProcessor _diffProcessor;
		private readonly MercurialClient _mercurial;

        public MercurialVersionControlSystem(
            ISourceControlConnectionSettingsSource settings, 
            ICheckConnectionErrorResolver errorResolver, 
            IActivityLogger logger, 
            IDiffProcessor diffProcessor,
            IStorageRepository profile) : base(settings, errorResolver, logger)
		{
			_diffProcessor = diffProcessor;
            _mercurial = new MercurialClient(settings, profile.Get<MercurialRepositoryFolder>());
		}

		public override RevisionInfo[] GetRevisions(RevisionRange revisionRange)
		{
            return _mercurial.GetRevisions(revisionRange.FromChangeset, revisionRange.ToChangeset);
		}

		public override string GetTextFileContent(RevisionId changeset, string path)
		{
            var commit = _mercurial.GetCommit(changeset);

            return GetFileContent(commit, path);
		}

		public override byte[] GetBinaryFileContent(RevisionId changeset, string path)
		{
			throw new NotImplementedException();
		}

		public override void CheckRevision(RevisionId revision, PluginProfileErrorCollection errors)
		{
            MercurialRevisionId revisionId = revision;
            if (revisionId.Time > MercurialRevisionId.UtcTimeMax)
			{
				_errorResolver.HandleConnectionError(new InvalidRevisionException(), errors);
			}
		}

		public override string[] RetrieveAuthors(DateRange dateRange)
		{
			return _mercurial.RetrieveAuthors(dateRange);
		}

		public override RevisionRange[] GetFromTillHead(RevisionId @from, int pageSize)
		{
            return _mercurial.GetFromTillHead(from.Time.Value, pageSize).ToArray();
		}

		public override RevisionRange[] GetAfterTillHead(RevisionId @from, int pageSize)
		{
            return _mercurial.GetAfterTillHead(from, pageSize).ToArray();
		}

		public override RevisionRange[] GetFromAndBefore(RevisionId @from, RevisionId to, int pageSize)
		{
            return _mercurial.GetFromAndBefore(from, to, pageSize).ToArray();
		}

		public override DiffResult GetDiff(RevisionId changeset, string path)
		{
            var commit = _mercurial.GetCommit(changeset);
            var parent = _mercurial.GetParentCommit(commit);

			try
			{
				return GetDiff(path, parent, commit);
			}
			catch (MercurialException ex)
			{
				throw new VersionControlException(String.Format("Mercurial exception: {0}", ex.Message));
			}
		}

        private DiffResult GetDiff(string path, Changeset prevCommmit, Changeset commit)
		{
			var fileContent = GetTextFileContentSafe(commit, path);
            var previousRevisionFileContent = GetTextFileContentSafe(prevCommmit, path);
			var diff = _diffProcessor.GetDiff(previousRevisionFileContent, fileContent);

            diff.LeftPanRevisionId = prevCommmit.RevisionNumber.ToString();
            diff.RightPanRevisionId = commit.RevisionNumber.ToString();

			return diff;
		}

		private string GetTextFileContentSafe(Changeset commit, string path)
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

        private string GetFileContent(Changeset commit, string path)
        {
            return _mercurial.GetFileContent(commit, path);
        }
	}
}