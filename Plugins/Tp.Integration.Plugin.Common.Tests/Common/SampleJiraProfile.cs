// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Tp.Integration.Messages.ComponentModel;
using Tp.Integration.Messages.Ticker;

namespace Tp.Integration.Plugin.Common.Tests.Common
{
    [Serializable]
    public class SampleJiraProfile : ICloneable, ISynchronizableProfile
    {
        private readonly Dictionary<string, SerializableStringDictionary> _maps =
            new Dictionary<string, SerializableStringDictionary>();

        private string _assigneeRole = "Developer";
        private const string FILTER_KEY = "FILTER";
        private string _jiraLogin;
        private string _jiraPassword;
        private string _jiraUrl = "http://";
        private int _projectID = int.MinValue;
        private string _reporterRole = "QA Engineer";
        private int _synchronizationInterval = 5;
        private const string SOAP_SERVICE_V2 = "/rpc/soap/jirasoapservice-v2";

        private string _key;

        public SampleJiraProfile()
        {
            _maps.Add("Severities", new SerializableStringDictionary());
            _maps.Add("TransitionEntityStates", new SerializableStringDictionary());
            _maps.Add("Users", new SerializableStringDictionary());
            _maps.Add("CustomFields", new SerializableStringDictionary());
        }

        [Browsable(false)]
        [XmlIgnore]
        public string JiraSoapService
        {
            get { return _jiraUrl + SOAP_SERVICE_V2; }
        }

        [Category("Jira Connection")]
        [Description(@"Ex: http://Jira.mysite.com")]
        public string JiraUrl
        {
            get { return _jiraUrl; }
            set { _jiraUrl = value; }
        }

        [Browsable(false)]
        [XmlIgnore]
        public string FilterName
        {
            get
            {
                if (string.IsNullOrEmpty(Key))
                    return null;
                var keyParts = Key.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (keyParts.Length > 1 && keyParts[0].ToUpper() == FILTER_KEY)
                    return keyParts[1];
                return null;
            }
        }


        [Category("Jira Connection")]
        [Description(@"Specify sync interval in minutes (short intervals may reduce system performance)")]
        public int SynchronizationInterval
        {
            get { return _synchronizationInterval; }
            set
            {
                if (value < 5)
                {
                    throw new ApplicationException("The synchronization interval must not be less than 5 minutes.");
                }
                _synchronizationInterval = value;
            }
        }

        [Category("Jira Authentication")]
        public string JiraLogin
        {
            get { return _jiraLogin; }
            set { _jiraLogin = value; }
        }

        [Category("Jira Authentication")]
        public string JiraPassword
        {
            get { return _jiraPassword; }
            set { _jiraPassword = value; }
        }

        [Category("Project Mapping")]
        [Description(
             @"Specify the project you want to synchronize with Jira. Bugs from Jira will be added to this project.")]
        public int ProjectID
        {
            set { _projectID = value; }
            get { return _projectID; }
        }

        [Browsable(false)]
        [XmlIgnore]
        public string ProjectKey
        {
            get
            {
                if (string.IsNullOrEmpty(Key))
                    return null;
                var keyParts = Key.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (keyParts.Length == 1)
                    return keyParts[0];
                return null;
            }
        }

        [Category("Project Mapping")]
        [Description(
             @"Specify Jira key to retrieve issues. The key can be Jira project key or saved filter. To specify the filter use 'Filter:' prefix (Ex: Filter:MyFilter)."
         )]
        public string Key
        {
            set { _key = value; }
            get { return _key; }
        }

        [Browsable(false)]
        public string Maps
        {
            set
            {
                var reader = new StringReader(value);

                foreach (var map in _maps)
                {
                    ReadMap(map.Value, reader, map.Key);
                }
            }
            get
            {
                var builder = new StringBuilder();

                foreach (var map in _maps)
                {
                    WriteMap(builder, map.Key, map.Value);
                }

                return builder.ToString();
            }
        }


        [Category("Custom Fields Mapping")]
        [Description("(optional) Jira-TargetProcess custom fields mapping. Use names of custom fields.")]
        [XmlIgnore]
        public SerializableStringDictionary CustomFields
        {
            get { return _maps["CustomFields"]; }
            set { _maps["CustomFields"] = value; }
        }

        [Category("Severities Mapping")]
        [Description(
             @"Specify Jira-TargetProcess severities mapping. Use names of priorities for Jira and names of severities for TargetProcess (Ex: Blocker -> Blocking)."
         )]
        [XmlIgnore]
        public SerializableStringDictionary Severities
        {
            get { return _maps["Severities"]; }
            set { _maps["Severities"] = value; }
        }

        [Category("States Mapping")]
        [Description(
             @"Specify Jira-TargetProcess transitions and states. Use names of transition and state for Jira (Ex: Resolve Issue,Resolved -> Fixed).
			  The transition name is required for TargetProcess-Jira states syncrhornization. Note that TargetProcess workflow must be the same as for Jira for correct TargetProcess-to-Jira states syncrhornization."
         )]
        [XmlIgnore]
        public SerializableStringDictionary TransitionEntityStates
        {
            get { return _maps["TransitionEntityStates"]; }
            set { _maps["TransitionEntityStates"] = value; }
        }

