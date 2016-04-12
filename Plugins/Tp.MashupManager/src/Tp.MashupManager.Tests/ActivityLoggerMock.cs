// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using Tp.Integration.Plugin.Common.Activity;
using log4net.Core;

namespace Tp.MashupManager.Tests
{
	public class LevelMessages
	{
		public Level Key { get; set; }
		public List<string> Messages { get; set; }
	}

	public class MessagesContainer : List<LevelMessages>
	{
		public LevelMessages this[Level key]
		{
			get
			{
				return Find(x => x.Key == key);
			}
		}

		public void Add(Level key, string message)
		{
			var levelEntry = this.Where(l => l.Key == key).SingleOrDefault();
			if(levelEntry != null)
			{
				levelEntry.Messages.Add(message);
			}
			else
			{
				Add(new LevelMessages {Key = key, Messages = new List<string> {message}});
			}
		}
	}

	public class ActivityLoggerMock : IActivityLogger
	{
		public ActivityLoggerMock()
		{
			Messages = new MessagesContainer();
		}

		public MessagesContainer Messages { get; private set; }

		public bool IsDebugEnabled { get { return true; } }

		public void Debug(string message)
		{
			Messages.Add(Level.Debug, message);
		}

		public void DebugFormat(string format, params object[] args)
		{
			Messages.Add(Level.Debug, string.Format(format, args));
		}

		public void Info(string message)
		{
			Messages.Add(Level.Info, message);
		}

		public void InfoFormat(string format, params object[] args)
		{
			Messages.Add(Level.Info, string.Format(format, args));
		}

		public void Warn(string message)
		{
			Messages.Add(Level.Warn, message);
		}

		public void WarnFormat(string format, params object[] args)
		{
			Messages.Add(Level.Warn, string.Format(format, args));
		}

		public void Error(string message)
		{
			Messages.Add(Level.Error, message);
		}

		public void Error(Exception ex)
		{
			Messages.Add(Level.Error, ex.ToString());
		}

		public void Error(string message, Exception exception)
		{
			Messages.Add(Level.Error, string.Format("{0}\n{1}", message, exception));
		}

		public void ErrorFormat(string format, params object[] args)
		{
			Messages.Add(Level.Error, string.Format(format, args));
		}

		public void Fatal(string message)
		{
			Messages.Add(Level.Fatal, message);
		}

		public void Fatal(Exception ex)
		{
			Messages.Add(Level.Fatal, ex.ToString());
		}
	}
}