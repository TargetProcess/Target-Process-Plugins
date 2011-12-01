// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;

namespace Tp.Integration.Plugin.Common.Events
{
	abstract class EventBase
	{
		private readonly List<Action<EventArgs>> _handlers;

		protected EventBase()
		{
			_handlers = new List<Action<EventArgs>>();
		}

		protected IDisposable Subscribe(Action<EventArgs> handler)
		{
			lock (_handlers)
			{
				_handlers.Add(handler);
			}	
			return new ActionOnDispose(() =>
			                        	{
											lock (_handlers)
											{
												_handlers.Remove(handler);
											}
			                        	});
		}

		protected void Raise(EventArgs args)
		{
			List<Action<EventArgs>> handlers;
			lock (_handlers)
			{
				handlers = new List<Action<EventArgs>>(_handlers);
			}
			handlers.ForEach(a => a(args));
		}

		private class ActionOnDispose : IDisposable
		{
			private readonly Action _action;

			public ActionOnDispose(Action action)
			{
				_action = action;
			}

			public void Dispose()
			{
				_action();
			}
		}
	}
}