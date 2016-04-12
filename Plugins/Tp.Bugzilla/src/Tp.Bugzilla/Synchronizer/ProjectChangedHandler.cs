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
	using System;

	using log4net;

	public class ProjectChangedHandler : EntityChangedHandler<ProjectDTO>,
	                                     IHandleMessages<ProjectUpdatedMessage>,
	                                     IHandleMessages<ProjectDeletedMessage>
	{
		private readonly ILog _log;
		public ProjectChangedHandler(IStorageRepository storage) : base(storage)
		{
			_log = LogManager.GetLogger(GetType());
		}

		public void Handle(ProjectUpdatedMessage message)
		{
			if (NeedToProcess(message.Dto) && ProcessChanged(message.Dto))
			{
				Profile.StatesMapping.Clear();
			}
			Update(message.Dto);
		}

		private bool ProcessChanged(ProjectDTO dto)
		{
			return !dto.ProcessID.Equals(EntitiesStorage.Where(x => x.ID == dto.ID).Select(x => x.ProcessID).SingleOrDefault());
		}

		public void Handle(ProjectDeletedMessage message)
		{
			Delete(message.Dto);
		}

		protected override bool NeedToProcess(ProjectDTO dto)
		{
			if (dto == null)
			{
				throw new ArgumentNullException("dto");
			}

			if (Profile == null)
			{
				_log.Error("Profile is null");
				return false;
			}

			return dto.ProjectID == Profile.Project;
		}
	}
}