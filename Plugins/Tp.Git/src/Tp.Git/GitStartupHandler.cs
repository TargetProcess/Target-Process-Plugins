using System.Net;
using NServiceBus;
using Tp.Core;

namespace Tp.Git
{
    public class GitStartupHandler : IWantToRunAtStartup
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
