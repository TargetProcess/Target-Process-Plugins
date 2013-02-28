// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Tp.Integration.Messages.Ticker;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Mapping;
using Tp.Integration.Plugin.Common.Validation;
using Tp.Mercurial.VersionControlSystem;
using Tp.SourceControl.Settings;

namespace Tp.Mercurial
{
	[Profile]
	[DataContract]
	public class MercurialPluginProfile : ConnectionSettings, ISynchronizableProfile, IValidatable
	{
		public const string StartRevisionField = "StartRevision";

		public MercurialPluginProfile()
		{
			UserMapping = new MappingContainer();
		    StartRevision = MercurialRevisionId.UtcTimeMin.ToShortDateString();
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
			if (result > MercurialRevisionId.UtcTimeMax)
			{
				errors.Add(new PluginProfileError
				{
				    FieldName = StartRevisionField, 
                    Message = string.Format("Start Revision Date should be not behind {0}", MercurialRevisionId.UtcTimeMax.ToShortDateString())
				});
				return false;
			}
			return true;
		}

		private bool StartRevisionShouldBeNotLessThanMin(PluginProfileErrorCollection errors)
		{
			DateTime result = DateTime.Parse(StartRevision, CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AdjustToUniversal);
            if (result < MercurialRevisionId.UtcTimeMin)
			{
				errors.Add(new PluginProfileError
				{
				    FieldName = StartRevisionField, 
                    Message = string.Format("Start Revision Date should be not before {0}", MercurialRevisionId.UtcTimeMin.ToShortDateString())
				});
				return false;
			}
			return true;
		}

		public void Validate(PluginProfileErrorCollection errors)
		{
			ValidateUri(errors);
			ValidateStartRevision(errors);
			ValidateUserMapping(errors);
		}

		public void ValidateUri(PluginProfileErrorCollection errors)
		{
			ValidateUriIsNotEmpty(errors);
			ValidateUriFormat(errors);
            ValidateCredentialsInUri(errors);
		}

        private void ValidateCredentialsInUri(PluginProfileErrorCollection errors)
        {
            if (!Uri.StartsWith("http://") && !Uri.StartsWith("https://"))
                return;

            if (UriContainsCreditials())
                return;

            if (UriContainsLoginOnly() && !string.IsNullOrEmpty(Password))
            {
                Uri = Uri.Insert(Uri.IndexOf("@"), ":" + Password);
            }
            else if (!string.IsNullOrEmpty(Login) && !UriContainsLoginOnly())
            {
                if (!string.IsNullOrEmpty(Password))
                {
                    Uri = Uri.Insert(Uri.IndexOf(@"://") + @"://".Length, Login + ":" + Password + "@");
                }
                else
                {
                    Uri = Uri.Insert(Uri.IndexOf(@"://") + @"://".Length, Login + "@");
                }
            }
        }

        private bool UriContainsCreditials()
        {
            return Regex.IsMatch(Uri, @"^((http|https):)//((\w+):(\w+)@)[\w\d\.~-]+(:\d+)?(/[\S]*)?$");
        }

        private bool UriContainsLoginOnly()
        {
            return Uri.StartsWith("http://" + Login + "@") || Uri.StartsWith("https://" + Login + "@");
        }

        private void ValidateUriFormat(PluginProfileErrorCollection errors)
		{
            if (!string.IsNullOrEmpty(Uri) && !IsCommonUri() && !IsFileUri() && !IsFileshareUri())
			{
				errors.Add(IsSshUri()
				           	? new PluginProfileError {Message = "Connection via SSH is not supported.", FieldName = UriField}
				           	: new PluginProfileError {Message = "Wrong Uri format.", FieldName = UriField});
			}
		}

		private bool IsSshUri()
		{
			return Regex.IsMatch(Uri, @"^ssh://(.+@)?([\w\d\.]+)(:\d+)?/(~\S+)?(/\S)*$", RegexOptions.IgnoreCase)
				|| Regex.IsMatch(Uri, @"^(?!file:)(?!http:)(?!https:)(?!ftp:)(?!ftps:)(?!rsync:)(?!git:)(?!ssh:)(.+@)?[\w\d]+[\.][\w\d]+:(/~.+/)?\S*$", RegexOptions.IgnoreCase);
		}

        private bool IsFileUri()
		{
			return Regex.IsMatch(Uri, @"^file:///\S+$", RegexOptions.IgnoreCase);
		}

		private bool IsCommonUri()
		{
			return Regex.IsMatch(Uri, @"^((http|https|ftp|ftps|rsync):)?//(.*@)?[\w\d\.~-]+(:\d+)?(/[\S]*)?$", RegexOptions.IgnoreCase);
		}

        private bool IsFileshareUri()
        {
            return Regex.IsMatch(Uri, @"^\\(\\[^\s\\\/\:*?<>|]+)+$", RegexOptions.IgnoreCase);
        }

		private void ValidateUriIsNotEmpty(PluginProfileErrorCollection errors)
		{
			if (string.IsNullOrEmpty(Uri))
			{
				errors.Add(new PluginProfileError { FieldName = UriField, Message = "Uri should not be empty." });
			}
		}

		private void ValidateUserMapping(PluginProfileErrorCollection errors)
		{
			if (UserMapping.Select(x => x.Key.ToLower()).Distinct().Count() != UserMapping.Count)
			{
				errors.Add(new PluginProfileError {FieldName = "user-mapping", Message = "Can't map an mercurial user to TargetProcess user twice."});
			}
		}

		#endregion
	}
}