using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Queries
{
	[Serializable]
	public abstract class QueryBase : ITargetProcessCommand
	{
		public abstract DtoType DtoType { get; }
		public bool IgnoreMessageSizeOverrunFailure { get; set; }
		public int? Take { get; set; }
		public int? Skip { get; set; }
	}

	[Serializable]
	public class QueryResult<TDto> : SagaMessage
		where TDto : DataTransferObject
	{
		public TDto[] Dtos { get; set; }
		public int QueryResultCount { get; set; }

		/// <summary>
		/// Because of Msmq limits(4 mb per message), large entities could not be sent.
		/// This Field contains number of failed dtos.
		/// </summary>
		public int FailedDtosCount { get; set; }

		public int TotalQueryResultCount { get; set; }
	}
}
