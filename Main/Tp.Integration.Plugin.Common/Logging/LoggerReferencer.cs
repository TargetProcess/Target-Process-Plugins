using Common.Logging;
using Common.Logging.Log4Net;
using Tp.Core.Annotations;

namespace Tp.Integration.Plugin.Common.Logging
{
    // Required to propagate Log4Net dependency to PushNotification, Follow (and other?) plugins
    [UsedImplicitly]
    public class LoggerReferencer
    {
        public ILog Log { get; set; }
        public Log4NetLogger Logger { get; set; }
    }
}
