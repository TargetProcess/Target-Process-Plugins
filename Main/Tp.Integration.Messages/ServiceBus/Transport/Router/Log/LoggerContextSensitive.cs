using System;
using log4net;

namespace Tp.Integration.Messages.ServiceBus.Transport.Router.Log
{
    public class LoggerContextSensitive : ILoggerContextSensitive
    {
        public void Debug(LoggerContext context, string message)
        {
            Log(context, message, (l, m) => l.Debug(m));
        }

        public void Warn(LoggerContext context, string message, Exception error)
        {
            Log(context, message, (l, m) => l.Warn(m, error));
        }

        public void Info(LoggerContext context, string message)
        {
            Log(context, message, (l, m) => l.Info(m));
        }

        public void Fatal(LoggerContext context, string message, Exception error)
        {
            Log(context, message, (l, m) => l.Fatal(m, error));
        }

        public void Error(LoggerContext context, string message, Exception error = null)
        {
            Log(context, message, (l, m) =>
                {
                    if (error != null)
                    {
                        l.Error(m, error);
                    }
                    else
                    {
                        l.Error(m);
                    }
                }
            );
        }

        private void Log(LoggerContext context, string message, Action<ILog, string> log)
        {
            ILog logger = LogManager.GetLogger(context.QueueName);
            log(logger, string.Format("{0}{1}", context.ToString(logger.IsDebugEnabled), message));
        }
    }
}
