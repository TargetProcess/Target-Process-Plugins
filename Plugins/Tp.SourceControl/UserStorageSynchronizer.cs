// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using NServiceBus;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Plugins.Toolkit.Repositories;

namespace Tp.SourceControl
{   
    public class UserStorageSynchronizer
        :
            IHandleMessages<UserCreatedMessage>,
            IHandleMessages<UserDeletedMessage>,
            IHandleMessages<UserUpdatedMessage>
    {
        private readonly IRepository<TpUserData> _userRepository;

        public UserStorageSynchronizer(IRepository<TpUserData> repository)
        {
            _userRepository = repository;
        }

        public void Handle(UserDeletedMessage message) => _userRepository.Delete(new TpUserData(message.Dto));

        public void Handle(UserCreatedMessage message)
        {
            if (message.Dto.DeleteDate == null && message.Dto.IsActive == true)
            {
                _userRepository.Add(new TpUserData(message.Dto));
            }
        }
       
        public void Handle(UserUpdatedMessage message)
        {
            if (message.ChangedFields.Contains(UserField.DeleteDate))
            {
                if (message.Dto.DeleteDate != null)
                {
                    _userRepository.Delete(new TpUserData(message.Dto));
                }
                else if (message.Dto.IsActive == true)
                {
                    _userRepository.Add(new TpUserData(message.Dto));
                }
            }

            if (message.ChangedFields.Contains(UserField.IsActive))
            {
                if (message.Dto.IsActive != true)
                {
                    _userRepository.Delete(new TpUserData(message.Dto));
                }
                else if (message.Dto.DeleteDate == null)
                {
                    _userRepository.Add(new TpUserData(message.Dto));
                }
                return;
            }

            if (message.ChangedFields.Any(f => TpUserData.RequiredFields.Contains(f)))
            {
                _userRepository.Update(new TpUserData(message.Dto));
            }
        }
    }    
}
