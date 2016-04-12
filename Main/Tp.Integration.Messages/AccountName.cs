using System;

namespace Tp.Integration.Messages
{
	[Serializable]
	public class AccountName
	{
		//Empty account means onSite mode.
		public static readonly AccountName Empty = new AccountName("~~~EMPTY~~~");
		// must be a unique, non existing account name
		public AccountName()
			: this(Empty.Value)
		{
		}

		public AccountName(string name)
		{
			Value = name;
		}

		public string Value { get; set; }

		public bool Equals(AccountName other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.Value, Value);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(AccountName)) return false;
			return Equals((AccountName) obj);
		}

		public override int GetHashCode()
		{
			return (Value != null ? Value.GetHashCode() : 0);
		}

		public static implicit operator AccountName(string accountName)
		{
			return new AccountName(accountName);
		}

		public static bool operator ==(AccountName left, AccountName right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(AccountName left, AccountName right)
		{
			return !(left == right);
		}

		public override string ToString()
		{
			return Value;
		}

		public bool IsEmpty
		{
			get { return this == Empty; }
		}
	}
}
