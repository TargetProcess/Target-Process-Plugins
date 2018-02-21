using System;
using System.Collections.Generic;

namespace Tp.Core
{
    public sealed class CompositeDisposable : IDisposable
    {
        private readonly List<IDisposable> _disposables;

        public bool IsDisposed { get; private set; }

        public CompositeDisposable(IEnumerable<IDisposable> disposables)
        {
            if (disposables == null)
            {
                throw new ArgumentNullException(nameof(disposables));
            }

            _disposables = new List<IDisposable>(disposables);
        }

        public void Dispose()
        {
            IDisposable[] disposableArray = null;

            lock (_disposables)
            {
                if (!IsDisposed)
                {
                    IsDisposed = true;
                    disposableArray = _disposables.ToArray();
                    _disposables.Clear();
                }
            }

            if (disposableArray == null)
            {
                return;
            }

            foreach (var disposable in disposableArray)
            {
                disposable.Dispose();
            }
        }
    }
}
