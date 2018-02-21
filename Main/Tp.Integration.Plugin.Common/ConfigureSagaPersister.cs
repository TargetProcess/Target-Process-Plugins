// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NServiceBus;
using NServiceBus.ObjectBuilder;
using Tp.Integration.Plugin.Common.SagaPersister;

namespace Tp.Integration.Plugin.Common
{
    public static class ConfigureSagaPersister
    {
        public static Configure TpDatabaseSagaPersister(this Configure config)
        {
            config.Configurer.ConfigureComponent(typeof(TpDatabaseSagaPersister), ComponentCallModelEnum.Singleton);
            return config;
        }
    }
}
