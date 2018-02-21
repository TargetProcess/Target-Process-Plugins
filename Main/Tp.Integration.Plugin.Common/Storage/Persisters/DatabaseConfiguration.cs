// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Plugin.Common.Properties;

namespace Tp.Integration.Plugin.Common.Storage.Persisters
{
    internal class DatabaseConfiguration : IDatabaseConfiguration
    {
        public string ConnectionString => Settings.Default.pluginDatabaseConnectionString;
        public int CommandTimeoutSeconds  => Settings.Default.SqlCommandTimeoutSeconds; 
    }
}
