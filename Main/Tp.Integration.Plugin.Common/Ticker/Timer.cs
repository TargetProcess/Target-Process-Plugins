// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using NServiceBus;
using NServiceBus.Unicast.Transport;
using StructureMap;
using Tp.Integration.Messages.Ticker;
using Tp.Integration.Plugin.Common.PluginLifecycle;
using log4net;

namespace Tp.Integration.Plugin.Common.Ticker
{
	public class Timer : IWantCustomInitialization, IDisposable
	{
		private readonly IBus _bus;
		private System.Timers.Timer _checkTimer;
		private System.Timers.Timer _infoSenderTimer;
		private TimeSpan _defaultCheckInterval = TimeSpan.FromSeconds(15);
		private TimeSpan _infoSendInterval = TimeSpan.FromSeconds(300);

		public Timer()
		{
			_bus = ObjectFactory.GetInstance<IBus>();
		}

		public void Init()
		{
			_checkTimer = new System.Timers.Timer { Interval = _defaultCheckInterval.TotalMilliseconds };
			_checkTimer.Elapsed += (sender, elapsedEventArgs) =>
			                  	{
			                  		try
			                  		{
			                  		var transport = ObjectFactory.GetInstance<ITransport>();
									if (transport != null)
									{
			                  				if (transport.QueueIsNotEmpty())
											return;
									}
			                  	

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

		public void Dispose()
		{
			if (_checkTimer != null)
				_checkTimer.Dispose();

			if (_infoSenderTimer != null)
				_infoSenderTimer.Dispose();
		}
	}
}
