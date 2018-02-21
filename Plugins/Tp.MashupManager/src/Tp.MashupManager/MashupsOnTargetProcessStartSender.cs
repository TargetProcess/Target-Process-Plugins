using System.Collections.Generic;
using System.Linq;
using NServiceBus;
using StructureMap;
using Tp.Integration.Messages.TargetProcessLifecycle;
using Tp.Integration.Plugin.Common.Domain;
using Tp.MashupManager.MashupStorage;

namespace Tp.MashupManager
{
    public class MashupsOnTargetProcessStartSender : IHandleMessages<TargetProcessStartedMessage>
    {
        private IEnumerable<IAccountReadonly> Accounts => ObjectFactory.GetInstance<IAccountCollection>();

        private IMashupScriptStorage MashupStorage => ObjectFactory.GetInstance<IMashupScriptStorage>();

        public void Handle(TargetProcessStartedMessage message)
        {
            foreach (var account in Accounts)
            {
                if (!account.Profiles.Any())
                {
                    continue;
                }

                var profile = account.Profiles.First();
                var mashupManagerProfile = profile.GetProfile<MashupManagerProfile>();
                var bus = ObjectFactory.GetInstance<IBus>();

                mashupManagerProfile.MashupNames
                    .Select(mashupName => MashupStorage.GetMashup(account.Name, mashupName))
                    .Where(mashup => mashup != null)
                    .Select(mashup => mashup.CreatePluginMashupMessage(account.Name))
                    .ForEach(mashupMessage => bus.Send(mashupMessage));
            }
        }
    }
}
