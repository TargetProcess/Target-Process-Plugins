using System;
using Tp.Integration.Plugin.Common.Activity;

namespace Tp.Search.Tests.Log
{
	class ConsoleLogger : IActivityLogger
	{
		public bool IsDebugEnabled { get { return true; } }

		public void Debug(string message)
		{
			Dump(message);
		}

		public void DebugFormat(string format, params object[] args)
		{
			Console.WriteLine(format, args);
		}

		public void Info(string message)
		{
			Dump(message);
		}
		
		public void InfoFormat(string format, params object[] args)
		{
			DumpFormat(format, args);
		}

		public void Warn(string message)
		{
			Dump(message);
		}

		public void WarnFormat(string format, params object[] args)
		{
			DumpFormat(format, args);
		}

		public void Error(string message)
		{
			throw new NotImplementedException();
		}

		public void Error(Exception ex)
		{
			Dump(ex);
		}

		public void Error(string _, Exception exception)
		{
			Dump(exception);
		}

		public void ErrorFormat(string format, params object[] args)
		{
			DumpFormat(format, args);
		}

		public void Fatal(string message)
		{
			Dump(message);
		}

		public void Fatal(Exception ex)
		{
			Dump(ex);
		}

		private static void Dump(object message)
		{
			Console.WriteLine(message);
		}

		private static void DumpFormat(string format, params object[] parameters)
		{
			Console.WriteLine(format, parameters);
		}
	}
}