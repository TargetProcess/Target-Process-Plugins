// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using NServiceBus;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Tfs.WorkItemsIntegration;
using IProfile = Tp.Integration.Plugin.Common.Domain.IProfile;

namespace Tp.Tfs.Handlers
{
	public class TfsProfileCreatedHandler : IHandleMessages<ProfileAddedMessage>
	{
		private readonly IProfile _profile;

		public TfsProfileCreatedHandler(IProfile profile)
		{
			_profile = profile;
		}

		public void Handle(ProfileAddedMessage message)
		{
			var tfsProfile = _profile.Settings as TfsPluginProfile;

			if (!tfsProfile.WorkItemsEnabled)
				return;

			var projectsMapping = new ProjectsMappingHistoryElement()
			{
				Key = tfsProfile.ProjectsMapping[0].Key,
				Value = tfsProfile.ProjectsMapping[0].Value,
				WorkItemsRange = new CreatedWorkItemsRange() { Min = int.Parse(tfsProfile.StartWorkItem), Max = -1 },
				IsCurrent = true,
				ImportedTypes = tfsProfile.EntityMapping.Select(
						x => new ImportedType { StartID = int.Parse(tfsProfile.StartWorkItem), Type = x.First, IsFirstSync = false }).ToList()
			};

			var projectsMappingHistory = new ProjectsMappingHistory();
			projectsMappingHistory.Add(projectsMapping);

			_profile.Get<ProjectsMappingHistory>().ReplaceWith(projectsMappingHistory);
		}
	}
}
