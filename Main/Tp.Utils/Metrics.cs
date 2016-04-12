using System;

namespace Tp.Utils
{
	/// <summary>
	/// Length unit.
	/// </summary>
	public enum LengthUnit
	{
		Pixel,
		Percent,
	}

	/// <summary>
	/// Length value with unit.
	/// </summary>
	[Serializable]
	public struct Length
	{
		public int Value;
		public LengthUnit Unit;

		public Length(int value, LengthUnit unit)
		{
			Value = value;
			Unit = unit;
		}

		/// <summary>
		/// Coerce int value to pixel length.
		/// </summary>
		/// <param name="l">Length in pixels.</param>
		/// <returns>Length in pixels.</returns>
		public static implicit operator Length(int l)
		{
			return new Length(l, LengthUnit.Pixel);
		}

		/// <summary>
		/// Coerce length to string, e.g. "100px", "100%".
		/// </summary>
		/// <param name="length">Length.</param>
		/// <returns>String.</returns>
		public static implicit operator string(Length length)
		{
			switch (length.Unit)
			{
				case LengthUnit.Pixel:
					return Convert.ToString(length.Value) + "px";
				case LengthUnit.Percent:
					return Convert.ToString(length.Value) + "%";
			}
			throw new InvalidOperationException("Unreachable");
		}

		/// <summary>
		/// Parse string to length.
		/// </summary>
		/// <param name="length">String with length, e.g. "100", "100px", "100%".</param>
		/// <returns>Length.</returns>
		public static implicit operator Length(string length)
		{
			if (length == null)
			{
				throw new ArgumentNullException(nameof(length), "Value cannot be empty string");
			}

			int off = 0;
			while (off < length.Length && Char.IsDigit(length[off]))
			{
				off++;
			}
			if (off == 0)
			{
				throw new FormatException("Value must begin with a number");
			}
			int value = Int32.Parse(length.Substring(0, off));
			switch (length.Substring(off))
			{
				case "":
				case "px":
				case "Px":
				case "pX":
				case "PX":
					return new Length(value, LengthUnit.Pixel);
				case "%":
					return new Length(value, LengthUnit.Percent);
			}
			throw new FormatException("Value must end with a unit specifier such as 'px' or '%'");
		}

		public override string ToString()
		{
			return this;
		}
	}
}
