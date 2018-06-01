// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using AutoMapper;
using Tp.Integration.Common;

namespace Tp.PopEmailIntegration.Data
{
    [Serializable]
    public class UserLite
    {
        public int? Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public UserType UserType { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? DeleteDate { get; set; }
        public int? CompanyId { get; set; }

        public static UserLite Create(RequesterDTO requesterDto)
        {
            var userLite = new UserLite();
            Mapper.DynamicMap(requesterDto, userLite);
            userLite.UserType = UserType.Requester;
            return userLite;
        }

        public static UserLite Create(UserDTO userDto)
        {
            var userLite = new UserLite();
            Mapper.DynamicMap(userDto, userLite);
            userLite.UserType = UserType.User;
            return userLite;
        }

        public bool IsDeleted => DeleteDate != null;

        public bool IsDeletedOrInactiveUser
        {
            get
            {
                if (UserType != UserType.User)
                    return false;

                if (IsDeleted)
                    return true;

                return IsActive == false;
            }
        }

        public bool IsDeletedRequester => UserType == UserType.Requester && IsDeleted;
    }
}
