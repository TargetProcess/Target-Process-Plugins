using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Tp.Core.Extensions
{
	public static class Objects
	{
		/// <summary>
		/// Creates a shallow copy of specified object (ONLY PROPERTIES ARE SUPPORTED)
		/// </summary>
		/// <remarks><b>Only properties supported</b></remarks>
		/// <param name="original">Specified object to clone</param>
		/// <returns>Object clone</returns>
		public static object CloneObject(this object original)
		{
			Type type = original.GetType();
			PropertyInfo[] properties = type.GetProperties();

			var clone = type.InvokeMember("", BindingFlags.CreateInstance, null, original, null);

			foreach (PropertyInfo pi in properties)
			{
				if (pi.CanWrite)
				{
					pi.SetValue(clone, pi.GetValue(original, null), null);
				}
			}

			return clone;
		}

		public static Func<object, object> CreateCloner(Type type)
		{
			var originalParam = Expression.Parameter(typeof(object), "original");
			var convert = Expression.Convert(originalParam, type);

			var propAssignment = type.GetProperties().Where(pi => pi.CanWrite).Select(pi =>
			{
				var memberAccess = Expression.MakeMemberAccess(convert, pi);
				return Expression.Bind(pi, memberAccess);
			}).ToList();

			var memberInit = Expression.MemberInit(Expression.New(type), propAssignment);

			return Expression.Lambda<Func<object, object>>(memberInit, originalParam).Compile();
		}
	}
}
