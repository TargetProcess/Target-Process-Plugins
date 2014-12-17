//
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Tp.Core;
using Tp.SourceControl.Settings;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.Tfs.VersionControlSystem
{
	public class TfsClient
	{
		private const int UriTfsProjectCollection = 1;
		private const int UriTfsTeamProject = 2;

		private readonly VersionControlServer _versionControl;
		private readonly ISourceControlConnectionSettingsSource _settings;

		private TfsTeamProjectCollection _teamProjectCollection;
		private TeamProject[] _teamProjects;

		public TfsClient(ISourceControlConnectionSettingsSource settings)
		{
			_settings = settings;
			_versionControl = GetVersionControl(_settings);
		}

		~TfsClient()
		{
			if (_teamProjectCollection != null)
				_teamProjectCollection.Dispose();
		}

		public IEnumerable<RevisionRange> GetFromTillHead(Int32 from, int pageSize)
		{
			return GetChangesetsRanges(changeset => changeset.ChangesetId >= from, pageSize);
		}

		public IEnumerable<RevisionRange> GetAfterTillHead(RevisionId revisionId, int pageSize)
		{
			return GetChangesetsRanges(changeset => changeset.ChangesetId > int.Parse(revisionId.Value), pageSize);
		}

		public IEnumerable<RevisionRange> GetFromAndBefore(RevisionId fromRevision, RevisionId toRevision, int pageSize)
		{
			return GetChangesetsRanges(
					changeset => changeset.ChangesetId >= int.Parse(fromRevision.Value) && changeset.ChangesetId <= int.Parse(toRevision.Value),
					pageSize);
		}

		public Changeset GetParentCommit(Changeset commit)
		{
			int changesetId = GetChangesets(changeset => changeset.ChangesetId < commit.ChangesetId).Max(ch => ch.ChangesetId);
			return _versionControl.GetChangeset(changesetId);
		}

		public Changeset GetCommit(RevisionId id)
		{
			return _versionControl.GetChangeset(int.Parse(id.Value), true, true);
		}

		public string[] RetrieveAuthors(DateRange dateRange)
		{
			var authors = GetChangesets(changeset => changeset.CreationDate >= dateRange.StartDate && changeset.CreationDate <= dateRange.EndDate)
					.Select(changeset => changeset.Committer)
					.Distinct()
					.ToArray();

			return authors;
		}

		public RevisionInfo[] GetRevisions(RevisionId fromChangeset, RevisionId toChangeset)
		{
			var revisionInfos = GetChangesets(changeset => changeset.ChangesetId >= int.Parse(fromChangeset.Value) && changeset.ChangesetId <= int.Parse(toChangeset.Value))
					.Select(changeset => changeset.ToRevisionInfo())
					.ToArray();

			return revisionInfos;
		}

		public string GetFileContent(Changeset commit, string path)
		{
			Changeset changeset = _versionControl.GetChangeset(commit.ChangesetId, true, true);
			Change change = changeset.Changes.FirstOrDefault(ch => ch.Item.ServerItem.Equals(path));

			if (change == null)
				return string.Empty;

			if (change.Item.ItemType != ItemType.File)
				return "This is a folder.";

			string content;
			Stream stream = change.Item.DownloadFile();

			using (StreamReader reader = new StreamReader(stream))
			{
				content = reader.ReadToEnd();
			}

			return content;
		}

		private VersionControlServer GetVersionControl(ISourceControlConnectionSettingsSource settings)
		{
			VersionControlServer versionControl = null;

			TfsConnectionParameters parameters = TfsConnectionHelper.GetTfsConnectionParameters(settings);

			switch (parameters.SegmentsCount)
			{
				case UriTfsProjectCollection:
					{
						_teamProjectCollection = new TfsTeamProjectCollection(parameters.TfsCollectionUri, parameters.Credential);
						_teamProjectCollection.EnsureAuthenticated();

						versionControl = _teamProjectCollection.GetService<VersionControlServer>();
						_teamProjects = versionControl.GetAllTeamProjects(false);

						break;
					}
				case UriTfsTeamProject:
					{
						_teamProjectCollection = new TfsTeamProjectCollection(parameters.TfsCollectionUri, parameters.Credential);
						_teamProjectCollection.EnsureAuthenticated();

						versionControl = _teamProjectCollection.GetService<VersionControlServer>();
						_teamProjects = new TeamProject[] { versionControl.GetTeamProject(parameters.TeamProjectName) };

						break;
					}
				default:
					throw new Exception("Wrong URI format.");
			}

			return versionControl;
		}

		private IEnumerable<Changeset> GetChangesets(Func<Changeset, bool> predicate)
		{
			List<Changeset> changesets = new List<Changeset>();

			foreach (var teamProject in _teamProjects)
			{
				changesets.AddRange(
						_versionControl.QueryHistory(
								teamProject.ServerItem,
								VersionSpec.Latest,
								0,
								RecursionType.Full,
								null,
								null,
								null,
								Int32.MaxValue,
								true,
								true).Cast<Changeset>().Where(predicate).OrderBy(changeset => changeset.ChangesetId));
			}

			return changesets;
		}

		private IEnumerable<RevisionRange> GetChangesetsRanges(Func<Changeset, bool> predicate, int pageSize)
		{
			var changesets = GetChangesets(predicate).ToList();
			var pages = changesets.Split(pageSize);
			var result = pages.Select(page => new RevisionRange(page.First().ToRevisionId(), page.Last().ToRevisionId()));

			return result;
		}
	}
}