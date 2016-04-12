// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.IO;
using MailBee;
using MailBee.BounceMail;
using MailBee.Mime;
using MailBee.Pop3Mail;
using MailBee.Security;
using Tp.Integration.Plugin.Common.Storage;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Validation;

namespace Tp.PopEmailIntegration.EmailReader.Client
{
	public class MailBeePop3EmailClient : IEmailClient
	{
		private readonly ConnectionSettings _settings;
		private Pop3 _client;
		private const string MAILBEE_LICENSE_KEY = "MN600-1AD2EDF3D215D284D25F423AC217-B912";

		public MailBeePop3EmailClient(IStorageRepository storageRepository)
			: this(storageRepository.GetProfile<ProjectEmailProfile>())
		{
		}

		public MailBeePop3EmailClient(ConnectionSettings settings)
		{
			_settings = settings;

			Pop3.LicenseKey = MAILBEE_LICENSE_KEY;

			// Make sure to disable throwing exception explicitly. Otherwise strange things might happen. See bug #5748 for details.
			// So please don't change this option unless you know what you are doing!!!
			_client = new Pop3 {ThrowExceptions = false};
		}

		private void ConnectInternal()
		{
			if (!TryConnect())
			{
				throw new EmailException(String.Format("Cannot connect: {0}; Connection settings: {1}",
				                                       _client.GetErrorDescription(), _settings));
			}
		}

		private void LoginInternal()
		{
			if (!TryLogin())
			{
				throw new EmailException(String.Format("Cannot login: {0}; Connection settings: {1}", _client.GetErrorDescription(),
				                                       _settings));
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
			errors.Add(new PluginProfileError {FieldName = ConnectionSettings.LoginField, Message = "Check credentials"});
			errors.Add(new PluginProfileError {FieldName = ConnectionSettings.PasswordField, Message = "Check credentials"});
		}

		private static void AddConnectionErrors(PluginProfileErrorCollection errors)
		{
			errors.Add(new PluginProfileError {FieldName = ConnectionSettings.PortField, Message = "Check connection settings"});
			errors.Add(new PluginProfileError
			           	{FieldName = ConnectionSettings.MailServerField, Message = "Check connection settings"});
		}

		private bool TryConnect()
		{
			if (_client.IsConnected)
			{
				return true;
			}

			if (_settings.UseSSL)
			{
				_client.SslMode = _settings.Port == 110 ? SslStartupMode.UseStartTls : SslStartupMode.OnConnect;
			}

			_client.Connect(_settings.MailServer, _settings.Port);

			return _client.IsConnected;
		}

		private bool TryLogin()
		{
			ConnectInternal();

			if (_client.IsLoggedIn)
			{
				return true;
			}

			if (_settings.UseSSL)
			{
				_client.Login(_settings.Login, _settings.Password);
			}
			else
			{
				_client.Login(_settings.Login, _settings.Password, AuthenticationMethods.Regular);
			}

			return _client.IsLoggedIn;
		}

		public MailMessage DownloadMessage(string uid)
		{
			return _client.DownloadEntireMessage(_client.GetMessageIndexFromUid(uid));
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
			_client = null;
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
