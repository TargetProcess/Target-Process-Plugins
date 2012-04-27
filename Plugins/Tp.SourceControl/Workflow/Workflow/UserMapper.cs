// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using Tp.Integration.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Mapping;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.SourceControl.Workflow.Workflow
{
	public abstract class UserMapper
	{
		protected readonly Func<IStorageRepository> StorageRepository;
		private readonly Func<IActivityLogger> _logger;
		public static readonly MappingLookup DefaultUser = new MappingLookup();

		protected UserMapper(Func<IStorageRepository> storageRepository, Func<IActivityLogger> logger)
		{
			StorageRepository = storageRepository;
			_logger = logger;
		}

		public MappingLookup GetAuthorBy(RevisionInfo revision)
		{
			if (!AuthorIsSpecified(revision))
			{
				_logger().Warn(string.Format("No author in revision. Revision ID: {0}", revision.Id));
				return DefaultUser;
			}

			var tpUser = GetTpUserFromMapping(revision);
			var userDtos = StorageRepository().Get<UserDTO>().Where(x => x.DeleteDate == null && x.IsActive == true).ToList();

			if (tpUser != null)
			{
				if (userDtos.FirstOrDefault(x => x.ID.GetValueOrDefault() == tpUser.Id) != null)
				{
					return tpUser;
				}

				_logger().Warn(string.Format("TP user not found. TargetProcess User ID: {0}; Name: {1}", tpUser.Id, tpUser.Name));
			}

			var user = GuessUser(revision, userDtos);

			if (user != null)
			{
				return user.ConvertToUserLookup();
			}

			_logger().Warn(string.Format("Revision author doesn't match any TP User name. There is no valid mapping for user {0}", revision.Author));
			return DefaultUser;
		}

		protected abstract UserDTO GuessUser(RevisionInfo revision, IEnumerable<UserDTO> userDtos);

		protected abstract bool AuthorIsSpecified(RevisionInfo revision);

		protected abstract MappingLookup GetTpUserFromMapping(RevisionInfo revision);

		public bool IsAuthorMapped(RevisionInfo revision)
		{
			return !GetAuthorBy(revision).Equals(DefaultUser);
		}
	}
}