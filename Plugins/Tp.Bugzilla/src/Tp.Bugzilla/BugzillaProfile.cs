// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using System.Runtime.Serialization;
using Tp.BugTracking.Settings;
using Tp.Integration.Messages.Ticker;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Mapping;
using Tp.Integration.Plugin.Common.Validation;

namespace Tp.Bugzilla
{
	[Profile, Serializable]
	[DataContract]
	public class BugzillaProfile : ISynchronizableProfile, IValidatable, IBugTrackingMappingSource,
	                               IBugTrackingConnectionSettingsSource
	{
		public const string ProfileField = "Profile";
		public const string PasswordField = "Password";
		public const string LoginField = "Login";
		public const string UrlField = "Url";
		public const string ProjectField = "Project";
		public const string QueriesField = "SavedSearches";
		public const string UserMappingField = "UserMapping";
		public const string RolesMappingField = "RolesMapping";
		private string _savedSearches;
		private string _url;

		public BugzillaProfile()
		{
			UserMapping = new MappingContainer();
			StatesMapping = new MappingContainer();
			SeveritiesMapping = new MappingContainer();
			PrioritiesMapping = new MappingContainer();
			RolesMapping = new MappingContainer();
		}

		[DataMember]
		public string Url
		{
			get { return _url; }
			set { _url = value.Trim(); }
		}

		[DataMember]
		public string Login { get; set; }

		[DataMember]
		public string Password { get; set; }

		[DataMember]
		public int Project { get; set; }

		[DataMember]
		public string SavedSearches
		{
			get { return _savedSearches; }
			set
			{
				_savedSearches = string.Join(",", value.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
				                                  	.Select(x => x.Trim())
				                                  	.ToArray());
			}
		}

		[DataMember]
		public MappingContainer StatesMapping { get; set; }

		[DataMember]
		public MappingContainer SeveritiesMapping { get; set; }

		[DataMember]
		public MappingContainer PrioritiesMapping { get; set; }

		[DataMember]
		public MappingContainer UserMapping { get; set; }

		[DataMember]
		public MappingContainer RolesMapping { get; set; }

		public MappingLookup GetAssigneeRole()
		{
			return RolesMapping[DefaultRoles.Assignee];
		}

		public MappingLookup GetReporterRole()
		{
			return RolesMapping[DefaultRoles.Reporter];
		}

		public void Validate(PluginProfileErrorCollection errors)
		{
			ValidateCredentials(errors);
			ValidateMappings(errors);
		}

		public void ValidateCredentials(PluginProfileErrorCollection errors)
		{
			ValidateUrl(errors);
			ValidateLogin(errors);
			ValidatePassword(errors);
			ValidateProject(errors);
			ValidateQueries(errors);
		}

		public void ValidateMappings(PluginProfileErrorCollection errors)
		{
			ValidateUserMapping(errors);
			ValidateRolesMapping(errors);
		}

		private void ValidateQueries(PluginProfileErrorCollection errors)
		{
			if (string.IsNullOrEmpty(SavedSearches))
			{
				errors.Add(new PluginProfileError {FieldName = QueriesField, Message = "Saved Search is required"});
			}
		}

		private void ValidateProject(PluginProfileErrorCollection errors)
		{
			if (Project <= 0)
			{
				errors.Add(new PluginProfileError {FieldName = ProjectField, Message = "Project is required"});
			}
		}

		private void ValidatePassword(PluginProfileErrorCollection errors)
		{
			if (string.IsNullOrEmpty(Password))
			{
				errors.Add(new PluginProfileError {FieldName = PasswordField, Message = "Password is required"});
			}
		}

		private void ValidateLogin(PluginProfileErrorCollection errors)
		{
			if (string.IsNullOrEmpty(Login))
			{
				errors.Add(new PluginProfileError {FieldName = LoginField, Message = "Login is required"});
			}
		}

		private void ValidateUrl(PluginProfileErrorCollection errors)
		{
			if (string.IsNullOrEmpty(Url))
			{
				errors.Add(new PluginProfileError {FieldName = UrlField, Message = "URL is required"});
			}
		}

		private void ValidateUserMapping(PluginProfileErrorCollection errors)
		{
			if (UserMapping.Select(x => x.Key.ToLower()).Distinct().Count() != UserMapping.Count)
			{
				errors.Add(new PluginProfileError
				           	{FieldName = UserMappingField, Message = "Can't map a Bugzilla user to TargetProcess user twice."});
			}
		}

		private void ValidateRolesMapping(PluginProfileErrorCollection errors)
		{
			if (RolesMapping.Any(x => x.Value == null || string.IsNullOrEmpty(x.Value.Name)))
			{
				errors.Add(new PluginProfileError
				           	{FieldName = RolesMappingField, Message = "All Bugzilla roles should be mapped to TP roles"});
			}
		}

		public int SynchronizationInterval
		{
			get { return 5; }
		}

		public override string ToString()
		{
			return string.Format("URL:'{0}', Login: '{1}'", Url, Login);
		}
	}
}