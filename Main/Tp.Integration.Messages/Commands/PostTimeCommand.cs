using Tp.Integration.Messages.EntityLifecycle;

namespace Tp.Integration.Messages.Commands
{
	public class PostTimeCommand : ITargetProcessCommand
	{
		public int? EntityId { get; set; }
		public decimal Spent { get; set; }
		public decimal? Left { get; set; }
		public int UserID { get; set; }
		public string Description { get; set; }
	}
}
