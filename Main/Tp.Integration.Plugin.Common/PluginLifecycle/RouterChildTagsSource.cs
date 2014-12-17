// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Linq;
using StructureMap;
using Tp.Integration.Messages.ServiceBus.Transport;
using Tp.Integration.Messages.ServiceBus.Transport.Router;
using Tp.Integration.Messages.ServiceBus.Transport.Router.MsmqRx;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Integration.Plugin.Common.PluginLifecycle
{
	internal class RouterChildTagsSource : IRouterChildTagsSource
	{
		private readonly IAccountCollection _accountCollection;
		private readonly IMsmqTransport _transport;

		public RouterChildTagsSource(IAccountCollection accountCollection, IMsmqTransport transport)
		{
			_accountCollection = accountCollection;
			_transport = transport;
		}

		public IEnumerable<string> GetChildTags()
		{
			return _accountCollection.Where(a => a.Profiles.Any()).Select(x => _transport.GetQueueName(x.Name.Value));
		}

		public bool NeedToHandleMessage(MessageEx message)
		{
			if (_accountCollection.GetOrCreate(message.AccountTag).Profiles.Any())
			{
				return true;
			}
			var conditionalMessageRouter = ObjectFactory.TryGetInstance<ITargetProcessConditionalMessageRouter>();

			return Properties.Settings.Default.AlwaysRouteMessage ||
			       conditionalMessageRouter != null && conditionalMessageRouter.Handle(message);
		}
	}
}