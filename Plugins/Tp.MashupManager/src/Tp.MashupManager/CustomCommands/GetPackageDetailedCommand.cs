// 
// Copyright (c) 2005-2013 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.MashupManager.CustomCommands.Args;

namespace Tp.MashupManager.CustomCommands
{
    public class GetPackageDetailedCommand : LibraryCommand<PackageCommandArg>
    {
        protected override PluginCommandResponseMessage ExecuteOperation(PackageCommandArg commandArg)
        {
            var packageDetailedDto = Library.GetPackageDetailed(commandArg);
            return new PluginCommandResponseMessage
            {
                PluginCommandStatus = PluginCommandStatus.Succeed,
                ResponseData = packageDetailedDto.Serialize()
            };
        }

        public override string Name
        {
            get { return "GetPackageDetailed"; }
        }
    }
}
