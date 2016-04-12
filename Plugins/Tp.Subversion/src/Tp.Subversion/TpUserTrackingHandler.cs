// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceBus;
using Tp.Core.UserMapping;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Plugin.Common.Storage;

namespace Tp.Subversion
{
	public class TpUserTrackingHandler :
		IHandleMessages<UserUpdatedMessage>,
		IHandleMessages<UserCreatedMessage>,
		IHandleMessages<UserDeletedMessage>
	{
		private readonly IStorageRepository _storage;

		public TpUserTrackingHandler(IStorageRepository storage)
		{
			_storage = storage;
		}

		public void Handle(UserUpdatedMessage message)
		{
			var userLookups = _storage.Get<UserLookup>(message.Dto.Login);
			if (!userLookups.Empty())
			{
				if ((message.ChangedFields.Contains(UserField.IsActive) && message.Dto.IsActive == false)
				    || message.ChangedFields.Contains(UserField.DeleteDate) && message.Dto.DeleteDate != null)
				{
					userLookups.Clear();
				}
				else
				{
					userLookups.Add(message.Dto.ConvertToUserLookup());
				}
			}
			else
			{
				userLookups.Add(message.Dto.ConvertToUserLookup());
			}
		}

		public void Handle(UserCreatedMessage message)
		{
			var userLookups = _storage.Get<UserLookup>(message.Dto.Login);
			userLookups.Clear();
			userLookups.Add(message.Dto.ConvertToUserLookup());
		}

		public void Handle(UserDeletedMessage message)
		{
			_storage.Get<UserLookup>(message.Dto.Login).Clear();
		}
	}
}