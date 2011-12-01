// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
namespace Tp.Integration.Messages.Ticker
{
	/// <summary>
	/// Inherit your profile from this interface to make it perform some repeated actions.
	/// </summary>
	public interface ISynchronizableProfile
	{
		/// <summary>
		/// Number of minutes after which you will receive <see cref="TickMessage"/>.
		/// Perform your logic in <see cref="TickMessage"/> handler.
		/// </summary>
		int SynchronizationInterval { get; }
	}
}