using System;
using MailBee;
using MailBee.Mime;
using MailBee.Pop3Mail;
using MailBee.Security;

namespace Tp.PopEmailIntegration.EmailReader.Client.MailBee
{
    public class Pop3MailBeeClient : IMailBeeClient
    {
        private readonly Pop3 _client;
        private bool _disposed;

        public Pop3MailBeeClient()
        {
            Global.LicenseKey = MailBeeEmailClient.MAILBEE_LICENSE_KEY;

            // Make sure to disable throwing exception explicitly. Otherwise strange things might happen. See bug #5748 for details.
            // So please don't change this option unless you know what you are doing!!!
            _client = new Pop3
            {
                ThrowExceptions = true, InboxPreloadOptions = Pop3InboxPreloadOptions.List | Pop3InboxPreloadOptions.Uidl
            };
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

        public MailMessage DownloadMessage(string uid) => _client.DownloadEntireMessage(_client.GetMessageIndexFromUid(uid));

        public string GetErrorDescription() => _client.GetErrorDescription();

        public string[] GetMessageUids() => _client.GetMessageUids();

        public bool IsConnected => _client.IsConnected;

        public bool LogIn(string login, string password, AuthenticationMethods authenticationMethod)
        {
            try
            {
                return _client.Login(login, password, authenticationMethod);
            }
            catch (MailBeePop3LoginBadCredentialsException)
            {
                return false;
            }
        }

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
