using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security;

namespace System
{
	[DebuggerDisplay("Count = {InnerExceptions.Count}")]
	[Serializable]
	public class AggregateException : Exception
	{
		private const string DEFAULT_MESSAGE = "One or more errors occurred";
		private readonly ReadOnlyCollection<Exception> _innerExceptions;

		public ReadOnlyCollection<Exception> InnerExceptions
		{
			get
			{
				return _innerExceptions;
			}
		}

		public AggregateException()
			: base(DEFAULT_MESSAGE)
		{
			_innerExceptions = new ReadOnlyCollection<Exception>(new Exception[0]);
		}

		public AggregateException(string message)
			: base(message)
		{
			_innerExceptions = new ReadOnlyCollection<Exception>(new Exception[0]);
		}

		public AggregateException(string message, Exception innerException)
			: base(message, innerException)
		{
			if (innerException == null)
				throw new ArgumentNullException("innerException");
			_innerExceptions = new ReadOnlyCollection<Exception>(new[] { innerException });
		}

		public AggregateException(IEnumerable<Exception> innerExceptions)
			: this(DEFAULT_MESSAGE, innerExceptions)
		{
		}

		public AggregateException(params Exception[] innerExceptions)
			: this(DEFAULT_MESSAGE, innerExceptions)
		{
		}

		public AggregateException(string message, IEnumerable<Exception> innerExceptions)
			: this(message, innerExceptions == null ? null : new List<Exception>(innerExceptions))
		{
		}

		public AggregateException(string message, params Exception[] innerExceptions)
			: this(message, (IList<Exception>) innerExceptions)
		{
		}

		private AggregateException(string message, IList<Exception> innerExceptions)
			: base(message, innerExceptions == null || innerExceptions.Count <= 0 ? null : innerExceptions[0])
		{
			if (innerExceptions == null)
				throw new ArgumentNullException("innerExceptions");
			var exceptionArray = new Exception[innerExceptions.Count];
			for (int index = 0; index < exceptionArray.Length; ++index)
			{
				exceptionArray[index] = innerExceptions[index];
				if (exceptionArray[index] == null)
					throw new ArgumentException("An element of innerExceptions was null.");
			}
			_innerExceptions = new ReadOnlyCollection<Exception>(exceptionArray);
		}

		[SecurityCritical]
		protected AggregateException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			if (info == null)
				throw new ArgumentNullException("info");
			var exceptionArray = info.GetValue("InnerExceptions", typeof (Exception[])) as Exception[];
			if (exceptionArray == null)
				throw new SerializationException("Deserialization Failure");
			_innerExceptions = new ReadOnlyCollection<Exception>(exceptionArray);
		}

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
				throw new ArgumentNullException("info");
			base.GetObjectData(info, context);
			var array = new Exception[_innerExceptions.Count];
			_innerExceptions.CopyTo(array, 0);
			info.AddValue("InnerExceptions", array, typeof (Exception[]));
		}

		public override Exception GetBaseException()
		{
			var exception = (Exception) this;
			for (AggregateException aggregateException = this; aggregateException != null && aggregateException.InnerExceptions.Count == 1; aggregateException = exception as AggregateException)
				exception = exception.InnerException;
			return exception;
		}

		public void Handle(Func<Exception, bool> predicate)
		{
			if (predicate == null)
				throw new ArgumentNullException("predicate");
			var list = (List<Exception>) null;
			foreach (Exception t in _innerExceptions)
			{
				if (!predicate(t))
				{
					if (list == null)
						list = new List<Exception>();
					list.Add(t);
				}
			}
			if (list != null)
				throw new AggregateException(Message, list);
		}

		public AggregateException Flatten()
		{
			var list1 = new List<Exception>();
			var list2 = new List<AggregateException> {this};
			int num = 0;
			while (list2.Count > num)
			{
				foreach (Exception exception in list2[num++].InnerExceptions)
				{
					if (exception != null)
					{
						var aggregateException = exception as AggregateException;
						if (aggregateException != null)
							list2.Add(aggregateException);
						else
							list1.Add(exception);
					}
				}
			}
			return new AggregateException(Message, list1);
		}

		public override string ToString()
		{
			string str = base.ToString();
			for (int index = 0; index < _innerExceptions.Count; ++index)
				str = string.Format(CultureInfo.InvariantCulture, "{0}{1}(Inner Exception #{2}) {3}{4}{5}", str, Environment.NewLine, index, _innerExceptions[index], "<---", Environment.NewLine);
			return str;
		}
	}
}