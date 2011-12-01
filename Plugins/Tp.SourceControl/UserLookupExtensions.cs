// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using Tp.Integration.Common;
using Tp.Integration.Plugin.Common.Mapping;

namespace Tp.SourceControl
{
	public static class UserLookupExtensions
	{
		public static MappingLookup ConvertToUserLookup(this UserDTO userDto)
		{
			return new MappingLookup
			{
				Id = userDto.ID.GetValueOrDefault(),
				Name = String.Format("{0} {1}", userDto.FirstName, userDto.LastName)
			};
		}
	}
}