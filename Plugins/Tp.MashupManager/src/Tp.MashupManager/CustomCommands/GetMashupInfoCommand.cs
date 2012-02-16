// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Runtime.Serialization;
using StructureMap;
using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;
using Tp.Integration.Plugin.Common.Validation;
using Tp.MashupManager.Dtos;
using Tp.MashupManager.MashupStorage;

namespace Tp.MashupManager.CustomCommands
{
	public class GetMashupInfoCommand : IPluginCommand
	{
		public PluginCommandResponseMessage Execute(string args)
		{
			var name = args.Deserialize<MashupName>();

			var mashup = ObjectFactory.GetInstance<IMashupScriptStorage>().GetMashup(name.Value);

			if (mashup != null)
			{
				return new PluginCommandResponseMessage
				       	{PluginCommandStatus = PluginCommandStatus.Succeed, ResponseData = mashup.Serialize()};
			}

			return GetErrorResponse(string.Format("Mashup with name \"{0}\" doesn't exist", name.Value));
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

	[DataContract]
	public class MashupName
	{
		[DataMember]
		public string Value { get; set; }
	}
}