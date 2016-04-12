// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Mapping;
using Tp.SourceControl.Settings;
using Tp.SourceControl.VersionControlSystem;
using Tp.SourceControl.Workflow.Workflow;

namespace Tp.Subversion.Workflow
{
	public class SubversionUserMapper : UserMapper
	{
		public SubversionUserMapper(Func<IStorageRepository> storageRepository, Func<IActivityLogger> logger)
			: base(storageRepository, logger)
		{
		}

		protected override bool AuthorIsSpecified(RevisionInfo revision)
		{
			return !string.IsNullOrEmpty(revision.Author);
		}

		protected override MappingLookup GetTpUserFromMapping(RevisionInfo revision)
		{
			return ObjectFactory.GetInstance<ISourceControlConnectionSettingsSource>().UserMapping[revision.Author];
		}

		protected override UserDTO GuessUser(RevisionInfo revision, IEnumerable<UserDTO> userDtos)
		{
			return userDtos.FirstOrDefault(x => x.Login.ToLower() == revision.Author.ToLower());
		}
	}
}