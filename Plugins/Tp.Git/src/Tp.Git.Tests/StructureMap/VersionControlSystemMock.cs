// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using Tp.Core;
using Tp.Integration.Plugin.Common.Validation;
using Tp.SourceControl.Diff;
using Tp.SourceControl.Settings;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.Subversion.StructureMap
{
	public class VersionControlSystemMock : IVersionControlSystem
	{
		public VersionControlSystemMock()
		{
			Revisions = new List<RevisionInfo>();
		}

		public List<RevisionInfo> Revisions { get; private set; }

		public void Dispose() {}

		public void Connect(ConnectionSettings settings) {}

		public RevisionId GetLastRevisionID()
		{
			return Revisions.Select(x => x.Id).Last();
		}

		public RevisionInfo[] GetRevisions(RevisionRange revisionRange, string path)
		{
			throw new NotImplementedException();
		}

		public RevisionInfo[] GetRevisions(RevisionRange revisionRange)
		{
			return Revisions.FindAll(x =>
			                         long.Parse(x.Id.Value) >= long.Parse(revisionRange.FromChangeset.Value) &&
			                         long.Parse(x.Id.Value) <= long.Parse(revisionRange.ToChangeset.Value)).ToArray();
		}

		public string GetTextFileContent(RevisionId changeset, string path)
		{
			throw new NotImplementedException();
		}

		public byte[] GetBinaryFileContent(RevisionId changeset, string path)
		{
			throw new NotImplementedException();
		}

		public void CheckConnection(ConnectionSettings settings, PluginProfileErrorCollection errors) {}

		public void CheckRevision(RevisionId revision, PluginProfileErrorCollection errors) {}

		public string[] RetrieveAuthors(DateRange dateRange)
		{
			return Revisions.Where(x => dateRange.Contains(x.Time)).Select(x => x.Author).ToArray();
		}

		public RevisionRange[] GetFromTillHead(RevisionId @from, int pageSize)
		{
			var currentRevision = from;
			var lastRevision = long.Parse(Revisions.Last().Id.Value);

			return GetFromTo(currentRevision, lastRevision, pageSize);
		}

		private static RevisionRange[] GetFromTo(RevisionId @from, long to, int pageSize)
		{
			var result = new List<RevisionRange>();
			while (long.Parse(@from.Value) <= to)
			{
				var currentRevisionId = long.Parse(@from.Value);

				if ((currentRevisionId + pageSize) < to)
				{
					result.Add(new RevisionRange(@from, (currentRevisionId + pageSize - 1).ToString()));
				}
				else
				{
					result.Add(new RevisionRange(@from, to.ToString()));
				}

				@from = (currentRevisionId + pageSize).ToString();
			}
			return result.ToArray();
		}

		public RevisionRange[] GetAfterTillHead(RevisionId @from, int pageSize)
		{
			var revisionInfo = Revisions.Find(x => long.Parse(x.Id.Value) == long.Parse(@from.Value));
			if (revisionInfo == null)
			{
				return new RevisionRange[] {};
			}

			if (Revisions.Last() == revisionInfo)
			{
				return new RevisionRange[] {};
			}

			var revision = Revisions[Revisions.IndexOf(revisionInfo) + 1];
			return GetFromTillHead(revision.Id, pageSize);
		}

		public RevisionRange[] GetFromAndBefore(RevisionId @from, RevisionId to, int pageSize)
		{
			return GetFromTo(@from, long.Parse(to.Value) - 1, pageSize);
		}

		public DiffResult GetDiff(RevisionId changeset, string path)
		{
			throw new NotImplementedException();
		}
	}
}