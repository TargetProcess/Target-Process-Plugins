using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;

namespace Mercurial
{
    /// <summary>
    /// This class encapsulates a changeset from the log.
    /// </summary>
    [DebuggerDisplay("Changeset ({RevisionNumber}:{Hash} ({Branch} - {AuthorName} <{AuthorEmail}> - {Timestamp}: {CommitMessage}")]
    public sealed class Changeset : IEquatable<Changeset>
    {
        /// <summary>
        /// This is the backing field for the <see cref="PathActions"/> property.
        /// </summary>
        private readonly List<ChangesetPathAction> _PathActions = new List<ChangesetPathAction>();

        /// <summary>
        /// Gets the timestamp of this <see cref="Changeset"/>.
        /// </summary>
        public DateTime Timestamp
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the name of the author of this <see cref="Changeset"/>.
        /// </summary>
        public string AuthorName
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the email address of the author of this <see cref="Changeset"/>.
        /// </summary>
        public string AuthorEmailAddress
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the commit message of this <see cref="Changeset"/>.
        /// </summary>
        public string CommitMessage
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the branch this <see cref="Changeset"/> is on.
        /// </summary>
        public string Branch
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the unique hash of this <see cref="Changeset"/>.
        /// </summary>
        public string Hash
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the locally unique revision number of this <see cref="Changeset"/>.
        /// </summary>
        public int RevisionNumber
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the <see cref="RevSpec"/> for this <see cref="Changeset"/>.
        /// </summary>
        public RevSpec Revision
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the collection of tags for this <see cref="Changeset"/>, or an empty collection if this changeset has no tags.
        /// </summary>
        public IEnumerable<string> Tags
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the locally unique revision number of the left parent, or -1 if this is the initial changeset.
        /// </summary>
        public int LeftParentRevision
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the hash of the left parent, or <see cref="string.Empty"/> if this is the initial changeset.
        /// </summary>
        public string LeftParentHash
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the locally unique revision number of the right parent, or -1 if this is the initial changeset.
        /// </summary>
        public int RightParentRevision
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the hash of the right parent, or <see cref="string.Empty"/> if this is the initial changeset.
        /// </summary>
        public string RightParentHash
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the collection of path actions this changeset contains.
        /// </summary>
        public Collection<ChangesetPathAction> PathActions
        {
            get
            {
                return new Collection<ChangesetPathAction>(_PathActions);
            }
        }

        #region IEquatable<Changeset> Members

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        public bool Equals(Changeset other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return other.Timestamp.Equals(Timestamp) && Equals(other.AuthorName, AuthorName) && Equals(other.AuthorEmailAddress, AuthorEmailAddress) &&
                   Equals(other.CommitMessage, CommitMessage) && Equals(other.Branch, Branch) && Equals(other.Hash, Hash) &&
                   Equals(other.LeftParentHash, LeftParentHash) && Equals(other.RightParentHash, RightParentHash);
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
            if (obj.GetType() != typeof(Changeset))
                return false;
            return Equals((Changeset)obj);
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
                int result = Timestamp.GetHashCode();
                result = (result * 397) ^ (AuthorName != null ? AuthorName.GetHashCode() : 0);
                result = (result * 397) ^ (AuthorEmailAddress != null ? AuthorEmailAddress.GetHashCode() : 0);
                result = (result * 397) ^ (CommitMessage != null ? CommitMessage.GetHashCode() : 0);
                result = (result * 397) ^ (Branch != null ? Branch.GetHashCode() : 0);
                result = (result * 397) ^ (Hash != null ? Hash.GetHashCode() : 0);
                result = (result * 397) ^ (LeftParentHash != null ? LeftParentHash.GetHashCode() : 0);
                result = (result * 397) ^ (RightParentHash != null ? RightParentHash.GetHashCode() : 0);
                return result;
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
            return string.Format(
                CultureInfo.InvariantCulture, "{0}:{1} - {2} <{3}> ({4})", RevisionNumber, Hash, AuthorName, AuthorEmailAddress, Timestamp);
        }
    }
}