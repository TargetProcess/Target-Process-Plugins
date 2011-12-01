// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using Tp.Git.VersionControlSystem;
using Tp.Integration.Messages.Ticker;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Mapping;
using Tp.Integration.Plugin.Common.Validation;
using Tp.SourceControl.Settings;

namespace Tp.Git
{
	[Profile]
	[DataContract]
	public class GitPluginProfile : ConnectionSettings, ISynchronizableProfile, IValidatable
	{
		public const string StartRevisionField = "StartRevision";

		public GitPluginProfile()
		{
			UserMapping = new MappingContainer();
			StartRevision = GitRevisionId.UtcTimeMin.ToShortDateString();
		}

		[IgnoreDataMember]
		public int SynchronizationInterval
		{
			get { return 5; }
			set { }
		}

		#region Validation

		public bool ValidateStartRevision(PluginProfileErrorCollection errors)
		{
			return StartRevisionShouldBeValidDate(errors) && (StartRevisionShouldBeNotLessThanMin(errors) && StartRevisionShouldNotExceedTheMax(errors));
		}

		private bool StartRevisionShouldBeValidDate(PluginProfileErrorCollection errors)
		{
			DateTime result;
			if (!DateTime.TryParse(StartRevision, CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AdjustToUniversal, out result))
			{
				errors.Add(new PluginProfileError {FieldName = StartRevisionField, Message = string.Format("Start Revision Date should be specified in mm/dd/yyyy format")});
				return false;
			}
			return true;
		}

		private bool StartRevisionShouldNotExceedTheMax(PluginProfileErrorCollection errors)
		{
			DateTime result = DateTime.Parse(StartRevision, CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AdjustToUniversal);
			if (result > GitRevisionId.UtcTimeMax)
			{
				errors.Add(new PluginProfileError
				{FieldName = StartRevisionField, Message = string.Format("Start Revision Date should be not behind {0}", GitRevisionId.UtcTimeMax.ToShortDateString())});
				return false;
			}
			return true;
		}

		private bool StartRevisionShouldBeNotLessThanMin(PluginProfileErrorCollection errors)
		{
			DateTime result = DateTime.Parse(StartRevision, CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AdjustToUniversal);
			if (result < GitRevisionId.UtcTimeMin)
			{
				errors.Add(new PluginProfileError
				{FieldName = StartRevisionField, Message = string.Format("Start Revision Date should be not before {0}", GitRevisionId.UtcTimeMin.ToShortDateString())});
				return false;
			}
			return true;
		}

		public void Validate(PluginProfileErrorCollection errors)
		{
			ValidateStartRevision(errors);
			ValidateUserMapping(errors);
		}

		private void ValidateUserMapping(PluginProfileErrorCollection errors)
		{
			if (UserMapping.Select(x => x.Key.ToLower()).Distinct().Count() != UserMapping.Count)
			{
				errors.Add(new PluginProfileError {FieldName = "user-mapping", Message = "Can't map an svn user to TargetProcess user twice."});
			}
		}

		#endregion
	}
}