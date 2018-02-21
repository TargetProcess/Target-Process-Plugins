using System.Collections.Generic;
using Tp.Integration.Messages.ServiceBus.Transport.Router.MsmqRx;

namespace Tp.Integration.Messages.ServiceBus.Transport.Router
{
    public interface IRouterChildTagsSource
    {
        IEnumerable<string> GetChildTags();
        bool NeedToHandleMessage(MessageEx message);
    }
}
