// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using Tp.Integration.Common;
using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.PluginCommand.Embedded;

namespace Tp.Tfs.CustomCommand
{
    public class GetTeamProjectsCommand : IPluginCommand
    {
        private readonly IProfileCollection _profileCollection;

        public GetTeamProjectsCommand(IProfileCollection profileCollection)
        {
            _profileCollection = profileCollection;
        }

        public PluginCommandResponseMessage Execute(string args, UserDTO user)
        {
            return new PluginCommandResponseMessage
            {
                ResponseData = OnExecute(args),
                PluginCommandStatus = PluginCommandStatus.Succeed
            };
        }

        private string OnExecute(string args)
        {
            var profile = args.DeserializeProfile(p => _profileCollection[p]);

            try
            {
                var teamProjects = TfsConnectionHelper.GetAvailableTeamProjects(profile.Settings as TfsPluginProfile);
                var array = teamProjects.Select(x => x.Name).ToArray();
                return array.Serialize();
            }
            catch
            {
                return string.Empty;
            }
        }

        public string Name => "GetTeamProjects";
    }
}
