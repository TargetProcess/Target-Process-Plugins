using System.Net;
using NServiceBus;
using Tp.Core;

namespace Tp.MashupManager
{
    public class MashupManagerStartupHandler : IWantToRunAtStartup
    {
        private SecurityProtocolTypeScope _securityProtocolTypeScope;

        public void Run()
        {
            _securityProtocolTypeScope = new SecurityProtocolTypeScope(SecurityProtocolType.Tls12);
        }

        public void Stop()
        {
            _securityProtocolTypeScope?.Dispose();
        }
    }
}
