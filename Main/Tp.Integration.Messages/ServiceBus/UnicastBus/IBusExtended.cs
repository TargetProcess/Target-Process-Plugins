// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NServiceBus;
using NServiceBus.Unicast;

namespace Tp.Integration.Messages.ServiceBus.UnicastBus
{
	public interface IBusExtended : IUnicastBus
	{
		void SendLocalUi(params IMessage[] message);
		void CleanupOutgoingHeaders();
	}
}