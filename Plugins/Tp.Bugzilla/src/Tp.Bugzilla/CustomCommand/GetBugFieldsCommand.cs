// 
// Copyright (c) 2005-2016 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Common;
using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;

namespace Tp.Bugzilla.CustomCommand
{
    public class GetBugFieldsCommand : IPluginCommand
    {
        private readonly IBugzillaCustomFieldsMapper _bugzillaCustomFieldsMapper;

        public GetBugFieldsCommand(IBugzillaCustomFieldsMapper bugzillaCustomFieldsMapper)
        {
            _bugzillaCustomFieldsMapper = bugzillaCustomFieldsMapper;
        }

        public PluginCommandResponseMessage Execute(string args, UserDTO user)
        {
            return new PluginCommandResponseMessage
            {
                PluginCommandStatus = PluginCommandStatus.Succeed,
                ResponseData = _bugzillaCustomFieldsMapper.GetMappableProperties.Serialize()
            };
        }

        public string Name => "GetBugFields";
    }
}
