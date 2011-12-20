// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using Tp.Integration.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Mapping;

namespace Tp.Bugzilla.BugFieldConverters
{
	public class EntityStateConverter : GuessConverter<EntityStateDTO>
	{
		private readonly IBugzillaService _bugzillaService;

		public EntityStateConverter(IStorageRepository storageRepository, IBugzillaService bugzillaService, IActivityLogger logger)
			: base(storageRepository, logger)
		{
			_bugzillaService = bugzillaService;
		}

		protected override void SetValue(ConvertedBug dto, int id)
		{
			dto.BugDto.EntityStateID = id;
		}

		protected override EntityStateDTO GetFromStorage(string value)
		{
			return GetStorage().SingleOrDefault(s => s.Name.Equals(value, StringComparison.InvariantCultureIgnoreCase));
		}

		protected override string GetBugzillaValue(BugzillaBug bugzillaBug)
		{
			return bugzillaBug.bug_status;
		}

		protected override MappingContainer Map
		{
			get { return Profile.StatesMapping; }
		}

		protected override BugField BugField
		{
			get { return BugField.EntityStateID; }
		}

		protected override string BugFieldName
		{
			get { return "Entity State"; }
		}

		public string GetMappedBugzillaStatus(BugDTO bug)
		{
			var stateId = bug.EntityStateID.GetValueOrDefault();
			var mappedStates = Map.Where(x => x.Value.Id == stateId).Select(x => x.Key).ToList();

			if (mappedStates.Count() > 1)
			{
				_logger.WarnFormat("State mapping is ambiguous. Bugzilla state is not changed. Entity State: {0}", bug.EntityStateName);
				return null;
			}

			var mappedValue = mappedStates.SingleOrDefault();
			var bugStatus = mappedValue ?? GetStorage().Single(s => s.EntityStateID == stateId).Name;
			var statuses = _bugzillaService.GetStatuses();
			return statuses.Find(s => s.Equals(bugStatus, StringComparison.InvariantCultureIgnoreCase));
		}
	}
}