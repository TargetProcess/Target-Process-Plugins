// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Plugin.Common.Mapping;

namespace Tp.SourceControl
{
    public static class UserLookupExtensions
    {
        public static MappingLookup ConvertToUserLookup(this TpUserData userDto)
        {
            return new MappingLookup
            {
                Id = userDto.ID.GetValueOrDefault(),
                Name = $"{userDto.FirstName} {userDto.LastName}"
            };
        }
    }
}
