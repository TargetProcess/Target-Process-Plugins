// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using NServiceBus;
using Tp.BugTracking;
using Tp.Bugzilla.BugzillaQueries;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Bugzilla.ImportToBugzilla
{
    public class TeamChangedHandler
        : IHandleMessages<TeamUpdatedMessage>,
          IHandleMessages<TeamCreatedMessage>,
          IHandleMessages<TeamDeletedMessage>
    {
        private readonly IStorageRepository _storage;
        private readonly IBugzillaInfoStorageRepository _bugzillaInfoStorageRepository;
        private readonly IBugzillaService _service;
        private readonly IBugzillaActionFactory _actionFactory;
        private readonly IUserMapper _userMapper;
        private readonly IActivityLogger _logger;

        public TeamChangedHandler(IStorageRepository storage, IBugzillaInfoStorageRepository bugzillaInfoStorageRepository,
            IBugzillaService service, IBugzillaActionFactory actionFactory, IUserMapper userMapper, IActivityLogger logger)
        {
            _storage = storage;
            _bugzillaInfoStorageRepository = bugzillaInfoStorageRepository;
            _service = service;
            _actionFactory = actionFactory;
            _userMapper = userMapper;
            _logger = logger;
        }

        public void Handle(TeamUpdatedMessage message)
        {
            if (!ShouldUpdateTeam(message.Dto)) return;
            Action success = () => UpdateTeam(message.Dto);

            AssignUser(message.Dto,
                _userMapper.GetThirdPartyIdBy(_storage.Get<UserDTO>(message.Dto.UserID.ToString()).Single().ID),
                success);
        }

        private void UpdateTeam(TeamDTO dto)
        {
            RemoveTeamFromStorageIfExist(dto);
            _storage.Get<TeamDTO>(dto.ID.ToString()).Add(dto);
        }

        private void RemoveTeamFromStorageIfExist(TeamDTO dto)
        {
            if (_storage.Get<TeamDTO>(dto.ID.ToString()).Any())
            {
                _storage.Get<TeamDTO>(dto.ID.ToString()).Clear();
            }
        }

        public void Handle(TeamCreatedMessage message)
        {
            Action success = () => UpdateTeam(message.Dto);

            AssignUser(message.Dto,
                _userMapper.GetThirdPartyIdBy(_storage.Get<UserDTO>(message.Dto.UserID.ToString()).Single().ID),
                success);
        }

        public void Handle(TeamDeletedMessage message)
        {
            Action success = () => RemoveTeamFromStorageIfExist(message.Dto);

            if (AlreadyReassign(message.Dto))
            {
                success();
                return;
            }

            AssignUser(message.Dto, null, success);
        }

        private bool NeedToProcess(TeamDTO dto)
        {
            return _bugzillaInfoStorageRepository.GetBugzillaBug(dto.AssignableID) != null;
        }

        private void AssignUser(TeamDTO team, string userEmail, Action sucess)
        {
            if (!NeedToProcess(team) || AnotherRole(team)) return;

            var bugzillaBug = _bugzillaInfoStorageRepository.GetBugzillaBug(team.AssignableID);
            _logger.InfoFormat("Changing bug assignment in Bugzilla. TargetProcess Bug ID: {0}; Email: {1}", bugzillaBug.TpId, userEmail);

            try
            {
                _service.Execute(_actionFactory.GetAssigneeAction(bugzillaBug.Id, userEmail));
                sucess();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            _logger.InfoFormat("Bug assignment changed in Bugzilla. TargetProcess Bug ID: {0}; Email: {1}", bugzillaBug.TpId, userEmail);
        }

        private bool AnotherRole(TeamDTO team)
        {
            return team.RoleName != GetAssigneeRole();
        }

        private string GetAssigneeRole()
        {
            return _storage.GetProfile<BugzillaProfile>().GetAssigneeRole().Name;
        }

        private bool ShouldUpdateTeam(TeamDTO team)
        {
            var teamDto = _storage.Get<TeamDTO>(team.ID.ToString()).FirstOrDefault();
            return teamDto == null || teamDto.UserID != team.UserID;
        }

        private bool AlreadyReassign(TeamDTO team)
        {
            var teams = _storage.Get<TeamDTO>()
                .Where(t => t.AssignableID == team.AssignableID)
                .Where(t => t.RoleName == GetAssigneeRole())
                .ToList();

            if (teams.Any())
            {
                var oldTeamId = teams.Max(t => t.TeamID);
                return oldTeamId > team.ID;
            }

            return false;
        }
    }
}
