// 
// Copyright (c) 2005-2017 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.PopEmailIntegration.Data
{
    public class UserRepository
    {
        private readonly IStorageRepository _storageRepository;

        public UserRepository(IStorageRepository storageRepository)
        {
            _storageRepository = storageRepository;
        }

        public void Add(UserLite userLite)
        {
            var oldUser = _storageRepository.Get<UserLite>(userLite.Id.ToString()).SingleOrDefault();
            if (oldUser == null)
            {
                _storageRepository.Get<UserLite>(userLite.Id.ToString()).Add(userLite);
                _storageRepository.Get<UserEmailSearchField>(userLite.Email).Add(new UserEmailSearchField { UserId = userLite.Id });
            }
            else
            {
                Update(userLite);
            }
        }

        public void Remove(int? userId, string userEmail)
        {
            _storageRepository.Get<UserLite>(userId.ToString()).Clear();
            _storageRepository.Get<UserEmailSearchField>(userEmail).Remove(x => x.UserId == userId);
        }

        public void RemoveAll()
        {
            _storageRepository.Get<UserLite>().Clear();
            _storageRepository.Get<UserEmailSearchField>().Clear();
        }

        public void Update(UserLite user)
        {
            // 4 queries
            var oldUser = _storageRepository.Get<UserLite>(user.Id.ToString()).First();
            _storageRepository.Get<UserLite>(user.Id.ToString()).Update(user,
                x => x.Id == user.Id);

            _storageRepository.Get<UserEmailSearchField>(oldUser.Email).Remove(x => x.UserId == user.Id);
            _storageRepository.Get<UserEmailSearchField>(user.Email).Add(new UserEmailSearchField
                { UserId = user.Id });
        }

        public IEnumerable<UserLite> GetByEmail(string userEmail)
        {
            var userEmailSearchFields = _storageRepository.Get<UserEmailSearchField>(userEmail).ToArray();
            var result = new List<UserLite>();
            foreach (var userEmailSearchField in userEmailSearchFields)
            {
                result.AddRange(_storageRepository.Get<UserLite>(userEmailSearchField.UserId.ToString()));
            }

            return result;
        }

        public UserLite GetById(int? userId)
        {
            return _storageRepository.Get<UserLite>(userId.ToString()).SingleOrDefault();
        }
    }

    [Serializable]
    public class UserEmailSearchField
    {
        public int? UserId { get; set; }
    }
}
