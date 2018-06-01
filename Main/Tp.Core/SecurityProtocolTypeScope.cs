using System;
using System.Net;

namespace Tp.Core
{
    /// <summary>
    /// Scope to add security protocols types during its lifetime. 
    /// </summary>
    public class SecurityProtocolTypeScope : IDisposable
    {
        private readonly SecurityProtocolType _oldSecurityProtocolType;
        private readonly SecurityProtocolType _newSecurityProtocolType;
        private bool _isDisposed;

        public SecurityProtocolTypeScope(SecurityProtocolType scopedSecurityProtocolType)
        {
            _oldSecurityProtocolType = ServicePointManager.SecurityProtocol;
            _newSecurityProtocolType = ServicePointManager.SecurityProtocol |= scopedSecurityProtocolType;
        }

        public void Dispose()
        {
            if (!_isDisposed && ServicePointManager.SecurityProtocol == _newSecurityProtocolType)
            {
                ServicePointManager.SecurityProtocol = _oldSecurityProtocolType;
                _isDisposed = true;
            }
        }
    }
}
