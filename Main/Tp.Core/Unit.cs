using System;
using System.Runtime.InteropServices;

namespace Tp.Core
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct Unit : IEquatable<Unit>
	{
		private static readonly Unit _default = new Unit();

		/// <summary>
		/// Gets the single unit value.
		/// 
		/// </summary>
		public static Unit Default
		{
			get { return Unit._default; }
		}

		static Unit()
		{
		}

		/// <summary>
		/// Indicates whether first and second arguments are equal.
		/// 
		/// </summary>
		/// <param name="first">The first Unit to compare, or null.</param><param name="second">The second Unit to compare, or null.</param>
		/// <returns>
		/// Always returns true.
		/// </returns>
		public static bool operator ==(Unit first, Unit second)
		{
			return true;
		}

		/// <summary>
		/// Indicates whether first and second arguments are not equal.
		/// 
		/// </summary>
		/// <param name="first">The first Unit to compare, or null.</param><param name="second">The second Unit to compare, or null.</param>
		/// <returns>
		/// Always returns false.
		/// </returns>
		public static bool operator !=(Unit first, Unit second)
		{
			return false;
		}

		/// <summary>
		/// Indicates whether the current unit is equal to the specified unit.
		/// 
		/// </summary>
		/// 
		/// <remarks>
		/// Always returns true.
		/// </remarks>
		public bool Equals(Unit other)
		{
			return true;
		}

		/// <summary>
		/// Indicates whether the current unit is equal to the specified object.
		/// 
		/// </summary>
		/// <param name="obj">The System.Object to compare with the current Unit.</param>
		/// <returns>
		/// true if the current unit is equal to the specified object; otherwise, false.
		/// </returns>
		public override bool Equals(object obj)
		{
			return obj is Unit;
		}

		/// <summary>
		/// Gets the unit value's hash code.
		/// 
		/// </summary>
		/// 
		/// <returns>
		/// The unit value's hash code.
		/// </returns>
		public override int GetHashCode()
		{
			return 0;
		}
	}
}
