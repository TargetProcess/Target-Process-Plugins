// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap;
using log4net;

namespace Tp.Core.Services
{
	public class ObservableService<T> : IService
	{
		private readonly IObservable<T> _source;
		private readonly IEnumerable<IObserver<T>> _observers;
		private readonly ILog _log;
		private IDisposable _token;

		public ObservableService(IContainer container, ITpLogManager logManager)
		{
			_source = container.GetInstance<IObservable<T>>();
			_observers = container.GetAllInstances<IObserver<T>>();
			_log = logManager.GetLog(GetType());
		}

		public void Start()
		{
			var tokens = _observers.Select(subscriber => _source.Subscribe(subscriber)).ToList();
			_token = new CompositeDisposable(tokens);
			_log.InfoFormat("{0} started. Observers count = {1}, Observers types: {2}", _source.GetType().FullName, _observers.Count(), string.Join(",", _observers.Select(o => o.GetType().FullName)));
		}

		public void Stop()
		{
			if (_token != null)
			{
				_log.Info("Token disposed");
				_token.Dispose();
			}
			_log.Info("Stopped");
		}
	}
}