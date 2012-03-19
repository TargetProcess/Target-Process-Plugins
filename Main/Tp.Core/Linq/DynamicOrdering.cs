using System.Linq.Expressions;

namespace System.Linq.Dynamic
{
	internal class DynamicOrdering
	{
		public bool Ascending;
		public Expression Selector;
	}
}