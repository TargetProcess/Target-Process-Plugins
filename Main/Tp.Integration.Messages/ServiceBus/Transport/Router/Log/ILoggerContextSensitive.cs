using System;

namespace Tp.Integration.Messages.ServiceBus.Transport.Router.Log
{
	public interface ILoggerContextSensitive
	{
		void Debug(LoggerContext context, string message);
		void Warn(LoggerContext context, string message, Exception error);
		void Info(LoggerContext context, string message);
		void Fatal(LoggerContext context, string message, Exception error);
		void Error(LoggerContext context, string message, Exception error = null);
	};
}
