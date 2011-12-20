// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using NServiceBus;
using Rhino.Mocks;
using StructureMap.Configuration.DSL;
using Tp.Integration.Messages;
using Tp.Integration.Messages.ServiceBus;
using Tp.Integration.Messages.ServiceBus.UnicastBus;

namespace Tp.Integration.Plugin.Common.Tests
{
	public class NServiceBusMockRegistry : Registry
	{
		public NServiceBusMockRegistry()
		{
			For<IBusExtended>().HybridHttpOrThreadLocalScoped().Use(() =>
			                                                	{
																	var bus = MockRepository.GenerateStub<IBusExtended>();
			                                                		Setup(bus);
			                                                		return bus;
			                                                	});
			Forward<IBusExtended, IBus>();
		}

		public static void Setup(IBus bus)
		{
			bus.Stub(x => x.OutgoingHeaders).Return(new Dictionary<string, string>());

			var messageContext = MockRepository.GenerateStub<IMessageContext>();
			messageContext.Stub(x => x.Headers).Return(new Dictionary<string, string>());

			bus.Stub(x => x.CurrentMessageContext).Return(messageContext);
			bus.SetIn(AccountName.Empty);
			bus.SetIn((ProfileName) string.Empty);
		}
	}
}