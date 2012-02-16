// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Core;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Validation;
using Tp.SourceControl.Commands;
using Tp.SourceControl.Diff;
using Tp.SourceControl.Settings;

namespace Tp.SourceControl.VersionControlSystem
{
	public abstract class VersionControlSystem : IVersionControlSystem
	{
		protected readonly ISourceControlConnectionSettingsSource _settings;
		protected readonly ICheckConnectionErrorResolver _errorResolver;
		protected readonly IActivityLogger _logger;

		protected VersionControlSystem(ISourceControlConnectionSettingsSource settings,
		                               ICheckConnectionErrorResolver errorResolver, IActivityLogger logger)
		{
			_settings = settings;
			_errorResolver = errorResolver;
			_logger = logger;
		}

		public virtual void Dispose()
		{
		}

		public abstract RevisionInfo[] GetRevisions(RevisionRange revisionRange);
		public abstract string GetTextFileContent(RevisionId changeset, string path);
		public abstract byte[] GetBinaryFileContent(RevisionId changeset, string path);
		public abstract void CheckRevision(RevisionId revision, PluginProfileErrorCollection errors);
		public abstract string[] RetrieveAuthors(DateRange dateRange);
		public abstract RevisionRange[] GetFromTillHead(RevisionId @from, int pageSize);
		public abstract RevisionRange[] GetAfterTillHead(RevisionId @from, int pageSize);
		public abstract RevisionRange[] GetFromAndBefore(RevisionId @from, RevisionId to, int pageSize);
		public abstract DiffResult GetDiff(RevisionId changeset, string path);
	}
}