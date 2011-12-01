// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using NServiceBus;
using NServiceBus.Saga;
using StructureMap;
using Tp.Integration.Messages.EntityLifecycle.Queries;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common;
using Tp.PopEmailIntegration.Data;

namespace Tp.PopEmailIntegration.Initialization
{
	public class LoadMessageUidsSaga : TpSaga<LoadMessageUidsSagaData>,
	                                   IAmStartedByMessages<LoadMessageUidsCommandInternal>,
	                                   IHandleMessages<MessageUidQueryResult>
	{
		public override void ConfigureHowToFindSaga()
		{
			ConfigureMapping<MessageUidQueryResult>(
				saga => saga.Id,
				message => message.SagaId
				);
		}

		public void Handle(LoadMessageUidsCommandInternal message)
		{
			Data.OuterSagaId = message.OuterSagaId;
			var profile = StorageRepository().GetProfile<ProjectEmailProfile>();
			Send(new RetrieveAllMessageUidsQuery {MailLogin = profile.Login, MailServer = profile.MailServer});
		}

		public void Handle(MessageUidQueryResult message)
		{
			if (message.Dtos != null)
			{
				Data.MessageUidsRetrievedCount += message.Dtos.Length;
				ObjectFactory.GetInstance<MessageUidRepository>().AddRange(message.Dtos.Select(messageUidDto => messageUidDto.UID).ToArray());
			}

			if (Data.MessageUidsRetrievedCount == message.QueryResultCount)
			{
				SendLocal(new MessageUidsLoadedMessageInternal {SagaId = Data.OuterSagaId});
				MarkAsComplete();
			}
		}
	}

	[Serializable]
	public class MessageUidsLoadedMessageInternal : ISagaMessage, IPluginLocalMessage
	{
		public Guid SagaId { get; set; }
	}

	[Serializable]
	public class LoadMessageUidsCommandInternal : IPluginLocalMessage
	{
		public Guid OuterSagaId { get; set; }
	}

	public class LoadMessageUidsSagaData : ISagaEntity
	{
		public Guid Id { get; set; }
		public string Originator { get; set; }
		public string OriginalMessageId { get; set; }
		public Guid OuterSagaId { get; set; }
		public int AllMessageUidsCount { get; set; }
		public int MessageUidsRetrievedCount { get; set; }
	}
}