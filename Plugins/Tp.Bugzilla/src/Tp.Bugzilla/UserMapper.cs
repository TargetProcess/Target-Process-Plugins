// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Linq;
using Tp.Integration.Common;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Bugzilla
{
	public class UserMapper : IUserMapper
	{
		private readonly IStorageRepository _storageRepository;
		public const string WildcardSymbol = "*";

		public UserMapper(IStorageRepository storageRepository)
		{
			_storageRepository = storageRepository;
		}

		private BugzillaProfile Profile
		{
			get { return _storageRepository.GetProfile<BugzillaProfile>(); }
		}

		public int? GetTpIdBy(string bugzillaEmail)
		{
			if (string.IsNullOrEmpty(bugzillaEmail))
			{
				return null;
			}

			return GetTpIdFromUserMapping(bugzillaEmail) ?? GetTpIdFromStorage(bugzillaEmail);
		}

		public string GetBugzillaEmailBy(int? tpUserId)
		{
			var mappedUsers = GetMappedBugzillaUsers(tpUserId);

			if (mappedUsers.Count() == 1)
			{
				var m = mappedUsers.Single();

				return m != WildcardSymbol ? m : null;
			}

			return mappedUsers.Count() == 0 ? GetBugzillaEmailFromStorage(tpUserId) : null;
		}

		private List<string> GetMappedBugzillaUsers(int? tpUserId)
		{
			return Profile.UserMapping
				.Where(x => x.Value.Id == tpUserId)
				.Select(x => x.Key)
				.ToList();
		}

		private string GetBugzillaEmailFromStorage(int? tpUserId)
		{
			var userFromStorage = _storageRepository.Get<UserDTO>(tpUserId.ToString()).SingleOrDefault();

			return userFromStorage != null
			       	? userFromStorage.Email
			       	: null;
		}

		private int? GetTpIdFromStorage(string bugzillaEmail)
		{
			var users = _storageRepository.Get<UserDTO>(bugzillaEmail).ToList();

			return users.Count == 1
			       	? users.Single().ID
			       	: null;
		}

		private int? GetTpIdFromUserMapping(string userEmail)
		{
			var result = Profile.UserMapping[userEmail];
			if (result != null)
			{
				var user = _storageRepository.Get<UserDTO>(result.Id.ToString()).SingleOrDefault();

				if (user != null && user.IsActiveNotDeletedUser())
					return result.Id;
			}

			var wildcardElement = Profile.UserMapping[WildcardSymbol];
			if (wildcardElement != null)
			{
				return wildcardElement.Id;
			}

			return null;
		}
	}
}