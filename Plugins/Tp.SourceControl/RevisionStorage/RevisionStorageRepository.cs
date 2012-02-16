// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Linq;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Storage.Persisters;
using Tp.SourceControl.Settings;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.SourceControl.RevisionStorage
{
	public abstract class RevisionStorageRepository : IRevisionStorageRepository
	{
		private readonly IStorageRepository _repository;

		private readonly IProfileCollectionReadonly _profiles;

		protected RevisionStorageRepository(IStorageRepository repository, IProfileCollectionReadonly profiles)
		{
			_repository = repository;
			_profiles = profiles;
		}

		protected IStorageRepository Repository
		{
			get { return _repository; }
		}

		public ImportedRevisionInfo GetRevisionId(int? tpRevisionId)
		{
			if (!tpRevisionId.HasValue)
			{
				return null;
			}

			return GetImportedRevisionIds(new[] {tpRevisionId.Value}).FirstOrDefault();
		}

		public virtual void SaveRevisionIdTpIdRelation(int tpRevisionId, RevisionId revisionId)
		{
			SaveRevisionsRelation(tpRevisionId, revisionId);
		}

		private void SaveRevisionsRelation(int tpRevisionId, RevisionId revisionId)
		{
			_repository.Get<RevisionIdRelation>(tpRevisionId.ToString())
				.ReplaceWith(new RevisionIdRelation {RevisionId = revisionId.Value, TpId = tpRevisionId});
		}

		public IEnumerable<int> GetImportedTpIds(IEnumerable<int> tpRevisionIds)
		{
			return GetImportedRevisionIds(tpRevisionIds)
				.Select(x => x.RevisionId.TpId)
				.ToList();
		}

		public abstract bool SaveRevisionInfo(RevisionInfo revision, out string key);
		public abstract void RemoveRevisionInfo(string revisionKey);

		protected virtual IEnumerable<ImportedRevisionInfo> GetImportedRevisionIds(IEnumerable<int> tpRevisionIds)
		{
			var storageNames = tpRevisionIds
				.Select(x => new StorageName(x.ToString()))
				.ToArray();

			return _profiles
				.Select(
					profile =>
					new
						{
							Revisions = profile.Get<RevisionIdRelation>(storageNames),
							Profile = profile.GetProfile<ISourceControlConnectionSettingsSource>()
						})
				.SelectMany(
					x => x.Revisions.Select(rev => new ImportedRevisionInfo {ConnectionSettings = x.Profile, RevisionId = rev}))
					.ToList();
		}
	}
}