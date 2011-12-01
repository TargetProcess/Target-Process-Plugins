// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.SourceControl.Messages
{
	public class NewRevisionDetectedLocalMessage : SagaMessage, IPluginLocalMessage
	{
		public RevisionInfo Revision { get; set; }
	}
}