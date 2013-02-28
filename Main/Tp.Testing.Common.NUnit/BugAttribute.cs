namespace  NUnit.Framework
{
	public sealed class BugAttribute : TestAttribute
	{
		public BugAttribute(int id)
		{
			ID = id;
			Description = "Test for Bug #" + id;
		}

		public int ID { get; private set; }
	}
}