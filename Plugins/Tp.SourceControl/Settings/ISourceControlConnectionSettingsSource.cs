//  
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Plugin.Common.Mapping;

namespace Tp.SourceControl.Settings
{
	public interface ISourceControlConnectionSettingsSource
	{
		string Uri { get; }
		string Login { get; }
		string Password { get; }
		string StartRevision { get; set; }

		MappingContainer UserMapping { get; set; }
	}
}