        [Browsable(false)]
        [XmlIgnore]
        public SerializableStringDictionary EntityStates
        {
            get
            {
                var dictionary = new SerializableStringDictionary();
                foreach (var entry in TransitionEntityStates)
                {
                    string jiraState;
                    var keyValue = entry.Key.Split(new[] { ',' });
                    if (keyValue.Length > 1)
                        jiraState = keyValue[1];
                    else
                        jiraState = keyValue[0];
                    if (!string.IsNullOrEmpty(jiraState))
                        dictionary[jiraState.Trim()] = entry.Value;
                }
                return dictionary;
            }
        }

        [Browsable(false)]
        [XmlIgnore]
        public SerializableStringDictionary JiraTransitions
        {
            get
            {
                var dictionary = new SerializableStringDictionary();
                foreach (string stateSettings in TransitionEntityStates.Keys)
                {
                    var keyValue = stateSettings.Split(new[] { ',' });
                    if (keyValue.Length > 1)
                        dictionary[keyValue[1].Trim()] = keyValue[0];
                }
                return dictionary;
            }
        }


        [Category("Users Mapping")]
        [Description(
             @"Specify Jira-to-TargetProcess users. 
			Required to correct bugs' comments sync and assigned users sync."
         )]
        [XmlIgnore]
        public SerializableStringDictionary Users
        {
            get { return _maps["Users"]; }
            set { _maps["Users"] = value; }
        }

        [Category("Users Mapping")]
        [Description(
             @"Specify default TargetProcess user. 
			All unmapped Jira users will be assigned to this TargetProcess user."
         )]
        public string DefaultTargetProcessUser { get; set; }

        [Category("Roles Mapping")]
        [Description(@"Specify TargetProcess role name who relates to Assignee role in Jira (Ex: Developer(s)).")]
        public string AssigneeRole
        {
            get { return _assigneeRole; }
            set { _assigneeRole = value; }
        }


        [Category("Roles Mapping")]
        [Description(
             @"Specify TargetProcess role name who relates to Reporter role in Jira (Ex: Verifier).")]
        public string ReporterRole
        {
            get { return _reporterRole; }
            set { _reporterRole = value; }
        }


        [Category("Jira Properties Mapping")]
        [Description(@"Specify TargetProcess custom field name.")]
        public string AffectsVersion { get; set; }

        [Category("Jira Properties Mapping")]
        [Description(@"Specify TargetProcess custom field name.")]
        public string FixVersions { get; set; }

        [Category("Jira Properties Mapping")]
        [Description(@"Specify TargetProcess custom field name.")]
        public string DueDate { get; set; }

        [Category("Jira Properties Mapping")]
        [Description(@"Specify TargetProcess custom field name.")]
        public string Environment { get; set; }


        [Category("Jira Properties Mapping")]
        [Description(@"Specify TargetProcess custom field name.")]
        public string Components { get; set; }

        #region ICloneable Members

        public object Clone()
        {
            return new SampleJiraProfile
            {
                JiraLogin = JiraLogin,
                JiraPassword = JiraPassword,
                JiraUrl = JiraUrl,
                AssigneeRole = AssigneeRole,
                ReporterRole = ReporterRole,
                Maps = Maps,
                SynchronizationInterval = SynchronizationInterval,
                AffectsVersion = AffectsVersion,
                FixVersions = FixVersions,
                DueDate = DueDate,
                Environment = Environment,
                Components = Components
            };
        }

        #endregion

        //TODO: Maybe move this into Utils? It is quite general method
        private static void ReadMap(SerializableStringDictionary map, TextReader reader, string mapName)
        {
            map.Clear();

            var mappings =
                reader.ReadLine().Replace(string.Format("{0}:", mapName), string.Empty).Split(new[] { ';' },
                    StringSplitOptions.
                        RemoveEmptyEntries);

            foreach (string mapping in mappings)
            {
                var parts = mapping.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                map.Add(parts[0], parts[1]);
            }
        }

        //TODO: Maybe move this into Utils? It is quite general method
        private static void WriteMap(StringBuilder builder, string mapName, SerializableStringDictionary map)
        {
            builder.AppendFormat("{0}:", mapName);

            foreach (var keyValuePair in map)
            {
                builder.AppendFormat("{0}:{1};", keyValuePair.Key, keyValuePair.Value);
            }

            builder.AppendLine();
        }

        public override string ToString()
        {
            var value = string.Format("Jira Integration Settings: TargetProcess ProjectID='{0}'; Jira Url='{1}';", ProjectID,
                JiraUrl);
            if (string.IsNullOrEmpty(FilterName))
                value += string.Format(" Jira ProjectKey='{0}'", ProjectKey);
            else
                value += string.Format(" Jira FilterName='{0}';", FilterName);
            return value;
        }
    }
}
