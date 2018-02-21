using System;
using Tp.Integration.Messages.PluginLifecycle;

namespace Tp.Integration.Messages.AccountLifecycle
{
    [Serializable]
    public sealed class AccountRemovedLastStepMessage
        : IPluginLocalMessage
    {
        public AccountRemovedLastStepMessage()
        {
        }

        public AccountRemovedLastStepMessage(string accountName)
        {
            AccountName = accountName;
        }

        public string AccountName { get; set; }
    }
}
