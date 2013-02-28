namespace Tp.Core.PropertyBag
{
	public interface IPropertyBagGetter
	{
		Maybe<T> GetProperty<T>(TypedKey<T> key); 
	}
}