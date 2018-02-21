using System;
using NServiceBus;
using StructureMap;
using Tp.Integration.Messages.ServiceBus.Transport;
using Tp.Integration.Messages.ServiceBus.UnicastBus;
using Tp.Integration.Messages.Ticker;
using Tp.Integration.Plugin.Common.PluginLifecycle;
using Tp.Integration.Plugin.Common.Properties;
using log4net;

namespace Tp.Integration.Plugin.Common.Ticker
{
    public class Timer : IWantCustomInitialization, IDisposable
    {
        private readonly IBusExtended _bus;
        private System.Timers.Timer _checkTimer;
        private TimeSpan _infoSendInterval = TimeSpan.FromSeconds(600);
        private System.Timers.Timer _infoSenderTimer;

        public Timer()
        {
            _bus = ObjectFactory.GetInstance<IBusExtended>();
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_checkTimer != null)
                _checkTimer.Dispose();

            if (_infoSenderTimer != null)
                _infoSenderTimer.Dispose();
        }

        #endregion

        #region IWantCustomInitialization Members

        public void Init()
        {
            var checkInterval = TimeSpan.FromSeconds(Settings.Default.DefaultCheckInterval);

            _checkTimer = new System.Timers.Timer { Interval = checkInterval.TotalMilliseconds };
            _checkTimer.Elapsed += (sender, elapsedEventArgs) =>
            {
                try
                {
                    var transport = ObjectFactory.GetInstance<IMsmqTransport>();
                    if (transport != null)
                    {
                        if (transport.QueueIsNotEmpty())
                        {
                            return;
                        }

                        if (
                            !MsmqHelper.QueueIsNotOverloaded(transport.InputQueue,
                                "Failed to count messages in main queue '{0}'".Fmt(
                                    transport.InputQueue),
                                Settings.Default.MessagesInQueueCountThreshold))
                        {
                            return;
                        }
                    }

                    _bus.CleanupOutgoingHeaders();
                    _bus.SendLocal(new CheckIntervalElapsedMessage());
                }
                catch (Exception e)
                {
                    LogManager.GetLogger(GetType()).Fatal(
                        "Failed to send CheckIntervalElapsedMessage. Plugin will not do any synchronization work.", e);
                }
            };

            _checkTimer.Start();

            _infoSenderTimer = new System.Timers.Timer { Interval = _infoSendInterval.TotalMilliseconds };
            _infoSenderTimer.Elapsed += (sender, elapsedEventArgs) => new PluginInitializer().SendInfoMessages();
            _infoSenderTimer.Start();
        }

        #endregion
    }
}
