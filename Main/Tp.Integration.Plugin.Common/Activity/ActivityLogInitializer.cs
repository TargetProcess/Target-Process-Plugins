// 
// Copyright (c) 2005-2016 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NServiceBus.Unicast;
using StructureMap;
using Tp.Core;
using log4net;
using log4net.Core;
using log4net.Repository.Hierarchy;

namespace Tp.Integration.Plugin.Common.Activity
{
    class ActivityLogInitializer
    {
        public void Init()
        {
            if (LogManager.GetAllRepositories().Length > 0)
            {
                var repository = ((Hierarchy) LoggerManager.GetRepository("log4net-default-repository"));
                repository.LoggerFactory = new ActivityLoggerFactory(repository.LoggerFactory, ObjectFactory.GetInstance<Locker>());
            }
            var c = ObjectFactory.Container;
            var logger = c.GetInstance<IActivityLogger>();
            c.GetInstance<IUnicastBus>().UnhandledExceptionCaught += (sender, e) => { logger.Error(e.Exception); };
        }
    }
}
