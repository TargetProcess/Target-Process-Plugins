using System;
using MSMQ;
using Tp.Integration.Messages.ServiceBus.Transport;
using log4net;

namespace Tp.Integration.Plugin.Common
{
    public static class MsmqHelper
    {
        public static bool QueueIsNotOverloaded(string queueName, string errorMessage, int messagesInQueueCountThreshold)
        {
            try
            {
                var queue = new PluginQueue(queueName);
                var qMgmt = new MSMQManagement();
                object machine = Environment.MachineName;
                var missing = Type.Missing;
                object formatName = queue.FormatName;
                qMgmt.Init(ref machine, ref missing, ref formatName);
                return qMgmt.MessageCount < messagesInQueueCountThreshold;
            }
            catch (Exception e)
            {
                LogManager.GetLogger(typeof(MsmqHelper)).Warn(errorMessage, e);
                return true;
            }
        }
    }
}
