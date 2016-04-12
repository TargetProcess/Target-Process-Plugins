namespace Tp.Core
{
	public abstract class SafeNull<TNullObject, TNullObjectBaseInterface>
		where TNullObject : TNullObjectBaseInterface, INullable, new()
	{
		public static readonly TNullObjectBaseInterface Instance = new TNullObject();

		public bool IsNull
		{
			get { return true; }
		}
	}
}
