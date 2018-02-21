// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using Tp.Integration.Messages.Ticker;

namespace Tp.Integration.Plugin.Common.Domain
{
    /// <summary>
    /// Provides access to plugin metadata.
    /// Injected into StructureMap container.
    /// </summary>
    public interface IPluginMetadata
    {
        /// <summary>
        /// Information about plugin. It is defined in <see cref="PluginAssemblyAttribute"/>.
        /// </summary>
        PluginData PluginData { get; }

        /// <summary>
        /// Profile type.
        /// </summary>
        Type ProfileType { get; }

        /// <summary>
        /// True, if profile implements <see cref="ISynchronizableProfile"/> interface. Otherwise false.
        /// </summary>
        bool IsSyncronizableProfile { get; }

        /// <summary>
        /// True, if initialization Saga is defined in plugin. Initialization logic will be executed when new profile is added.
        /// </summary>
        bool IsNewProfileInitializable { get; }

        /// <summary>
        /// True, if update initialization Saga is defined in plugin. Initialization logic will be executed when profile is updated.
        /// </summary>
        bool IsUpdatedProfileInitializable { get; }
    }
}
