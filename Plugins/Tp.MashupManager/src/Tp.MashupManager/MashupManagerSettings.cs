using System;
using System.Collections.Generic;
using System.Linq;
using Tp.Core;
using Tp.Integration.Plugin.Common;

namespace Tp.MashupManager
{
    public static class MashupManagerSettings
    {
        public static bool EnableAccountLockForMashupOperations =>
            PluginSettings.LoadBool(nameof(EnableAccountLockForMashupOperations), true);

        // 20 seconds looks like a reasonable timeout - TP gives up on waiting for a reply to a message after 30 seconds,
        // so having a shorter timeout here means the plugin will report lock timeout error before TP gives up
        public static int AccountLockAcquiringTimeoutMs =>
            PluginSettings.LoadInt(nameof(AccountLockAcquiringTimeoutMs), 20 * 10000);

        public static Maybe<IReadOnlyCollection<string>> MashupFileExtensionsWhiteList =>
            PluginSettings
                .LoadString(nameof(MashupFileExtensionsWhiteList))
                .NothingIfNull()
                .Select(value => (IReadOnlyCollection<string>) value.Split('|').ToHashSet(StringComparer.InvariantCultureIgnoreCase));
    }
}
