// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Linq;
using NServiceBus;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Bugzilla.Synchronizer
{
	public class UserChangedHandler : IHandleMessages<UserUpdatedMessage>,
	                                  IHandleMessages<UserCreatedMessage>,
	                                  IHandleMessages<UserDeletedMessage>
	{
		private readonly IStorageRepository _storage;

		public UserChangedHandler(IStorageRepository storage)
		{
			_storage = storage;
		}

		public void Handle(UserUpdatedMessage message)
		{
			var userByIdStorage = UserByIdStorage(message.Dto);
			var userByEmailStorage = GetChangedUserEmailStorage(message, userByIdStorage);

			userByEmailStorage.Clear();
			userByIdStorage.Clear();

			Create(message.Dto);
		}

		public void Handle(UserCreatedMessage message)
		{
			Create(message.Dto);
		}

		public void Handle(UserDeletedMessage message)
		{
			UserByIdStorage(message.Dto).Clear();
			UserByEmailStorage(message.Dto).Clear();
		}

		private IStorage<UserDTO> GetChangedUserEmailStorage(UserUpdatedMessage message, IEnumerable<UserDTO> userByIdStorage)
		{
			var user = message.ChangedFields.Contains(UserField.Email)
			           	? userByIdStorage.Single()
			           	: message.Dto;

			return UserByEmailStorage(user);
		}

		private void Create(UserDTO dto)
		{
			if (!dto.IsActiveNotDeletedUser())
				return;

			UserByIdStorage(dto).Add(dto);
			UserByEmailStorage(dto).Add(dto);
		}

		private IStorage<UserDTO> UserByIdStorage(UserDTO user)
		{
			return _storage.Get<UserDTO>(user.ID.ToString());
		}

		private IStorage<UserDTO> UserByEmailStorage(UserDTO user)
		{
			return _storage.Get<UserDTO>(user.Email);
		}
	}
}