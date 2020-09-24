using System;
using System.Linq;
using log4net;
using StructureMap;
using Tp.Core.Services;

namespace Tp.Core
{
    public class ServiceManager
    {
        public static void StartServices(IContainer container, ILog log)
        {
            RunOnServices(container, s => s.Start(), () => log.Info("Services starting..."), () => log.Info("Services started..."),
                log.Error);
        }

        public static void StopServices(IContainer container, ILog log)
        {
            RunOnServices(container, s => s.Stop(), () => log.Info("Services stopping..."), () => log.Info("Services stopped..."), log.Error);
        }

        private static void RunOnServices(IContainer container, Action<IService> action, Action starting, Action finishing,
            Action<Exception> onError)
        {
            starting();
            try
            {
                var services = container.GetAllInstances<IService>().OrderByDescending(s => s.Priority);
                foreach (IService service in services)
                {
                    action(service);
                }
            }
            catch (Exception exception)
            {
                onError(exception);
                throw;
            }
            finishing();
        }
    }
}
