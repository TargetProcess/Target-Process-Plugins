// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using NServiceBus;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.SourceControl
{
	public class UserStorageSynchronizer :
		IHandleMessages<UserCreatedMessage>,
		IHandleMessages<UserDeletedMessage>,
		IHandleMessages<UserUpdatedMessage>
	{
		private readonly IStorageRepository _storage;

		public UserStorageSynchronizer(IStorageRepository storage)
		{
			_storage = storage;
		}

		public void Handle(UserCreatedMessage message)
		{
			_storage.Get<UserDTO>().Add(message.Dto);
		}

		public void Handle(UserDeletedMessage message)
		{
			_storage.Get<UserDTO>().Remove(x => x.ID == message.Dto.ID);
		}

		public void Handle(UserUpdatedMessage message)
		{
			if (_storage.Get<UserDTO>().FirstOrDefault(x => x.ID == message.Dto.ID) != null)
			{
				_storage.Get<UserDTO>().Update(message.Dto, x => x.ID == message.Dto.ID);
			}
			else
			{
				_storage.Get<UserDTO>().Add(message.Dto);
			}
		}
	}
}