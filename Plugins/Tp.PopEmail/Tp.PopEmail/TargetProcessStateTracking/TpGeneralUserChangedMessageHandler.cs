// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NServiceBus;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Plugin.Common;
using Tp.PopEmailIntegration.Data;
using Tp.PopEmailIntegration.Sagas;

namespace Tp.PopEmailIntegration.TargetProcessStateTracking
{
	public class TpGeneralUserChangedMessageHandler : IHandleMessages<UserCreatedMessage>,
	                                                  IHandleMessages<UserDeletedMessage>,
	                                                  IHandleMessages<UserUpdatedMessage>,
	                                                  IHandleMessages<RequesterCreatedMessage>,
	                                                  IHandleMessages<RequesterDeletedMessage>,
	                                                  IHandleMessages<RequesterUpdatedMessage>
	{
		private readonly ITpBus _tpBus;
		private readonly UserRepository _userRepository;

		public TpGeneralUserChangedMessageHandler(ITpBus tpBus, UserRepository userRepository)
		{
			_tpBus = tpBus;
			_userRepository = userRepository;
		}

		public void Handle(UserCreatedMessage message)
		{
			var user = UserLite.Create(message.Dto);
			AddToRepository(user);
		}

		public void Handle(UserDeletedMessage message)
		{
			DeleteFromRepsitory(message.Dto.ID, message.Dto.Email);
		}

		public void Handle(UserUpdatedMessage message)
		{
			UpdateInRepository(UserLite.Create(message.Dto));
		}

		public void Handle(RequesterCreatedMessage message)
		{
			AddToRepository(UserLite.Create(message.Dto));
		}

		public void Handle(RequesterDeletedMessage message)
		{
			DeleteFromRepsitory(message.Dto.ID, message.Dto.Email);
		}

		public void Handle(RequesterUpdatedMessage message)
		{
			UpdateInRepository(UserLite.Create(message.Dto));
		}

		private void AddToRepository(UserLite userLite)
		{
			_userRepository.Add(userLite);
		}

		private void UpdateInRepository(UserLite userLiteUpdated)
		{
			_userRepository.Update(userLiteUpdated);
		}

		private void DeleteFromRepsitory(int? userId, string userEmail)
		{
			_userRepository.Remove(userId, userEmail);
		}
	}
}
