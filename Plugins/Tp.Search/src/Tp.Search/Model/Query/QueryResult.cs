namespace Tp.Search.Model.Query
{
	public class QueryResult
	{
		public string QueryString { get; set; }
		public string[] GeneralIds { get; set; }
		public string[] AssignableIds { get; set; }
		public string[] TestCaseIds { get; set; }
		public string[] ImpedimentIds { get; set; }
		public string[] CommentIds { get; set; }
		public int Total { get; set; }
		public int LastIndexedEntityId { get; set; }
		public int LastIndexedCommentId { get; set; }
	}
}