using System;
using Tp.Integration.Messages;

namespace Tp.Integration.Plugin.Common.Domain
{
    public class PluginDisabledForAccountException : ApplicationException
    {
        public PluginDisabledForAccountException(PluginName pluginName, AccountName accountName)
            : base($"'{pluginName}' plugin is disabled for '{accountName}' account")
        {
            
        }
    }
}