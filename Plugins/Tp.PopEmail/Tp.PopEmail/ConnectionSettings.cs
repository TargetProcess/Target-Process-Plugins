//
// Copyright (c) 2005-2020 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
//

using System;
using System.Runtime.Serialization;
using Tp.Integration.Plugin.Common.PluginCommand.Embedded;
using Tp.Integration.Plugin.Common.Validation;
using Tp.PopEmailIntegration.EmailReader;

namespace Tp.PopEmailIntegration
{
    public enum SecureAccessMethods
    {
        LoginAndPassword,
        GoogleOAuth,
        OutlookOAuth,
    }

    [Serializable, DataContract]
    public class OAuthState
    {
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public Uri TokenEndpoint { get; set; }
        [DataMember]
        public string Callback { get; set; }
        [DataMember]
        public string AccessToken { get; set; }
        [DataMember]
        public string RefreshToken { get; set; }
        [DataMember]
        public DateTime? AccessTokenExpirationUtc { get; set; }
        [DataMember]
        public DateTime? AccessTokenIssueDateUtc { get; set; }
        [DataMember]
        public string Scope { get; set; }
        [DataMember]
        public bool IsDeleted { get; set; }
    }

    [Serializable, DataContract]
    public class ConnectionSettings : IValidatable
    {
        public const string ProtocolField = "Protocol";
        public const string MailServerField = "MailServer";
        public const string PortField = "Port";
        public const string LoginField = "Login";
        public const string PasswordField = "Password";
        public const string SignInField = "SignIn";

        [DataMember]
        public SecureAccessMethods SecureAccessMethod { get; set; }

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
        [SecretMember]
        public string Password { get; set; }

        private bool? _hasPassword;
        [DataMember]
        public bool HasPassword
        {
            get => _hasPassword ?? !Password.IsNullOrEmpty();
            set => _hasPassword = value;
        }

        [DataMember]
        public Uri OAuthDiscoverUri { get; set; }

        [DataMember]
        public OAuthState OAuthState { get; set; }

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
            ValidateOAuthState(errors);
        }

        private void ValidatePassword(PluginProfileErrorCollection errors)
        {
            if (Password.IsNullOrWhitespace())
            {
                errors.Add(new PluginProfileError
                {
                    FieldName = PasswordField,
                    Message = $"{(SecureAccessMethod == SecureAccessMethods.LoginAndPassword ? "Password" : "Client secret")} should not be empty"
                });
            }
        }

        private void ValidateLogin(PluginProfileErrorCollection errors)
        {
            if (Login.IsNullOrWhitespace())
            {
                errors.Add(new PluginProfileError
                {
                    FieldName = LoginField,
                    Message = $"{(SecureAccessMethod == SecureAccessMethods.LoginAndPassword ? "Login" : "Client id")} should not be empty"
                });
            }
        }

        private void ValidatePort(PluginProfileErrorCollection errors)
        {
            if (Port <= 0)
            {
                errors.Add(new PluginProfileError { FieldName = PortField, Message = "Port should not be empty" });
            }
        }

        private void ValidateMailServer(PluginProfileErrorCollection errors)
        {
            if (MailServer.IsNullOrWhitespace())
            {
                errors.Add(new PluginProfileError { FieldName = MailServerField, Message = "Server should not be empty" });
            }
        }

        private void ValidateProtocol(PluginProfileErrorCollection errors)
        {
            if (Protocol.IsNullOrWhitespace())
            {
                errors.Add(new PluginProfileError { FieldName = ProtocolField, Message = "Protocol should not be empty" });
            }
        }

        private void ValidateOAuthState(PluginProfileErrorCollection errors)
        {
            if (SecureAccessMethod != SecureAccessMethods.LoginAndPassword)
            {
                if (OAuthState == null)
                    errors.Add(new PluginProfileError { FieldName = SignInField, Message = "OAuth client should be authorized" });
                else if (string.IsNullOrEmpty(OAuthState.RefreshToken))
                    errors.Add(new PluginProfileError
                    {
                        FieldName = SignInField,
                        Message =
                            $"The refresh_token is only provided on the first authorization from {(string.IsNullOrEmpty(OAuthState.Email) ? "the user" : OAuthState.Email)}. Remove access for your app from the Third-party apps and try again."
                    });
            }
        }

        #endregion

        public override string ToString()
        {
            return $"Protocol={Protocol}, MailServer='{MailServer}', Port={Port}, Ssl={(UseSSL ? "Yes" : "No")}, Login='{Login}'";
        }
    }
}
