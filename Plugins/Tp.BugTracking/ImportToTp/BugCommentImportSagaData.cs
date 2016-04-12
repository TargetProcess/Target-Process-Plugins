// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using NServiceBus.Saga;

namespace Tp.BugTracking.ImportToTp
{
	public class BugCommentImportSagaData : ISagaEntity
	{
		public Guid Id { get; set; }
		public string Originator { get; set; }
		public string OriginalMessageId { get; set; }

		public int CommentsToCreateCount { get; set; }
		public int CreatedCommentsCount { get; set; }
	}
}