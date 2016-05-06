using System;
using System.Linq;
using MailBee;
using MailBee.ImapMail;
using MailBee.Mime;
using MailBee.Security;

namespace Tp.PopEmailIntegration.EmailReader.Client.MailBee
{
	public class ImapMailBeeClient : IMailBeeClient
	{
		private readonly Imap _client;
		private bool _disposed;
		private const string DefaultFolderName = "Inbox";

		public ImapMailBeeClient()
		{
			Imap.LicenseKey = MailBeeEmailClient.MAILBEE_LICENSE_KEY;

			// Make sure to disable throwing exception explicitly. Otherwise strange things might happen. See bug #5748 for details.
			// So please don't change this option unless you know what you are doing!!!
			_client = new Imap { ThrowExceptions = false };
		}

		public bool Connect(string serverName, int port, SslStartupMode? sslMode)
		{
			if (sslMode.HasValue)
			{
				_client.SslMode = sslMode.Value;
			}

			return _client.Connect(serverName, port);
		}

		public bool Disconnect() => _client.Disconnect();

		public MailMessage DownloadMessage(string uid)
		{
			_client.ExamineFolder(DefaultFolderName);
			return _client.DownloadEntireMessage(long.Parse(uid), indexIsUid: true);
		}

		public string GetErrorDescription() => _client.GetErrorDescription();

		public string[] GetMessageUids()
		{
			_client.ExamineFolder(DefaultFolderName);
			return _client.Search()?.OfType<long>().Select(uid => uid.ToString()).ToArray();
		}

		public bool IsConnected => _client.IsConnected;

		public bool LogIn(string login, string password, AuthenticationMethods authenticationMethod)
			=> _client.Login(login, password, authenticationMethod);

		public bool IsLoggedIn => _client.IsLoggedIn;

		public void Dispose()
		{
			if (_disposed) return;
			_disposed = true;
			GC.SuppressFinalize(this);
			_client.Dispose();
		}
	}
}
