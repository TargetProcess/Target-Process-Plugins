using System.Text;
using System.Threading;
using NServiceBus;
using NServiceBus.Unicast.Transport;
using Tp.Integration.Messages.ServiceBus.Transport.Router.MsmqRx;

namespace Tp.Integration.Messages.ServiceBus.Transport.Router.Log
{
	public struct LoggerContext
	{
		public string QueueName { get; private set; }
		public TransportMessage Message { get; private set; }

		public static LoggerContext New(string queueName, TransportMessage message = null)
		{
			return new LoggerContext { QueueName = queueName, Message = message };
		}

		public string ToString(bool fullContext)
		{
			var buf = new StringBuilder();
			Begin(buf);
			PutKeyValueInto(buf, "queue", QueueName);
			if (Message != null)
			{
				AddMessage(buf);
			}
			if (fullContext)
			{
				AddThreadPoolInfo(buf);
			}
			End(buf);
			return buf.ToString();
		}

		private void AddThreadPoolInfo(StringBuilder buf)
		{
			int workerThreads;
			int ioThreads;
			ThreadPool.GetAvailableThreads(out workerThreads, out ioThreads);
			PutKeyValueInto(buf, "workerThreads", workerThreads.ToString());
			PutKeyValueInto(buf, "ioThreads", ioThreads.ToString());
		}

		private void AddMessage(StringBuilder buf)
		{
			MessageAccount messageAccount = MessageAccountParser.Instance.Parse(Message.Headers);
			PutKeyValueInto(buf, "account", messageAccount.Name);
			PutKeyValueInto(buf, "msgType", GetMessageType(Message));
			PutKeyValueInto(buf, "msgId", Message.Id);
			PutKeyValueInto(buf, "corrId", Message.CorrelationId);
			PutKeyValueInto(buf, "returnAddress", Message.ReturnAddress);
			PutKeyValueInto(buf, "winIdentName", Message.WindowsIdentityName);
		}

		private static void Begin(StringBuilder buf)
		{
			buf.Append("[");
			return;
		}

		private static void End(StringBuilder buf)
		{
			buf.Append("]");
			return;
		}

		private static void PutKeyValueInto(StringBuilder buf, string key, string value)
		{
			buf.AppendFormat("{0}={1},", key, value);
			return;
		}

		private static string GetMessageType(TransportMessage message)
		{
			if (message.Body == null || message.Body.Length == 0)
			{
				return string.Empty;
			}
			IMessage firstBody = message.Body[0];
			return firstBody == null
				? string.Empty
				: firstBody.GetType().FullName;
		}
	}
}
