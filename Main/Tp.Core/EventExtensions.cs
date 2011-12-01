// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;

namespace Tp.Core
{
	public static class EventExtensions
	{
		public static void Raise(this EventHandler handler, object sender, EventArgs args)
		{
			if (handler != null)
			{
				handler(sender, args);
			}
		}

		public static void Raise<TEventArgs>(this EventHandler<TEventArgs> handler, object sender, TEventArgs args) 
			where TEventArgs : EventArgs
		{
			if (handler != null)
			{
				handler(sender, args);
			}
		}

		public static void Raise(this EventHandler handler, object sender)
		{
			Raise(handler, sender, EventArgs.Empty);
		}
	}
}