using System;
using System.Globalization;

namespace Mercurial
{
    /// <summary>
    /// Contains the status of a single modified file in the working folder.
    /// </summary>
    public sealed class FileStatus : IEquatable<FileStatus>
    {
        /// <summary>
        /// This is the backing field for the <see cref="Path"/> property.
        /// </summary>
        private readonly string _Path;

        /// <summary>
        /// This is the backing field for the <see cref="State"/> property.
        /// </summary>
        private readonly FileState _State;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStatus"/> class.
        /// </summary>
        /// <param name="state">
        /// The state of the file.
        /// </param>
        /// <param name="path">
        /// The path to the file, relative to the root of the working folder.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="path"/> is <c>null</c> or empty.</para>
        /// </exception>
        public FileStatus(FileState state, string path)
        {
            if (StringEx.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException("path");

            _State = state;
            _Path = path;
        }

        /// <summary>
        /// Gets the state of the file this <see cref="FileStatus"/> refers to.
        /// </summary>
        public FileState State
        {
            get
            {
                return _State;
            }
        }

        /// <summary>
        /// Gets the path to the file that this <see cref="FileStatus"/> refers to,
        /// relative to the root of the working folder.
        /// </summary>
        public string Path
        {
            get
            {
                return _Path;
            }
        }

        #region IEquatable<FileStatus> Members

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.
        /// </param>
        public bool Equals(FileStatus other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Equals(other._Path, _Path) && Equals(other._State, _State);
        }

        #endregion

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">
        /// The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. 
        /// </param>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != typeof(FileStatus))
                return false;
            return Equals((FileStatus)obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((_Path != null ? _Path.GetHashCode() : 0) * 397) ^ _State.GetHashCode();
            }
        }

        /// <summary>
        /// Returns a <see cref="String"/> that represents the current <see cref="Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="String"/> that represents the current <see cref="Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "FileStatus (State={0}, Path={1})", _State, _Path);
        }
    }
}