// 
// Copyright (c) 2005-2019 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Tp.Integration.Messages.Ticker;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Mapping;
using Tp.Integration.Plugin.Common.Validation;
using Tp.Tfs.WorkItemsIntegration;

namespace Tp.Tfs
{
    [Profile]
    [DataContract]
    public class TfsPluginProfile : TfsConnectionSettings, ISynchronizableProfile, IValidatable
    {
        public const string StartRevisionField = "StartRevision";
        public const string StartWorkItemField = "StartWorkItem";
        public const string SynchronizationIntervalField = "SynchronizationInterval";

        public TfsPluginProfile()
        {
            UserMapping = new MappingContainer();
            EntityMapping = new SimpleMappingContainer();
            ProjectsMapping = new MappingContainer();
            SourceControlEnabled = true;
            WorkItemsEnabled = false;
            StartRevision = "1";
            StartWorkItem = "1";
        }

        [DataMember]
        public string SyncInterval { get; set; }

        private const int MinimumSynchronizationIntervalInMinutes = 5;

        [IgnoreDataMember]
        public int SynchronizationInterval
        {
            get
            {
                int.TryParse(SyncInterval, out var value);
                return Math.Max(MinimumSynchronizationIntervalInMinutes, value);
            }
            set { }
        }

        #region Validation

        public void ValidateStartRevision(PluginProfileErrorCollection errors)
        {
            if (!Int32.TryParse(StartRevision, out var startRevision) || startRevision < 1)
            {
                errors.Add(new PluginProfileError
                {
                    FieldName = StartRevisionField,
                    Message = $"Specify a start revision number in the range of 1 - {Int32.MaxValue}",
                    Status = PluginProfileErrorStatus.WrongRevisionNumberError
                });
            }
        }

        public void ValidateStartWorkItem(PluginProfileErrorCollection errors)
        {
            if (!Int32.TryParse(StartWorkItem, out var startWorkItem) || startWorkItem < 1)
            {
                errors.Add(new PluginProfileError
                {
                    FieldName = StartWorkItemField,
                    Message = $"Specify a start workitem number in the range of 1 - {Int32.MaxValue}",
                    Status = PluginProfileErrorStatus.WrongRevisionNumberError
                });
            }
        }

        public void Validate(PluginProfileErrorCollection errors)
        {
            ValidateUri(errors);
            ValidateStartRevision(errors);
            ValidateUserMapping(errors);

            if (WorkItemsEnabled)
                ValidateStartWorkItem(errors);
        }

        public void ValidateUri(PluginProfileErrorCollection errors)
        {
            ValidateUriIsNotEmpty(errors);
            ValidateUriFormat(errors);
        }

        private void ValidateUriFormat(PluginProfileErrorCollection errors)
        {
            if (!string.IsNullOrEmpty(Uri) && !IsTfsUri())
            {
                errors.Add(IsSshUri()
                    ? new PluginProfileError { Message = "Connection via SSH is not supported.", FieldName = UriField }
                    : new PluginProfileError { Message = "Wrong URI format.", FieldName = UriField });
            }
        }

        private bool IsSshUri()
        {
            return Regex.IsMatch(Uri, @"^ssh://(.+@)?([\w\d\.]+)(:\d+)?/(~\S+)?(/\S)*$", RegexOptions.IgnoreCase)
                || Regex.IsMatch(Uri,
                    @"^(?!file:)(?!http:)(?!https:)(?!ftp:)(?!ftps:)(?!rsync:)(?!git:)(?!ssh:)(.+@)?[\w\d]+[\.][\w\d]+:(/~.+/)?\S*$",
                    RegexOptions.IgnoreCase);
        }

        private bool IsTfsUri()
        {
            return Regex.IsMatch(Uri,
                @"^((?#protocol)https?)://((?#domain)[-A-Z0-9.]+)((?#port):\d{1,5})((?#file)/[-A-Z0-9+&@#/%= ~_|!:,.;]+)$",
                RegexOptions.IgnoreCase);
        }

        private void ValidateUriIsNotEmpty(PluginProfileErrorCollection errors)
        {
            if (string.IsNullOrEmpty(Uri))
            {
                errors.Add(new PluginProfileError { FieldName = UriField, Message = "URI should not be empty." });
            }
        }

        private void ValidateUserMapping(PluginProfileErrorCollection errors)
        {
            if (UserMapping.Select(x => x.Key.ToLower()).Distinct().Count() != UserMapping.Count)
            {
                errors.Add(new PluginProfileError
                {
                    FieldName = "user-mapping",
                    Message = "Can't map a TFS user to TargetProcess user twice."
                });
            }
        }

        #endregion
    }
}
