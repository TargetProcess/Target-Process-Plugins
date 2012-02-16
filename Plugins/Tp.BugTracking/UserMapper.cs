// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Linq;
using Tp.BugTracking.Settings;
using Tp.Integration.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Mapping;

namespace Tp.BugTracking
{
	public class UserMapper : IUserMapper
	{
		private readonly IStorageRepository _storageRepository;
		private readonly IActivityLogger _logger;
		public const string WildcardSymbol = "*";

		public UserMapper(IStorageRepository storageRepository, IActivityLogger logger)
		{
			_storageRepository = storageRepository;
			_logger = logger;
		}

		private IBugTrackingMappingSource Profile
		{
			get { return _storageRepository.GetProfile<IBugTrackingMappingSource>(); }
		}

		public int? GetTpIdBy(string thirdPartyId)
		{
			if (string.IsNullOrEmpty(thirdPartyId))
			{
				return null;
			}

			var result = GetTpIdFromUserMapping(thirdPartyId) ?? GetTpIdFromStorage(thirdPartyId);

			if (!result.HasValue)
			{
				_logger.WarnFormat("Cannot map user. Email: {0}", thirdPartyId);
			}

			return result;
		}

		public string GetThirdPartyIdBy(int? tpUserId)
		{
			var mappedUsers = GetMappedBugzillaUsers(tpUserId);

			if (mappedUsers.Count() == 1)
			{
				var user = mappedUsers.Single();

				if (user.Key != WildcardSymbol)
				{
					return user.Key;
				}

				_logger.WarnFormat(
					"Mapping user by wildcard. User mapping is ambiguous. Bugzilla assignment is not changed. User: {0}",
					user.Value.Name);

				return null;
			}

			var result = !mappedUsers.Any() ? GetBugzillaUserFromStorage(tpUserId) : null;

			if (result == null)
			{
				_logger.WarnFormat("Cannot map user. TargetProcess User ID: {0}", tpUserId);
				return null;
			}

			return result.Email;
		}

		private List<MappingElement> GetMappedBugzillaUsers(int? tpUserId)
		{
			return Profile.UserMapping
				.Where(x => x.Value.Id == tpUserId)
				.ToList();
		}

		private UserDTO GetBugzillaUserFromStorage(int? tpUserId)
		{
			return _storageRepository.Get<UserDTO>(tpUserId.ToString()).SingleOrDefault();
		}

		private int? GetTpIdFromStorage(string bugzillaEmail)
		{
			var users = _storageRepository.Get<UserDTO>(bugzillaEmail).ToList();

			if (users.Count == 1)
			{
				var user = users.Single();
				return user.ID;
			}

			return null;
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