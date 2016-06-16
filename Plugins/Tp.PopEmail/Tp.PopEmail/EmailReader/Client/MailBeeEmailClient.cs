//
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
//

using System;
using System.IO;
using MailBee;
using MailBee.BounceMail;
using MailBee.Mime;
using MailBee.Security;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Validation;
using Tp.PopEmailIntegration.EmailReader.Client.MailBee;

namespace Tp.PopEmailIntegration.EmailReader.Client
{
	public class MailBeeEmailClient : IEmailClient
	{
		public const string MAILBEE_LICENSE_KEY = "MN600-1AD2EDF3D215D284D25F423AC217-B912";

		private readonly ConnectionSettings _settings;
		private readonly IMailBeeClient _client;

		public MailBeeEmailClient(IStorageRepository storageRepository)
			: this(storageRepository.GetProfile<ProjectEmailProfile>())
		{
		}

		public MailBeeEmailClient(ConnectionSettings settings)
		{
			_settings = settings;
			_client = settings.GetEmailProtocol() == EmailProtocol.Imap
				? (IMailBeeClient) new ImapMailBeeClient()
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

		private static void AddCredentialsErrors(PluginProfileErrorCollection errors)
		{
			errors.Add(new PluginProfileError { FieldName = ConnectionSettings.LoginField, Message = "Check credentials" });
			errors.Add(new PluginProfileError { FieldName = ConnectionSettings.PasswordField, Message = "Check credentials" });
		}

		private static void AddConnectionErrors(PluginProfileErrorCollection errors)
		{
			errors.Add(new PluginProfileError { FieldName = ConnectionSettings.PortField, Message = "Check connection settings" });
			errors.Add(new PluginProfileError
			{ FieldName = ConnectionSettings.MailServerField, Message = "Check connection settings" });
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

			_client.Connect(_settings.MailServer, _settings.Port, startupMode);

			return _client.IsConnected;
		}

		private bool TryLogin()
		{
			ConnectInternal();

			if (_client.IsLoggedIn)
			{
				return true;
			}

			var authMode = _settings.UseSSL ? AuthenticationMethods.Auto : AuthenticationMethods.Regular;

			_client.LogIn(_settings.Login, _settings.Password, authMode);

			return _client.IsLoggedIn;
		}

		public MailMessage DownloadMessage(string uid)
		{
			return _client.DownloadMessage(uid);
		}

		public bool IsDsnMessage(MailMessage message)
		{
			var parser = new DeliveryStatusParser(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"BounceDatabase\all.xml"), true);

			var result = parser.Process(message);
			return result != null;
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
