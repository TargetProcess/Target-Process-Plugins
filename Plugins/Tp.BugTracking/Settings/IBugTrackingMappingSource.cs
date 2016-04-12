// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Plugin.Common.Mapping;

namespace Tp.BugTracking.Settings
{
	public interface IBugTrackingMappingSource
	{
		MappingContainer StatesMapping { get; }

		MappingContainer SeveritiesMapping { get; }

		MappingContainer PrioritiesMapping { get; }

		MappingContainer UserMapping { get; }

		MappingContainer RolesMapping { get; }
	}
}