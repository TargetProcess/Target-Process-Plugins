// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using NServiceBus;

namespace Tp.Integration.Messages.PluginLifecycle
{
	/// <summary>
	/// Marker interface to indicate that a class represents plugin local message.
	/// </summary>
	public interface IPluginLocalMessage : IMessage
	{
	}
}