// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using NServiceBus;
using NServiceBus.Saga;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Search.Bus.Utils;
using Tp.Search.Messages;
using Tp.Search.Model;

namespace Tp.Search.Bus
{
	class SearcherPluginUpdatedProfileInitializationSaga :
		UpdatedProfileInitializationSaga<SearcherPluginProfileUpdatedInitializationData>,
		IHandleMessages<IndexExistingEntitiesDoneLocalMessage>
	{
		private readonly IProfileReadonly _profile;
		private readonly IActivityLogger _log;
		private readonly SagaServices _sagaServices;

		public SearcherPluginUpdatedProfileInitializationSaga()
		{
		}

		public SearcherPluginUpdatedProfileInitializationSaga(IProfileReadonly profile, IActivityLogger log, SagaServices sagaServices)
		{
			_profile = profile;
			_log = log;
			_sagaServices = sagaServices;
		}

		protected override void OnStartInitialization()
		{
			_sagaServices.TryCompleteInprogressSaga<SearcherPluginProfileInitializationSagaData>(Data.Id);
			_sagaServices.TryCompleteInprogressSaga<SearcherPluginProfileUpdatedInitializationData>(Data.Id);
			_log.Info("Started rebuilding indexes");
			var storage = _profile.Get<IndexProgress>();
			storage.Clear();
			storage.Add(new IndexProgress());
			SendLocal(new IndexExistingEntitiesLocalMessage { OuterSagaId = Data.Id });
		}

		public override void ConfigureHowToFindSaga()
		{
			ConfigureMapping<IndexExistingEntitiesDoneLocalMessage>(saga => saga.Id, message => message.SagaId);
		}

		public void Handle(IndexExistingEntitiesDoneLocalMessage message)
		{
			_log.Info("Finished rebuilding indexes");
			_profile.Get<IndexProgress>().Clear();
			MarkAsComplete();
		}
	}

	public class SearcherPluginProfileUpdatedInitializationData : ISagaEntity
	{
		public Guid Id { get; set; }
		public string Originator { get; set; }
		public string OriginalMessageId { get; set; }
	}
}
