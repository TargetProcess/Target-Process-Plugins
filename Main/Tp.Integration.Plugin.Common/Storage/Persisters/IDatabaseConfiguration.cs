// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
namespace Tp.Integration.Plugin.Common.Storage.Persisters
{
	public interface IDatabaseConfiguration
	{
		string ConnectionString { get; }
	}
}