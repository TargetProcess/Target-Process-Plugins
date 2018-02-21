using System;
using System.Messaging;

namespace Tp.Integration.Messages.ServiceBus.Transport
{
    public static class TransportTransactionHelper
    {
        public static void RunInQueueOnlyTransaction(Action<MessageQueueTransaction> transactionalAction)
        {
            var mqTx = new MessageQueueTransaction();
            try
            {
                mqTx.Begin();
                transactionalAction(mqTx);
                mqTx.Commit();
            }
            catch (Exception)
            {
                mqTx.Abort();
                throw;
            }
            finally
            {
                mqTx.Dispose();
            }
        }
    }
}
