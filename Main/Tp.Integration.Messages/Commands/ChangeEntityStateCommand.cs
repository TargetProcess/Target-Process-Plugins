using Tp.Integration.Messages.EntityLifecycle;

namespace Tp.Integration.Messages.Commands
{
	public class ChangeEntityStateCommand : ITargetProcessCommand
	{
		public int? EntityId { get; set; }
		public string State { get; set; }
		public int UserID { get; set; }
		public string Comment { get; set; }
		public string DefaultComment { get; set; }
	}
}
