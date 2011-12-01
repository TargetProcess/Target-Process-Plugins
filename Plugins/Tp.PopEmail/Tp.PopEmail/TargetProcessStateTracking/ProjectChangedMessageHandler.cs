// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NServiceBus;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.PopEmailIntegration.TargetProcessStateTracking
{
	public class ProjectChangedMessageHandler : IHandleMessages<ProjectCreatedMessage>,
	                                            IHandleMessages<ProjectUpdatedMessage>,
	                                            IHandleMessages<ProjectDeletedMessage>
	{
		private readonly IStorageRepository _storageRepository;

		public ProjectChangedMessageHandler(IStorageRepository storageRepository)
		{
			_storageRepository = storageRepository;
		}

		public void Handle(ProjectCreatedMessage message)
		{
			_storageRepository.Get<ProjectDTO>().Add(message.Dto);
		}

		public void Handle(ProjectUpdatedMessage message)
		{
			_storageRepository.Get<ProjectDTO>().Update(message.Dto, x => x.ID == message.Dto.ID);
		}

		public void Handle(ProjectDeletedMessage message)
		{
			_storageRepository.Get<ProjectDTO>().Remove(x => x.ID == message.Dto.ID);
		}
	}
}
