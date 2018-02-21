using System;
using System.Collections.Generic;
using System.Linq;
using Tp.Core;
using Tp.Integration.Messages;

namespace Tp.Integration.Plugin.Common.Domain
{
    internal class DisabledAccountCollection : HashSet<AccountName>, IDisabledAccountCollection
    {
        private DisabledAccountCollection()
        {
        }

        private DisabledAccountCollection(IEnumerable<AccountName> collection) : base(collection)
        {
        }

        internal static IDisabledAccountCollection Load()
        {
            return PluginSettings.Load(
                nameof(DisabledAccountCollection),
                new DisabledAccountCollection(),
                value => Maybe.Just(new DisabledAccountCollection(value.Split(',').Select(a => new AccountName(a.TrimSafe())))));
        }
    }
}
