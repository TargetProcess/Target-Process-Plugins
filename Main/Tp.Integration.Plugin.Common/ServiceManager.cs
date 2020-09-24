// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Microsoft.Web.Services3;
using Tp.Core;
using Tp.Integration.Plugin.Common.Properties;

namespace Tp.Integration.Plugin.Common
{
    /// <summary>
    /// Provides access to TargetProcess SOAP web services. Uses values from plugin config for creating service instance(TargetProcessPath, AdminLogin, AdminPassword)
    /// </summary>
    public class ServiceManager
    {
        /// <summary>
        /// Gets new service instance by type.
        /// </summary>
        /// <typeparam name="T">The type of the service.</typeparam>
        /// <returns>New service instance.</returns>
        public T GetService<T>() where T : WebServicesClientProtocol, new()
        {
            var serviceWse = new T
            {
                Url =
                    (string.Format("{0}/Services/{1}.asmx", Settings.Default.TargetProcessPath, typeof(T).Name))
            };
            TpPolicy.ApplyAuthenticationTicket(serviceWse, Settings.Default.AdminLogin, Settings.Default.AdminPassword);

            return serviceWse;
        }
    }
}
