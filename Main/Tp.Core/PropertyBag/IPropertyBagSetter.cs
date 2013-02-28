namespace Tp.Core.PropertyBag
{
	public interface IPropertyBagSetter
	{
		void SetProperty<T>(TypedKey<T> key, T value);
	}
}