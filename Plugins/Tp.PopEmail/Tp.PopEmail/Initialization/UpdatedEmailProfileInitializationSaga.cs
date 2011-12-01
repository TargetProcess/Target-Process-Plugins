// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using NServiceBus;
using NServiceBus.Saga;
using StructureMap;
using Tp.Integration.Plugin.Common;
using Tp.PopEmailIntegration.Data;

namespace Tp.PopEmailIntegration.Initialization
{
	public class UpdatedEmailProfileInitializationSaga :
		UpdatedProfileInitializationSaga<UpdatedEmailProfileInitializationSagaData>,
		IHandleMessages<MessageUidsLoadedMessageInternal>
	{
		public override void ConfigureHowToFindSaga()
		{
			ConfigureMapping<MessageUidsLoadedMessageInternal>(saga => saga.Id, message => message.SagaId);
		}

		protected override void OnStartInitialization()
		{
			var profileServerAndLogins = StorageRepository().Get<ProfileServerAndLogin>();
			var severAndLogin = profileServerAndLogins.FirstOrDefault();
			var profile = StorageRepository().GetProfile<ProjectEmailProfile>();

			if (severAndLogin != null)
			{
				if (severAndLogin.MailServer == profile.MailServer && severAndLogin.Login == profile.Login)
				{
					MarkAsComplete();
					return;
				}
			}

			profileServerAndLogins.ReplaceWith(new ProfileServerAndLogin {MailServer = profile.MailServer, Login = profile.Login});

			ObjectFactory.GetInstance<MessageUidRepository>().RemoveAll();

			SendLocal(new LoadMessageUidsCommandInternal {OuterSagaId = Data.Id});
		}

		public void Handle(MessageUidsLoadedMessageInternal message)
		{
			MarkAsComplete();
		}
	}

	public class UpdatedEmailProfileInitializationSagaData : ISagaEntity
	{
		public Guid Id { get; set; }
		public string Originator { get; set; }
		public string OriginalMessageId { get; set; }
	}
}