// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Messages;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Integration.Plugin.Common.Gateways
{
    internal class ProfileGateway : IProfileGateway
    {
        private readonly IProfileReadonly _profile;
        private AccountName _accountName;
        private ITpBus _bus;
        private IStorage<ITargetProcessMessage> _storage;
        private bool _disposed;

        public ProfileGateway(IProfileReadonly profile, AccountName accountName, ITpBus bus)
        {
            _profile = profile;
            _accountName = accountName;
            _bus = bus;
            _storage = _profile.Get<ITargetProcessMessage>();
        }

        public void Send(ITargetProcessMessage message)
        {
            if (!_profile.Initialized)
            {
                _storage.Add(message);
            }
            else
            {
                _bus.SendLocalWithContext(_profile.Name, _accountName, message);
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _accountName = null;
            _bus = null;
            _storage = null;
        }
    }
}
