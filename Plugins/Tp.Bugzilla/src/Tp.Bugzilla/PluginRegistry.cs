// 
// Copyright (c) 2005-2019 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using NServiceBus;
using NServiceBus.Sagas.Impl;
using StructureMap.Configuration.DSL;
using Tp.BugTracking;
using Tp.BugTracking.BugFieldConverters;
using Tp.BugTracking.ImportToTp;
using Tp.BugTracking.Mappers;
using Tp.Bugzilla.BugFieldConverters;
using Tp.Bugzilla.BugzillaQueries;
using Tp.Bugzilla.CustomCommand;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Gateways;
using Tp.Plugin.Core.Attachments;

namespace Tp.Bugzilla
{
    public class PluginRegistry : Registry
    {
        public PluginRegistry()
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            For<ICustomPluginSpecifyMessageHandlerOrdering>().Singleton().Use<BugzillaPluginSpecifyMessageHandlerOrdering>();
            For<IBugzillaService>().Use<BugzillaService>();
            For<IBugChunkSize>().Singleton().Use<BugChunkSize>();
            For<IBufferSize>().Singleton().Use<BufferSize>();
            For<IBugConverter<BugzillaBug>>().Singleton().Use<ConverterComposite>();
            For<IBugzillaInfoStorageRepository>().Use<BugzillaInfoStorageRepository>();
            For<IBugzillaActionFactory>().Use<BugzillaActionFactory>();
            For<IUserMapper>().Use<UserMapper>();
            For<IThirdPartyFieldsMapper>().Use<BugzillaThirdPartyFieldsMapper>();
            For<IBugzillaCustomFieldsMapper>().Use<BugzillaCustomFieldsMapper>();
        }
    }

    public class BugzillaPluginSpecifyMessageHandlerOrdering : ICustomPluginSpecifyMessageHandlerOrdering
    {
        //Here we specify that sagas will always be executed before message hanlders.
        public void SpecifyHandlersOrder(First<PluginGateway> ordering)
        {
            ordering.AndThen<SagaMessageHandler>();
        }
    }
}
