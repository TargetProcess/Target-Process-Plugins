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
	public class EntityStateChangedHandler : EntityChangedHandler<EntityStateDTO>,
	                                         IHandleMessages<EntityStateUpdatedMessage>,
	                                         IHandleMessages<EntityStateCreatedMessage>,
	                                         IHandleMessages<EntityStateDeletedMessage>
	{
		public EntityStateChangedHandler(IStorageRepository storage)
			: base(storage)
		{
		}

		public void Handle(EntityStateUpdatedMessage message)
		{
			Update(message.Dto);

			Storage.GetProfile<BugzillaProfile>().StatesMapping
				.Where(m => m.Value.Id == message.Dto.ID)
				.ForEach(m => m.Value.Name = message.Dto.Name);
		}

		public void Handle(EntityStateCreatedMessage message)
		{
			Create(message.Dto);
		}

		public void Handle(EntityStateDeletedMessage message)
		{
			Delete(message.Dto);

			Storage.GetProfile<BugzillaProfile>().StatesMapping
				.RemoveAll(m => m.Value.Id == message.Dto.ID);
		}

		protected override bool NeedToProcess(EntityStateDTO dto)
		{
			var project = Storage.Get<ProjectDTO>().Single(p => p.ID == Profile.Project);
			return dto.ProcessID == project.ProcessID &&
			       dto.EntityTypeName == BugzillaProfileInitializationSaga.BugEntityTypeName;
		}
	}
}