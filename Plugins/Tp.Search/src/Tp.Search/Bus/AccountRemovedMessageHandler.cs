// 
// Copyright (c) 2005-2013 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NServiceBus;
using Tp.Integration.Messages;
using Tp.Integration.Messages.AccountLifecycle;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Search.Model.Document;

namespace Tp.Search.Bus
{
    class AccountRemovedMessageHandler : IHandleMessages<AccountRemovedMessage>
    {
        private readonly IDocumentIndexProvider _documentIndexProvider;
        private readonly IActivityLogger _log;
        private readonly IPluginContext _context;

        public AccountRemovedMessageHandler()
        {
        }

        public AccountRemovedMessageHandler(IDocumentIndexProvider documentIndexProvider, IActivityLogger log, IPluginContext context)
        {
            _documentIndexProvider = documentIndexProvider;
            _log = log;
            _context = context;
        }

        public void Handle(AccountRemovedMessage message)
        {
            _documentIndexProvider.ShutdownDocumentIndexes(
                new PluginContextSnapshot(message.AccountName, new ProfileName(string.Empty), _context.PluginName),
                new DocumentIndexShutdownSetup(forceShutdown: true, cleanStorage: true), _log);
            _log.Info(string.Format("Account {0} removed with all indexes", message.AccountName));
        }
    }
}
