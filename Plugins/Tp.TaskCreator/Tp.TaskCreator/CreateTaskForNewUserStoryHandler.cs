// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using System.Text.RegularExpressions;
using NServiceBus;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.EntityLifecycle.Commands;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Storage;
using TaskDTO = Tp.Integration.Common.TaskDTO;
using UserStoryDTO = Tp.Integration.Common.UserStoryDTO;

namespace Tp.Integration.Plugin.TaskCreator
{
	public class CreateTaskForNewUserStoryHandler : IHandleMessages<UserStoryCreatedMessage>
	{
		private readonly ICommandBus _bus;
		private readonly IStorageRepository _storage;
		private TaskCreatorProfile _profile;

		public CreateTaskForNewUserStoryHandler(ICommandBus bus, IStorageRepository storage)
		{
			_bus = bus;
			_storage = storage;
			_profile = _storage.GetProfile<TaskCreatorProfile>();
		}

		public void Handle(UserStoryCreatedMessage message)
		{
			if (NeedToProcessUserStory(message))
			{
				var taskNames = _profile.TaskNames.
			}
		}

		private bool NeedToProcessUserStory(EntityCreatedMessage<UserStoryDTO> message)
		{
			if (_profile.UserStoryNameFilter == null) return false;
			
			var regExp = new Regex(_profile.UserStoryNameFilter, RegexOptions.Compiled | RegexOptions.IgnoreCase);

			return _profile.ProjectId == message.Dto.ProjectID && regExp.IsMatch(message.Dto.Name);
		}

		private CreateTaskCommand CreateTask(string taskName, UserStoryDTO userStoryDto)
		{
			_storage.Get<UserStoryDTO>().Add(userStoryDto);
			return new CreateTaskCommand(new TaskDTO
			                             	{
			                             		UserStoryID = userStoryDto.UserStoryID,
			                             		Name = taskName
			                             	});
		}
	}
}