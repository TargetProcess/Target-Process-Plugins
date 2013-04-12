// 
// Copyright (c) 2005-2013 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NServiceBus;
using Tp.Integration.Messages.AccountLifecycle;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Search.Model.Document;

namespace Tp.Search.Bus
{
	class AccountRemovedMessageHandler : IHandleMessages<AccountRemovedMessage>
	{
		private readonly IDocumentIndexProvider _documentIndexProvider;
		private readonly IActivityLogger _log;

		public AccountRemovedMessageHandler()
		{
		}

		public AccountRemovedMessageHandler(IDocumentIndexProvider documentIndexProvider, IActivityLogger log)
		{
			_documentIndexProvider = documentIndexProvider;
			_log = log;
		}

		public void Handle(AccountRemovedMessage message)
		{
			_documentIndexProvider.ShutdownDocumentIndexes(message.AccountName, new DocumentIndexShutdownSetup(forceShutdown:true, cleanStorage:true));
			_log.Info(string.Format("Account {0} removed with all indexes", message.AccountName));
		}
	}
}