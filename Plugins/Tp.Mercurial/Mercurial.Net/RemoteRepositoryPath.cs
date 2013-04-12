using System;
using System.Globalization;

namespace Mercurial
{
    /// <summary>
    /// This class encapsulates information about the path to a remote repository related to the current
    /// repository.
    /// </summary>
    public class RemoteRepositoryPath : IEquatable<RemoteRepositoryPath>
    {
        /// <summary>
        /// This is the backing field for the <see cref="Name"/> property.
        /// </summary>
        private readonly string _Name;

        /// <summary>
        /// This is the backing field for the <see cref="Path"/> property.
        /// </summary>
        private readonly string _Path;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteRepositoryPath"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the path to the remote repository.
        /// </param>
        /// <param name="path">
        /// The path to the remote repository.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="name"/> is <c>null</c> or empty.</para>
        /// <para>- or -</para>
        /// <para><paramref name="path"/> is <c>null</c> or empty.</para>
        /// </exception>
        public RemoteRepositoryPath(string name, string path)
        {
            if (StringEx.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");
            if (StringEx.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException("path");

            _Name = name.Trim();
            _Path = path.Trim();
        }

        /// <summary>
        /// Gets the path to the remote repository.
        /// </summary>
        public string Path
        {
            get
            {
                return _Path;
            }
        }

        /// <summary>
        /// Gets the name of the path to the remote repository.
        /// </summary>
        public string Name
        {
            get
            {
                return _Name;
            }
        }

        #region IEquatable<RemoteRepositoryPath> Members

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        public bool Equals(RemoteRepositoryPath other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Equals(other._Name, _Name) && Equals(other._Path, _Path);
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
            if (obj.GetType() != typeof(RemoteRepositoryPath))
                return false;
            return Equals((RemoteRepositoryPath)obj);
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
                return ((_Name != null ? _Name.GetHashCode() : 0) * 397) ^ (_Path != null ? _Path.GetHashCode() : 0);
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} = {1}", _Name, _Path);
        }
    }
}