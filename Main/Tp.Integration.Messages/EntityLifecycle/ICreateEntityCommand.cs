// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle
{
	public interface ICreateEntityCommand<out TEntityDto>
		where TEntityDto : DataTransferObject, new()
	{
		TEntityDto Dto { get; }
	}
}