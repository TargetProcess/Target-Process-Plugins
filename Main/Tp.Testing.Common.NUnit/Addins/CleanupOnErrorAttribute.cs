using System;

namespace Tp.Testing.Common.NUnit.Addins
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class CleanupOnErrorAttribute : Attribute
	{
	}
}
