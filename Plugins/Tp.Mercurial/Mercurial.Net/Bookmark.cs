using System;
using System.Globalization;

namespace Mercurial
{
    /// <summary>
    /// This class encapsulates information about a single bookmark in the repository.
    /// </summary>
    public class Bookmark : NamedRevision, IEquatable<Bookmark>
    {
        /// <summary>
        /// This is the backing field for the <see cref="IsWorkingFolder"/> property.
        /// </summary>
        private readonly bool _IsWorkingFolder;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bookmark"/> class.
        /// </summary>
        /// <param name="revisionNumber">
        /// The revision number the bookmark is currently at.
        /// </param>
        /// <param name="name">
        /// The name of the bookmark.
        /// </param>
        /// <param name="isWorkingFolder">
        /// If set to <c>true</c>, then the working folder is currently at this bookmark.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="name"/> is <c>null</c> or empty.</para>
        /// </exception>
        public Bookmark(int revisionNumber, string name, bool isWorkingFolder = false)
            : base(revisionNumber, name)
        {
            _IsWorkingFolder = isWorkingFolder;
        }

        /// <summary>
        /// Gets a value indicating whether the working folder is currently at this bookmark.
        /// </summary>
        public bool IsWorkingFolder
        {
            get
            {
                return _IsWorkingFolder;
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
        public bool Equals(Bookmark other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return base.Equals(other);
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
            return Equals(obj as Bookmark);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="String"/> that represents the current <see cref="Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="String"/> that represents the current <see cref="Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Bookmark ({2}Name={0}, Revision=#{1})", Name, RevisionNumber, (_IsWorkingFolder ? "* " : string.Empty));
        }
    }
}