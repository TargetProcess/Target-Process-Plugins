// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;

namespace Tp.Core
{
	public sealed class CompositeDisposable : IDisposable
	{
		private bool _disposed;
		private readonly List<IDisposable> _disposables;
		
		public bool IsDisposed
		{
			get { return _disposed; }
		}

		public CompositeDisposable(IEnumerable<IDisposable> disposables)
		{
			if (disposables == null)
			{
				throw new ArgumentNullException("disposables");
			}
			_disposables = new List<IDisposable>(disposables);
		}
		
		public void Dispose()
		{
			IDisposable[] disposableArray = null;
			lock (_disposables)
			{
				if (!_disposed)
				{
					_disposed = true;
					disposableArray = _disposables.ToArray();
					_disposables.Clear();
				}
			}
			if (disposableArray == null)
			{
				return;
			}
			foreach (IDisposable disposable in disposableArray)
			{
				disposable.Dispose();
			}
		}
	}
}