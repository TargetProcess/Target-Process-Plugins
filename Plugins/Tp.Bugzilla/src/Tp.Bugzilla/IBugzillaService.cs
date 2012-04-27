// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using Tp.Bugzilla.BugzillaQueries;
using Tp.Bugzilla.Schemas;

namespace Tp.Bugzilla
{
	public interface IBugzillaService
	{
		int[] GetChangedBugIds(DateTime? date);

		bugzilla_properties CheckConnection();

		bugCollection GetBugs(int[] bugIDs);

		List<string> GetStatuses();

		List<string> GetResolutions();

		void Execute(IBugzillaQuery query);

		TimeSpan GetTimeOffset();
	}
}
