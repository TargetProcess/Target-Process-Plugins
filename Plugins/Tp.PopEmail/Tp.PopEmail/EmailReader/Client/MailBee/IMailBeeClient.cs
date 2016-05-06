using System;
using MailBee;
using MailBee.Mime;
using MailBee.Security;

namespace Tp.PopEmailIntegration.EmailReader.Client.MailBee
{
	// MailBee Imap and Pop3 clients have lots of absolutely similar methods and behave in the same way,
	// but actually have no common ancestor. This interface and its implementations are an attempt to unify them.
	public interface IMailBeeClient : IDisposable
	{
		bool Connect(string serverName, int port, SslStartupMode? sslMode);
		bool Disconnect();
		bool IsConnected { get; }

		bool LogIn(string login, string password, AuthenticationMethods authenticationMethod);
		bool IsLoggedIn { get; }

		string[] GetMessageUids();
		MailMessage DownloadMessage(string uid);
		string GetErrorDescription();
	}
}
