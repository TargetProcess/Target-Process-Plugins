// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using NServiceBus;
using NServiceBus.Saga;
using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Queries;
using Tp.Integration.Plugin.Common;
using Tp.PopEmailIntegration.Data;

namespace Tp.PopEmailIntegration.Initialization
{
	public class NewEmailProfileInitializationSaga : NewProfileInitializationSaga<NewEmailProfileInitializationSagaData>,
	                                                 IHandleMessages<UserQueryResult>,
	                                                 IHandleMessages<RequesterQueryResult>,
	                                                 IHandleMessages<ProjectQueryResult>,
	                                                 IHandleMessages<GlobalSettingQueryResult>,
	                                                 IHandleMessages<MessageUidsLoadedMessageInternal>
	{
		public override void ConfigureHowToFindSaga()
		{
			ConfigureMapping<UserQueryResult>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<RequesterQueryResult>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<ProjectQueryResult>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<GlobalSettingQueryResult>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<MessageUidsLoadedMessageInternal>(saga => saga.Id, message => message.SagaId);
		}

		protected override void OnStartInitialization()
		{
			Data.AllRequestersCount = int.MinValue;
			Data.AllUsersCount = int.MinValue;
			Data.AllProjectsCount = int.MinValue;
			Send(new RetrieveAllUsersQuery());
			Send(new RetrieveAllRequestersQuery());
			Send(new RetrieveAllProjectsQuery());
			Send(new RetrieveGlobalSettingQuery());

			SendLocal(new LoadMessageUidsCommandInternal {OuterSagaId = Data.Id});
		}

		public void Handle(UserQueryResult message)
		{
			Data.AllUsersCount = message.QueryResultCount;
			if (message.Dtos != null)
			{
				Data.UsersRetrievedCount += message.Dtos.Length;
				foreach (var userDto in message.Dtos)
				{
					AddUserLite(UserLite.Create(userDto));
				}
			}
			CompleteSagaIfNecessary();
		}

		private void CompleteSagaIfNecessary()
		{
			if (Data.UsersRetrievedCount == Data.AllUsersCount && Data.RequestersRetrievedCount == Data.AllRequestersCount &&
			    Data.ProjectsRetrievedCount == Data.AllProjectsCount && Data.MessageUidsLoaded &&
			    Data.TargetProcessGlobalSettingLoaded)
				MarkAsComplete();
		}

		public void Handle(RequesterQueryResult message)
		{
			Data.AllRequestersCount = message.QueryResultCount;
			if (message.Dtos != null)
			{
				foreach (var userDto in message.Dtos)
				{
					AddUserLite(UserLite.Create(userDto));
				}

				Data.RequestersRetrievedCount += message.Dtos.Length;
			}
			CompleteSagaIfNecessary();
		}

		public void Handle(ProjectQueryResult message)
		{
			Data.AllProjectsCount = message.QueryResultCount;
			if (message.Dtos != null)
			{
				foreach (var projectDto in message.Dtos)
				{
					StorageRepository().Get<ProjectDTO>().Add(projectDto);
				}
				Data.ProjectsRetrievedCount += message.Dtos.Length;
			}
			CompleteSagaIfNecessary();
		}

		private static void AddUserLite(UserLite userLite)
		{
			ObjectFactory.GetInstance<UserRepository>().Add(userLite);
		}

		public void Handle(MessageUidsLoadedMessageInternal message)
		{
			Data.MessageUidsLoaded = true;
			CompleteSagaIfNecessary();
		}

		public void Handle(GlobalSettingQueryResult message)
		{
			StorageRepository().Get<GlobalSettingDTO>().Add(message.Dtos.First());
			Data.TargetProcessGlobalSettingLoaded = true;
			CompleteSagaIfNecessary();
		}
	}

	public class NewEmailProfileInitializationSagaData : ISagaEntity
	{
		public Guid Id { get; set; }
		public string Originator { get; set; }
		public string OriginalMessageId { get; set; }

		public int UsersRetrievedCount { get; set; }
		public int RequestersRetrievedCount { get; set; }
		public int ProjectsRetrievedCount { get; set; }

		public int AllUsersCount { get; set; }
		public int AllRequestersCount { get; set; }
		public int AllProjectsCount { get; set; }

		public bool MessageUidsLoaded { get; set; }

		public bool TargetProcessGlobalSettingLoaded { get; set; }
	}
}
