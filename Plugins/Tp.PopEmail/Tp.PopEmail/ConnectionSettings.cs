//
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
//

using System;
using System.Runtime.Serialization;
using System.Text;
using Tp.Integration.Plugin.Common.Validation;
using Tp.PopEmailIntegration.EmailReader;

namespace Tp.PopEmailIntegration
{
    [Serializable, DataContract]
    public class ConnectionSettings : IValidatable
    {
        public const string ProtocolField = "Protocol";
        public const string MailServerField = "MailServer";
        public const string PortField = "Port";
        public const string LoginField = "Login";
        public const string PasswordField = "Password";

        [DataMember]
        public string Protocol { get; set; }

        [DataMember]
        public string MailServer { get; set; }

        private int _port = 110;

        [DataMember]
        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        [DataMember]
        public bool UseSSL { get; set; }

        [DataMember]
        public string Login { get; set; }

        [DataMember]
        public string Password { get; set; }

        public EmailProtocol GetEmailProtocol()
        {
            switch (Protocol.Trim().ToLowerInvariant())
            {
                case "pop":
                case "pop3":
                    return EmailProtocol.Pop3;
                case "imap":
                case "imap4":
                    return EmailProtocol.Imap;
                default:
                    throw new EmailException($"Protocol '{Protocol}' not recognized");
            }
        }

        #region Validation

        public virtual void Validate(PluginProfileErrorCollection errors)
        {
            ValidateConnection(errors);
        }

        public void ValidateConnection(PluginProfileErrorCollection errors)
        {
            ValidateProtocol(errors);
            ValidateMailServer(errors);
            ValidatePort(errors);
            ValidateLogin(errors);
            ValidatePassword(errors);
        }

        private void ValidatePassword(PluginProfileErrorCollection errors)
        {
            if (Password.IsNullOrWhitespace())
            {
                errors.Add(new PluginProfileError { FieldName = "Password", Message = "Password should not be empty" });
            }
        }

        private void ValidateLogin(PluginProfileErrorCollection errors)
        {
            if (Login.IsNullOrWhitespace())
            {
                errors.Add(new PluginProfileError { FieldName = "Login", Message = "Login should not be empty" });
            }
        }

        private void ValidatePort(PluginProfileErrorCollection errors)
        {
            if (Port <= 0)
            {
                errors.Add(new PluginProfileError { FieldName = "Port", Message = "Port should not be empty" });
            }
        }

        private void ValidateMailServer(PluginProfileErrorCollection errors)
        {
            if (MailServer.IsNullOrWhitespace())
            {
                errors.Add(new PluginProfileError { FieldName = "MailServer", Message = "Server should not be empty" });
            }
        }

        private void ValidateProtocol(PluginProfileErrorCollection errors)
        {
            if (Protocol.IsNullOrWhitespace())
            {
                errors.Add(new PluginProfileError { FieldName = "Protocol", Message = "Protocol should not be empty" });
            }
        }

        #endregion

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendFormat("Protocol={0}", Protocol);
            builder.AppendFormat(", MailServer='{0}'", MailServer);
            builder.AppendFormat(", Port={0}", Port);
            builder.AppendFormat(", Ssl={0}", UseSSL ? "Yes" : "No");
            builder.AppendFormat(", Login='{0}'", Login);
            return builder.ToString();
        }
    }
}
