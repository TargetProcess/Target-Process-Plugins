namespace Tp.Search.Bus.Data
{
	public class QueryData
	{
		public QueryData()
		{
			Page = new PageData
				{
					Number = 0,
					Size = 10
				};
		}
		public string Query { get; set; }
		public int? EntityTypeId { get; set; }
		public int[] EntityStateIds { get; set; }
		public int[] ProjectIds { get; set; }
		public int[] TeamIds { get; set; }
		public TeamProjectRelation[] TeamProjectRelations { get; set; }
		public bool IncludeNoProject { get; set; }
		public bool IncludeNoTeam { get; set; }
		public PageData Page { get; set; }

		public bool IsCommentEntityType
		{
			get { return EntityTypeId == 19; }
		}

		public bool ShouldSearchComment
		{
			get 
			{
				if (EntityStateIds != null && EntityStateIds.Length != 0)
				{
					return false;
				}
				return EntityTypeId == null || IsCommentEntityType;
			}
		}
	}
}
