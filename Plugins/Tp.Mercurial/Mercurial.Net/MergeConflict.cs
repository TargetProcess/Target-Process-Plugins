using System;
using System.Globalization;

namespace Mercurial
{
    /// <summary>
    /// This class contains information about a single file that had a merge conflict during the last merge.
    /// </summary>
    public sealed class MergeConflict : IEquatable<MergeConflict>
    {
        /// <summary>
        /// This is the backing field for the <see cref="Path"/> property.
        /// </summary>
        private readonly string _Path;

        /// <summary>
        /// This is the backing field for the <see cref="State"/> property.
        /// </summary>
        private readonly MergeConflictState _State;

        /// <summary>
        /// Initializes a new instance of the <see cref="MergeConflict"/> class.
        /// </summary>
        /// <param name="path">
        /// The path to the file that had a merge conflict.
        /// </param>
        /// <param name="state">
        /// The current state of the file.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="path"/> is <c>null</c> or empty.</para>
        /// </exception>
        public MergeConflict(string path, MergeConflictState state)
        {
            if (StringEx.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException("path");

            _Path = path;
            _State = state;
        }

        /// <summary>
        /// Gets the state of the file that had a merge conflict.
        /// </summary>
        public MergeConflictState State
        {
            get
            {
                return _State;
            }
        }

        /// <summary>
        /// Gets the path to the file that had a merge conflict.
        /// </summary>
        public string Path
        {
            get
            {
                return _Path;
            }
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        public bool Equals(MergeConflict other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Equals(other._Path, _Path) && Equals(other._State, _State);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Object"/> is equal to the current <see cref="Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="Object"/> is equal to the current <see cref="Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">
        /// The <see cref="Object"/> to compare with the current <see cref="Object"/>. 
        /// </param>
        /// <exception cref="NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != typeof(MergeConflict))
                return false;
            return Equals((MergeConflict)obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((_Path != null ? _Path.GetHashCode() : 0) * 397) ^ _State.GetHashCode();
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "MergeConflict (Path={0}, State={1})", _Path, _State);
        }
    }
}