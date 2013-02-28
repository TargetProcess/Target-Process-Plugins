// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Plugin.Common.Domain;

namespace Tp.SourceControl.RevisionStorage
{
	public class ImportedRevisionInfo
	{
		public RevisionIdRelation RevisionId { get; set; }

		public IStorageRepository Profile { get; set; }
	}

	public class RevisionIdRelation
	{
		public int TpId { get; set; }

		public string RevisionId { get; set; }
	}
}