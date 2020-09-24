using System;
using System.Runtime.Remoting.Messaging;
using Tp.Core;
using Tp.Core.Annotations;

namespace Tp.Integration.Messages.ServiceBus.UnicastBus
{
    public class SharedUnicastBusSessionContextHolder : IValueHolder<SharedUnicastBusSessionContext>
    {
        private const string _contextKey = "Tp.SharedUnicastBusSessionContext.Value";
        public static readonly SharedUnicastBusSessionContextHolder Instance = new SharedUnicastBusSessionContextHolder();

        private SharedUnicastBusSessionContextHolder()
        {
        }

        [Pure]
        public SharedUnicastBusSessionContext Get() => CallContext.GetData(_contextKey)
            .MaybeAs<SharedUnicastBusSessionContext>()
            .GetOrElse(() =>
            {
                var newContext = new SharedUnicastBusSessionContext();
                SetInternal(newContext);
                return newContext;
            });

        public void Set([NotNull] SharedUnicastBusSessionContext value)
        {
            Argument.NotNull(nameof(value), value);
            SetInternal(value);
        }

        public void Clear() => CallContext.FreeNamedDataSlot(_contextKey);

        public static IDisposable SwitchContextTo([NotNull] SharedUnicastBusSessionContext value)
        {
            Argument.NotNull(nameof(value), value);
            var original = Instance.Get();
            Instance.Set(value);
            return Disposable.Create(() => SetInternal(original));
        }

        private static void SetInternal(SharedUnicastBusSessionContext value) => CallContext.SetData(_contextKey, value);
    }
}
