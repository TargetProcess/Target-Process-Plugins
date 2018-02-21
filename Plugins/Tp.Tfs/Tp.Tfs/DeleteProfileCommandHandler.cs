// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.IO;
using NServiceBus;
using StructureMap;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Tfs
{
    public class DeleteProfileCommandHandler : IHandleMessages<ExecutePluginCommandCommand>
    {
        private readonly IStorageRepository _storage;

        public DeleteProfileCommandHandler(IStorageRepository storage)
        {
            _storage = storage;
        }

        public void Handle(ExecutePluginCommandCommand message)
        {
            if (message.CommandName == EmbeddedPluginCommands.DeleteProfile)
            {
                // When profile is deleted ProfileFileStorage.Clear() is called. 
                // The profile repository folder is created during this call. This folder is redundant for TFS plugin.
                // We need to delete it here

                string folderPath = ObjectFactory.GetInstance<PluginDataFolder>().Path;

                if (Directory.Exists(folderPath))
                    Directory.Delete(folderPath, true);
            }
        }
    }
}
