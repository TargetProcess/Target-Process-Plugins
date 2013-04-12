using System;

namespace Tp.Core.Interfaces
{
	public interface IContext
	{
		object GetValue(string name);
		bool Contains(string name);
		void SetValue(string name, object value);
		void Remove(string name);
		string Print();
	}

	public static class ContextExtensions
	{
		public static T GetOrAdd<T>(this IContext context, string key, Func<T> valueGetter)
		{
			if (context.Contains(key))
				return (T) context.GetValue(key);

			var value = valueGetter();
			context.SetValue(key, value);
			return value;
		}
	}
}