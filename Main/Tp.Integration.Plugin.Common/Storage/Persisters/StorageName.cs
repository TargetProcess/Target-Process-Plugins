// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;

namespace Tp.Integration.Plugin.Common.Storage.Persisters
{
    [Serializable]
    public class StorageName
    {
        public StorageName()
            : this(string.Empty)
        {
        }

        public StorageName(string profileName)
        {
            Value = profileName;
        }

        public string Value { get; set; }

        public bool IsEmpty
        {
            get { return string.IsNullOrEmpty(Value); }
        }

        public bool Equals(StorageName other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Value, Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(StorageName)) return false;
            return Equals((StorageName) obj);
        }

        public override int GetHashCode()
        {
            return (Value != null ? Value.GetHashCode() : 0);
        }

        public static implicit operator StorageName(string profileName)
        {
            return new StorageName(profileName);
        }

        public static bool operator ==(StorageName left, StorageName right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(StorageName left, StorageName right)
        {
            return !(left == right);
        }
    }
}
