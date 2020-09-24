using System;
using log4net;

namespace Tp.Core.Expressions
{
    internal class ExpressionLoggerAdapter : IExpressionLogger
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(ExpressionLoggerAdapter));

        public static readonly ExpressionLoggerAdapter Instance = new ExpressionLoggerAdapter();

        private ExpressionLoggerAdapter()
        {
        }

        public void Log(LogLevel level, Exception exception, string message, params object[] args)
        {
            switch (level)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                    if (_log.IsDebugEnabled) _log.Debug(args.Length > 0 ? string.Format(message, args) : message, exception);
                    break;
                case LogLevel.Info:
                    if (_log.IsInfoEnabled) _log.Info(args.Length > 0 ? string.Format(message, args) : message, exception);
                    break;
                case LogLevel.Warn:
                    if (_log.IsWarnEnabled) _log.Warn(args.Length > 0 ? string.Format(message, args) : message, exception);
                    break;
                case LogLevel.Error:
                    if (_log.IsErrorEnabled) _log.Error(args.Length > 0 ? string.Format(message, args) : message, exception);
                    break;
                case LogLevel.Fatal:
                    if (_log.IsFatalEnabled) _log.Fatal(args.Length > 0 ? string.Format(message, args) : message, exception);
                    break;
            }
        }
    }
}
