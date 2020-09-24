//
// Copyright (c) 2005-2020 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
//

using System;
using System.Threading;
using MailBee;
using MailBee.Mime;
using MailBee.Security;
using StructureMap;
using System.ComponentModel;
using System.Linq;
using DotNetOpenAuth.OAuth2;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Validation;
using Tp.PopEmailIntegration.EmailReader.Client.MailBee;

namespace Tp.PopEmailIntegration.EmailReader.Client
{
    public class MailBeeEmailClient : IEmailClient
    {
        private readonly IActivityLogger _log;

        public const string MAILBEE_LICENSE_KEY = "MN120-1AD2EDF3D215D284D25F423AC217-A00B";

        private readonly ConnectionSettings _settings;
        private IMailBeeClient _client;
        private readonly IDeliveryStatusMessageParser _statusMessageParser;

        public MailBeeEmailClient(IStorageRepository storageRepository)
            : this(storageRepository.GetProfile<ProjectEmailProfile>())
        {
        }

        public MailBeeEmailClient(ConnectionSettings settings)
        {
            _settings = settings;
            _client = CreateClient(settings);
            _log = ObjectFactory.GetInstance<IActivityLogger>();
            _statusMessageParser = ObjectFactory.GetInstance<IDeliveryStatusMessageParser>();
        }

        private static IMailBeeClient CreateClient(ConnectionSettings settings)
        {
            return settings.GetEmailProtocol() == EmailProtocol.Imap
                ? (IMailBeeClient)new ImapMailBeeClient()
                : new Pop3MailBeeClient();
        }

        private void ConnectInternal()
        {
            if (!TryConnect())
            {
                throw new EmailException($"Cannot connect: {_client.GetErrorDescription()}; Connection settings: {_settings}");
            }
        }

        private void LoginInternal()
        {
            if (!TryLogin())
            {
                throw new EmailException($"Cannot login: {_client.GetErrorDescription()}; Connection settings: {_settings}");
            }
        }

        public string[] GetServerUids()
        {
            return _client.GetMessageUids();
        }

        public void CheckConnection(PluginProfileErrorCollection errors)
        {
            if (!TryConnect())
            {
                AddConnectionErrors(errors);
            }
            else if (!TryLogin())
            {
                AddCredentialsErrors(errors);
            }
        }

        private void AddCredentialsErrors(PluginProfileErrorCollection errors)
        {
            var serverError = _client.GetErrorDescription();
            if (!string.IsNullOrEmpty(serverError))
            {
                errors.Add(new PluginProfileError { FieldName = ConnectionSettings.MailServerField, Message = serverError });
            }
            errors.Add(new PluginProfileError { FieldName = ConnectionSettings.LoginField, Message = "Check credentials" });
            errors.Add(new PluginProfileError { FieldName = ConnectionSettings.PasswordField, Message = "Check credentials" });
        }

        private void AddConnectionErrors(PluginProfileErrorCollection errors)
        {
            var serverError = _client.GetErrorDescription();
            errors.Add(new PluginProfileError { FieldName = ConnectionSettings.PortField, Message = "Check connection settings" });
            errors.Add(new PluginProfileError
                { FieldName = ConnectionSettings.MailServerField, Message = string.IsNullOrEmpty(serverError) ? "Check connection settings" : serverError });
        }

        private bool TryConnect()
        {
            if (_client.IsConnected)
            {
                return true;
            }

            SslStartupMode? startupMode = null;

            if (_settings.UseSSL)
            {
                startupMode = _settings.Port == 110 ? SslStartupMode.UseStartTls : SslStartupMode.OnConnect;
            }

            try
            {
                _client.Connect(_settings.MailServer, _settings.Port, startupMode);
            }
            catch (Exception e)
            {
                if (!(e.InnerException is Win32Exception))
                    throw;

                _log.Info($"Connect error: {e.InnerException.Message}. Reconnect");
                _client.Dispose();
                Thread.Sleep(TimeSpan.FromSeconds(1));
                _client = CreateClient(_settings);
                _client.Connect(_settings.MailServer, _settings.Port, startupMode);
            }

            return _client.IsConnected;
        }

