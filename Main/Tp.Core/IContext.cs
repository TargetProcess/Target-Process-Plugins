namespace Tp.Core.Interfaces
{
	public interface IContext
	{
		object GetValue(string name);
		bool Contains(string name);
		void SetValue(string name, object value);
		void Remove(string name);
	}
}