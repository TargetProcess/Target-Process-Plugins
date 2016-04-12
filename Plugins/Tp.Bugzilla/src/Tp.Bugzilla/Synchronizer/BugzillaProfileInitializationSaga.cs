// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using NServiceBus;
using StructureMap;
using Tp.BugTracking;
using Tp.BugTracking.Synchronizer;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Queries;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;

namespace Tp.Bugzilla.Synchronizer
{
	public class BugzillaProfileInitializationSaga : NewProfileInitializationSaga<BugTrackingProfileInitializationSagaData>,
	                                                 IHandleMessages<EntityStateQueryResult>,
	                                                 IHandleMessages<SeverityQueryResult>,
	                                                 IHandleMessages<PriorityQueryResult>,
	                                                 IHandleMessages<ProjectQueryResult>,
	                                                 IHandleMessages<UserQueryResult>,
	                                                 IHandleMessages<RoleQueryResult>
	{
		public const string BugEntityTypeName = "Tp.BusinessObjects.Bug";

		public override void ConfigureHowToFindSaga()
		{
			ConfigureMapping<EntityStateQueryResult>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<SeverityQueryResult>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<PriorityQueryResult>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<ProjectQueryResult>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<UserQueryResult>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<RoleQueryResult>(saga => saga.Id, message => message.SagaId);
		}

		protected override void OnStartInitialization()
		{
			ObjectFactory.GetInstance<IActivityLogger>().Info("Initializing profile");

			Data.AllEntityStatesCount = int.MinValue;
			Data.AllSeveritiesCount = int.MinValue;
			Data.AllPrioritiesCount = int.MinValue;
			Data.AllProjectsCount = int.MinValue;
			Data.AllUsersCount = int.MinValue;
			Data.AllRolesCount = int.MinValue;
			Send(new EntityStateQuery {ProjectId = StorageRepository().GetProfile<BugzillaProfile>().Project});
			Send(new PriorityQuery {EntityType = "Bug"});
			Send(new RetrieveAllSeveritiesQuery());
			Send(new RetrieveAllProjectsQuery());
			Send(new RetrieveAllUsersQuery());
			Send(new RetrieveAllRolesQuery());
		}

		public void Handle(EntityStateQueryResult message)
		{
			Data.AllEntityStatesCount = message.QueryResultCount;
			if (message.Dtos != null)
			{
				Data.EntityStatesRetrievedCount += message.Dtos.Length;
				foreach (var entityStateDto in message.Dtos.Where(entityStateDto => entityStateDto.EntityTypeName == BugEntityTypeName))
				{
					StorageRepository().Get<EntityStateDTO>().Add(entityStateDto);
				}
			}
			CompleteSagaIfNecessary();
		}

		private void CompleteSagaIfNecessary()
		{
			if (Data.EntityStatesRetrievedCount == Data.AllEntityStatesCount
			    && Data.AllSeveritiesCount == Data.SeveritiesRetrievedCount
			    && Data.AllPrioritiesCount == Data.PrioritiesRetrievedCount
			    && Data.AllProjectsCount == Data.ProjectsRetrievedCount
			    && Data.AllUsersCount == Data.UsersRetrievedCount
			    && Data.AllRolesCount == Data.RolesRetrievedCount)
				MarkAsComplete();
		}

		public void Handle(SeverityQueryResult message)
		{
			Data.AllSeveritiesCount = message.QueryResultCount;
			if (message.Dtos != null)
			{
				Data.SeveritiesRetrievedCount+=message.Dtos.Length;
				foreach (var severityDto in message.Dtos)
				{
					StorageRepository().Get<SeverityDTO>().Add(severityDto);
				}
				
			}
			CompleteSagaIfNecessary();
		}

		public void Handle(PriorityQueryResult message)
		{
			Data.AllPrioritiesCount = message.QueryResultCount;
			if (message.Dtos != null)
			{
				Data.PrioritiesRetrievedCount += message.Dtos.Length;
				foreach (var priorityDto in message.Dtos)
				{
					StorageRepository().Get<PriorityDTO>().Add(priorityDto);
				}
			}
			CompleteSagaIfNecessary();
		}

		public void Handle(ProjectQueryResult message)
		{
			Data.AllProjectsCount = message.QueryResultCount;
			if (message.Dtos != null)
			{
				var project = StorageRepository().GetProfile<BugzillaProfile>().Project;
				Data.ProjectsRetrievedCount += message.Dtos.Length;
				foreach (var projectDto in message.Dtos.Where(projectDto => projectDto.ID == project))
				{
					StorageRepository().Get<ProjectDTO>().Add(projectDto);
				}
			}
			CompleteSagaIfNecessary();
		}

		public void Handle(UserQueryResult message)
		{
			Data.AllUsersCount = message.QueryResultCount;
			if (message.Dtos != null)
			{
				Data.UsersRetrievedCount += message.Dtos.Length;
				foreach (var userDto in message.Dtos.Where(u => u.IsNotDeletedUser()))
				{
					StorageRepository().Get<UserDTO>(userDto.ID.ToString()).Add(userDto);
					StorageRepository().Get<UserDTO>(userDto.Email).Add(userDto);
				}
			}
			CompleteSagaIfNecessary();
		}

		public void Handle(RoleQueryResult message)
		{
			Data.AllRolesCount = message.QueryResultCount;
			if (message.Dtos != null)
			{
				Data.RolesRetrievedCount += message.Dtos.Length;
				foreach (var roleDto in message.Dtos)
				{
					StorageRepository().Get<RoleDTO>().Add(roleDto);
				}
			}
			CompleteSagaIfNecessary();
		}
	}
}