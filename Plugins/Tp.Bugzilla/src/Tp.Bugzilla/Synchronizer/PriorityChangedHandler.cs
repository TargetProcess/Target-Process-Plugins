// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using NServiceBus;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Bugzilla.Synchronizer
{
	public class PriorityChangedHandler : EntityChangedHandler<PriorityDTO>,
	                                      IHandleMessages<PriorityUpdatedMessage>,
	                                      IHandleMessages<PriorityCreatedMessage>,
	                                      IHandleMessages<PriorityDeletedMessage>
	{
		public PriorityChangedHandler(IStorageRepository storage) : base(storage)
		{
		}

		public void Handle(PriorityUpdatedMessage message)
		{
			Update(message.Dto);

			Storage.GetProfile<BugzillaProfile>().PrioritiesMapping
				.Where(m => m.Value.Id == message.Dto.ID)
				.ForEach(m => m.Value.Name = message.Dto.Name);
		}

		public void Handle(PriorityCreatedMessage message)
		{
			Create(message.Dto);
		}

		public void Handle(PriorityDeletedMessage message)
		{
			Delete(message.Dto);

			Storage.GetProfile<BugzillaProfile>().PrioritiesMapping
				.RemoveAll(m => m.Value.Id == message.Dto.ID);
		}

		protected override bool NeedToProcess(PriorityDTO dto)
		{
			return dto.EntityTypeName == BugzillaProfileInitializationSaga.BugEntityTypeName;
		}
	}
}