        private bool TryLogin()
        {
            ConnectInternal();

            if (_client.IsLoggedIn)
            {
                return true;
            }

            AuthorizationState authState = null;

            var authMode = _settings.SecureAccessMethod != SecureAccessMethods.LoginAndPassword ? AuthenticationMethods.SaslOAuth2 :
                _settings.UseSSL ? AuthenticationMethods.Auto : AuthenticationMethods.Regular;

            try
            {
                if (_settings.SecureAccessMethod != SecureAccessMethods.LoginAndPassword)
                {
                    if (_settings.OAuthState == null)
                    {
                        throw new ApplicationException("OAuthState should not be empty");
                    }

                    if (string.IsNullOrEmpty(_settings.OAuthState.RefreshToken))
                    {
                        throw new ApplicationException(
                            $"The refresh_token is only provided on the first authorization from {(string.IsNullOrEmpty(_settings.OAuthState.Email) ? "the user" : _settings.OAuthState.Email)}. Remove access for your app from the Third-party apps and try again.");
                    }

                    if (_settings.OAuthState.IsDeleted)
                    {
                        _log.Warn("OAuth AccessToken was deleted");
                        return false;
                    }

                    authState = new AuthorizationState(_settings.OAuthState.Scope
                        .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToHashSet())
                    {
                        AccessToken = _settings.OAuthState.AccessToken,
                        RefreshToken = _settings.OAuthState.RefreshToken,
                        AccessTokenIssueDateUtc = _settings.OAuthState.AccessTokenIssueDateUtc,
                        AccessTokenExpirationUtc = _settings.OAuthState.AccessTokenExpirationUtc
                    };

                    var refreshedAccessToken = RefreshAccessToken(_settings.OAuthState.TokenEndpoint, _settings.Login, _settings.Password, authState);

                    if (refreshedAccessToken != null)
                    {
                        authState = refreshedAccessToken;
                        var profile = ObjectFactory.GetInstance<IProfile>();
                        if (!profile.IsNull)
                        {
                            var profileSettings = (ProjectEmailProfile)profile.Settings;
                            profileSettings.OAuthState.AccessToken = authState.AccessToken;
                            profileSettings.OAuthState.RefreshToken = authState.RefreshToken;
                            profileSettings.OAuthState.AccessTokenIssueDateUtc = authState.AccessTokenIssueDateUtc;
                            profileSettings.OAuthState.AccessTokenExpirationUtc = authState.AccessTokenExpirationUtc;
                            profileSettings.OAuthState.IsDeleted = authState.IsDeleted;
                            profile.Save();
                        }
                        if (authState.IsDeleted)
                        {
                            _log.Warn("OAuth AccessToken was deleted");
                            return false;
                        }
                        _log.Info("OAuth AccessToken refreshed");
                    }
                    _client.LogIn(null, OAuth2.GetXOAuthKeyStatic(_settings.OAuthState.Email, authState.AccessToken), authMode);
                }
                else
                {
                    _client.LogIn(_settings.Login, _settings.Password, authMode);
                }
            }
            catch (Exception e)
            {
                if (!(e.InnerException is Win32Exception))
                    throw;

                _log.Info($"Login error: {e.InnerException.Message}. Re-login");
                _client.Dispose();
                Thread.Sleep(TimeSpan.FromSeconds(1));

                SslStartupMode? startupMode = null;

                if (_settings.UseSSL)
                {
                    startupMode = _settings.Port == 110 ? SslStartupMode.UseStartTls : SslStartupMode.OnConnect;
                }

                _client = CreateClient(_settings);
                _client.Connect(_settings.MailServer, _settings.Port, startupMode);
                if (authState != null)
                    _client.LogIn(null, OAuth2.GetXOAuthKeyStatic(_settings.OAuthState.Email, authState.AccessToken), authMode);
                else
                    _client.LogIn(_settings.Login, _settings.Password, authMode);
            }

            return _client.IsLoggedIn;
        }

        private static AuthorizationState RefreshAccessToken(Uri tokenEndpoint, string clientID, string clientSecret, AuthorizationState grantedAccess)
        {
            return new UserAgentClient(new AuthorizationServerDescription { TokenEndpoint = tokenEndpoint, ProtocolVersion = ProtocolVersion.V20 }, clientID, clientSecret)
            {
                ClientCredentialApplicator = ClientCredentialApplicator.PostParameter(clientSecret)
            }.RefreshAuthorization(grantedAccess, TimeSpan.FromMinutes(5)) ? grantedAccess : null;
        }

        public MailMessage DownloadMessage(string uid)
        {
            return _client.DownloadMessage(uid);
        }

        public bool IsDsnMessage(MailMessage message)
        {
            return _statusMessageParser.IsDsnMessage(message);
        }

        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            GC.SuppressFinalize(this);
            _client.Dispose();
        }

        public void Connect()
        {
            try
            {
                ConnectInternal();
            }
            catch (EmailException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new EmailException("Cannot connect", ex);
            }
        }

        public void Login()
        {
            try
            {
                LoginInternal();
            }
            catch (EmailException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new EmailException("Cannot login", ex);
            }
        }

        public void Disconnect()
        {
            try
            {
                if (_client.IsConnected)
                {
                    _client.Disconnect();
                }
            }
            catch (EmailException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new EmailException("Cannot disconnect", ex);
            }
        }
    }
}
