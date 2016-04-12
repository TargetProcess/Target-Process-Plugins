// 
// Copyright (c) 2005-2013 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using Tp.BugTracking.BugFieldConverters;
using Tp.Integration.Common;
using Tp.Integration.Plugin.Common.Activity;

namespace Tp.Bugzilla.BugFieldConverters
{
	public class NameConverter : IBugConverter<BugzillaBug>
	{
		private readonly IActivityLogger _logger;

		public NameConverter(IActivityLogger logger)
		{
			_logger = logger;
		}

		public void Apply(BugzillaBug bugzillaBug, ConvertedBug convertedBug)
		{
			var bugName = bugzillaBug.short_desc;
			if (bugName.Length > 255)
			{
				_logger.WarnFormat("Bug {0} name was shortened to 255 characters", Int32.Parse(bugzillaBug.bug_id));
				bugName = bugName.Remove(255);
			}

			convertedBug.ChangedFields.Add(BugField.Name);
			convertedBug.BugDto.Name = bugName;
		}
	}
}