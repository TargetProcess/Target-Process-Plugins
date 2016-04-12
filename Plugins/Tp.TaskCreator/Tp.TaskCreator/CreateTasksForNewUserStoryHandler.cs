// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using System.IO;
using System.Linq;
using NServiceBus;
using Tp.Integration.Common;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.EntityLifecycle.Commands;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Integration.Plugin.TaskCreator
{
	public class CreateTasksForNewUserStoryHandler : IHandleMessages<UserStoryCreatedMessage>,
	                                                 IHandleMessages<UserStoryUpdatedMessage>
	{
		private readonly ITpBus _bus;
		private readonly TaskCreatorProfileWrapper _profile;

		public CreateTasksForNewUserStoryHandler(ITpBus bus, IStorageRepository storage)
		{
			_bus = bus;
			_profile = new TaskCreatorProfileWrapper(storage.GetProfile<TaskCreatorProfile>());
		}

		public void Handle(UserStoryCreatedMessage message)
		{
			ProcessUserStory(message.Dto);
		}

		public void Handle(UserStoryUpdatedMessage message)
		{
			if (!message.ChangedFields.Any(x => x == UserStoryField.Name)) return;
			ProcessUserStory(message.Dto);
		}

		private void ProcessUserStory(UserStoryDTO userStoryDto)
		{
			if (string.IsNullOrEmpty(_profile.CommandName)) return;
			if (!NeedToProcessUserStory(userStoryDto)) return;

			CreateTasks(userStoryDto);

			UpdateUserStoryName(userStoryDto);
		}

		private void CreateTasks(UserStoryDTO userStoryDto)
		{
			if (string.IsNullOrEmpty(_profile.TasksList)) return;
			var reader = new StringReader(_profile.TasksList);

			string line;
			while (null != (line = reader.ReadLine()))
			{
				CreateTask(line, userStoryDto);
			}
		}

		private void UpdateUserStoryName(UserStoryDTO userStoryDto)
		{
			_bus.Send(
				new UpdateUserStoryCommand(new UserStoryDTO
				                           	{
				                           		ID = userStoryDto.ID,
				                           		Name = userStoryDto.Name.TrimStart().Substring(_profile.CommandName.Length)
				                           	}, new Enum[] {UserStoryField.Name}
					));
		}

		private bool NeedToProcessUserStory(UserStoryDTO userStoryDto)
		{
			return _profile.Project == userStoryDto.ProjectID &&
			       (userStoryDto.Name.TrimStart().StartsWith(_profile.CommandName,
			                                                 StringComparison.InvariantCultureIgnoreCase));
		}

		private void CreateTask(string taskName, UserStoryDTO userStoryDto)
		{
			_bus.Send(new CreateTaskForUserStoryWithTeamCommand
			{
				UserStoryID = userStoryDto.UserStoryID,
				Name = taskName,
				OwnerID = userStoryDto.OwnerID
			});
		}
	}
}
