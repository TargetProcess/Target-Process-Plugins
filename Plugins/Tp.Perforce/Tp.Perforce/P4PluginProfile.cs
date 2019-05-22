using System;
using System.Linq;
using System.Runtime.Serialization;
using Tp.Integration.Messages.Ticker;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Mapping;
using Tp.Integration.Plugin.Common.Validation;
using Tp.SourceControl.Settings;

namespace Tp.Perforce
{
    [Profile]
    [DataContract]
    public class P4PluginProfile : ConnectionSettings, ISynchronizableProfile, IValidatable
    {
        public const string WorkspaceField = "Workspace";

        public const string StartRevisionField = "StartRevision";

        public P4PluginProfile()
        {
            UserMapping = new MappingContainer();
            StartRevision = string.Empty;
        }

        private string _workspace;

        [DataMember]
        public string Workspace
        {
            get => _workspace;
            set => _workspace = value.Trim();
        }

        [IgnoreDataMember]
        public int SynchronizationInterval
        {
            get { return 5; }
            set { }
        }

        [IgnoreDataMember]
        public bool RevisionSpecified => !StartRevision.IsNullOrWhitespace();

        #region Validation

        public bool ValidateStartRevision(PluginProfileErrorCollection errors)
        {
            return StartRevisionShouldNotBeEmpty(errors) && StartRevisionShouldBeNumber(errors) && StartRevisionShouldBeNonNegative(errors);
        }

        private bool StartRevisionShouldNotBeEmpty(PluginProfileErrorCollection errors)
        {
            if (!RevisionSpecified)
            {
                errors.Add(new PluginProfileError { FieldName = StartRevisionField, Message = "Start Revision should not be empty." });
                return false;
            }

            return true;
        }

        private bool StartRevisionShouldBeNonNegative(PluginProfileErrorCollection errors)
        {
            int revisionNumber = int.Parse(StartRevision);
            if (revisionNumber < 0)
            {
                errors.Add(new PluginProfileError { FieldName = StartRevisionField, Message = "Start Revision cannot be less than zero." });
                return false;
            }
            return true;
        }

        private bool StartRevisionShouldBeNumber(PluginProfileErrorCollection errors)
        {
            if (!int.TryParse(StartRevision, out _))
            {
                errors.Add(new PluginProfileError { FieldName = StartRevisionField, Message = "Start Revision should be a number." });
                return false;
            }
            return true;
        }

        public void Validate(PluginProfileErrorCollection errors)
        {
            ValidateStartRevision(errors);
            ValidateUserMapping(errors);
            ValidateUri(errors);
        }

        public void ValidateUri(PluginProfileErrorCollection errors)
        {
            ValidateUriIsNotEmpty(errors);
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
                errors.Add(new PluginProfileError
                { FieldName = "UserMapping", Message = "Can't map an svn user to TargetProcess user twice." });
            }
        }

        #endregion
    }
}
