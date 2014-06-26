// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.PluginLifecycle;

namespace Tp.Search.Messages
{
	[Serializable]
	public class IndexExistingEntitiesDoneLocalMessage : SagaMessage, IPluginLocalMessage
	{
	}
}