// 
// Copyright (c) 2005-2017 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using StructureMap.Configuration.DSL;
using Tp.Integration.Plugin.Common;
using Tp.Plugin.Core.Attachments;
using Tp.PopEmailIntegration.EmailReader;
using Tp.PopEmailIntegration.EmailReader.Client;
using Tp.PopEmailIntegration.Initialization;

namespace Tp.PopEmailIntegration.StructureMap
{
    public class EmailPluginRegistry : Registry
    {
        public EmailPluginRegistry()
        {
            For<IExcludedAssemblyNamesSource>().Singleton().Use<EmailPluginExcludedAssemblies>();
            For<IBufferSize>().Singleton().Use(new BufferSize(1000000));
            For<IMessagePackSize>().Singleton().Use(new MessagePackSize(20));
            For<IEmailClient>().Use<MailBeeEmailClient>();
            For<IDeliveryStatusMessageParser>().Use<DeliveryStatusMessageParser>();
            For<IWantToRunBeforeBusStart>().HybridHttpOrThreadLocalScoped().Use<UserStorageMigrator>();
        }
    }
}
