using System;
using System.Diagnostics;
using System.Globalization;

namespace Mercurial
{
    /// <summary>
    /// This object contains information about an annotated line from a text file
    /// in the repository.
    /// </summary>
    [DebuggerDisplay("Annotation (#{LineNumber} - {RevisionNumber}: {Line})")]
    public sealed class Annotation : IEquatable<Annotation>
    {
        /// <summary>
        /// This is the backing field for the <see cref="Line"/> property.
        /// </summary>
        private readonly string _Line;

        /// <summary>
        /// This is the backing field for the <see cref="LineNumber"/> property.
        /// </summary>
        private readonly int _LineNumber;

        /// <summary>
        /// This is the backing field for the <see cref="RevisionNumber"/> property.
        /// </summary>
        private readonly int _RevisionNumber;

        /// <summary>
        /// Initializes a new instance of the <see cref="Annotation"/> class.
        /// </summary>
        /// <param name="lineNumber">
        /// The line number for this <see cref="Annotation"/>.
        /// </param>
        /// <param name="revisionNumber">
        /// The revision number of this <see cref="Annotation"/>
        /// </param>
        /// <param name="line">
        /// The line that was annotated.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="line"/> is <c>null</c> or empty.</para>
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <para><paramref name="lineNumber"/> cannot be negative.</para>
        /// <para>- or -</para>
        /// <para><paramref name="revisionNumber"/> cannot be negative.</para>
        /// </exception>
        public Annotation(int lineNumber, int revisionNumber, string line)
        {
            if (StringEx.IsNullOrWhiteSpace(line))
                throw new ArgumentNullException("line");
            if (lineNumber < 0)
                throw new ArgumentOutOfRangeException("lineNumber", lineNumber, "lineNumber cannot be negative");
            if (revisionNumber < 0)
                throw new ArgumentOutOfRangeException("revisionNumber", revisionNumber, "revisionNumber cannot be negative");

            _LineNumber = lineNumber;
            _RevisionNumber = revisionNumber;
            _Line = line;
        }

        /// <summary>
        /// Gets the zero-based line number for this <see cref="Annotation"/>.
        /// </summary>
        public int LineNumber
        {
            get
            {
                return _LineNumber;
            }
        }

        /// <summary>
        /// Gets the revision number of the changeset that last modified the line in this
        /// <see cref="Annotation"/>.
        /// </summary>
        public int RevisionNumber
        {
            get
            {
                return _RevisionNumber;
            }
        }

        /// <summary>
        /// Gets the annotated version of that line.
        /// </summary>
        public string Line
        {
            get
            {
                return _Line;
            }
        }

        #region IEquatable<Annotation> Members

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        public bool Equals(Annotation other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Equals(other._Line, _Line) && other._LineNumber == _LineNumber && other._RevisionNumber == _RevisionNumber;
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
        /// </exception><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != typeof(Annotation))
                return false;
            return Equals((Annotation)obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = _Line != null ? _Line.GetHashCode() : 0;
                result = (result * 397) ^ _LineNumber;
                result = (result * 397) ^ _RevisionNumber;
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
            return string.Format(CultureInfo.InvariantCulture, "#{0} - {1}: {2}", LineNumber, RevisionNumber, Line);
        }
    }
}