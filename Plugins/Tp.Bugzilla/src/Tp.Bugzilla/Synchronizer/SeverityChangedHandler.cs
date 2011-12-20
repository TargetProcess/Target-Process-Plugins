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
	public class SeverityChangedHandler : EntityChangedHandler<SeverityDTO>,
	                                      IHandleMessages<SeverityUpdatedMessage>,
	                                      IHandleMessages<SeverityCreatedMessage>,
	                                      IHandleMessages<SeverityDeletedMessage>
	{
		public SeverityChangedHandler(IStorageRepository storage)
			: base(storage)
		{
		}

		public void Handle(SeverityUpdatedMessage message)
		{
			Update(message.Dto);

			Storage.GetProfile<BugzillaProfile>().SeveritiesMapping
				.Where(m => m.Value.Id == message.Dto.ID)
				.ForEach(m => m.Value.Name = message.Dto.Name);
		}

		public void Handle(SeverityCreatedMessage message)
		{
			Create(message.Dto);
		}

		public void Handle(SeverityDeletedMessage message)
		{
			Delete(message.Dto);

			Storage.GetProfile<BugzillaProfile>().SeveritiesMapping
				.RemoveAll(m => m.Value.Id == message.Dto.ID);
		}
	}
}