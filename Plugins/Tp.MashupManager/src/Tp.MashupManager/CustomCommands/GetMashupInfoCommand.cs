// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using StructureMap;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.PluginCommand.Embedded;
using Tp.Integration.Plugin.Common.Validation;

namespace Tp.MashupManager.CustomCommands
{
	public class GetMashupInfoCommand : IPluginCommand
	{
		public PluginCommandResponseMessage Execute(string args)
		{
			var mashup = ObjectFactory.GetInstance<ISingleProfile>().Profile.Get<MashupDto>(args);
			var count = mashup.Count();

			if (count == 1)
			{
				return new PluginCommandResponseMessage
				       	{PluginCommandStatus = PluginCommandStatus.Succeed, ResponseData = mashup.Single().Serialize()};
			}

			if (count == 0)
			{
				return GetErrorResponse(string.Format("Mashup with name \"{0}\" doesn't exist", args));
			}

			return GetErrorResponse(string.Format("There are more than one mashup with name \"{0}\"", args));
		}

		public string Name
		{
			get { return "GetMashupInfo"; }
		}

		private PluginCommandResponseMessage GetErrorResponse(string message)
		{
			return new PluginCommandResponseMessage
			       	{
			       		PluginCommandStatus = PluginCommandStatus.Fail,
			       		ResponseData =
			       			new PluginProfileErrorCollection
			       				{new PluginProfileError {FieldName = MashupDto.NameField, Message = message}}.Serialize()
			       	};
		}
	}
}