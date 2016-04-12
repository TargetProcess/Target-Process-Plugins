using System;
using System.Text;
using Tp.Integration.Messages.EntityLifecycle;

namespace Tp.Integration.Messages.TargetProcessLifecycle
{
	[Serializable]
	public class TargetProcessExceptionThrown : Exception
	{
		private readonly string _exceptionString;

		public TargetProcessExceptionThrown(string exceptionString)
		{
			_exceptionString = exceptionString;
		}

		public override string ToString()
		{
			return _exceptionString;
		}
	}

	/// <summary>
	/// Represents an exception message from TargetProcess.
	/// </summary>
	[Serializable]
	public class TargetProcessExceptionThrownMessage : SagaMessage, ISagaMessage
	{
		public TargetProcessExceptionThrownMessage()
		{
		}

		public TargetProcessExceptionThrownMessage(Exception exception, string exceptionContext = null)
		{
			ExceptionString = Convert(exception) + exceptionContext;
		}

		public string ExceptionString { get; set; }

		public TargetProcessExceptionThrown GetException()
		{
			return new TargetProcessExceptionThrown(ExceptionString);
		}

		private static string Convert(Exception exception)
		{
			var description = new StringBuilder();
			description.AppendFormat("{0}: {1}", exception.GetType().Name, exception.Message);

			if (exception.InnerException != null)
			{
				description.AppendFormat(" ---> {0}", exception.InnerException);
				description.AppendFormat(
					"{0}   --- End of inner exception stack trace ---{0}",
					Environment.NewLine);
			}

			description.Append(exception.StackTrace);

			return description.ToString();
		}

		public override string ToString()
		{
			return ExceptionString;
		}
	}
}
