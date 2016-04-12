// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.BugTracking.Commands.Dtos;
using Tp.BugTracking.Mappers;
using Tp.Integration.Common;
using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;

namespace Tp.Bugzilla.CustomCommand
{
	public class GetMappingCommand : IPluginCommand
	{
		private readonly IThirdPartyFieldsMapper _mapper;

		public GetMappingCommand(IThirdPartyFieldsMapper mapper)
		{
			_mapper = mapper;
		}

		public PluginCommandResponseMessage Execute(string args, UserDTO user)
		{
			var source = args.Deserialize<MappingSource>();

			return new PluginCommandResponseMessage
			       	{
			       		PluginCommandStatus = PluginCommandStatus.Succeed,
			       		ResponseData = _mapper.Map(source).Serialize()
			       	};
		}

		public string Name
		{
			get { return "GetMapping"; }
		}
	}
}