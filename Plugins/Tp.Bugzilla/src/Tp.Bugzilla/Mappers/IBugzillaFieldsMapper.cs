// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Bugzilla.CustomCommand.Dtos;
using Tp.Integration.Plugin.Common.Mapping;

namespace Tp.Bugzilla.Mappers
{
	public interface IBugzillaFieldsMapper
	{
		Mappings Map(MappingSource source);
	}
}