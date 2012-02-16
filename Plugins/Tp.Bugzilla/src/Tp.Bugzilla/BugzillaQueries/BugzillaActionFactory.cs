// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using Tp.BugTracking;
using Tp.Integration.Common;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Bugzilla.BugzillaQueries
{
	public class BugzillaActionFactory : IBugzillaActionFactory
	{
		private readonly IBugzillaInfoStorageRepository _bugzillaStorage;
		private readonly IStorageRepository _repository;
		private readonly IUserMapper _userMapper;

		public BugzillaActionFactory(IBugzillaInfoStorageRepository bugzillaStorage, IStorageRepository repository,
		                             IUserMapper userMapper)
		{
			_bugzillaStorage = bugzillaStorage;
			_repository = repository;
			_userMapper = userMapper;
		}

		public IBugzillaAction GetAssigneeAction(string bugzillaBugId, string userEmail)
		{
			return new BugzillaAssigneeAction(bugzillaBugId, userEmail);
		}

		public IBugzillaAction GetCommentAction(CommentDTO comment, TimeSpan offset)
		{
			var bugzillaBug = _bugzillaStorage.GetBugzillaBug(comment.GeneralID);
			var owner = GetOwner(comment);

			return new BugzillaCommentAction(
				bugzillaBug.Id,
				comment.Description,
				owner,
				comment.CreateDate.GetValueOrDefault().ToUniversalTime().Add(offset));
		}

		public IBugzillaQuery GetChangeStatusAction(BugDTO tpBug, string bugzillaBugId, string status)
		{
			return new BugzillaChangeStatusAction(bugzillaBugId, status, tpBug.CommentOnChangingState);
		}

		private string GetOwner(CommentDTO comment)
		{
			var owner = _userMapper.GetThirdPartyIdBy(comment.OwnerID);
			return !string.IsNullOrEmpty(owner)
			       	? owner
			       	: _repository.Get<UserDTO>(comment.OwnerID.ToString())
			       	  	.Select(x => x.Email)
			       	  	.SingleOrDefault();
		}
	}

	public interface IBugzillaActionFactory
	{
		IBugzillaAction GetAssigneeAction(string bugzillaBugId, string userEmail);
		IBugzillaAction GetCommentAction(CommentDTO comment, TimeSpan offset);
		IBugzillaQuery GetChangeStatusAction(BugDTO tpBug, string bugzillaBugId, string status);
	}
}