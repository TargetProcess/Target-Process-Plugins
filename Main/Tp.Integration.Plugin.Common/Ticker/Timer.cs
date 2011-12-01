// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using NServiceBus;
using NServiceBus.Unicast.Transport;
using StructureMap;
using Tp.Integration.Messages.Ticker;
using log4net;

namespace Tp.Integration.Plugin.Common.Ticker
{
	public class Timer : IWantCustomInitialization, IDisposable
	{
		private readonly IBus _bus;
		private System.Timers.Timer _timer;
		private const int DEFAULT_INTERVAL = 15000;

		public Timer()
		{
			_bus = ObjectFactory.GetInstance<IBus>();
		}

		public void Init()
		{
			_timer = new System.Timers.Timer {Interval = DEFAULT_INTERVAL};
			_timer.Elapsed += (sender, elapsedEventArgs) =>
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

			_timer.Start();
		}

		public void Dispose()
		{
			if (_timer != null)
				_timer.Dispose();
		}
	}
}
