// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using NGit.Api.Errors;
using NGit.Revwalk;
using Tp.Core;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Validation;
using Tp.SourceControl.Commands;
using Tp.SourceControl.Diff;
using Tp.SourceControl.Settings;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.Git.VersionControlSystem
{
	public class GitVersionControlSystem : SourceControl.VersionControlSystem.VersionControlSystem
	{
		private readonly IDiffProcessor _diffProcessor;

		private readonly GitClient _git;

		public GitVersionControlSystem(ISourceControlConnectionSettingsSource settings, ICheckConnectionErrorResolver errorResolver, IActivityLogger logger, IDiffProcessor diffProcessor)
			: base(settings, errorResolver, logger)
		{
			_diffProcessor = diffProcessor;
			_git = new GitClient(settings);
		}

		public override RevisionInfo[] GetRevisions(RevisionRange revisionRange)
		{
			return _git.GetRevisions(revisionRange.FromChangeset, revisionRange.ToChangeset);
		}

		public override string GetTextFileContent(RevisionId changeset, string path)
		{
			var commit = _git.GetCommit(changeset);

			return GetFileContent(commit, path);
		}

		public override byte[] GetBinaryFileContent(RevisionId changeset, string path)
		{
			throw new NotImplementedException();
		}

		public override void CheckRevision(RevisionId revision, PluginProfileErrorCollection errors)
		{
			GitRevisionId revisionId = revision;
			if (revisionId.Time > GitRevisionId.UtcTimeMax)
			{
				_errorResolver.HandleConnectionError(new InvalidRevisionException(), errors);
			}
		}

		public override string[] RetrieveAuthors(DateRange dateRange)
		{
			return _git.RetrieveAuthors(dateRange);
		}

		public override RevisionRange[] GetFromTillHead(RevisionId @from, int pageSize)
		{
			return _git.GetFromTillHead(from.Time.Value, pageSize).ToArray();
		}

		public override RevisionRange[] GetAfterTillHead(RevisionId @from, int pageSize)
		{
			return _git.GetAfterTillHead(from, pageSize).ToArray();
		}

		public override RevisionRange[] GetFromAndBefore(RevisionId @from, RevisionId to, int pageSize)
		{
			return _git.GetFromAndBefore(from, to, pageSize).ToArray();
		}

		public override DiffResult GetDiff(RevisionId changeset, string path)
		{
			var commit = _git.GetCommit(changeset);
			var parent = _git.GetCommit(commit.GetParent(0).Id.Name);

			try
			{
				return GetDiff(path, parent, commit);
			}
			catch (GitAPIException ex)
			{
				throw new VersionControlException(String.Format("Git exception: {0}", ex.Message));
			}
		}

		private DiffResult GetDiff(string path, RevCommit parent, RevCommit commit)
		{
			var fileContent = GetTextFileContentSafe(commit, path);
			var previousRevisionFileContent = GetTextFileContentSafe(parent, path);
			var diff = _diffProcessor.GetDiff(previousRevisionFileContent, fileContent);

			diff.LeftPanRevisionId = parent.Id.Name;
			diff.RightPanRevisionId = commit.Id.Name;

			return diff;
		}

		private string GetTextFileContentSafe(RevCommit commit, string path)
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

		private string GetFileContent(RevCommit commit, string path)
		{
			return _git.GetFileContent(commit, path);
		}
	}
}