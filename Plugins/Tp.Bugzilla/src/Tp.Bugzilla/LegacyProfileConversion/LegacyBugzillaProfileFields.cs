// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

namespace Tp.Bugzilla.LegacyProfileConversion
{
	public class LegacyBugzillaProfileFields
	{
		public const string SyncInterval = "SyncInterval";
		public const string Url = "BugzillaUrl";
		public const string Login = "BugzillaLogin";
		public const string Password = "BugzillaPassword";
		public const string Project = "Project";
		public const string Queries = "Queries";
		public const string Maps = "Maps";
		public const string Priorities = "Priorities";
		public const string Severities = "Severities";
		public const string EntityStates = "EntityStates";
		public const string Users = "Users";
		public const string CustomFields = "CustomFields";
		public const string AssigneeRole = "AssigneeRole";
		public const string ReporterRole = "ReporterRole";

		public const string XmlRoot = "Settings";
	}

	public class BugzillaConstants
	{
		public const int BugEntityTypeId = 8;
		public const int CommentEntityTypeId = 19;
		public const int AttachmentEntityTypeId = 20;
		public const string PluginName = "Bugzilla";
		public const string LegacyPluginName = "Bugzilla Integration";
	}
}