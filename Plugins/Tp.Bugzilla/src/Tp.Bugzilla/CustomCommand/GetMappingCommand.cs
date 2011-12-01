// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Bugzilla.CustomCommand.Dtos;
using Tp.Bugzilla.Mappers;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;
using Tp.Integration.Plugin.Common.PluginCommand.Embedded;

namespace Tp.Bugzilla.CustomCommand
{
	public class GetMappingCommand : IPluginCommand
	{
		private readonly IBugzillaFieldsMapper _mapper;

		public GetMappingCommand(IBugzillaFieldsMapper mapper)
		{
			_mapper = mapper;
		}

		public PluginCommandResponseMessage Execute(string args)
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