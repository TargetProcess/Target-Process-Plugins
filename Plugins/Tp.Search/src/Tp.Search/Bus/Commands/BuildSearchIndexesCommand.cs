﻿using Tp.Integration.Common;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.PluginCommand.Embedded;

namespace Tp.Search.Bus.Commands
{
    class BuildSearchIndexesCommand : IPluginCommand
    {
        private readonly ITpBus _bus;
        private readonly IProfileCollection _profileCollection;
        private readonly IProfile _profile;
        private readonly IPluginContext _pluginContext;
        private readonly IPluginMetadata _pluginMetadata;

        public BuildSearchIndexesCommand(ITpBus bus, IProfileCollection profileCollection, IProfile profile, IPluginContext pluginContext,
            IPluginMetadata pluginMetadata)
        {
            _bus = bus;
            _profileCollection = profileCollection;
            _profile = profile;
            _pluginContext = pluginContext;
            _pluginMetadata = pluginMetadata;
        }

        public PluginCommandResponseMessage Execute(string _, UserDTO user = null)
        {
            var c = new AddOrUpdateProfileCommand(_bus, _profileCollection, _pluginContext, _pluginMetadata);
            return c.Execute(BuildProfileDto().Serialize());
        }

        public string Name => "BuildSearchIndexes";

        private PluginProfileDto BuildProfileDto()
        {
            return _profile.IsNull
                ? new PluginProfileDto
                {
                    Name = SearcherProfile.Name
                }
                : new PluginProfileDto
                {
                    Name = _profile.Name.Value,
                    Settings = _profile.Settings
                };
        }
    }
}
