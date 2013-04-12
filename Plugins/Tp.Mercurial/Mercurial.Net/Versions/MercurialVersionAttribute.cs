using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Mercurial.Versions
{
    /// <summary>
    /// This attribute can be applied to <see cref="MercurialVersionBase"/> descendants
    /// to specify which version(s) they apply to.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    [SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments", Justification = "This is just a shortcut for the other constructor, no new properties introduced.")]
    public sealed class MercurialVersionAttribute : Attribute, IComparable<MercurialVersionAttribute>, IEquatable<MercurialVersionAttribute>
    {
        /// <summary>
        /// This is the backing field for the <see cref="FromVersionString"/> property.
        /// </summary>
        private readonly string _FromVersionString = string.Empty;

        /// <summary>
        /// This is the backing field for the <see cref="ToVersionString"/> property.
        /// </summary>
        private readonly string _ToVersionString = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="MercurialVersionAttribute"/> class.
        /// </summary>
        /// <param name="fromVersionString">
        /// The lower-bound of the supported version, inclusive.
        /// </param>
        /// <param name="toVersionString">
        /// The upper-bound of the supported version, inclusive.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="fromVersionString"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="toVersionString"/> is <c>null</c>.</para>
        /// </exception>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "There are multiple ways to access the same version here, as a Version and as a string, keeping the name as-is.")]
        public MercurialVersionAttribute(string fromVersionString, string toVersionString)
        {
            if (fromVersionString == null)
                throw new ArgumentNullException("fromVersionString");
            if (toVersionString == null)
                throw new ArgumentNullException("toVersionString");

            _FromVersionString = fromVersionString;
            _ToVersionString = toVersionString;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MercurialVersionAttribute"/> class.
        /// </summary>
        /// <param name="versionString">
        /// The lower-bound and upper-bound of the supported version, inclusive.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="versionString"/> is <c>null</c>.</para>
        /// </exception>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "There are multiple ways to access the same version here, as a Version and as a string, keeping the name as-is.")]
        public MercurialVersionAttribute(string versionString)
            : this(versionString, versionString)
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets the lower-bound of the supported version, inclusive.
        /// </summary>
        public string FromVersionString
        {
            get
            {
                return _FromVersionString;
            }
        }

        /// <summary>
        /// Gets the <see cref="FromVersionString"/> as a properly formatted <see cref="Version"/>, with missing
        /// pieces appropriately set.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Incorrect format for <see cref="FromVersionString"/>.
        /// </exception>
        public Version FromVersion
        {
            get
            {
                string[] parts = FromVersionString.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

                switch (parts.Length)
                {
                    case 0:
                        return new Version(0, 0, 0, 0);

                    case 1:
                        return new Version(
                            int.Parse(parts[0], CultureInfo.InvariantCulture),
                            0,
                            0,
                            0);

                    case 2:
                        return new Version(
                            int.Parse(parts[0], CultureInfo.InvariantCulture),
                            int.Parse(parts[1], CultureInfo.InvariantCulture),
                            0,
                            0);

                    case 3:
                        return new Version(
                            int.Parse(parts[0], CultureInfo.InvariantCulture),
                            int.Parse(parts[1], CultureInfo.InvariantCulture),
                            int.Parse(parts[2], CultureInfo.InvariantCulture),
                            0);

                    case 4:
                        return new Version(
                            int.Parse(parts[0], CultureInfo.InvariantCulture),
                            int.Parse(parts[1], CultureInfo.InvariantCulture),
                            int.Parse(parts[2], CultureInfo.InvariantCulture),
                            int.Parse(parts[3], CultureInfo.InvariantCulture));

                    default:
                        throw new InvalidOperationException("Incorrect format for FromVersionString");
                }
            }
        }

        /// <summary>
        /// Gets the upper-bound of the supported version, inclusive.
        /// </summary>
        public string ToVersionString
        {
            get
            {
                return _ToVersionString;
            }
        }

        /// <summary>
        /// Gets the <see cref="ToVersionString"/> as a properly formatted <see cref="Version"/>, with missing
        /// pieces appropriately set.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Incorrect format for <see cref="ToVersionString"/>.
        /// </exception>
        public Version ToVersion
        {
            get
            {
                string[] parts = ToVersionString.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

                switch (parts.Length)
                {
                    case 0:
                        return new Version(65535, 65535, 65535, 65535);

                    case 1:
                        return new Version(
                            int.Parse(parts[0], CultureInfo.InvariantCulture),
                            65535,
                            65535,
                            65535);

                    case 2:
                        return new Version(
                            int.Parse(parts[0], CultureInfo.InvariantCulture),
                            int.Parse(parts[1], CultureInfo.InvariantCulture),
                            65535,
                            65535);

                    case 3:
                        return new Version(
                            int.Parse(parts[0], CultureInfo.InvariantCulture),
                            int.Parse(parts[1], CultureInfo.InvariantCulture),
                            int.Parse(parts[2], CultureInfo.InvariantCulture),
                            65535);

                    case 4:
                        return new Version(
                            int.Parse(parts[0], CultureInfo.InvariantCulture),
                            int.Parse(parts[1], CultureInfo.InvariantCulture),
                            int.Parse(parts[2], CultureInfo.InvariantCulture),
                            int.Parse(parts[3], CultureInfo.InvariantCulture));

                    default:
                        throw new InvalidOperationException("Incorrect format for ToVersionString");
                }
            }
        }

        /// <summary>
        /// Determines if the specified version is a match against this attribute.
        /// </summary>
        /// <param name="version">
        /// The version to compare against <see cref="FromVersionString"/> and <see cref="ToVersionString"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="version"/> is between <see cref="FromVersionString"/> and <see cref="ToVersionString"/>,
        /// inclusive; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="version"/> is <c>null</c>.</para>
        /// </exception>
        public bool IsMatch(Version version)
        {
            if (version == null)
                throw new ArgumentNullException("version");

            return FromVersion <= version && version <= ToVersion;
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Comparison only handles encapsulating overlaps.
        /// </exception>
        public int CompareTo(MercurialVersionAttribute other)
        {
            // null always before anything else
            if (other == null)
                return +1;

            if (FromVersion == other.FromVersion && ToVersion == other.ToVersion)
                return 0;

            // first determine if we have overlap
            bool overlap = ToVersion >= other.FromVersion && FromVersion <= other.ToVersion;

            if (overlap)
            {
                if (FromVersion <= other.FromVersion && ToVersion >= other.ToVersion)
                    return +1;
                if (other.FromVersion <= FromVersion && other.ToVersion >= ToVersion)
                    return -1;

                throw new InvalidOperationException("Comparison only handles encapsulating overlaps");
            }

            if (ToVersion < other.FromVersion)
                return -1;

            return +1;
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
        public bool Equals(MercurialVersionAttribute other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return base.Equals(other) && Equals(other._FromVersionString, _FromVersionString) && Equals(other._ToVersionString, _ToVersionString);
        }

        /// <summary>
        /// Returns a value that indicates whether this instance is equal to a specified object.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> equals the type and value of this instance; otherwise, false.
        /// </returns>
        /// <param name="obj">
        /// An <see cref="Object"/> to compare with this instance or null. 
        /// </param>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return Equals(obj as MercurialVersionAttribute);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer hash code.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = base.GetHashCode();
                result = (result * 397) ^ (_FromVersionString != null ? _FromVersionString.GetHashCode() : 0);
                result = (result * 397) ^ (_ToVersionString != null ? _ToVersionString.GetHashCode() : 0);
                return result;
            }
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">
        /// The left operand to the == operator.
        /// </param>
        /// <param name="right">
        /// The right operand to the == operator.
        /// </param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(MercurialVersionAttribute left, MercurialVersionAttribute right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">
        /// The left operand to the != operator.
        /// </param>
        /// <param name="right">
        /// The right operand to the != operator.
        /// </param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(MercurialVersionAttribute left, MercurialVersionAttribute right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="left">
        /// The left operand to the &lt; operator.
        /// </param>
        /// <param name="right">
        /// The right operand to the &lt; operator.
        /// </param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator <(MercurialVersionAttribute left, MercurialVersionAttribute right)
        {
            if (left == null && right == null)
                return false;
            if (left == null)
                return true;

            return left.CompareTo(right) < 0;
        }

        /// <summary>
        /// Implements the operator &gt;.
        /// </summary>
        /// <param name="left">
        /// The left operand to the &gt; operator.
        /// </param>
        /// <param name="right">
        /// The right operand to the &gt; operator.
        /// </param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator >(MercurialVersionAttribute left, MercurialVersionAttribute right)
        {
            if (left == null && right == null)
                return false;
            if (left == null)
                return true;

            return left.CompareTo(right) > 0;
        }
    }
}