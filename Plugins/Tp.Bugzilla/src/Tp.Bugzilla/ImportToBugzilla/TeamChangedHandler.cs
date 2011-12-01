// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using NServiceBus;
using Tp.Bugzilla.BugzillaQueries;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Plugin.Common.Storage;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Bugzilla.ImportToBugzilla
{
	public class TeamChangedHandler : IHandleMessages<TeamUpdatedMessage>,
	                                  IHandleMessages<TeamCreatedMessage>,
	                                  IHandleMessages<TeamDeletedMessage>
	{
		private readonly IStorageRepository _storage;
		private readonly IBugzillaInfoStorageRepository _bugzillaInfoStorageRepository;
		private readonly IBugzillaService _service;
		private readonly IBugzillaActionFactory _actionFactory;
		private readonly IUserMapper _userMapper;

		public TeamChangedHandler(IStorageRepository storage, IBugzillaInfoStorageRepository bugzillaInfoStorageRepository,
		                          IBugzillaService service, IBugzillaActionFactory actionFactory, IUserMapper userMapper)
		{
			_storage = storage;
			_bugzillaInfoStorageRepository = bugzillaInfoStorageRepository;
			_service = service;
			_actionFactory = actionFactory;
			_userMapper = userMapper;
		}

		public void Handle(TeamUpdatedMessage message)
		{
			if (!NeedToProcess(message.Dto))
			{
				return;
			}

			_storage.Get<TeamDTO>(message.Dto.ID.ToString()).Clear();
			_storage.Get<TeamDTO>(message.Dto.ID.ToString()).Add(message.Dto);
		}

		public void Handle(TeamCreatedMessage message)
		{
			if (!NeedToProcess(message.Dto))
			{
				return;
			}

			_storage.Get<TeamDTO>(message.Dto.ID.ToString()).Add(message.Dto);

			AssignUser(message.Dto,
			           _userMapper.GetBugzillaEmailBy(_storage.Get<UserDTO>(message.Dto.UserID.ToString()).Single().ID));
		}

		public void Handle(TeamDeletedMessage message)
		{
			if(!NeedToProcess(message.Dto))
			{
				return;
			}

			_storage.Get<TeamDTO>(message.Dto.ID.ToString()).Clear();
			AssignUser(message.Dto, null);
		}

		private bool NeedToProcess(TeamDTO dto)
		{
			return _bugzillaInfoStorageRepository.GetBugzillaBug(dto.AssignableID) != null;
		}

		private void AssignUser(TeamDTO team, string userEmail)
		{
			if (!NeedToProcess(team) || team.RoleName != _storage.GetProfile<BugzillaProfile>().GetAssigneeRole().Name) return;

			var bugzillaBug = _bugzillaInfoStorageRepository.GetBugzillaBug(team.AssignableID);

			_service.Execute(_actionFactory.GetAssigneeAction(bugzillaBug.Id, userEmail));
		}
	}
}