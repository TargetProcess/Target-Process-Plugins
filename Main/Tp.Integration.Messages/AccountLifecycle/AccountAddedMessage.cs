using System;

namespace Tp.Integration.Messages.AccountLifecycle
{
    [Serializable]
    public sealed class AccountAddedMessage
        : IAccountLifecycleMessage
    {
        public AccountAddedMessage()
        {
        }

        public AccountAddedMessage(string accountName)
        {
            AccountName = accountName;
        }

        public string AccountName { get; set; }
    }
}
