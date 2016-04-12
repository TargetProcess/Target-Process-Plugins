namespace Tp.Search.Model.Query
{
	internal struct Page
	{
		public int Number { get; set; }
		public int Size { get; set; }
		public int PositionWithin { get; set; }
	}
}