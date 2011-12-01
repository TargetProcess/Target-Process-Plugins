using System;

namespace Tp.Core
{
	public class DependentPropertyHelper
	{
		public static Maybe<TDependencyOtherEnd> GetDependentProperty<TDependencyEnd, TDependency, TDependencyOtherEnd>(TDependencyEnd obj, Func<TDependency, TDependencyOtherEnd> f)
			where TDependencyEnd : class
			where TDependency : class
		{
			if (obj == null)
			{
				return null;
			}
			var maybeDependent = obj as TDependency;
			return maybeDependent != null
					? Maybe.Just(f(maybeDependent))
					: Maybe.Nothing;
		}

		public static bool IsDependendUpon<TDependency>(Type type)
		{
			return typeof(TDependency).IsAssignableFrom(type);
		}
	}
}
