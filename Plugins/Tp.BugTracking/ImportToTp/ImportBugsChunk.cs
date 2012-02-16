// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Messages.PluginLifecycle;

namespace Tp.BugTracking.ImportToTp
{
	public class ImportBugsChunk : IPluginLocalMessage
	{
		public int[] ThirdPartyBugsIds { get; set; }
	}
}