using System.Runtime.Remoting.Messaging;

namespace Tp.Core
{
    public static class ClientId
    {
        private const string KEY = "clientId";

        public static void Set(string value)
        {
            CallContext.LogicalSetData(KEY, value);
        }

        public static string Get()
        {
            return CallContext.LogicalGetData(KEY) as string;
        }
    }
}
