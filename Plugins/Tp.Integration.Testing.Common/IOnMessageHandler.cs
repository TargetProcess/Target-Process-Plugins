// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Text;
using NServiceBus;
using Tp.Integration.Messages.EntityLifecycle;

namespace Tp.Integration.Testing.Common
{
	public interface IOnMessageHandler<TMessage> where TMessage : IMessage
	{
		void Reply(Func<TMessage, ISagaMessage[]> createMessage);
		void Reply(Func<TMessage, ISagaMessage> createMessage);
	}
}