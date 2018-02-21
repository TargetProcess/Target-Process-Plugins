using System;
using Tp.Integration.Common;
using Tp.Integration.Messages.PluginLifecycle;

namespace Tp.Integration.Messages.Commands
{
    [Serializable]
    public class ExecutePluginCommandCommand : IPluginLocalMessage
    {
        public string CommandName { get; set; }
        public string Arguments { get; set; }
        public UserDTO User { get; set; }
    }
}
