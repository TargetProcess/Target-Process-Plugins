// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using System.Runtime.Serialization;
using Tp.Integration.Messages.ComponentModel;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Validation;

namespace Tp.Integration.Plugin.TaskCreator
{
	[Profile, DataContract]
	public class TaskCreatorProfile : IValidatable
	{
		[DataMember]
		public int Project { get; set; }

		[DataMember]
		public string CommandName { get; set; }

		[DataMember]
		public string TasksList { get; set; }

		public void Validate(PluginProfileErrorCollection errors)
		{
			ValidateProject(errors);

			ValidateCommand(errors);

			ValidateTasksList(errors);
		}

		private void ValidateTasksList(PluginProfileErrorCollection errors)
		{
			if(TasksList.IsNullOrWhitespace())
			{
				errors.Add(new PluginProfileError { FieldName = "TasksList", Message = "Task list shoud not be empty" });
			}
		}

		private void ValidateCommand(PluginProfileErrorCollection errors)
		{
			if(CommandName.IsNullOrWhitespace())
			{
				errors.Add(new PluginProfileError { FieldName = "CommandName", Message = "Command name shoud not be empty" });
			}
		}

		private void ValidateProject(PluginProfileErrorCollection errors)
		{
			if(Project <= 0)
			{
				errors.Add(new PluginProfileError{FieldName = "Project", Message = "Project should not be empty"});
			}
		}
	}
}