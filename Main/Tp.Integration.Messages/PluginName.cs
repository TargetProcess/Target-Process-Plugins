using System;

namespace Tp.Integration.Messages
{
	/// <summary>
	/// A value object for holding plugin name value.
	/// </summary>
	[Serializable]
	public class PluginName
	{
		public PluginName()
			: this(string.Empty)
		{
		}

		public PluginName(string pluginName)
		{
			Value = pluginName;
		}

		public string Value { get; set; }

		public bool Equals(PluginName other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.Value, Value);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(PluginName)) return false;
			return Equals((PluginName) obj);
		}

		public override int GetHashCode()
		{
			return (Value != null ? Value.GetHashCode() : 0);
		}

		public static implicit operator PluginName(string pluginName)
		{
			return new PluginName(pluginName);
		}

		public static bool operator ==(PluginName left, PluginName right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(PluginName left, PluginName right)
		{
			return !(left == right);
		}

		public override string ToString()
		{
			return Value;
		}
	}
}
