using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml;
using log4net;
using log4net.Config;
using log4net.Util;

namespace Tp.Core
{
    public interface ITpLogManager
    {
        ILog GetLog(string loggerName);
        ILog DefaultLog { get; }
    }

    public class TpLogManager : ITpLogManager
    {
        private static TpLogManager _instance;
        private static ILog _defaultLog;

        static TpLogManager()
        {
            string[] configFiles = { "log4net.generated.config", "log4net.config" };

            var fileInfo = configFiles
                .Select(SystemInfo.ConvertToFullPath)
                .Select(x => new FileInfo(x))
                .Where(x => x.Exists)
                .FirstOrNothing();

            fileInfo.Do(XmlConfigurator.ConfigureAndWatch, () =>
            {
                var config = (XmlElement) ConfigurationManager.GetSection("log4net");
                XmlConfigurator.Configure(config);
                Instance.DefaultLog.Warn("Could not fnd log4net config files - use app config");
            });
        }


        public static TpLogManager Instance => _instance ?? (_instance = new TpLogManager());

        public ILog GetLog(string loggerName) => LogManager.GetLogger(loggerName);

        public ILog DefaultLog => _defaultLog ?? (_defaultLog = LogManager.GetLogger("General"));
    }

    public static class TpLogManagerExtensions
    {
        public static ILog GetLog(this ITpLogManager logManager, Type type) => logManager.GetLog(type.FullName);
    }
}
