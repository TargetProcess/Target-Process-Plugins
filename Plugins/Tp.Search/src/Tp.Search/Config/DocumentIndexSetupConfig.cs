// 
// Copyright (c) 2005-2015 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Plugin.Common;
using Tp.Search.Model.Document;
using Tp.Search.Model.Optimization;

namespace Tp.Search.Config
{
    internal class DocumentIndexSetupConfig
    {
        private const string IndexAliveTimeoutInMinutesName = "IndexAliveTimeoutInMinutes";
        private const string DeferredOptimizeCallsOnChangeName = "DeferredOptimizeCallsOnChange";
        private const string DeferredOptimizeTypeName = "DeferredOptimizeType";
        private const string SearchCheckIntervalInMinutesName = "SearchCheckIntervalInMinutes";
        private const string ManagedMemoryThresholdInMbName = "ManagedMemoryThresholdInMb";

        public DocumentIndexSetup Load()
        {
            int aliveTimeoutInMinutes = PluginSettings.LoadInt(IndexAliveTimeoutInMinutesName, 10);
            int deferredOptimizeCallsOnChange = PluginSettings.LoadInt(DeferredOptimizeCallsOnChangeName, 1);
            DeferredOptimizeType optimizeType = PluginSettings.LoadEnum(DeferredOptimizeTypeName, DeferredOptimizeType.None);
            int checkIntervalInMinutes = PluginSettings.LoadInt(SearchCheckIntervalInMinutesName, aliveTimeoutInMinutes);
            int? managedMemoryThresholdInMb = PluginSettings.LoadInt(ManagedMemoryThresholdInMbName);
            var folder = new PluginDataFolder();
            return new DocumentIndexSetup(indexPath: folder.Path, minStringLengthToSearch: 2, maxStringLengthIgnore: 60,
                aliveTimeoutInMinutes: aliveTimeoutInMinutes, deferredOptimizeCounter: deferredOptimizeCallsOnChange,
                deferredOptimizeType: optimizeType, checkIntervalInMinutes: checkIntervalInMinutes,
                managedMemoryThresholdInMb: managedMemoryThresholdInMb);
        }
    }
}
