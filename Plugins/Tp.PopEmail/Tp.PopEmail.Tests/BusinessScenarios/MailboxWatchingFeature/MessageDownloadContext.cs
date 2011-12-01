// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Rhino.Mocks;
using StructureMap;
using Tp.PopEmailIntegration.Data;
using Tp.PopEmailIntegration.EmailReader;
using Tp.PopEmailIntegration.EmailReader.Client;

namespace Tp.PopEmailIntegration.BusinessScenarios.MailboxWatchingFeature
{
	public class MessageDownloadContext : PopEmailIntegrationContext
	{
		public MessageDownloadContext()
		{
			ObjectFactory.Configure(x =>
			                        	{
			                        		x.For<EmailClientStub>().HybridHttpOrThreadLocalScoped().Use<EmailClientStub>();
			                        		x.Forward<EmailClientStub, IEmailClient>();
			                        		var messagePackSize = MockRepository.GenerateStub<IMessagePackSize>();
			                        		messagePackSize.Stub(y => y.Value).Return(1);
			                        		x.For<IMessagePackSize>().Use(messagePackSize);
			                        	});

			ObjectFactory.Configure(x => x.For<MessageDownloadContext>().Use(this));
		}

		public MessageUidRepository MessageUids
		{
			get { return ObjectFactory.GetInstance<MessageUidRepository>(); }
		}

		public EmailClientStub EmailClient
		{
			get { return ObjectFactory.GetInstance<EmailClientStub>(); }
		}
	}
}