// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using NUnit.Framework;
using Tp.Integration.Common;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Testing.Common;
using Tp.Testing.Common.NUnit;

namespace Tp.Integration.Plugin.TaskCreator.Tests
{
	[TestFixture]
    [Category("PartPlugins1")]
	public class CreateTasksForNewUserStoryHandlerSpecs
	{
		private IProfileReadonly _profileInstance;
		private TransportMock _transport;
		private UserStoryCreatedMessage _userStoryCreatedMessage;

		[SetUp]
		public void Init()
		{
			_transport = TransportMock.CreateWithoutStructureMapClear(typeof (CreateTasksForNewUserStoryHandler).Assembly);

			const int projectId = 123;
			const int userStoryId = 123;
			_profileInstance = _transport.AddProfile("Profile1",
			                                         new TaskCreatorProfile
			                                         	{Project = projectId, TasksList = "Task", CommandName = "{CT}"});

			_userStoryCreatedMessage = new UserStoryCreatedMessage
			                           	{
			                           		Dto =
			                           			new UserStoryDTO
			                           				{
			                           					ProjectID = projectId,
			                           					UserStoryID = userStoryId,
			                           					Name = "{CT}User Story Name"
			                           				}
			                           	};
		}


		[Test]
		public void ShouldCreateTask()
		{
			_transport.HandleMessageFromTp(_userStoryCreatedMessage);

			var taskDto = _transport.TpQueue.GetMessages<CreateTaskForUserStoryWithTeamCommand>().Last();
			taskDto.UserStoryID.Should(Be.EqualTo(_userStoryCreatedMessage.Dto.UserStoryID), "Task assigned to UserStory");
			taskDto.Name.Should(Be.EqualTo(_profileInstance.GetProfile<TaskCreatorProfile>().TasksList), "Task has name from plugin configration");
		}
	}
}
