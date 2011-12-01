// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using NServiceBus;
using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Mapping;
using Tp.SourceControl.Settings;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.SourceControl.Workflow.Workflow
{
	public class UserMapper
	{
		private readonly Func<IStorageRepository> StorageRepository;
		private readonly Func<IActivityLogger> _logger;
		private static readonly MappingLookup DefaultUser = new MappingLookup();

		public UserMapper(Func<IStorageRepository> storageRepository, Func<IActivityLogger> logger)
		{
			StorageRepository = storageRepository;
			_logger = logger;
		}

		public MappingLookup GetAuthorBy(RevisionInfo revision)
		{
			if (string.IsNullOrEmpty(revision.Author))
			{
				_logger().Warn(string.Format("Can't map svn revision {0} - no author in svn revision.", revision.Id));
				return DefaultUser;
			}

			var tpUser = ObjectFactory.GetInstance<ISourceControlConnectionSettingsSource>().UserMapping[revision.Author];

			var userDtos = StorageRepository().Get<UserDTO>().Where(x => x.DeleteDate == null && x.IsActive == true);

			if (tpUser != null)
			{
				if (userDtos.FirstOrDefault(x => x.ID.GetValueOrDefault() == tpUser.Id) != null)
				{
					return tpUser;
				}

				_logger().Warn(string.Format("TP user not found: tpUser.Id = {0}, Name = {1}", tpUser.Id, tpUser.Name));

				foreach (var dto in userDtos)
				{
					_logger().Warn(string.Format("userDto: ID = {0}, UserID = {1}, login = {2}", dto.ID, dto.UserID, dto.Login));
				}

				if (userDtos.Empty())
				{
					_logger().Warn("userDtos are empty!!!!");
				}

				_logger().Warn("Bus Headers:");
				ObjectFactory.GetInstance<IBus>().CurrentMessageContext.Headers.ForEach(
					x => _logger().WarnFormat("{0}:{1}", x.Key, x.Value));

				_logger().Warn(string.Format("StorageRepository.IsNull = {0}", StorageRepository().IsNull));
			}

			var userDto = userDtos.FirstOrDefault(x => x.Login.ToLower() == revision.Author.ToLower());
			if (userDto != null)
			{
				return userDto.ConvertToUserLookup();
			}

			_logger().Warn(string.Format("Can't map svn user '{0}' - {1}.", revision.Author, string.Format("no mapping")));


			return DefaultUser;
		}

		public bool IsAuthorMapped(RevisionInfo revision)
		{
			return !GetAuthorBy(revision).Equals(DefaultUser);
		}
	}
}