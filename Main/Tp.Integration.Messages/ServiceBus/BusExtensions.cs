using System;
using System.Collections.Generic;
using NServiceBus;

namespace Tp.Integration.Messages.ServiceBus
{
    public static class BusExtensions
    {
        public const string SAGAID_KEY = "SagaId";
        public const string ACCOUNTNAME_KEY = "AccountName";
        public const string PROFILENAME_KEY = "ProfileName";

        public static Guid GetOutSagaId(this IBus bus)
        {
            return GetSagaId(bus.OutgoingHeaders);
        }

        private static Guid GetSagaId(IDictionary<string, string> headers)
        {
            if (!headers.ContainsKey(SAGAID_KEY))
                return Guid.Empty;

            var sagaId = headers[SAGAID_KEY];
            return !string.IsNullOrEmpty(sagaId) ? new Guid(sagaId) : Guid.Empty;
        }

        public static Guid GetInSagaId(this IBus bus)
        {
            return bus.CurrentMessageContext == null ? Guid.Empty : GetSagaId(bus.CurrentMessageContext.Headers);
        }

        public static void SetInSagaId(this IBus bus, Guid sagaId)
        {
            bus.CurrentMessageContext.Headers[SAGAID_KEY] = sagaId.ToString();
        }

        public static void SetOutSagaId(this IBus bus, Guid sagaId)
        {
            bus.OutgoingHeaders[SAGAID_KEY] = sagaId.ToString();
        }

        public static AccountName GetInAccountName(this IBus bus)
        {
            // TODO : If account was not set in message header, we should not return Account.Empty, because this value is used for indicating onSite mode.
            // TODO : Return SafeNull instead.
            if (bus.CurrentMessageContext == null)
                return AccountName.Empty;

            return ConvertToAccountName(bus.CurrentMessageContext.Headers[ACCOUNTNAME_KEY]);
        }

        private static AccountName ConvertToAccountName(string name)
        {
            var accountName = name;
            return string.IsNullOrEmpty(accountName) ? AccountName.Empty : new AccountName(accountName);
        }

        public static AccountName GetOutAccountName(this IBus bus)
        {
            return ConvertToAccountName(bus.OutgoingHeaders[ACCOUNTNAME_KEY]);
        }

        public static void SetIn(this IBus bus, AccountName accountName)
        {
            bus.CurrentMessageContext.Headers[ACCOUNTNAME_KEY] = accountName.Value;
        }

        public static void SetIn(this IBus bus, ProfileName profileName)
        {
            bus.CurrentMessageContext.Headers[PROFILENAME_KEY] = profileName.Value;
        }

        public static void SetOut(this IBus bus, ProfileName profileName)
        {
            bus.OutgoingHeaders[PROFILENAME_KEY] = profileName.Value;
        }

        public static ProfileName GetInProfileName(this IBus bus)
        {
            if (bus.CurrentMessageContext == null)
                return string.Empty;

            var profileName = bus.CurrentMessageContext.Headers[PROFILENAME_KEY];
            return string.IsNullOrEmpty(profileName) ? string.Empty : profileName;
        }

        public static ProfileName GetOutProfileName(this IBus bus)
        {
            return bus.OutgoingHeaders[PROFILENAME_KEY];
        }

        public static void SetOut(this IBus bus, AccountName account)
        {
            bus.OutgoingHeaders[ACCOUNTNAME_KEY] = account.Value;
        }
    }
}
