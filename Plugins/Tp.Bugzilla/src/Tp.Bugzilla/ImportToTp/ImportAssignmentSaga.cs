// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using NServiceBus;
using NServiceBus.Saga;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Commands;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Mapping;

namespace Tp.Bugzilla.ImportToTp
{
	public class ImportAssignmentSaga : TpSaga<ImportAssignmentSagaData>,
	                                    IAmStartedByMessages<NewBugImportedToTargetProcessMessage>,
	                                    IAmStartedByMessages<ExistingBugImportedToTargetProcessMessage>,
	                                    IHandleMessages<TeamCreatedMessage>,
	                                    IHandleMessages<TeamDeletedMessage>
	{
		private readonly IStorageRepository _storageRepository;
		private readonly IUserMapper _userMapper;
		private readonly IActivityLogger _logger;

		public ImportAssignmentSaga()
		{
		}

		public ImportAssignmentSaga(IStorageRepository storageRepository, IUserMapper userMapper, IActivityLogger logger)
		{
			_storageRepository = storageRepository;
			_userMapper = userMapper;
			_logger = logger;
		}

		public override void ConfigureHowToFindSaga()
		{
			ConfigureMapping<TeamCreatedMessage>(saga => saga.Id, message => message.SagaId);
			ConfigureMapping<TeamDeletedMessage>(saga => saga.Id, message => message.SagaId);
		}

		public void Handle(NewBugImportedToTargetProcessMessage message)
		{
			AssignDeveloperToBug(message.TpBugId, message.BugzillaBug);
			AssignQAToBug(message.TpBugId, message.BugzillaBug);

			CompleteIfNecessary();
		}

		public void Handle(ExistingBugImportedToTargetProcessMessage message)
		{
			var role = _storageRepository.GetProfile<BugzillaProfile>().GetAssigneeRole();

			UnassignUsersInTargetProcess(role.Name, message.TpBugId);
			AssignDeveloperToBug(message.TpBugId, message.BugzillaBug);

			CompleteIfNecessary();
		}

		public void Handle(TeamCreatedMessage message)
		{
			_storageRepository.Get<TeamDTO>(message.Dto.ID.ToString()).Add(message.Dto);

			DoNotContinueDispatchingCurrentMessageToHandlers();

			Data.ActionsInProgress--;
			CompleteIfNecessary();
		}

		public void Handle(TeamDeletedMessage message)
		{
			_storageRepository.Get<TeamDTO>(message.Dto.ID.ToString()).Clear();

			DoNotContinueDispatchingCurrentMessageToHandlers();

			Data.ActionsInProgress--;
			CompleteIfNecessary();
		}

		private void CompleteIfNecessary()
		{
			if (Data.ActionsInProgress == 0)
				MarkAsComplete();
		}

		private void AssignQAToBug(int? tpBugId, BugzillaBug bugzillaBug)
		{
			var role = _storageRepository.GetProfile<BugzillaProfile>().GetReporterRole();
			var tpUserId = _userMapper.GetTpIdBy(bugzillaBug.reporter);

			AssignUserToBug(tpBugId, tpUserId, role);
		}

		private void AssignDeveloperToBug(int? tpBugId, BugzillaBug bugzillaBug)
		{
			var role = _storageRepository.GetProfile<BugzillaProfile>().GetAssigneeRole();
			var tpUserId = _userMapper.GetTpIdBy(bugzillaBug.assigned_to);

			AssignUserToBug(tpBugId, tpUserId, role);
		}

		private void AssignUserToBug(int? tpBugId, int? tpUserId, MappingLookup role)
		{
			if (role != null && tpUserId.HasValue)
			{
				_logger.InfoFormat("Assigning user. TargetProcess Bug ID: {0}; User ID: {1}; Role: {2}", tpBugId, tpUserId, role.Name);
				Data.ActionsInProgress++;
				Send(new CreateTeamCommand(new TeamDTO
				                           	{
				                           		AssignableID = tpBugId.GetValueOrDefault(),
				                           		UserID = tpUserId,
				                           		RoleID = role.Id
				                           	}));
			}
		}

		private void UnassignUsersInTargetProcess(string roleName, int? bugId)
		{
			var role = GetDefaultRole(roleName);
			if (role == null)
				return;

			var bugTeams = _storageRepository.Get<TeamDTO>().Where(t => t.AssignableID == bugId);
			var tpTeam = bugTeams.Where(t => t.RoleID == role.ID);

			tpTeam.ForEach(x =>
			               	{
								_logger.InfoFormat("Unassigning user. TargetProcess Bug ID: {0}; Role: {1}", bugId, role.Name);
			               		Data.ActionsInProgress++;
			               		Send(new DeleteTeamCommand(x.ID.GetValueOrDefault()));
			               	});
		}

		private RoleDTO GetDefaultRole(string roleName)
		{
			return _storageRepository.Get<RoleDTO>().SingleOrDefault(r => r.Name == roleName);
		}
	}

	public class ExistingBugImportedToTargetProcessMessage : IPluginLocalMessage
	{
		public int? TpBugId { get; set; }
		public BugzillaBug BugzillaBug { get; set; }
	}

	public class NewBugImportedToTargetProcessMessage : IPluginLocalMessage
	{
		public int? TpBugId { get; set; }
		public BugzillaBug BugzillaBug { get; set; }
	}

	public class ImportAssignmentSagaData : ISagaEntity
	{
		public Guid Id { get; set; }
		public string Originator { get; set; }
		public string OriginalMessageId { get; set; }
		public int ActionsInProgress { get; set; }
	}
}