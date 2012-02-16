// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.SourceControl.RevisionStorage
{
	public interface IRevisionStorageRepository
	{
		ImportedRevisionInfo GetRevisionId(int? tpRevisionId);

		void SaveRevisionIdTpIdRelation(int tpRevisionId, RevisionId revisionId);

		IEnumerable<int> GetImportedTpIds(IEnumerable<int> tpRevisionIds);

		bool SaveRevisionInfo(RevisionInfo revision, out string key);
		
		void RemoveRevisionInfo(string revisionKey);
	}
}