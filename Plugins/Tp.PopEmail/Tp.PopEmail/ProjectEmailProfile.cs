//
// Copyright (c) 2005-2020 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
//

using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Messages.Ticker;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Validation;
using Tp.PopEmailIntegration.Rules;

namespace Tp.PopEmailIntegration
{
    [Serializable, Profile, DataContract]
    public class ProjectEmailProfile : ConnectionSettings, ISynchronizableProfile, IRuleHandler
    {
        public const string RulesField = "Rules";

        [IgnoreDataMember]
        public int SynchronizationInterval => 5;

        [DataMember]
        public string Rules { get; set; }

        public bool IsMessageForCurrentEmailAccount(MessageDTO messageDto)
        {
            return messageDto.MessageUidDto != null && messageDto.MessageUidDto.MailServer == MailServer &&
                messageDto.MessageUidDto.MailLogin == Login;
        }

        public override void Validate(PluginProfileErrorCollection errors)
        {
            base.Validate(errors);
            ValidateRules(errors);
        }

        private void ValidateRules(PluginProfileErrorCollection errors)
        {
            if (Rules.IsNullOrWhitespace())
            {
                errors.Add(new PluginProfileError { FieldName = RulesField, Message = "Rules should not be empty" });
            }
            else
            {
                ValidateRulesFormat(errors);
            }
        }

        private void ValidateRulesFormat(PluginProfileErrorCollection errors)
        {
            var parser = ObjectFactory.GetInstance<RuleParser>();
            var parsed = parser.Parse(this);

            var stringRules = RuleParser.GetRuleLines(this);
            if (parsed.Count() != stringRules.Count())
            {
                errors.Add(new PluginProfileError { FieldName = RulesField, Message = "Invalid rules format" });
            }
        }

        [IgnoreDataMember]
        public string DecodedRules => HttpUtility.UrlDecode(Rules, Encoding.UTF7);

        [DataMember]
        public bool UsersMigrated { get; set; }
    }

    public interface IRuleHandler
    {
        string DecodedRules { get; }
    }
}
