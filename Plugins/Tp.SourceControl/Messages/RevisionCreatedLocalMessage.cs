// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Common;
using Tp.Integration.Messages.PluginLifecycle;

namespace Tp.SourceControl.Messages
{
	public class RevisionCreatedLocalMessage : IPluginLocalMessage
	{
		public RevisionDTO Dto { get; set; }
	}
}