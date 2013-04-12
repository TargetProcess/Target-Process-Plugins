using System;
using System.Diagnostics;

namespace Mercurial
{
    /// <summary>
    /// This class encapsulates a path with an associated action related to a
    /// <see cref="Changeset"/>.
    /// </summary>
    [DebuggerDisplay("ChangesetPathAction (Action={Action}, Path={Path})")]
    public sealed class ChangesetPathAction : IEquatable<ChangesetPathAction>
    {
        /// <summary>
        /// Gets the type of action that was performed on the path.
        /// </summary>
        public ChangesetPathActionType Action
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the path involved.
        /// </summary>
        public string Path
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the path of the source of the copy.
        /// </summary>
        /// <remarks>
        /// This property only contains a value if <see cref="Action"/> is <see cref="ChangesetPathActionType.Add"/>
        /// and the add is a copy of an existing file, otherwise it will be <see cref="string.Empty"/>.
        /// </remarks>
        public string Source
        {
            get;
            internal set;
        }

        #region IEquatable<ChangesetPathAction> Members

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        public bool Equals(ChangesetPathAction other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Equals(other.Action, Action) && Equals(other.Path, Path);
        }

        #endregion

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Action + ": " + Path;
        }

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
            if (obj.GetType() != typeof(ChangesetPathAction))
                return false;
            return Equals((ChangesetPathAction)obj);
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
                return (Action.GetHashCode() * 397) ^ (Path != null ? Path.GetHashCode() : 0);
            }
        }
    }
}