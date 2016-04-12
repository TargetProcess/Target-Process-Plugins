using System;

namespace Tp.Integration.Messages.EntityLifecycle.Commands
{
	[Serializable]
	public class AttachGeneralUserToRequestCommand : ITargetProcessCommand
	{
		public int? RequesterId { get; set; }
		public int? RequestId { get; set; }
	}
}
